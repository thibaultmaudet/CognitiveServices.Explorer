using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Contracts.ViewModels;
using CognitiveServices.Explorer.Core.Helpers;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Core.Services;
using CognitiveServices.Explorer.Helpers;
using CognitiveServices.Explorer.Models;
using CognitiveServices.Explorer.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitiveServices.Explorer.ViewModels
{
    public class FacePeopleViewModel : ObservableRecipient, INavigationAware
    {
        private bool isLoading;

        private readonly IDialogService dialogService;

        private readonly IFaceClientService faceClientService;

        private FaceProcessorService faceProcessor;

        private IList<PersonGroupWithUserData> personGroups;
        private IList<ExtendedPerson> people;

        private PersonGroupWithUserData selectedPersonGroup;

        private string groupDescription;
        private string groupName;
        private string loadingText;
        private string recognictionModelSelected;
        private string personName;
        private string personPicture;

        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        public PersonGroupWithUserData SelectedPersonGroup
        {
            get { return selectedPersonGroup; }
            set
            {
                SetProperty(ref selectedPersonGroup, value);
            }
        }

        public ICommand AddGroupCommand
        {
            get { return new RelayCommand(AddGroup); }
        }

        public ICommand AddGroupDialogValidateCommand
        {
            get { return new RelayCommand(AddGroupDialogValidate); }
        }
        public ICommand AddGroupDialogCancelCommand
        {
            get { return new RelayCommand(AddGroupDialogCancel); }
        }

        public ICommand AddPersonCommand
        {
            get { return new RelayCommand(AddPerson); }
        }

        public ICommand AddPersonDialogValidateCommand
        {
            get { return new RelayCommand(AddPersonDialogValidate); }
        }

        public ICommand AddPersonDialogCancelCommand
        {
            get { return new RelayCommand(AddPersonDialogCancel); }
        }

        public ICommand EditPersonDialogValidateCommand
        {
            get { return new RelayCommand<PersonWithUserData>(EditPersonDialogValidate); }
        }

        public ICommand GetPeopleCommand
        {
            get { return new AsyncRelayCommand(GetPeople); }
        }

        public ICommand RemoveGroupCommand
        {
            get { return new RelayCommand(RemoveGroup); }
        }

        public IList<PersonGroupWithUserData> PersonGroups
        {
            get { return personGroups; }
            set { SetProperty(ref personGroups, value); }
        }

        public IList<ExtendedPerson> People
        {
            get { return people; }
            set { SetProperty(ref people, value); }
        }

        public string GroupDescription
        {
            get { return groupDescription; }
            set { SetProperty(ref groupDescription, value); }
        }

        public string GroupName
        {
            get { return groupName; }

            set
            {
                SetProperty(ref groupName, value);

                dialogService.SetPrimaryButtonState(!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(recognictionModelSelected));
            }
        }

        public string LoadingText
        {
            get { return loadingText; }
            set { SetProperty(ref loadingText, value); }
        }

        public string PersonName
        {
            get { return personName; }
            set
            {
                SetProperty(ref personName, value);

                dialogService.SetPrimaryButtonState(!string.IsNullOrEmpty(personName));
            }
        }
        
        public string PersonPicture
        {
            get { return personPicture; }
            set
            {
                SetProperty(ref personPicture, value);

                dialogService.SetPrimaryButtonState(!string.IsNullOrEmpty(personName));
            }
        }

        public string RecognictionModelSelected
        {
            get { return recognictionModelSelected; }

            set
            {
                SetProperty(ref recognictionModelSelected, value);

                dialogService.SetPrimaryButtonState(!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(recognictionModelSelected));
            }
        }

        public Dictionary<string, string> RecognitionModels
        {
            get => new() { { "recognition_01", "Version 1" }, { "recognition_02", "Version 2" }, { "recognition_03", "Version 3" }, { "recognition_04", "Version 4" } };
        }

        public FacePeopleViewModel(IDialogService dialogService, IFaceClientService faceClientService)
        {
            this.dialogService = dialogService;
            this.faceClientService = faceClientService;

            faceProcessor = new FaceProcessorService(faceClientService.FaceClient);
        }

        public async void OnNavigatedTo(object parameter)
        {
            await GetGroups();
        }

        public void OnNavigatedFrom()
        {
        }

        private async void AddGroup()
        {
            await dialogService.ShowAsync(new AddGroupDialog());
        }
        
        private void AddGroupDialogCancel()
        {
            GroupName = "";
            RecognictionModelSelected = "";
        }

        private async void AddGroupDialogValidate()
        {
            IsLoading = true;

            LoadingText = "Face_Group_AddGroup".GetLocalized();

            if (!string.IsNullOrEmpty(GroupDescription))
            {
                string userData = await Json.StringifyAsync(new UserData() { Description = GroupDescription });
                
                await faceProcessor.CreatePersonGroupAsync(Guid.NewGuid().ToString().Trim('{').Trim('}'), groupName, recognictionModelSelected, userData);
            }
            else
                await faceProcessor.CreatePersonGroupAsync(Guid.NewGuid().ToString().Trim('{').Trim('}'), groupName, recognictionModelSelected); List<int> i = new();

            GroupName = "";
            RecognictionModelSelected = "";

            IsLoading = false;

            await GetGroups();
        }
        
        private async void AddPerson()
        {
            await dialogService.ShowAsync(new AddPersonDialog(SelectedPersonGroup));
        }
        
        private void AddPersonDialogCancel()
        {
            GroupName = "";
            RecognictionModelSelected = "";
        }

        private async void AddPersonDialogValidate()
        {
            IsLoading = true;

            LoadingText = "Face_Group_AddPerson".GetLocalized();

            if (!string.IsNullOrEmpty(PersonPicture))
            {
                string userData = await Json.StringifyAsync(new UserData { PictureUrl = PersonPicture });

                await faceProcessor.CreatePersonAsync(SelectedPersonGroup.PersonGroupId, PersonName, userData);
            }
            else
                await faceProcessor.CreatePersonAsync(SelectedPersonGroup.PersonGroupId, PersonName);

            PersonName = "";

            IsLoading = false;

            await GetPeople();
        }

        public async Task EditPerson(ExtendedPerson extendedPerson)
        {
            await dialogService.ShowAsync(new EditPersonDialog(SelectedPersonGroup, extendedPerson));
        }

        private async void EditPersonDialogValidate(PersonWithUserData personWithUserData)
        {
            IsLoading = true;

            LoadingText = "Face_Person_UpdatePerson".GetLocalized();

            if (!string.IsNullOrEmpty(personWithUserData.UserData.PictureUrl))
            {
                string userData = await Json.StringifyAsync(new UserData { PictureUrl = personWithUserData.UserData.PictureUrl });

                await faceProcessor.UpdatePersonAsync(SelectedPersonGroup.PersonGroupId, personWithUserData.PersonId, personWithUserData.Name, userData);
            }
            else
                await faceProcessor.UpdatePersonAsync(SelectedPersonGroup.PersonGroupId, personWithUserData.PersonId, personWithUserData.Name);

            IsLoading = false;

            await GetPeople();
        }

        private async Task GetGroups()
        {
            IsLoading = true;

            LoadingText = "Face_Group_GetGroups".GetLocalized();

            PersonGroups = await faceProcessor.GetPersonGroupsAsync();

            IsLoading = false;
        }

        public async Task GetPeople()
        {
            IsLoading = true;

            LoadingText = "Face_Group_GetPeople".GetLocalized();

            IList<ExtendedPerson> extendedPeople = new List<ExtendedPerson>();

            foreach (PersonWithUserData personWithUserData in await faceProcessor.GetPeopleAsync(SelectedPersonGroup.PersonGroupId))
                extendedPeople.Add(new(personWithUserData));

            People = extendedPeople;

            IsLoading = false;
        }

        private async void RemoveGroup()
        {
            await faceProcessor.DeletePersonGroupAsync(SelectedPersonGroup.PersonGroupId);

            await GetGroups();
        }
        
        public async void RemovePerson(ExtendedPerson extendedPerson)
        {
            await faceProcessor.DeletePersonAsync(SelectedPersonGroup.PersonGroupId, extendedPerson.PersonId);

            await GetPeople();
        }
    }
}
