using System;
using OwnID.Extensibility.Configuration;
using OwnID.Extensibility.Configuration.Validators;

namespace OwnID.Configuration.Validators
{
    public class MagicLinkConfigurationValidator : IConfigurationValidator<IMagicLinkConfiguration>
    {
        private readonly IUriValidationHelper _uriValidationHelper;

        public MagicLinkConfigurationValidator(IUriValidationHelper uriValidationHelper)
        {
            _uriValidationHelper = uriValidationHelper;
        }
        
        public void FillEmptyWithOptional(IMagicLinkConfiguration configuration)
        {
            if (configuration.TokenLifetime == default)
                configuration.TokenLifetime = (uint) TimeSpan.FromMinutes(10).TotalMilliseconds;
        }

        public void Validate(IMagicLinkConfiguration configuration)
        {
            if (!_uriValidationHelper.IsValid($"MagicLink.{nameof(configuration.RedirectUrl)}",
                configuration.RedirectUrl, true, out var errMessage))
                throw new InvalidOperationException(errMessage);
        }
    }
}