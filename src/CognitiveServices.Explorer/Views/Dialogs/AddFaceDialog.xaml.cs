using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.ViewModels.Dialogs;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;
using System.IO;

namespace CognitiveServices.Explorer.Views.Dialogs
{
    public sealed partial class AddFaceDialog : ContentDialog
    {
        public AddFaceViewModel ViewModel { get; }

        public AddFaceDialog(Stream stream)
        {
            ViewModel = Ioc.Default.GetService<AddFaceViewModel>();
            ViewModel.ImageStream = stream;
            InitializeComponent();
        }
    }
}
