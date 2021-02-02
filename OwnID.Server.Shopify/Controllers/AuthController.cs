using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OwnID.Server.Shopify.Configuration;
using Teference.Shopify.Api;

namespace OwnID.Server.Shopify.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IShopifyOAuth _oAuth;
        private readonly IMemoryCache _memoryCache;
        private readonly ShopifyOptions _shopifyOptions;

        public AuthController(IMemoryCache memoryCache, IOptions<ShopifyOptions> shopifyOptions)
        {
            _memoryCache = memoryCache;
            _shopifyOptions = shopifyOptions.Value;

            _oAuth = new ShopifyOAuth(new OAuthConfiguration
            {
                ApiKey = _shopifyOptions.ApiKey,
                SecretKey = _shopifyOptions.ApiSecretKey
            });
        }

        [Route("init")]
        public IActionResult Authorize(string hmac, string shop, string timestamp)
        {
            //     //
            //     // TODO: Verify hmac
            //     //
            //     
            //     var redirectUrl = _oAuth.GetOAuthUrl(shop,
            //         OAuthScope.read_customers | OAuthScope.write_customers | OAuthScope.read_themes
            //         | OAuthScope.read_content | OAuthScope.read_fulfillments | OAuthScope.read_script_tags);
            //
            //     //
            //     // TODO: add unauthenticated_write_customers scope
            //     //
            //
            //     redirectUrl += ",unauthenticated_write_customers,read_assigned_fulfillment_orders&access_mode=Offline";
            //
            //
            //     redirectUrl += $"&redirect_uri={Request.Scheme}://{Request.Host}/auth/callback&state={Guid.NewGuid()}";
            //
            //     return Redirect(redirectUrl);

            var redirect_uri = $"{Request.Scheme}://{Request.Host}/auth/callback&state={Guid.NewGuid()}";
            var scopes = "read_assigned_fulfillment_orders";
            var nonce = Guid.NewGuid().ToString("N");
            var access_mode = "offline";


            var result =
                $"https://{shop}/admin/oauth/authorize?client_id={_shopifyOptions.ApiKey}&scope={scopes}&redirect_uri={redirect_uri}&state={nonce}&grant_options[]={access_mode}";

            return Redirect(result);
        }

        [Route("callback")]
        public async Task<IActionResult> Callback(string hmac, string shop, string timestamp, string code)
        {
            //
            // TODO: Verify hmac
            //

            try
            {
                var accessToken = await ShopifySharp.AuthorizationService.Authorize(code, shop,
                    _shopifyOptions.ApiKey,
                    _shopifyOptions.ApiSecretKey);
                _memoryCache.Set("AccessToken", accessToken);
                _memoryCache.Set("Store", shop);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //var authResult = _oAuth.AuthorizeClient(shop, code, hmac, timestamp);
            return Ok();
        }
    }
}