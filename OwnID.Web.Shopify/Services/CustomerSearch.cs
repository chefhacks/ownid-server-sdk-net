using Newtonsoft.Json;

namespace OwnID.Web.Shopify.Services
{
    public partial class CustomersQueryResult
    {
        [JsonProperty("customers")]
        public Customers Customers { get; set; }
    }

    public partial class Customers
    {
        [JsonProperty("edges")]
        public Edge[] Edges { get; set; }
    }

    public partial class Edge
    {
        [JsonProperty("node")]
        public Node Node { get; set; }
    }

    public partial class Node
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public Password Password { get; set; }
    }

    public partial class Password
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}