using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Contracts.ViewModels;
using CognitiveServices.Explorer.Core.Helpers;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Core.Services;
using CognitiveServices.Explorer.Helpers;
using CognitiveServices.Explorer.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitiveServices.Explorer.ViewModels
{
    public class FacePeopleViewModel : ObservableRecipient, INavigationAware
    {
        private bool isLoading;

        private readonly IAddGroupDialogService addGroupDialogService;

        private readonly IFaceClientService faceClientService;

        private FaceProcessorService faceProcessor;

        private IList<PersonGroupWithUserData> personGroups;

        private PersonGroup selected;

        private string groupDescription;
        private string groupName;
        private string loadingText;
        private string recognictionModelSelected;

        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        public PersonGroup Selected
        {
            get { return selected; }
            set { SetProperty(ref selected, value); }
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

        public ICommand RemoveGroupCommand
        {
            get { return new RelayCommand(RemoveGroup); }
        }

        public IList<PersonGroupWithUserData> PersonGroups
        {
            get { return personGroups; }
            set { SetProperty(ref personGroups, value); }
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

                addGroupDialogService.SetPrimaryButtonState(!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(recognictionModelSelected));
            }
        }

        public string LoadingText
        {
            get { return loadingText; }
            set { SetProperty(ref loadingText, value); }
        }

        public string RecognictionModelSelected
        {
            get { return recognictionModelSelected; }

            set
            {
                SetProperty(ref recognictionModelSelected, value);

                addGroupDialogService.SetPrimaryButtonState(!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(recognictionModelSelected));
            }
        }

        public Dictionary<string, string> RecognitionModels
        {
            get => new() { { "recognition_01", "Version 1" }, { "recognition_02", "Version 2" }, { "recognition_03", "Version 3" }, { "recognition_04", "Version 4" } };
        }

        public FacePeopleViewModel(IAddGroupDialogService addGroupDialogService, IFaceClientService faceClientService)
        {
            this.addGroupDialogService = addGroupDialogService;
            this.faceClientService = faceClientService;

            faceProcessor = new FaceProcessorService(faceClientService.GetFaceClient());
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
            await addGroupDialogService.ShowAsync();
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

        private async Task GetGroups()
        {
            IsLoading = true;

            LoadingText = "Face_Group_GetGroups".GetLocalized();

            PersonGroups = await faceProcessor.GetPersonGroupsAsync();

            IsLoading = false;
        }

        private async void RemoveGroup()
        {
            await faceProcessor.DeletePersonGroupAsync(Selected.PersonGroupId);

            await GetGroups();
        }
    }
}
