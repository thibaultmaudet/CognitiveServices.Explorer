using CognitiveServices.Explorer.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Storage;

namespace CognitiveServices.Explorer.Contracts.Services
{
    public interface IImageInfoService
    {
        ObservableCollection<PersonInfo> People { get; set; }

        StorageFile FilePath { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}
