using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.ViewModels;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CognitiveServices.Explorer.Views.Dialogs
{
    public sealed partial class EditPersonDialog : ContentDialog, INotifyPropertyChanged
    {
        private PersonWithUserData personWithUserData;

        public FacePeopleViewModel ViewModel { get; }

        public PersonWithUserData PersonWithUserData { get => personWithUserData; set => Set(ref personWithUserData, value); }

        public event PropertyChangedEventHandler PropertyChanged;

        public EditPersonDialog(PersonGroupWithUserData personGroupWithUserData,  PersonWithUserData personWithUserData)
        {
            ViewModel = Ioc.Default.GetService<FacePeopleViewModel>();
            ViewModel.SelectedPersonGroup = personGroupWithUserData;
            InitializeComponent();

            PersonWithUserData = personWithUserData;
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return;

            storage = value;
            OnPropertyChanged(propertyName);
        }
    }
}
