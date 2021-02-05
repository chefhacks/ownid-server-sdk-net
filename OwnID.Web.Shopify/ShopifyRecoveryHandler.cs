using System.Threading.Tasks;
using OwnID.Extensibility.Flow.Abstractions;
using OwnID.Extensibility.Flow.Contracts;
using OwnID.Extensibility.Flow.Contracts.AccountRecovery;

namespace OwnID.Web.Shopify
{
    public class ShopifyRecoveryHandler : IAccountRecoveryHandler
    {
        public Task<AccountRecoveryResult> RecoverAsync(string accountRecoveryPayload)
        {
            throw new System.NotImplementedException();
        }

        public Task OnRecoverAsync(string did, OwnIdConnection connection)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveConnectionsAsync(string publicKey)
        {
            throw new System.NotImplementedException();
        }
    }
}