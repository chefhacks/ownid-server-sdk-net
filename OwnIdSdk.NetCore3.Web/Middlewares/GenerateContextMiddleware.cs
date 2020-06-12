using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OwnIdSdk.NetCore3.Configuration;
using OwnIdSdk.NetCore3.Contracts;
using OwnIdSdk.NetCore3.Contracts.Jwt;
using OwnIdSdk.NetCore3.Store;
using OwnIdSdk.NetCore3.Web.Extensibility.Abstractions;

namespace OwnIdSdk.NetCore3.Web.Middlewares
{
    public class GenerateContextMiddleware : BaseMiddleware
    {
        private readonly IAccountLinkHandlerAdapter _linkHandlerAdapter;

        public GenerateContextMiddleware(RequestDelegate next, ICacheStore cacheStore,
            IOwnIdCoreConfiguration coreConfiguration, ILocalizationService localizationService,
            IAccountLinkHandlerAdapter linkHandlerAdapter = null) : base(next,
            coreConfiguration,
            cacheStore, localizationService)
        {
            _linkHandlerAdapter = linkHandlerAdapter;
        }

        protected override async Task Execute(HttpContext context)
        {
            context.Request.EnableBuffering();
            var request = await JsonSerializer.DeserializeAsync<GenerateContextRequest>(context.Request.Body);
            context.Request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(request.Type) ||
                !Enum.TryParse(request.Type, true, out ChallengeType challengeType) ||
                challengeType == ChallengeType.Link && _linkHandlerAdapter == null)
            {
                BadRequest(context.Response);
                return;
            }

            var challengeContext = OwnIdProvider.GenerateContext();
            var nonce = OwnIdProvider.GenerateNonce();

            string did = null;

            if (challengeType == ChallengeType.Link)
                did = await _linkHandlerAdapter.GetCurrentUserIdAsync(context.Request);
            
            await OwnIdProvider.CreateAuthFlowSessionItemAsync(challengeContext, nonce, challengeType, did);

            await Json(context, new GetChallengeLinkResponse(challengeContext,
                OwnIdProvider.GetDeepLink(challengeContext, challengeType),
                nonce
            ), StatusCodes.Status200OK, false);
        }
    }
}