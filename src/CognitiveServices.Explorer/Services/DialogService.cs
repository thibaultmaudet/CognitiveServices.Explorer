using CognitiveServices.Explorer.Contracts.Services;

using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Services;

public class DialogService : IDialogService
{
    private ContentDialog contentDialog;

    public DialogService()
    {
        contentDialog = new();
    }

    public void SetPrimaryButtonState(bool isEnabled)
    {
        contentDialog.IsPrimaryButtonEnabled = isEnabled;
    }

    public async Task<ContentDialogResult> ShowAsync(ContentDialog dialog)
    {
        dialog.XamlRoot = App.MainWindow.Content.XamlRoot;

        contentDialog = dialog;

        return await dialog.ShowAsync();
    }
}
