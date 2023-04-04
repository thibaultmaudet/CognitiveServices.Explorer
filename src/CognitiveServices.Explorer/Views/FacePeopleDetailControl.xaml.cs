using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Models;
using CognitiveServices.Explorer.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views;

public sealed partial class FacePeopleDetailControl : UserControl
{
    public FacePeopleViewModel ViewModel { get; }

    public PersonGroupWithUserData PersonGroup
    {
        get => (PersonGroupWithUserData)GetValue(PersonGroupProperty);
        set => SetValue(PersonGroupProperty, value);
    }

    public static readonly DependencyProperty PersonGroupProperty = DependencyProperty.Register("PersonGroup", typeof(PersonGroupWithUserData), typeof(FacePeopleDetailControl), new PropertyMetadata(null, OnPersonGroupPropertyChanged));

    public FacePeopleDetailControl()
    {
        ViewModel = App.GetService<FacePeopleViewModel>();
        InitializeComponent();
    }

    private static async void OnPersonGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as FacePeopleDetailControl;

        if (control == null)
            return;

        control.ForegroundElement.ChangeView(0, 0, 1);

        control.ViewModel.SelectedPersonGroup = (PersonGroupWithUserData)e.NewValue;

        await control.ViewModel.GetPeople();
    }

    private async void EditPerson_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.EditPerson((ExtendedPerson)((Button)sender).CommandParameter);
    }

    private async void RemovePerson_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.RemovePerson((ExtendedPerson)((Button)sender).CommandParameter);
    }
}
