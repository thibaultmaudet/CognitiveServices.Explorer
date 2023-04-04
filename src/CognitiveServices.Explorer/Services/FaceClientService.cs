using CognitiveServices.Explorer.Contracts.Services;

using Microsoft.Azure.CognitiveServices.Vision.Face;

namespace CognitiveServices.Explorer.Services;

public class FaceClientService : IFaceClientService
{
    private string faceEndpoint;
    private string faceKey;

    public FaceClientService(ILocalSettingsService localSettingsService)
    {
        faceEndpoint = "";
        faceKey = "";

        Task task = Task.Run(async () =>
        {
            faceEndpoint = await localSettingsService.ReadSettingAsync<string>("faceEndpoint") ?? "";
            faceKey = await localSettingsService.ReadSettingAsync<string>("faceKey") ?? "";
        });

        Task.WaitAll(task);
    }

    public FaceClient? FaceClient
    {
        get
        {            
            if (!string.IsNullOrEmpty(faceKey))
                return new FaceClient(new ApiKeyServiceClientCredentials(faceKey)) { Endpoint = faceEndpoint };

            return default;
        }
    }

    public string FaceEndpoint => faceEndpoint;

    public string FaceKey => faceKey;
}
