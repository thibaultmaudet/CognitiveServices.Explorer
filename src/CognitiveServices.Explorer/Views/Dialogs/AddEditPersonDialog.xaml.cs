using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Helpers;

using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views.Dialogs;

public sealed partial class AddEditPersonDialog : ContentDialog
{
    public PersonWithUserData PersonWithUserData { get; set; }

    public AddEditPersonDialog(PersonWithUserData personWithUserData)
    {
        InitializeComponent();

        PersonWithUserData = personWithUserData;

        IsPrimaryButtonEnabled = !string.IsNullOrEmpty(PersonWithUserData.Name);

        Title = IsPrimaryButtonEnabled ? "EditPerson".GetLocalized() : "AddPerson".GetLocalized();
    }

    private void PersonName_TextChanged(object sender, TextChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = !string.IsNullOrEmpty(PersonWithUserData.Name);
    }
}
