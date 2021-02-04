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
            //
            // TODO: Verify hmac
            //

            
            var redirect_uri = $"{Request.Scheme}://{Request.Host}/auth/callback&state={Guid.NewGuid()}";
            var scopes = "read_content, write_content,read_customers,write_customers,unauthenticated_write_customers,unauthenticated_read_customers,read_script_tags,write_script_tags";
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
                
                return Content($"Access token: {accessToken}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}