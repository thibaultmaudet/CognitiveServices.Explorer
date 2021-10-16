﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Contracts.ViewModels;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Core.Services;
using CognitiveServices.Explorer.Helpers;
using CognitiveServices.Explorer.Models;
using CognitiveServices.Explorer.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        private string loadingText;

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

        public ICommand AddPersonCommand
        {
            get { return new RelayCommand(AddPerson); }
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

        public string LoadingText
        {
            get { return loadingText; }
            set { SetProperty(ref loadingText, value); }
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

            await GetGroups();
        }
        
        private async void AddPerson()
        {
            await dialogService.ShowAsync(new AddEditPersonDialog(SelectedPersonGroup, new(new())));

            await GetPeople();
        }

        public async Task EditPerson(ExtendedPerson extendedPerson)
        {
            await dialogService.ShowAsync(new AddEditPersonDialog(SelectedPersonGroup, extendedPerson));

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
