namespace OwnIdSdk.NetCore3.Server.Gigya.Middlewares.SecurityHeaders.Constants
{
    /// <summary>
    /// X-Content-Type-Options-related constants.
    /// </summary>
    public static class ContentTypeOptionsConstants
    {
        /// <summary>
        /// Header value for X-Content-Type-Options
        /// </summary>
        public static readonly string Header = "X-Content-Type-Options";

        /// <summary>
        /// Disables content sniffing
        /// </summary>
        public static readonly string NoSniff = "nosniff";
        
    }
}