using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            get { return detectedFace; }
            set { SetProperty(ref detectedFace, value); }
        }

        public IdentifyResult IdentifyResult
        {
            get { return identifyResult; }
            set { SetProperty(ref identifyResult, value); }
        }
        
        public PersonGroup Group
        {
            get { return group; }
            set { SetProperty(ref group, value); }
        }

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
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
