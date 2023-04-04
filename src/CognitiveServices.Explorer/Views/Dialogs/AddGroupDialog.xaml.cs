using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views.Dialogs;

public sealed partial class AddGroupDialog : ContentDialog
{
    public Dictionary<string, string> RecognitionModels => new() { { "recognition_01", "Version 1" }, { "recognition_02", "Version 2" }, { "recognition_03", "Version 3" }, { "recognition_04", "Version 4" } };

    public string GroupDescription;
    public string GroupName;
    public string SelectedRecognictionModel;

    public AddGroupDialog()
    {
        InitializeComponent();

        GroupDescription = "";
        GroupName = "";
        SelectedRecognictionModel = RecognitionModels.Keys.Last();
    }

    private void GroupNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = !string.IsNullOrEmpty(GroupName);
    }
}
