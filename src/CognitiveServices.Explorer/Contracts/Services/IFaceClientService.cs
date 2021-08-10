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
        IFaceClient GetFaceClient();

        string GetFaceEndpoint();
        string GetFaceKey();
    }
}
