using CognitiveServices.Explorer.ViewModels.Dialogs;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views.Dialogs
{
    public sealed partial class AddGroupDialog : ContentDialog
    {
        public AddGroupViewModel ViewModel { get; }

        public AddGroupDialog()
        {
            ViewModel = Ioc.Default.GetService<AddGroupViewModel>();
            InitializeComponent();
        }
    }
}
