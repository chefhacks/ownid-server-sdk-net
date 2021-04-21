using Microsoft.Extensions.DependencyInjection;
using OwnID.Cryptography;
using OwnID.Web.Extensibility;
using OwnID.Web.IAS.Handlers;
using System.IO;

namespace OwnID.Web.IAS
{
    public static class OwnIdConfigurationBuilderExtension
    {
        public static void UseIAS<TProfile>(this IExtendableConfigurationBuilder builder, StringReader public_key, StringReader private_key)
            where TProfile : class, IIASUserProfile
        {
            builder.Services.AddHttpClient();
            var iasFeature = new IASIntegrationFeature();

            iasFeature.WithConfig<TProfile>(x =>
            {
                x.jwtSigningCredentials = RsaHelper.LoadKeys(public_key, private_key);
            });

            builder.AddOrUpdateFeature(iasFeature);
            builder.UseUserHandlerWithCustomProfile<IASUserProfile, IASUserHandler<IASUserProfile>>();
            builder.UseAccountLinking<IASAccountLinkHandler<IASUserProfile>>();
            builder.UseAccountRecovery<IASAccountRecoveryHandler<IASUserProfile>>();
        }

        public static void UseIAS(this IExtendableConfigurationBuilder builder, string publicKey, string privateKey)
        {
            UseIAS<IASUserProfile>(builder, new StringReader(publicKey) , new StringReader(privateKey));
        }
    }
}
