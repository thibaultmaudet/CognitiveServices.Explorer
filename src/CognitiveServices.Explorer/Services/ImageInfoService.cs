using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace CognitiveServices.Explorer.Services
{
    public class ImageInfoService : IImageInfoService, INotifyPropertyChanged
    {
        private ObservableCollection<PersonInfo> people;

        private StorageFile filePath;

        public StorageFile FilePath
        {
            get { return filePath; }
            set
            {
                SetProperty(ref filePath, value);

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
            set
            {
                SetProperty(ref people, value);
            }
        }

        private void People_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(People));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return;

            storage = value;
            OnPropertyChanged(propertyName);
        }
    }
}
