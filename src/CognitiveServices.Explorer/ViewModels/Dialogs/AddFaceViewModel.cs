using CommunityToolkit.Mvvm.ComponentModel;
using CognitiveServices.Explorer.Core.Models;
using System.Collections.Generic;
using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Core.Services;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.IO;

namespace CognitiveServices.Explorer.ViewModels.Dialogs
{
    public class AddFaceViewModel : ObservableRecipient
    {
        private bool enabledPrimaryButton;

        private Dictionary<PersonWithUserData, PersonGroupWithUserData> people;

        private FaceProcessorService faceProcessor;

        private KeyValuePair<PersonWithUserData, PersonGroupWithUserData> selectedPerson;

        private List<string> filteredPeople;

        private Stream imageStream;
        
        public bool EnabledPrimaryButton
        {
            get { return enabledPrimaryButton; }
            set { SetProperty(ref enabledPrimaryButton, value); }
        }

        public Dictionary<PersonWithUserData, PersonGroupWithUserData> People
        {
            get { return people; }
            set { SetProperty(ref people, value); }
        }
        
        public KeyValuePair<PersonWithUserData, PersonGroupWithUserData> SelectedPerson
        {
            get { return selectedPerson; }
            set { SetProperty(ref selectedPerson, value); }
        }

        public ICommand ValidateCommand
        {
            get { return new RelayCommand(Validate); }
        }

        public ICommand SuggestionChosenEventCommand
        {
            get { return new RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs>(SuggestionChosenEvent); }
        }

        public ICommand TextChangedEventCommand
        {
            get { return new RelayCommand<string>(TextChangedEvent); }
        }

        public List<string> FilteredPeople
        {
            get { return filteredPeople; }
            set { SetProperty(ref filteredPeople, value); }
        }

        public Stream ImageStream
        {
            get { return imageStream; }
            set { SetProperty(ref imageStream, value); }
        }

        public AddFaceViewModel(IFaceClientService faceClientService)
        {
            faceProcessor = new FaceProcessorService(faceClientService.FaceClient);

            people = new Dictionary<PersonWithUserData, PersonGroupWithUserData>();

            Task.Run(async () =>
            {
                foreach (PersonGroupWithUserData group in await faceProcessor.GetPersonGroupsAsync())
                    foreach (PersonWithUserData person in await faceProcessor.GetPeopleAsync(group.PersonGroupId))
                        people.Add(person, group);
            });
        }

        private void TextChangedEvent(string text)
        {
            List<string> suggestions = SearchPeople(text);

            if (suggestions.Count > 0)
                FilteredPeople = suggestions;
            else
                FilteredPeople = new() { "Aucun résultat trouvé" };
        }
        
        private void SuggestionChosenEvent(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            KeyValuePair<PersonWithUserData, PersonGroupWithUserData> person = people.FirstOrDefault(x => x.Key.Name.Equals(args.SelectedItem.ToString(), StringComparison.CurrentCultureIgnoreCase));

            if (!person.Equals(default(KeyValuePair<PersonWithUserData, PersonGroupWithUserData>)))
            {
                SelectedPerson = person;

                EnabledPrimaryButton = true;
            }
            else
            {
                EnabledPrimaryButton = false;

                SelectedPerson = default;
            }
        }

        private List<string> SearchPeople(string query)
        {
            List<string> suggestions = new();

            string[] querySplit = query.Split("");

            IEnumerable<PersonWithUserData> matchingItems = people.Keys.Where(item =>
            {
                bool flag = true;
                foreach (string queryToken in querySplit)
                    if (item.Name.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                        flag = false;

                return flag;
            });

            foreach (var item in matchingItems)
                suggestions.Add(item.Name);

            return suggestions.OrderByDescending(i => i.StartsWith(query, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i).ToList();
        }

        private async void Validate()
        {
            await faceProcessor.AddFace(selectedPerson.Value.PersonGroupId, selectedPerson.Key.PersonId, imageStream);
        }
    }
}
