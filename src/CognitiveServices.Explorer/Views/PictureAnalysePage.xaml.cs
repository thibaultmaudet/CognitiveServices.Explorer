using CognitiveServices.Explorer.ViewModels;

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

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

        private void PictureImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutMenu?.ShowAt(PictureImage, new() { Position = new(PictureImage.ActualWidth / 2, 50), ShowMode = FlyoutShowMode.Transient });
        }
    }
}
