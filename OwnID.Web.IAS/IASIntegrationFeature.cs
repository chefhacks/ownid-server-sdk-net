using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OwnID.Web.Extensibility;
using OwnID.Web.IAS.ApiClient;

namespace OwnID.Web.IAS
{
    class IASIntegrationFeature : IFeature
    {
        private readonly IASConfiguration _configuration;
        private Action<IServiceCollection> _setupServicesAction;
        
        public IASIntegrationFeature()
        {
            _configuration = new IASConfiguration();
        }

        public void ApplyServices([NotNull] IServiceCollection services)
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
            //validate configuration
        }

        public IASIntegrationFeature WithConfig<TProfile>(Action<IASConfiguration> configAction)
            where TProfile : class, IIASUserProfile
        {
            configAction(_configuration);
            _setupServicesAction = collection => collection.TryAddSingleton<IASRestApiClient<TProfile>>();
            return this;
        }

/*        IFeature IFeature.FillEmptyWithOptional()
        {
            throw new NotImplementedException();
        }*/
    }
}
