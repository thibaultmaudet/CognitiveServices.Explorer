using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CognitiveServices.Explorer.Core.Models
{
    public class PersonInfo : INotifyPropertyChanged
    {
        private DetectedFace detectedFace;


        public DetectedFace DetectedFace
        {
            get => detectedFace;
            set
            {
                detectedFace = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
