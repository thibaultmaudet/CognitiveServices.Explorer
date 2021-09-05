using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.ViewModels;

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views.Dialogs
{
    public sealed partial class AddPersonDialog : ContentDialog
    {
        public FacePeopleViewModel ViewModel { get; }

        public AddPersonDialog(PersonGroupWithUserData personGroup)
        {
            ViewModel = Ioc.Default.GetService<FacePeopleViewModel>();
            ViewModel.SelectedPersonGroup = personGroup;
            InitializeComponent();
        }
    }
}
