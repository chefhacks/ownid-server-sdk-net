using System.Threading.Tasks;
using OwnIdSdk.NetCore3.Extensibility.Cache;
using OwnIdSdk.NetCore3.Extensibility.Flow;
using OwnIdSdk.NetCore3.Extensibility.Flow.Abstractions;
using OwnIdSdk.NetCore3.Extensibility.Flow.Contracts;
using OwnIdSdk.NetCore3.Extensibility.Flow.Contracts.Jwt;
using OwnIdSdk.NetCore3.Flow.Interfaces;
using OwnIdSdk.NetCore3.Flow.Steps;
using OwnIdSdk.NetCore3.Services;

namespace OwnIdSdk.NetCore3.Flow.Commands.Fido2
{
    public class Fido2RecoverWithPinCommand : BaseFlowCommand
    {
        private readonly ICacheItemService _cacheItemService;
        private readonly IFlowController _flowController;
        private readonly IJwtComposer _jwtComposer;
        private readonly IAccountRecoveryHandler _recoveryHandler;

        public Fido2RecoverWithPinCommand(IAccountRecoveryHandler recoveryHandler, ICacheItemService cacheItemService,
            IJwtComposer jwtComposer, IFlowController flowController)
        {
            _recoveryHandler = recoveryHandler;
            _cacheItemService = cacheItemService;
            _jwtComposer = jwtComposer;
            _flowController = flowController;
        }

        protected override void Validate(ICommandInput input, CacheItem relatedItem)
        {
        }

        protected override async Task<ICommandResult> ExecuteInternalAsync(ICommandInput input, CacheItem relatedItem,
            StepType currentStepType)
        {
            var recoverResult = await _recoveryHandler.RecoverAsync(relatedItem.Payload);
            await _recoveryHandler.OnRecoverAsync(recoverResult.DID, new OwnIdConnection
            {
                PublicKey = relatedItem.PublicKey,
                Fido2CredentialId = relatedItem.Fido2CredentialId,
                Fido2SignatureCounter = relatedItem.Fido2SignatureCounter
            });

            await _cacheItemService.FinishAuthFlowSessionAsync(relatedItem.Context, recoverResult.DID,
                relatedItem.PublicKey);

            var jwt = _jwtComposer.GenerateBaseStepJwt(relatedItem.Context, input.ClientDate,
                _flowController.GetExpectedFrontendBehavior(relatedItem, currentStepType), recoverResult.DID,
                input.CultureInfo?.Name, true);

            return new JwtContainer(jwt);
        }
    }
}