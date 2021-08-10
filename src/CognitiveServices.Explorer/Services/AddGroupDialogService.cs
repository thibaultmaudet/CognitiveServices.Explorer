using CognitiveServices.Explorer.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Services
{
    public interface IAddGroupDialogService
    {
        void SetPrimaryButtonState(bool isEnabled);

        Task ShowAsync();
    }

    public class AddGroupDialogService : IAddGroupDialogService
    {
        AddGroupDialog addGroupDialog;

        public async Task ShowAsync()
        {
            if (addGroupDialog == null)
                addGroupDialog = new() { XamlRoot = App.MainWindow.Content.XamlRoot };

            await addGroupDialog.ShowAsync();
        }

        public void SetPrimaryButtonState(bool isEnabled)
        {
            if (addGroupDialog != null)
                addGroupDialog.IsPrimaryButtonEnabled = isEnabled;
        }
    }
}
