using System.Collections.ObjectModel;
using System.Globalization;
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

using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.ViewModels;

public class FacePeopleViewModel : ObservableRecipient, INavigationAware
{
    private bool isLoading;

    private readonly IDialogService dialogService;

    private readonly FaceProcessorService faceProcessor;

    private IList<PersonGroupWithUserData> personGroups;
    private IList<ExtendedPerson> people;

    private ObservableCollection<ExtendedPerson> filteredPeople;

    private PersonGroupWithUserData selectedPersonGroup;

    private string filterQuery;
    private string loadingText;

    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    public PersonGroupWithUserData SelectedPersonGroup
    {
        get => selectedPersonGroup;
        set => SetProperty(ref selectedPersonGroup, value);
    }

    public ICommand AddGroupCommand => new RelayCommand(AddGroup);

    public ICommand AddPersonCommand => new RelayCommand(AddPerson);

    public ICommand GetPeopleCommand => new AsyncRelayCommand(GetPeople);

    public ICommand PeopleFilterTextChangedEventCommand => new RelayCommand(PeopleFilterTextChangedEvent);

    public ICommand RemoveGroupCommand => new RelayCommand(RemoveGroup);

    public ICommand TrainGroupCommand => new RelayCommand(TrainGroup);

    public IList<PersonGroupWithUserData> PersonGroups
    {
        get => personGroups;
        set => SetProperty(ref personGroups, value);
    }

    public IList<ExtendedPerson> People
    {
        get => people;
        set => SetProperty(ref people, value);
    }

    public ObservableCollection<ExtendedPerson> FilteredPeople
    {
        get => filteredPeople;
        set => SetProperty(ref filteredPeople, value);
    }

    public string FilterQuery
    {
        get => filterQuery;
        set => SetProperty(ref filterQuery, value);
    }

    public string LoadingText
    {
        get => loadingText;
        set => SetProperty(ref loadingText, value);
    }

    public FacePeopleViewModel(IDialogService dialogService, IFaceClientService faceClientService)
    {
        this.dialogService = dialogService;

        faceProcessor = new FaceProcessorService(faceClientService.FaceClient);

        personGroups = new List<PersonGroupWithUserData>();

        people = new List<ExtendedPerson>();

        filteredPeople = new();

        selectedPersonGroup = new(new());

        filterQuery = "";
        loadingText = "";
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
        AddGroupDialog addGroupDialog = new();

        ContentDialogResult result = await dialogService.ShowAsync(addGroupDialog);

        if (result == ContentDialogResult.Primary)
        {
            if (!string.IsNullOrEmpty(addGroupDialog.GroupDescription))
                await faceProcessor.CreatePersonGroupAsync(Guid.NewGuid().ToString().Trim('{').Trim('}'), addGroupDialog.GroupName, addGroupDialog.SelectedRecognictionModel, await Json.StringifyAsync(new UserData() { Description = addGroupDialog.GroupDescription }));
            else
                await faceProcessor.CreatePersonGroupAsync(Guid.NewGuid().ToString().Trim('{').Trim('}'), addGroupDialog.GroupName, addGroupDialog.SelectedRecognictionModel);

            await GetGroups();
        }
    }
    
    private async void AddPerson()
    {
        AddEditPersonDialog addEditPersonDialog = new(new(new()));

        ContentDialogResult result = await dialogService.ShowAsync(addEditPersonDialog);

        if (result == ContentDialogResult.Primary)
        {
            PersonWithUserData personWithUserData = addEditPersonDialog.PersonWithUserData;

            if (!string.IsNullOrEmpty(personWithUserData.UserData.ImageUrl) && Uri.IsWellFormedUriString(personWithUserData.UserData.ImageUrl, UriKind.Absolute))
                await faceProcessor.CreatePersonAsync(SelectedPersonGroup.PersonGroupId, personWithUserData.Name, await Json.StringifyAsync(personWithUserData.UserData));
            else
                await faceProcessor.CreatePersonAsync(SelectedPersonGroup.PersonGroupId, personWithUserData.Name);

            await GetPeople();
        }
    }

    public async Task EditPerson(ExtendedPerson extendedPerson)
    {
        AddEditPersonDialog addEditPersonDialog = new AddEditPersonDialog(extendedPerson);

        ContentDialogResult result = await dialogService.ShowAsync(addEditPersonDialog);

        if (result == ContentDialogResult.Primary)
        {
            PersonWithUserData personWithUserData = addEditPersonDialog.PersonWithUserData;

            if (!string.IsNullOrEmpty(personWithUserData.UserData.ImageUrl) && Uri.IsWellFormedUriString(personWithUserData.UserData.ImageUrl, UriKind.Absolute))
                await faceProcessor.UpdatePersonAsync(SelectedPersonGroup.PersonGroupId, personWithUserData.PersonId, personWithUserData.Name, await Json.StringifyAsync(personWithUserData.UserData));
            else
                await faceProcessor.UpdatePersonAsync(SelectedPersonGroup.PersonGroupId, personWithUserData.PersonId, personWithUserData.Name);

            await GetPeople();
        }
    }

    private async Task GetGroups()
    {
        IsLoading = true;

        LoadingText = "Face_Group_GetGroups".GetLocalized();

        PersonGroups = await faceProcessor.GetPersonGroupsAsync();

        IsLoading = false;
    }
    
    public async void TrainGroup()
    {
        IsLoading = true;

        LoadingText = "Face_People_TrainingOnGoing".GetLocalized();

        await faceProcessor.TrainingGroupAsync(SelectedPersonGroup.PersonGroupId);

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

        if (string.IsNullOrEmpty(FilterQuery))
            FilteredPeople = new(People);
        else
            PeopleFilterTextChangedEvent();

        IsLoading = false;
    }

    private async void RemoveGroup()
    {
        await faceProcessor.DeletePersonGroupAsync(SelectedPersonGroup.PersonGroupId);

        await GetGroups();
    }
    
    public async Task RemovePerson(ExtendedPerson extendedPerson)
    {
        await faceProcessor.DeletePersonAsync(SelectedPersonGroup.PersonGroupId, extendedPerson.PersonId);

        await GetPeople();
    }

    private void PeopleFilterTextChangedEvent()
    {
        FilteredPeople = new(from person in People orderby person.Name where new CultureInfo("fr-FR").CompareInfo.IndexOf(person.Name, FilterQuery, CompareOptions.IgnoreCase) >= 0 select person);
    }
}
