using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OwnIdSdk.NetCore3.Configuration;
using OwnIdSdk.NetCore3.Contracts.Jwt;
using OwnIdSdk.NetCore3.Flow;
using OwnIdSdk.NetCore3.Store;
using OwnIdSdk.NetCore3.Web.Exceptions;
using OwnIdSdk.NetCore3.Web.Extensibility.Abstractions;
using OwnIdSdk.NetCore3.Web.FlowEntries.RequestHandling;

namespace OwnIdSdk.NetCore3.Web.Middlewares.Link
{
    [RequestDescriptor(BaseRequestFields.Context | BaseRequestFields.RequestToken | BaseRequestFields.ResponseToken)]
    public class SaveAccountLinkMiddleware : BaseMiddleware
    {
        private readonly IAccountLinkHandlerAdapter _accountLinkHandlerAdapter;
        private readonly FlowController _flowController;

        public SaveAccountLinkMiddleware(RequestDelegate next, IAccountLinkHandlerAdapter accountLinkHandlerAdapter,
            IOwnIdCoreConfiguration coreConfiguration, ICacheStore cacheStore, ILocalizationService localizationService,
            ILogger<SaveAccountLinkMiddleware> logger, FlowController flowController)
            : base(next, coreConfiguration, cacheStore, localizationService, logger)
        {
            _accountLinkHandlerAdapter = accountLinkHandlerAdapter;
            _flowController = flowController;
        }

        protected override async Task Execute(HttpContext httpContext)
        {
            var cacheItem = await GetRequestRelatedCacheItemAsync();

            if (!cacheItem.IsValidForLink)
                throw new RequestValidationException(
                    "Cache item should be not Finished with Link challenge type. " +
                    $"Actual Status={cacheItem.Status.ToString()} ChallengeType={cacheItem.ChallengeType}");

            ValidateCacheItemTokens(cacheItem);

            var userData = await GetRequestJwtDataAsync<UserProfileData>(httpContext);

            //preventing data substitution
            userData.DID = cacheItem.DID;

            var formContext = _accountLinkHandlerAdapter.CreateUserDefinedContext(userData, LocalizationService);
            formContext.Validate();

            if (formContext.HasErrors)
                throw new BusinessValidationException(formContext);

            await _accountLinkHandlerAdapter.OnLink(formContext);

            if (!formContext.HasErrors)
            {
                await OwnIdProvider.FinishAuthFlowSessionAsync(RequestIdentity.Context, userData.DID);
                var jwt = OwnIdProvider.GenerateFinalStepJwt(cacheItem.Context,
                    _flowController.GetNextStep(cacheItem, StepType.Link), GetRequestCulture(httpContext).Name);

                await Json(httpContext, new JwtContainer(jwt), StatusCodes.Status200OK);
            }
            else
            {
                throw new BusinessValidationException(formContext);
            }
        }
    }
}