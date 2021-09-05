using CognitiveServices.Explorer.Core.Models;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;

namespace CognitiveServices.Explorer.Models
{
    public class ExtendedPerson : PersonWithUserData
    {
        private Contact contact;

        public Contact Contact { get => contact; }

        public ExtendedPerson(PersonWithUserData personWithUserData) : base(personWithUserData)
        {
            contact = new() { Name = Name };

            if (UserData != null && !string.IsNullOrEmpty(UserData.PictureUrl))
                contact.SourceDisplayPicture = RandomAccessStreamReference.CreateFromUri(new(UserData.PictureUrl));
        }
    }
}
