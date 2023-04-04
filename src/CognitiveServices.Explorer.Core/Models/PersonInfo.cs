using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitiveServices.Explorer.Core.Models
{
    public class PersonInfo : INotifyPropertyChanged
    {
        private DetectedFace detectedFace;

        private IdentifyResult identifyResult;

        private PersonGroup group;

        private string name;

        public DetectedFace DetectedFace
        {
            get => detectedFace;
            set => SetProperty(ref detectedFace, value);
        }

        public IdentifyResult IdentifyResult
        {
            get => identifyResult;
            set => SetProperty(ref identifyResult, value);
        }

        public PersonGroup Group
        {
            get => group;
            set => SetProperty(ref group, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
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
