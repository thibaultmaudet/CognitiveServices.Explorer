using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

using Windows.Storage;

namespace CognitiveServices.Explorer.Services;

public class ImageInfoService : ObservableRecipient, IImageInfoService
{
    private ObservableCollection<PersonInfo>? people;

    private StorageFile? file;

    public StorageFile? File
    {
        get => file;
        set
        {
            SetProperty(ref file, value);

            People.Clear();
        }
    }

    public ObservableCollection<PersonInfo> People
    {
        get
        {
            if (people == null)
            {
                people = new();

                people.CollectionChanged += People_CollectionChanged;
            }

            return people;

        }
        set => SetProperty(ref people, value);
    }

    private void People_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(People));
    }
}
