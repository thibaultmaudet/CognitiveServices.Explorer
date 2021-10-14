using CognitiveServices.Explorer.Core.Behaviors;
using CognitiveServices.Explorer.Core.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task CreatePersonAsync(string groupId, string personName)
        {
            await CreatePersonAsync(groupId, personName, "");
        }
        
        public async Task CreatePersonAsync(string groupId, string personName, string userData)
        {
            await faceClient.PersonGroupPerson.CreateAsync(groupId, personName, userData: userData);
        }

        public async Task DeletePersonGroupAsync(string groupId)
        {
            await faceClient.PersonGroup.DeleteAsync(groupId);
        }

        public async Task DeletePersonAsync(string groupId, Guid personId)
        {
            await faceClient.PersonGroupPerson.DeleteAsync(groupId, personId);
        }
        
        public async Task UpdatePersonAsync(string groupId, Guid personId, string personName)
        {
            await UpdatePersonAsync(groupId, personId, personName, "");
        }

        public async Task UpdatePersonAsync(string groupId, Guid personId, string personName, string userData)
        {
            await faceClient.PersonGroupPerson.UpdateAsync(groupId, personId, personName, userData);
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
        
        public async Task<IList<PersonWithUserData>> GetPeopleAsync(string groupId)
        {
            return await GetPeopleAsync(groupId, SortOrder.Ascending);
        }

        public async Task<IList<PersonWithUserData>> GetPeopleAsync(string groupId, SortOrder sortOrder)
        {
            IList<PersonWithUserData> peopleWithUserData = new List<PersonWithUserData>();

            foreach (Person person in await faceClient.PersonGroupPerson.ListAsync(groupId))
                peopleWithUserData.Add(new(person));

            if (sortOrder == SortOrder.Ascending)
                return peopleWithUserData.OrderBy(x => x.Name).ToList();
            else if (sortOrder == SortOrder.Descending)
                return peopleWithUserData.OrderByDescending(x => x.Name).ToList();

            return peopleWithUserData;
        }

        public async Task<IList<DetectedFace>> DetectFacesAsync(Stream stream, bool returnFaceLandmarks, IList<FaceAttributeType> returnFaceAttributes)
        {
            return await faceClient.Face.DetectWithStreamAsync(stream, returnFaceLandmarks: returnFaceLandmarks, returnFaceAttributes: returnFaceAttributes);
        }
    }
}
