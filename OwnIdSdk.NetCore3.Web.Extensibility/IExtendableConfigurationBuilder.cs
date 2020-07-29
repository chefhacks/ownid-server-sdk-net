using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using OwnIdSdk.NetCore3.Extensibility.Flow.Abstractions;

namespace OwnIdSdk.NetCore3.Web.Extensibility
{
    public interface IExtendableConfigurationBuilder
    {
        IServiceCollection Services { get; }

        void AddOrUpdateFeature<TFeature>([NotNull] TFeature feature) where TFeature : class, IFeatureConfiguration;

        void UseAccountLinking<TProfile, THandler>() where THandler : class, IAccountLinkHandler<TProfile>
            where TProfile : class;

        void UseAccountRecovery<THandler>()
            where THandler : class, IAccountRecoveryHandler;

        void UseUserHandlerWithCustomProfile<TProfile, THandler>()
            where THandler : class, IUserHandler<TProfile> where TProfile : class;
    }
}