using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Contracts.Services
{
    public interface IFaceClientService
    {
        public IFaceClient FaceClient { get; }

        public string FaceEndpoint { get; }
        public string FaceKey { get; }
    }
}
