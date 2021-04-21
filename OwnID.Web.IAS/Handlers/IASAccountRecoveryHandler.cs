using OwnID.Extensibility.Flow.Abstractions;
using OwnID.Extensibility.Flow.Contracts;
using OwnID.Extensibility.Flow.Contracts.AccountRecovery;
using System;
using System.Threading.Tasks;

namespace OwnID.Web.IAS.Handlers
{
    class IASAccountRecoveryHandler<IASUserProfile> : IAccountRecoveryHandler
    {
        public Task OnRecoverAsync(string did, OwnIdConnection connection)
        {
            throw new NotImplementedException();
        }

        public Task<AccountRecoveryResult> RecoverAsync(string accountRecoveryPayload)
        {
            throw new NotImplementedException();
        }

        public Task RemoveConnectionsAsync(string publicKey)
        {
            throw new NotImplementedException();
        }
    }
}
