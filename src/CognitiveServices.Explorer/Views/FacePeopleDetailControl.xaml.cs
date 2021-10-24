using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Models;
using CognitiveServices.Explorer.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CognitiveServices.Explorer.Views
{
    public sealed partial class FacePeopleDetailControl : UserControl
    {
        public FacePeopleViewModel ViewModel { get; }

        public PersonGroupWithUserData PersonGroup
        {
            get { return GetValue(PersonGroupProperty) as PersonGroupWithUserData; }
            set { SetValue(PersonGroupProperty, value); }
        }

        public static readonly DependencyProperty PersonGroupProperty = DependencyProperty.Register("PersonGroup", typeof(PersonGroupWithUserData), typeof(FacePeopleDetailControl), new PropertyMetadata(null, OnPersonGroupPropertyChanged));

        public FacePeopleDetailControl()
        {
            ViewModel = Ioc.Default.GetService<FacePeopleViewModel>();
            InitializeComponent();
        }

        private static async void OnPersonGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FacePeopleDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);

            control.ViewModel.SelectedPersonGroup = e.NewValue as PersonGroupWithUserData;

            control.ViewModel.FilterQuery = "";

            await control.ViewModel.GetPeople();
        }

        private async void EditPerson_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.EditPerson((sender as Button).CommandParameter as ExtendedPerson);
        }

        private void RemovePerson_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemovePerson((sender as Button).CommandParameter as ExtendedPerson);
        }
    }
}
