using CognitiveServices.Explorer.Contracts.Services;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Services
{
    public class DialogService : IDialogService
    {
        private ContentDialog contentDialog;

        public void SetPrimaryButtonState(bool isEnabled)
        {
            contentDialog.IsPrimaryButtonEnabled = isEnabled;
        }

        public async Task ShowAsync(ContentDialog dialog)
        {
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;

            contentDialog = dialog;

            await dialog.ShowAsync();
        }
    }
}
