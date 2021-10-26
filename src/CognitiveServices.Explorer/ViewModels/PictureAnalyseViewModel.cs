using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Core.Services;
using CognitiveServices.Explorer.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CognitiveServices.Explorer.ViewModels
{
    public class PictureAnalyseViewModel : ObservableRecipient
    {
        private IDialogService dialogService;

        private IImageInfoService imageInfoService;

        private IList<PersonGroupWithUserData> personGroups;

        private readonly FaceProcessorService faceProcessor;

        public IImageInfoService ImageInfoService
        {
            get { return imageInfoService; }
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

                void ImageInfoServiceChanged(object sender, PropertyChangedEventArgs args) => OnPropertyChanged(nameof(ImageInfoService));
            }
        }

        public ICommand AddFaceCommand
        {
            get { return new RelayCommand(AddFaceAsync); }
        }

        public PictureAnalyseViewModel(IDialogService dialogService, IFaceClientService faceClientService, IImageInfoService imageInfoService)
        {
            this.dialogService = dialogService;

            faceProcessor = new FaceProcessorService(faceClientService.FaceClient);

            this.ImageInfoService = imageInfoService;

            Task.Run(async () =>
            {
                personGroups = await faceProcessor.GetPersonGroupsAsync();
            });
        }

        public async void AddFaceAsync()
        {
            IRandomAccessStreamWithContentType randomAccessStream = await ImageInfoService.FilePath.OpenReadAsync();

            await dialogService.ShowAsync(new AddFaceDialog(randomAccessStream.AsStreamForRead()));
        }

        public async Task StartFaceDetection()
        {
            IRandomAccessStreamWithContentType randomAccessStream = await ImageInfoService.FilePath.OpenReadAsync();
            Stream stream = randomAccessStream.AsStreamForRead();

            ImageInfoService.People.Clear();

            foreach (DetectedFace detectedFace in await faceProcessor.DetectFacesAsync(stream, true, null))
                ImageInfoService.People.Add(new() { DetectedFace = detectedFace });
        }

        public async Task StartFaceRecognitionAsync()
        {
            ImageInfoService.People.Clear();

            foreach (PersonGroup personGroup in personGroups)
            {
                IRandomAccessStreamWithContentType randomAccessStream = await ImageInfoService.FilePath.OpenReadAsync();
                Stream stream = randomAccessStream.AsStreamForRead();

                foreach (DetectedFace detectedFace in await faceProcessor.DetectFacesAsync(stream, true, null, personGroup.RecognitionModel))
                {
                    double latestConfidence = 0;

                    IList<IdentifyResult> identifyResults = await faceProcessor.IdentifyFaceAsync(new List<Guid>() { detectedFace.FaceId.Value }, personGroup.PersonGroupId);

                    if (identifyResults == null)
                        continue;

                    foreach (var identifyResult in identifyResults)
                        if (identifyResult.Candidates.Count > 0 && latestConfidence < identifyResult.Candidates[0].Confidence)
                        {
                            latestConfidence = identifyResult.Candidates[0].Confidence;

                            if (ImageInfoService.People.Count > 0 && ImageInfoService.People.First(x => x.DetectedFace == detectedFace) != null)
                                ImageInfoService.People.Remove(ImageInfoService.People.First(x => x.DetectedFace == detectedFace));
                                
                            ImageInfoService.People.Add(new() { DetectedFace = detectedFace, Group = personGroup, IdentifyResult = identifyResult });
                        }
                }
            }

            foreach (PersonInfo personInfo in ImageInfoService.People)
                personInfo.Name = (personInfo.IdentifyResult != null && personInfo.IdentifyResult.Candidates.Count > 0) ? await faceProcessor.GetPersonNameAsync(personInfo.Group.PersonGroupId, personInfo.IdentifyResult.Candidates[0].PersonId) : "<Inconnu>";
        }
    }
}
