using CognitiveServices.Explorer.Core.Helpers;

using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitiveServices.Explorer.Core.Models;

public class PersonGroupWithUserData : PersonGroup
{
    public new UserData UserData
    {
        get
        {
            if (string.IsNullOrEmpty(base.UserData))
                return default;

            UserData userData = new();

            Task task = Task.Run(async () =>
            {
                userData = await Json.ToObjectAsync<UserData>(base.UserData);
            });

            task.Wait();

            return userData;
        }
        set => Task.Run(async () =>
                            {
                                base.UserData = await Json.StringifyAsync(value);
                            });
    }

    public PersonGroupWithUserData(PersonGroup personGroup) : base(personGroup.PersonGroupId, personGroup.Name, personGroup.UserData, personGroup.RecognitionModel) { }
}
