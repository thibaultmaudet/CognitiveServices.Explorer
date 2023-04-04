using CognitiveServices.Explorer.Contracts.Services;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace CognitiveServices.Explorer.Services;

public class PickDialogService : IPickDialogService
{
    public async Task<StorageFile> PickSingleFolderDialog()
    {
        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(new MainWindow());

        FileOpenPicker openPicker = new() { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary };
        openPicker.FileTypeFilter.Add(".jpg");
        openPicker.FileTypeFilter.Add(".jpeg");
        openPicker.FileTypeFilter.Add(".png");

        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

        return await openPicker.PickSingleFileAsync();
    }
}
