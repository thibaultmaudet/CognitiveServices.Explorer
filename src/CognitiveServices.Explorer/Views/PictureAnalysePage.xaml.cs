using CognitiveServices.Explorer.ViewModels;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views
{
    public sealed partial class PictureAnalysePage : Page
    {
        public PictureAnalyseViewModel ViewModel { get; }

        public PictureAnalysePage()
        {
            ViewModel = Ioc.Default.GetService<PictureAnalyseViewModel>();
            InitializeComponent();
        }
    }
}
