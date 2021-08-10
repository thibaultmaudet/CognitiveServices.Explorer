using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServices.Explorer.Core.Models
{
    public class UserData
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
