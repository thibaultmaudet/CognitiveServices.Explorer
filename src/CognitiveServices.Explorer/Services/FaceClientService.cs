using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.ViewModels;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Services
{
    public class FaceClientService : IFaceClientService
    {
        private IFaceClient faceClient;

        private SettingsViewModel settingsViewModel;

        public FaceClientService(IThemeSelectorService themeSelectorService)
        {
            settingsViewModel = new(themeSelectorService);
        }

        public IFaceClient FaceClient
        {
            get 
            {
                if (faceClient == null)
                    faceClient = new FaceClient(new ApiKeyServiceClientCredentials(FaceKey)) { Endpoint = FaceEndpoint };

                return faceClient;
            }
        }

        public string FaceEndpoint
        {
            get { return settingsViewModel.FaceEndpoint; }
        }

        public string FaceKey
        {
            get { return settingsViewModel.FaceKey; }
        }
    }
}
