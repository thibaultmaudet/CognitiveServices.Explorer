using Microsoft.Azure.CognitiveServices.Vision.Face;

namespace CognitiveServices.Explorer.Contracts.Services;

public interface IFaceClientService
{
    public FaceClient? FaceClient { get; }

    public string FaceEndpoint { get; }
    public string FaceKey { get; }
}
