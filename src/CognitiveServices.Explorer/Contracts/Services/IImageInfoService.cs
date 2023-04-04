using System.Collections.ObjectModel;
using System.ComponentModel;

using CognitiveServices.Explorer.Core.Models;

using Windows.Storage;

namespace CognitiveServices.Explorer.Contracts.Services;

public interface IImageInfoService
{
    ObservableCollection<PersonInfo> People { get; set; }

    StorageFile? File { get; set; }

    event PropertyChangedEventHandler PropertyChanged;
}
