using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CognitiveServices.Explorer.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace CognitiveServices.Explorer.ViewModels
{
    public class PictureAnalyseViewModel : ObservableRecipient
    {
        private StorageFile openedFile;

        public StorageFile OpenedImage
        {
            get { return openedFile; }
            set { SetProperty(ref openedFile, value); }
        }

        public ICommand OpenLocalImageCommand
        {
            get { return new RelayCommand(OpenLocalImage); }
        }
        
        public ICommand GetStorageItemsCommand
        {
            get { return new RelayCommand<IReadOnlyList<IStorageItem>>(GetStorageItems); }
        }

        public PictureAnalyseViewModel()
        {

        }

        private async void OpenLocalImage()
        {
            FileOpenPicker openPicker = new() { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            WinUIConversionHelper.InitFileOpenPicker(openPicker);

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
                OpenedImage = file;
        }

        private void GetStorageItems(IReadOnlyList<IStorageItem> items)
        {
            OpenedImage = items[0] as StorageFile;
        }
    }
}
