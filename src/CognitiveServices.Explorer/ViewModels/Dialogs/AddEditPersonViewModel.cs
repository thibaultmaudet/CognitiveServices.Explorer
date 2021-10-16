using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Core.Helpers;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CognitiveServices.Explorer.ViewModels.Dialogs
{
    public class AddEditPersonViewModel : ObservableRecipient
    {
        private bool enabledPrimaryButton;

        private FaceProcessorService faceProcessor;

        private PersonGroupWithUserData personGroup;

        private PersonWithUserData person;

        private string title;

        public bool EnabledPrimaryButton
        {
            get { return enabledPrimaryButton; }
            set { SetProperty(ref enabledPrimaryButton, value); }
        }

        public ICommand TextChangedEventCommand
        {
            get { return new RelayCommand(TextChangedEvent); }
        }

        public ICommand ValidateCommand
        {
            get { return new RelayCommand(Validate); }
        }

        public PersonGroupWithUserData PersonGroup
        {
            get { return personGroup; }
            set { SetProperty(ref personGroup, value); }
        }

        public PersonWithUserData Person
        {
            get { return person; }
            set { SetProperty(ref person, value); }
        }

        public string Title
        {
            get { return !string.IsNullOrEmpty(Person.Name) ? "EditPerson".GetLocalized() : "AddPerson".GetLocalized(); }
        }

        public AddEditPersonViewModel(IFaceClientService faceClientService)
        {
            faceProcessor = new(faceClientService.FaceClient);
        }

        private void TextChangedEvent()
        {
            EnabledPrimaryButton = !string.IsNullOrEmpty(person.Name);
        }

        private async void Validate()
        {
            if (person.PersonId == default)
            {
                if (!string.IsNullOrEmpty(person.UserData.PictureUrl) && Uri.IsWellFormedUriString(person.UserData.PictureUrl, UriKind.Absolute))
                    await faceProcessor.CreatePersonAsync(PersonGroup.PersonGroupId, person.Name, await Json.StringifyAsync(person.UserData));
                else
                    await faceProcessor.CreatePersonAsync(PersonGroup.PersonGroupId, person.Name);
            }
            else
            {
                if (!string.IsNullOrEmpty(person.UserData.PictureUrl) && Uri.IsWellFormedUriString(person.UserData.PictureUrl, UriKind.Absolute))
                    await faceProcessor.UpdatePersonAsync(PersonGroup.PersonGroupId, person.PersonId, person.Name, await Json.StringifyAsync(person.UserData));
                else
                    await faceProcessor.UpdatePersonAsync(PersonGroup.PersonGroupId, person.PersonId, person.Name);
            }
        }
    }
}
