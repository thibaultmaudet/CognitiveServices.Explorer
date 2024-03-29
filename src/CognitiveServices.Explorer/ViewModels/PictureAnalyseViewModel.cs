﻿using System.ComponentModel;
using System.Windows.Input;

using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Contracts.ViewModels;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Core.Services;
using CognitiveServices.Explorer.Helpers;
using CognitiveServices.Explorer.Views.Dialogs;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;

namespace CognitiveServices.Explorer.ViewModels;

public class PictureAnalyseViewModel : ObservableRecipient, INavigationAware
{
    private bool isLoading;

    private readonly IDialogService dialogService;

    private IImageInfoService imageInfoService;

    private readonly IFaceClientService faceClientService;

    private IList<PersonGroupWithUserData> personGroups;

    private readonly IPickDialogService pickDialogService;

    private readonly FaceProcessorService faceProcessor;

    private string loadingText;

    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    public IImageInfoService ImageInfoService
    {
        get => imageInfoService;
        set
        {
            if (imageInfoService != value)
            {
                if (imageInfoService != null)
                    imageInfoService.PropertyChanged -= ImageInfoServiceChanged;

                imageInfoService = value;

                if (imageInfoService != null)
                    imageInfoService.PropertyChanged += ImageInfoServiceChanged;

                OnPropertyChanged(nameof(ImageInfoService));
            }

            void ImageInfoServiceChanged(object? sender, PropertyChangedEventArgs args) => OnPropertyChanged(nameof(ImageInfoService));
        }
    }

    public ICommand AddFaceCommand => new RelayCommand(AddFaceAsync);

    public string LoadingText
    {
        get => loadingText;
        set => SetProperty(ref loadingText, value);
    }

    public PictureAnalyseViewModel(IDialogService dialogService, IFaceClientService faceClientService, IImageInfoService imageInfoService, IPickDialogService pickDialogService)
    {
        this.dialogService = dialogService;

        this.faceClientService = faceClientService;

        faceProcessor = new FaceProcessorService(faceClientService.FaceClient);

        ImageInfoService = imageInfoService;
        this.imageInfoService = imageInfoService;

        this.pickDialogService = pickDialogService;

        personGroups = new List<PersonGroupWithUserData>();

        loadingText = "";
    }

    public async void OnNavigatedTo(object parameter)
    {
        personGroups = await faceProcessor.GetPersonGroupsAsync();
    }

    public void OnNavigatedFrom()
    {

    }

    public async void AddFaceAsync()
    {
        IRandomAccessStreamWithContentType randomAccessStream = await ImageInfoService?.File?.OpenReadAsync();

        AddFaceDialog addFaceDialog = new AddFaceDialog(faceClientService);

        ContentDialogResult result = await dialogService.ShowAsync(addFaceDialog);

        if (result == ContentDialogResult.Primary)
        {
            IsLoading = true;

            LoadingText = "PictureAnalyse_AddingFace".GetLocalized();

            await faceProcessor.AddFace(addFaceDialog.SelectedPerson.Value.PersonGroupId, addFaceDialog.SelectedPerson.Key.PersonId, randomAccessStream.AsStreamForRead());

            IsLoading = false;
        }
    }

    public async Task StartFaceDetection()
    {
        IsLoading = true;

        LoadingText = "PictureAnalyse_DetectingPeopleFace".GetLocalized();

        IRandomAccessStreamWithContentType randomAccessStream = await ImageInfoService.File?.OpenReadAsync();
        Stream stream = randomAccessStream.AsStreamForRead();

        ImageInfoService.People.Clear();

        foreach (DetectedFace detectedFace in await faceProcessor.DetectFacesAsync(stream, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Hair }))
            ImageInfoService.People.Add(new() { DetectedFace = detectedFace });

