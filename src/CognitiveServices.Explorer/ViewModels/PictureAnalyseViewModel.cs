using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using CognitiveServices.Explorer.Contracts.Services;
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

        private readonly IFaceClientService faceClientService;

        private IList<DetectedFace> detectedFaces;

        private readonly FaceProcessorService faceProcessor;

        private StorageFile openedFile;

        public IList<DetectedFace> DetectedFaces 
        {
            get { return detectedFaces; }
            set { SetProperty(ref detectedFaces, value); }
        }

        public StorageFile OpenedImage
        {
            get { return openedFile; }
            set { SetProperty(ref openedFile, value); }
        }
        
        public ICommand AddFaceCommand
        {
            get { return new RelayCommand(AddFaceAsync); }
        }

        public PictureAnalyseViewModel(IDialogService dialogService, IFaceClientService faceClientService)
        {
            this.dialogService = dialogService;

            this.faceClientService = faceClientService;

            faceProcessor = new FaceProcessorService(faceClientService.FaceClient);
        }

        public async void AddFaceAsync()
        {
            IRandomAccessStreamWithContentType randomAccessStream = await OpenedImage.OpenReadAsync();

            await dialogService.ShowAsync(new AddFaceDialog(randomAccessStream.AsStreamForRead()));
        }

        public async Task StartFaceDetection()
        {
            IRandomAccessStreamWithContentType randomAccessStream = await OpenedImage.OpenReadAsync();
            Stream stream = randomAccessStream.AsStreamForRead();

            DetectedFaces = await faceProcessor.DetectFacesAsync(stream, true, null);
        }
    }
}
