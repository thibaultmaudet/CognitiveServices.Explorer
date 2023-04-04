using CognitiveServices.Explorer.Core.Models;

using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;

namespace CognitiveServices.Explorer.Models;

public class ExtendedPerson : PersonWithUserData
{
    public Contact Contact { get; }

    public ExtendedPerson(PersonWithUserData personWithUserData) : base(personWithUserData)
    {
        Contact = new() { Name = Name };

        if (UserData != null && !string.IsNullOrEmpty(UserData.ImageUrl))
            Contact.SourceDisplayPicture = RandomAccessStreamReference.CreateFromUri(new(UserData.ImageUrl));
    }
}