        IsLoading = false;
    }

    public async Task StartFaceRecognitionAsync()
    {
        IsLoading = true;

        LoadingText = "PictureAnalyse_FaceDetecting".GetLocalized();

        ImageInfoService.People.Clear();

        double latestConfidence = 0;

        foreach (PersonGroup personGroup in personGroups)
        {
            var randomAccessStream = await ImageInfoService.File?.OpenReadAsync();
            var stream = randomAccessStream.AsStreamForRead();

            foreach (var detectedFace in await faceProcessor.DetectFacesAsync(stream, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Hair }, personGroup.RecognitionModel))
            {
                IList<IdentifyResult> identifyResults = await faceProcessor.IdentifyFaceAsync(new List<Guid>() { detectedFace.FaceId.GetValueOrDefault() }, personGroup.PersonGroupId);

                if (identifyResults == null)
                    continue;

                foreach (var identifyResult in identifyResults)
                    latestConfidence = IdentifyPerson(identifyResult, detectedFace, personGroup, latestConfidence);
            }
        }

        foreach (var personInfo in ImageInfoService.People)
            personInfo.Name = (personInfo is { IdentifyResult: not null, IdentifyResult.Candidates.Count: > 0 }) ? await faceProcessor.GetPersonNameAsync(personInfo.Group.PersonGroupId, personInfo.IdentifyResult.Candidates[0].PersonId) : "Person_Unknown".GetLocalized();

        IsLoading = false;
    }

    public static StackPanel GetPersonInformations(PersonInfo personInfo)
    {
        StackPanel stackPanel = new();

        if (personInfo.IdentifyResult != null && personInfo.IdentifyResult.Candidates.Count > 0)
        {
            stackPanel.Children.Add(new TextBlock() { Text = personInfo.Name, FontSize = 18, TextAlignment = TextAlignment.Center });
            stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Positive_Match".GetLocalized() + Math.Round(personInfo.IdentifyResult.Candidates[0].Confidence * 100, 2) + "%" });
            stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Group".GetLocalized() + personInfo.Group.Name });
            stackPanel.Children.Add(new MenuFlyoutSeparator());
        }

        stackPanel.Children.Add(new TextBlock() { Text = "Person_Informations_Title".GetLocalized(), FontSize = 16, TextAlignment = TextAlignment.Center, TextDecorations = TextDecorations.Underline });

        stackPanel.Children.Add(new TextBlock() { Text = (personInfo.DetectedFace.FaceAttributes.Gender == Gender.Female) ? "Person_Sex_Female".GetLocalized() : "Person_Sex_Male".GetLocalized() });
        stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Age_Prefix".GetLocalized() + personInfo.DetectedFace.FaceAttributes.Age + "Person_Recognition_Age_Suffix".GetLocalized() });

        if (personInfo.DetectedFace.FaceAttributes.Hair.HairColor.Count > 0)
            switch (personInfo.DetectedFace.FaceAttributes.Hair.HairColor[0].Color)
            {
                case HairColorType.Black:
                    stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_Black".GetLocalized() });
                    break;
                case HairColorType.Blond:
                    stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_Blond".GetLocalized() });
                    break;
                case HairColorType.Brown:
                    stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_Brown".GetLocalized() });
                    break;
                case HairColorType.Gray:
                    stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_Gray".GetLocalized() });
                    break;
                case HairColorType.Other:
                    stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_Other".GetLocalized() });
                    break;
                case HairColorType.Red:
                    stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_Red".GetLocalized() });
                    break;
                case HairColorType.White:
                    stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_White".GetLocalized() });
                    break;
                case HairColorType.Unknown:
                default:
                    stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_Unknown".GetLocalized() });
                    break;
            }
        else
            stackPanel.Children.Add(new TextBlock() { Text = "Person_Recognition_Hair_Unknown".GetLocalized() });

        return stackPanel;
    }

    public async Task<StorageFile?> OpenImageAsync()
    {
        var file = await pickDialogService.PickSingleFolderDialog();

        ImageInfoService.File = file;

        return file != null ? file : default;
    }

    private bool IsGoodFaceForDetectedFace(DetectedFace detectedFace, PersonInfo person)
    {
        return person.DetectedFace.FaceRectangle.Height == detectedFace.FaceRectangle.Height && person.DetectedFace.FaceRectangle.Left == detectedFace.FaceRectangle.Left && person.DetectedFace.FaceRectangle.Top == detectedFace.FaceRectangle.Top && person.DetectedFace.FaceRectangle.Width == detectedFace.FaceRectangle.Width;
    }

    private double IdentifyPerson(IdentifyResult identifyResult, DetectedFace detectedFace, PersonGroup personGroup, double latestConfidence)
    {
        if (identifyResult.Candidates.Count > 0 && latestConfidence < identifyResult.Candidates[0].Confidence)
        {
            latestConfidence = identifyResult.Candidates[0].Confidence;

            if (ImageInfoService.People.Count > 0 && ImageInfoService.People.Any(x => IsGoodFaceForDetectedFace(detectedFace, x)))
                ImageInfoService.People.Remove(ImageInfoService.People.First(x => x.DetectedFace.FaceRectangle.Height == detectedFace.FaceRectangle.Height && x.DetectedFace.FaceRectangle.Left == detectedFace.FaceRectangle.Left && x.DetectedFace.FaceRectangle.Top == detectedFace.FaceRectangle.Top && x.DetectedFace.FaceRectangle.Width == detectedFace.FaceRectangle.Width));
            ImageInfoService.People.Add(new() { DetectedFace = detectedFace, IdentifyResult = identifyResult, Group = personGroup });
        }
        else if (ImageInfoService.People.Count > 0 && ImageInfoService.People.Any(x => IsGoodFaceForDetectedFace(detectedFace, x) && x.IdentifyResult == null))
        {
            ImageInfoService.People.Remove(ImageInfoService.People.First(x => IsGoodFaceForDetectedFace(detectedFace, x)));

            ImageInfoService.People.Add(new() { DetectedFace = detectedFace });
        }
        else if (!ImageInfoService.People.Any(x => IsGoodFaceForDetectedFace(detectedFace, x)))
            ImageInfoService.People.Add(new() { DetectedFace = detectedFace });

        return latestConfidence;
    }
}
