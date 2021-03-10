using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OwnID.Web.Extensibility;
using OwnID.Web.Gigya.ApiClient;

namespace OwnID.Web.Gigya
{
    public class GigyaIntegrationFeature : IFeature
    {
        private readonly IGigyaConfiguration _configuration;
        private Action<IServiceCollection> _setupServicesAction;

        public GigyaIntegrationFeature()
        {
            _configuration = new GigyaConfiguration();
        }

        public void ApplyServices(IServiceCollection services)
        {
            services.TryAddSingleton(_configuration);
            _setupServicesAction?.Invoke(services);
        }

        public IFeature FillEmptyWithOptional()
        {
            return this;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(_configuration.ApiKey) ||
                string.IsNullOrWhiteSpace(_configuration.SecretKey) ||
                string.IsNullOrWhiteSpace(_configuration.DataCenter))
                throw new InvalidOperationException(
                    $"{nameof(_configuration.ApiKey)}, {nameof(_configuration.SecretKey)} and {nameof(_configuration.DataCenter)} should be provided");
        }

        public GigyaIntegrationFeature WithConfig<TProfile>(Action<IGigyaConfiguration> configAction)
            where TProfile : class, IGigyaUserProfile
        {
            configAction(_configuration);
            _setupServicesAction = collection => collection.TryAddSingleton<GigyaRestApiClient<TProfile>>();
            return this;
        }
    }
}