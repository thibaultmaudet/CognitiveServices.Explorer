using CognitiveServices.Explorer.Core.Behaviors;
using CognitiveServices.Explorer.Core.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Core.Services
{
    public class FaceProcessorService
    {
        private readonly IFaceClient faceClient;

        public FaceProcessorService(IFaceClient faceClient)
        {
            this.faceClient = faceClient;
        }

        public async Task CreatePersonGroupAsync(string groupId, string groupName, string recognitionModel)
        {
            await CreatePersonGroupAsync(groupId, groupName, recognitionModel, "");
        }

        public async Task CreatePersonGroupAsync(string groupId, string groupName, string recognitionModel, string userData)
        {
            await faceClient.PersonGroup.CreateAsync(groupId, groupName, userData: userData, recognitionModel: recognitionModel);
        }

        public async Task DeletePersonGroupAsync(string groupId)
        {
            await faceClient.PersonGroup.DeleteAsync(groupId);
        }
        
        public async Task<IList<PersonGroupWithUserData>> GetPersonGroupsAsync()
        {
            return await GetPersonGroupsAsync(SortOrder.Ascending);
        }

        public async Task<IList<PersonGroupWithUserData>> GetPersonGroupsAsync(SortOrder sortOrder)
        {
            IList<PersonGroupWithUserData> personGroupWithUserDatas = new List<PersonGroupWithUserData>();

            foreach (PersonGroup personGroup in await faceClient.PersonGroup.ListAsync())
                personGroupWithUserDatas.Add(new(personGroup));

            if (sortOrder == SortOrder.Ascending)
                return personGroupWithUserDatas.OrderBy(x => x.Name).ToList();
            else if (sortOrder == SortOrder.Descending)
                return personGroupWithUserDatas.OrderByDescending(x => x.Name).ToList();

            return personGroupWithUserDatas;
        }
    }
}
