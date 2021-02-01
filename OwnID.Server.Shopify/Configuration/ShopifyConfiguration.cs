namespace OwnID.Server.Shopify.Configuration
{
    /// <summary>
    ///     Shopify configuration settings
    /// </summary>
    public class ShopifyOptions
    {
        public const string SectionName = "Shopify";
        
        /// <summary>
        ///     API Key
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        ///     API Secret key
        /// </summary>
        public string ApiSecretKey { get; set; }
    }
}