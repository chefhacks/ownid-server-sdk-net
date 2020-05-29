using System.Text.Json.Serialization;

namespace OwnIdSdk.NetCore3.Contracts
{
    /// <summary>
    ///     POST /ownid/ response body structure
    /// </summary>
    public class GetChallengeLinkResponse
    {
        /// <summary>
        ///     Create instance of <see cref="GetChallengeLinkResponse" /> with required parameters
        /// </summary>
        /// <param name="context">Value for <see cref="Context" />. Context identifier</param>
        /// <param name="url">Value for <see cref="Url" />. Url for qr or link that leads to OwnId app</param>
        /// <param name="nonce">Value for <see cref="Nonce" /></param>
        public GetChallengeLinkResponse(string context, string url, string nonce)
        {
            Context = context;
            Url = url;
            Nonce = nonce;
        }

        /// <summary>
        ///     Url for qr or link that leads to OwnId app
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }

        /// <summary>
        ///     Context identifier
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; }

        /// <summary>
        ///     Generated nonce
        /// </summary>
        [JsonPropertyName("nonce")]
        public string Nonce { get; }
    }
}