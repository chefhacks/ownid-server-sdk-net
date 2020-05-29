using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OwnIdSdk.NetCore3.Contracts
{
    /// <summary>
    ///     Describes 400 BadRequest response for OwnId application
    /// </summary>
    public class BadRequestResponse
    {
        /// <summary>
        ///     General, non-field-specific localized error texts
        /// </summary>
        [JsonPropertyName("generalErrors")]
        public IEnumerable<string> GeneralErrors { get; set; }

        /// <summary>
        ///     Field-specific localized error texts
        /// </summary>
        /// <remarks>
        ///     Key should contain <see cref="OwnIdSdk.NetCore3.Configuration.Profile.ProfileFieldMetadata.Key" />
        ///     Value should contain already localized error texts with localized field label inside if needed
        /// </remarks>
        [JsonPropertyName("fieldErrors")]
        public IDictionary<string, IList<string>> FieldErrors { get; set; }
    }
}