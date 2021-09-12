using CognitiveServices.Explorer.Core.Helpers;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Core.Models
{
    public class PersonWithUserData : Person, INotifyPropertyChanged
    {
        private UserData userData;

        public new UserData UserData
        {
            get
            {
                if (userData == null)
                    userData = new();

                if (!string.IsNullOrEmpty(base.UserData))
                {
                    Task task = Task.Run(async () =>
                    {
                        userData = await Json.ToObjectAsync<UserData>(base.UserData);
                    });

                    task.Wait();
                }

                return userData;
            }
            set
            {
                Task.Run(async () =>
                {
                    base.UserData = await Json.StringifyAsync(value);

                    SetProperty(ref userData, value);
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PersonWithUserData(Person person) : base(person.PersonId, person.Name, person.UserData, person.PersistedFaceIds)
        {
        }

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
