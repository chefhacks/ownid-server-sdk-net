using Microsoft.Extensions.DependencyInjection;
using OwnID.Web.Extensibility;
using OwnID.Web.IAS.Handlers;

namespace OwnID.Web.IAS
{
    public static class OwnIdConfigurationBuilderExtension
    {
        public static void UseIAS<TProfile>(this IExtendableConfigurationBuilder builder, string dataCenter, 
            string apiKey, string secret, string useKey)
            where TProfile : class, IIASUserProfile
        {
            builder.Services.AddHttpClient();
            var iasFeature = new IASIntegrationFeature();

            iasFeature.WithConfig<TProfile>(x =>
            {
                x.DataCenter = dataCenter;
            });

            builder.AddOrUpdateFeature(iasFeature);
            builder.UseUserHandlerWithCustomProfile<IASUserProfile, IASUserHandler<IASUserProfile>>();
            builder.UseAccountLinking<IASAccountLinkHandler<IASUserProfile>>();
            builder.UseAccountRecovery<IASAccountRecoveryHandler<IASUserProfile>>();
        }

        public static void UseIAS(this IExtendableConfigurationBuilder builder, string dataCenter,
            string apiKey, string secret, string useKey)
        {
            UseIAS<IASUserProfile>(builder, dataCenter, apiKey, secret, useKey);
        }
    }
}
