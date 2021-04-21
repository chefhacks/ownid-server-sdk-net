using OwnID.Extensibility.Flow.Abstractions;
using OwnID.Extensibility.Flow.Contracts;
using OwnID.Extensibility.Flow.Contracts.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwnID.Web.IAS.Handlers
{
    class IASAccountLinkHandler<IASUserProfile> : IAccountLinkHandler
    {
        public Task<LinkState> GetCurrentUserLinkStateAsync(string payload)
        {
            throw new NotImplementedException();
        }

        public Task OnLinkAsync(string did, OwnIdConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
