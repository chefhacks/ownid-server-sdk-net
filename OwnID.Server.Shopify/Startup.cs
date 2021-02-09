using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OwnID.Extensibility.Configuration;
using OwnID.Extensibility.Flow.Abstractions;
using OwnID.Server.Shopify.Services;
using OwnID.Web;
using OwnID.Web.Shopify;
using OwnID.Web.Shopify.Configuration;
using OwnID.Web.Shopify.Services;
using Teference.Shopify.Api;

namespace OwnID.Server.Shopify
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ShopifyOptions>(Configuration.GetSection(ShopifyOptions.SectionName));

            services.AddSingleton<IShopService, ShopService>();
            services.AddSingleton<ICustomerService, CustomerService>();


            services.AddCors(x =>
            {
                x.AddPolicy("MyAllowAllHeadersPolicy", builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains();

                    var originsList = new List<string>();

                    builder.SetIsOriginAllowed(origin => true);
                    
                    originsList.Add("https://*.c0f2ea2ff2e8.ngrok.io");
                    originsList.Add("http://*.c0f2ea2ff2e8.ngrok.io");
                    originsList.Add("https://c0f2ea2ff2e8.ngrok.io");
                    originsList.Add("http://c0f2ea2ff2e8.ngrok.io");
                    originsList.Add("https://sign.dev.ownid.com");
                    originsList.Add("https://avosp-test-store.myshopify.com");
                    originsList.Add("https://*.myshopify.com");


                    //         break;
                    //     case ServerMode.Local:
                    //         builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                    //         break;
                    //     default:
                    //         originsList.Add($"https://*.{topDomain}");
                    //         originsList.Add($"https://{topDomain}");
                    //         originsList.Add(webAppUrl.ToString().TrimEnd('/'));
                    //
                    //         var additionalOrigins = ownIdSection["add_cors_origins"];
                    //
                    //         if (!string.IsNullOrWhiteSpace(additionalOrigins))
                    //             originsList.AddRange(additionalOrigins.Split(';').Select(o => o.Trim()));
                    //         break;
                    // }

                    // builder.WithOrigins(originsList.ToArray());
                });

                // x.AddPolicy(CorsPolicyName, builder =>
                // {
                //     builder.AllowAnyHeader();
                //     builder.AllowAnyMethod();
                //     builder.AllowCredentials();
                //     builder.SetIsOriginAllowedToAllowWildcardSubdomains();
                //
                //     var originsList = new List<string>();
                //
                //     builder.SetIsOriginAllowed(origin => true);
                //     builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                //     originsList.Add("https://*.c0f2ea2ff2e8.ngrok.io");
                //     originsList.Add("http://*.c0f2ea2ff2e8.ngrok.io");
                //     originsList.Add("https://c0f2ea2ff2e8.ngrok.io");
                //     originsList.Add("http://c0f2ea2ff2e8.ngrok.io");
                //     originsList.Add("https://sign.dev.ownid.com");
                //     originsList.Add("https://avosp-test-store.myshopify.com");
                //     
                //
                //     builder.WithOrigins(originsList.ToArray());
                // });
            });

            services.AddOwnId(builder =>
            {
                builder.UseUserHandlerWithCustomProfile<ShopifyUserProfile, ShopifyUserHandler>();
                builder.UseAccountLinking<ShopifyLinkHandler>();
                builder.UseAccountRecovery<ShopifyRecoveryHandler>();


                builder.SetKeys(
                    "-----BEGIN PUBLIC KEY-----\nMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAwRf/z+C5QOxWeDklDm1r\nThqeN94sBC00Fau/NYOU1PdgEkjrvP9NXhoHvUm7iv3CoLibjGad8lSeJ7my7sgE\n7EwnlFf5WyU9d74AEvGqJGmkjJfXc+0DC80eRx6nO9luDoqVpsv+wBVs0EbChwjY\nsZt51WZRJj6OO/W7eup062FPTP0HMy+uthpppnYWxZ2bY8tZN5IVRSUv5l0f4oMT\nskbM8Af75h6ICfqWOhXuv3SFSOS5viwx+beUB+px+wVm2xJBRWnlCL2l5uzyiP2S\nTPr6ZZiDcOByBcq61eeF/lkSm9io5O66dLFE8zEH92hPJqsC70ZszXRVifg706AL\nlnO+8BIuTPwsEe01mrUm0sSy/yzgPAJd6BZ4u0WRNIsEpP2fPZbLvfpSllqiBSOF\nVJu7pnrsnuRIi9n4u+BTv7V49NcjMDDYJ0nvtOdcg4X5TgqFBpYYZHl39dOz0J7A\nzBAfh/zDs0DtHGVloxXUMtXFbBs/f2k37V9te8krsh5pCljDDoPOB4Iwc01yDxwj\n0DK7+XDYID1QHvi/Fl4vsmti1VCackMDhWP3cb9OxDB2OW6aa+QvKP8Co8hDG6j/\nbz0n+mXPbU/jQA2s1pNzKBgN2jF09pwAld5foTg/3QIh417S+hsPGWrlldr1MneM\nkGYACfNcJ8JCm5RwK4izypkCAwEAAQ==\n-----END PUBLIC KEY-----",
                    "-----BEGIN RSA PRIVATE KEY-----\n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\n-----END RSA PRIVATE KEY-----");

                builder.WithBaseSettings(s =>
                {
                    s.TopDomain = "localhost";
                    // s.TopDomain = "c0f2ea2ff2e8.ngrok.io";
                    s.Name = "Shopify";
                    s.DID = "Shopify-DID-sdf";
                    s.IsDevEnvironment = true;

                    //s.OwnIdApplicationUrl = new Uri("https://sign.dev.ownid.com/");
                    s.OwnIdApplicationUrl = new Uri("http://localhost:4200/");

                    s.Fido2FallbackBehavior = Fido2FallbackBehavior.Basic;
                    
                    // s.CallbackUrl = new Uri("https://c0f2ea2ff2e8.ngrok.io/");
                    s.CallbackUrl = new Uri("http://localhost:5000");
                });
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization(x =>
            {
                var supportedCultures = new[]
                {
                    "en"
                };

                x.AddSupportedCultures(supportedCultures);
                x.DefaultRequestCulture = new RequestCulture("en", "en");
                x.AddSupportedUICultures(supportedCultures);
            });


            app.UseCors("MyAllowAllHeadersPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseOwnId();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}