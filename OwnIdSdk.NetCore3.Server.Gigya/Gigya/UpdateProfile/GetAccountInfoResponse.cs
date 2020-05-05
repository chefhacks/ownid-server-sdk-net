using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OwnIdSdk.NetCore3.Server.Gigya.Gigya.UpdateProfile
{
    public class GetAccountInfoResponse : BaseGigyaResponse
    {
        [JsonPropertyName("UID")]
        public string DID { get; set; }
        
        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; }
        
        [JsonPropertyName("profile")]
        public Dictionary<string, string> Profile { get; set; }
    }
}