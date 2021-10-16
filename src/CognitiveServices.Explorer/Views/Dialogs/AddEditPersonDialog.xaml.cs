using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.ViewModels.Dialogs;

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views.Dialogs
{
    public sealed partial class AddEditPersonDialog : ContentDialog
    {
        public AddEditPersonViewModel ViewModel { get; }

        public AddEditPersonDialog(PersonGroupWithUserData personGroup, PersonWithUserData person)
        {
            ViewModel = Ioc.Default.GetService<AddEditPersonViewModel>();
            ViewModel.PersonGroup = personGroup;
            ViewModel.Person = person;

            InitializeComponent();
        }
    }
}
