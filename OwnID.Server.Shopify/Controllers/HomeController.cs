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

namespace OwnID.Server.Shopify.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, IMemoryCache cache, IOptions<ShopifyOptions> options)
        {
            var s = options.Value;
            _logger = logger;
            _cache = cache;
        }

        public IActionResult Index()
        {
            if (_cache.TryGetValue<string>("AccessToken", out var accessToken))
            {
                var s = accessToken;
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