using System.Threading.Tasks;
using OwnID.Extensibility.Flow.Abstractions;
using OwnID.Extensibility.Flow.Contracts;
using OwnID.Extensibility.Flow.Contracts.Link;

namespace OwnID.Web.Shopify
{
    public class ShopifyLinkHandler : IAccountLinkHandler
    {
        public Task<LinkState> GetCurrentUserLinkStateAsync(string payload)
        {
            throw new System.NotImplementedException();
        }

        public Task OnLinkAsync(string did, OwnIdConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}