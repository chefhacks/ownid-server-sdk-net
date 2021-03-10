using Microsoft.Extensions.Options;
using OwnID.Extensibility.Configuration;

namespace OwnID.Configuration.Validators
{
    public class Fido2ConfigurationValidator
    {
        private readonly IUriValidationHelper _uriValidationHelper;

        public Fido2ConfigurationValidator(IUriValidationHelper uriValidationHelper)
        {
            _uriValidationHelper = uriValidationHelper;
        }
        
        public ValidateOptionsResult Validate(IFido2Configuration configuration, bool isDevEnvironment)
        {
            // Validate Fido2Url
            if (configuration.IsEnabled && !_uriValidationHelper.IsValid(
                nameof(configuration.PasswordlessPageUrl),
                configuration.PasswordlessPageUrl, isDevEnvironment, out var error))
                return ValidateOptionsResult.Fail(error);
            
            // Validate Origin
            if (configuration.IsEnabled && !_uriValidationHelper.IsValid(
                nameof(configuration.Origin),
                configuration.Origin, isDevEnvironment, out var errorOrigin))
                return ValidateOptionsResult.Fail(errorOrigin);
            
            // Validate RelyingPartyId
            if(configuration.IsEnabled && string.IsNullOrEmpty(configuration.RelyingPartyId))
                return ValidateOptionsResult.Fail($"{nameof(configuration.RelyingPartyId)} is required");
            
            // Validate UserName
            if(configuration.IsEnabled && string.IsNullOrEmpty(configuration.UserName))
                return ValidateOptionsResult.Fail($"{nameof(configuration.UserName)} is required");

            return ValidateOptionsResult.Success;
        }
    }
}