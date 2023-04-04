using CognitiveServices.Explorer.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views;

public sealed partial class FacePeoplePage : Page
{
    public FacePeopleViewModel ViewModel { get; }

    public FacePeoplePage()
    {
        ViewModel = App.GetService<FacePeopleViewModel>();
        InitializeComponent();
    }
}
