using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OwnID.Server.Shopify.Configuration;
using OwnID.Server.Shopify.Models;
using OwnID.Server.Shopify.Services;

namespace OwnID.Server.Shopify.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IShopService _shopService;
        private readonly ShopifyOptions _shopifyOptions;

        public HomeController(ILogger<HomeController> logger, IMemoryCache cache, IShopService shopService, IOptions<ShopifyOptions> shopifyOptions)
        {
            
            _logger = logger;
            _cache = cache;
            _shopService = shopService;
            _shopifyOptions = shopifyOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            if (!string.IsNullOrEmpty(_shopifyOptions.AccessToken) && !string.IsNullOrEmpty(_shopifyOptions.Shop))
            {
                var s = await  _shopService.GetId(_shopifyOptions.AccessToken, _shopifyOptions.Shop);
            }
            else if (_cache.TryGetValue<string>("AccessToken", out var accessToken))
            {
                if (_cache.TryGetValue<string>("Store", out var name))
                {
                   var s = await  _shopService.GetId(accessToken, name);
                }
                
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}