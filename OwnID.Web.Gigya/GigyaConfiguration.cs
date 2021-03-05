namespace OwnID.Web.Gigya
{
    public interface IGigyaConfiguration
    {
        string DataCenter { get; set; }
        string ApiKey { get; set; }
        string UserKey { get; set; }
        string SecretKey { get; set; }
        GigyaLoginType LoginType { get; set; }
    }

    public class GigyaConfiguration : IGigyaConfiguration
    {
        public string DataCenter { get; set; }

        public string ApiKey { get; set; }

        public string UserKey { get; set; }

        public string SecretKey { get; set; }

        public GigyaLoginType LoginType { get; set; }
    }
}