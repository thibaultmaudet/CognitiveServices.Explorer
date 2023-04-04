using Newtonsoft.Json;

namespace CognitiveServices.Explorer.Core.Models;

public class UserData
{
    [JsonProperty(PropertyName = "Description")]
    public string Description { get; set; }
    
    [JsonProperty(PropertyName = "ImageUrl")]
    public string ImageUrl { get; set; }
}
