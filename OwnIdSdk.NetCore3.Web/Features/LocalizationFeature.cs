using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using OwnIdSdk.NetCore3.Web.Extensibility.Abstractions;
using OwnIdSdk.NetCore3.Web.Resources;

namespace OwnIdSdk.NetCore3.Web.Features
{
    public class LocalizationFeature : IFeatureConfiguration
    {
        private readonly Type _stringLocalizerType;

        private readonly Type _resourceType;

        private readonly string _resourceName;
        
        public LocalizationFeature()
        {
        }

        protected LocalizationFeature(Type stringLocalizerType)
        {
            _stringLocalizerType = stringLocalizerType;
        }

        protected LocalizationFeature(Type resourceType, string resourceName)
        {
            _resourceType = resourceType;
            _resourceName = resourceName;
        }

        public LocalizationFeature WithStringLocalizer<TLocalizer>() where TLocalizer : IStringLocalizer
        {
            return new LocalizationFeature(typeof(TLocalizer));
        }

        public LocalizationFeature WithResource(Type resourceType, string resourceName)
        {
            return new LocalizationFeature(resourceType, resourceName);
        }
        
        public void ApplyServices(IServiceCollection services)
        {
            services.AddLocalization();
            services.TryAddSingleton<ILocalizationService>(x =>
            {
                var factory = x.GetService<IStringLocalizerFactory>();
                var type = typeof(OwnIdSdkDefault);
                var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName!);
                var defaultLocalizer = factory.Create(type.FullName, assemblyName.Name);
                IStringLocalizer custom = null;
                
                if (_stringLocalizerType != null)
                    custom = x.GetService(_stringLocalizerType) as IStringLocalizer;
                else if(_resourceType != null)
                {
                    var customLocalizerAssemblyName =
                        new AssemblyName(_resourceType.GetTypeInfo().Assembly.FullName!);
                    custom = factory.Create(_resourceName, customLocalizerAssemblyName.Name);
                }
                
                return new LocalizationService(defaultLocalizer, custom);
            });
        }

        public IFeatureConfiguration FillEmptyWithOptional()
        {
            return this;
        }

        public void Validate()
        {
        }
    }
}