using System;
using System.Security.Cryptography;
using OwnIdSdk.NetCore3.Configuration.Profile;

namespace OwnIdSdk.NetCore3.Configuration
{
    /// <summary>
    ///     Describes OwnId core configuration structure
    /// </summary>
    public interface IOwnIdCoreConfiguration
    {
        /// <summary>
        ///     Own Id application uri that will be used for authorization
        /// </summary>
        /// <remarks>
        ///     HTTPS is required.
        ///     Can only be changed for development use only. Should be <c>https://ownid.com/sign</c> in other way
        /// </remarks>
        Uri OwnIdApplicationUrl { get; set; }

        /// <summary>
        ///     Uri of OwnIdSdk host. We be used for entire OwnID challenge process
        /// </summary>
        /// <remarks>
        ///     Required.
        ///     Should use HTTPS on production environments.
        ///     Should be accessible by OwinId application endpoint <see cref="OwnIdApplicationUrl" />
        ///     HTTP can only be used for development cases with <see cref="IsDevEnvironment" /> set to <c>true</c>
        /// </remarks>
        Uri CallbackUrl { get; set; }

        /// <summary>
        ///     RSA keys for signing JWT token that will be provided for OwnId application requests
        /// </summary>
        /// <remarks>
        ///     Required.
        ///     You can use <see cref="OwnIdSdk.NetCore3.Cryptography.RsaHelper" /> to load from files easily
        /// </remarks>
        RSA JwtSignCredentials { get; set; }

        /// <summary>
        ///     Profile form fields configuration
        /// </summary>
        /// <seealso cref="IProfileConfiguration" />
        /// <remarks>
        ///     Required
        /// </remarks>
        IProfileConfiguration ProfileConfiguration { get; }

        /// <summary>
        ///     Unique identity that refers to
        /// </summary>
        /// <remarks>Required</remarks>
        public string DID { get; set; }

        /// <summary>
        ///     Name of organization / website that will be shown to end user on OwinId application page on registration / login /
        ///     managing profile
        /// </summary>
        /// <remarks>
        ///     Required
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        ///     Icon of organization / website that will be shown to end user on OwinId application page on registration / login /
        ///     managing profile
        /// </summary>
        /// <remarks>
        ///     Can be stored as URI or base64 encoded format string
        /// </remarks>
        /// <example>
        ///     data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==
        /// </example>
        public string Icon { get; set; }

        /// <summary>
        ///     Description text that will be shown near the <see cref="Name" /> on OwnId application page for end-user
        /// </summary>
        /// <remarks>
        ///     Can be localized
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        ///     Interval for polling operations that will be used by client side.
        /// </summary>
        /// <remarks>
        ///     In milliseconds
        /// </remarks>
        public int PollingInterval { get; set; }

        /// <summary>
        ///     Marks if OwnIdSdk is using for development cases
        /// </summary>
        bool IsDevEnvironment { get; set; }
        
        /// <summary>
        ///     Cache timeout expiration in milliseconds 
        /// </summary>
        /// <remarks>Default value is 10 minutes</remarks>
        uint CacheExpirationTimeout { get; set; }
    }
}