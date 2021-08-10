using CognitiveServices.Explorer.ViewModels;

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI.UI.Controls;

using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views
{
    public sealed partial class FacePeoplePage : Page
    {
        public FacePeopleViewModel ViewModel { get; }

        public FacePeoplePage()
        {
            ViewModel = Ioc.Default.GetService<FacePeopleViewModel>();
            InitializeComponent();
        }
    }
}
