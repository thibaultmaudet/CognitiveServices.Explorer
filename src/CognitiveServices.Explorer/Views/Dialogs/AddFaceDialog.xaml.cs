using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Core.Services;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views.Dialogs;

public sealed partial class AddFaceDialog : ContentDialog
{
    private readonly Dictionary<PersonWithUserData, PersonGroupWithUserData> people;

    private readonly FaceProcessorService faceProcessor;

    public KeyValuePair<PersonWithUserData, PersonGroupWithUserData> SelectedPerson;

    public AddFaceDialog(IFaceClientService faceClientService)
    {
        InitializeComponent();

        faceProcessor = new FaceProcessorService(faceClientService.FaceClient);

        people = new Dictionary<PersonWithUserData, PersonGroupWithUserData>();
    }

    private void PeopleAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        KeyValuePair<PersonWithUserData, PersonGroupWithUserData> person = people.FirstOrDefault(x => x.Key.Name.Equals(args.SelectedItem.ToString(), StringComparison.CurrentCultureIgnoreCase));

        if (!person.Equals(default(KeyValuePair<PersonWithUserData, PersonGroupWithUserData>)))
        {
            SelectedPerson = person;

            IsPrimaryButtonEnabled = true;
        }
        else
        {
            IsPrimaryButtonEnabled = false;

            SelectedPerson = default;
        }
    }

    private void PeopleAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var items = new List<string>();
            var splitText = sender.Text.ToLower().Split(" ");
            
            foreach (var name in people.Keys.Select(person => person.Name))
            {
                var found = splitText.All((key) =>
                {
                    return name.ToLower().Contains(key);
                });

                if (found)
                    items.Add(name);
            }

            if (items.Count == 0)
                items.Add("Aucun résultat trouvé");

            sender.ItemsSource = items;
        }

    }

    private async void ContentDialog_Loading(FrameworkElement sender, object args)
    {
        foreach (PersonGroupWithUserData group in await faceProcessor.GetPersonGroupsAsync())
            foreach (PersonWithUserData person in await faceProcessor.GetPeopleAsync(group.PersonGroupId))
                people.Add(person, group);
    }
}
