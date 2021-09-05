using CognitiveServices.Explorer.Core.Helpers;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Core.Models
{
    public class PersonWithUserData : Person
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
            set
            {
                Task.Run(async () =>
                {
                    base.UserData = await Json.StringifyAsync(value);
                });
            }
        }

        public PersonWithUserData(Person person) : base(person.PersonId, person.Name, person.UserData, person.PersistedFaceIds) { }
    }
}
