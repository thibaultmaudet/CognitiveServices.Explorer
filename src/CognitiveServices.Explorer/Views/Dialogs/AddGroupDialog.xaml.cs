using CognitiveServices.Explorer.ViewModels;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views.Dialogs
{
    public sealed partial class AddGroupDialog : ContentDialog
    {
        public FacePeopleViewModel ViewModel { get; }

        public AddGroupDialog()
        {
            ViewModel = Ioc.Default.GetService<FacePeopleViewModel>();
            InitializeComponent();
        }
    }
}
