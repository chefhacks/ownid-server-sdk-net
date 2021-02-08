using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OwnID.Server.Shopify.Models;
using OwnID.Server.Shopify.Services;
using OwnID.Web.Shopify.Configuration;
using OwnID.Web.Shopify.Services;

namespace OwnID.Server.Shopify.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IShopService _shopService;
        private readonly ICustomerService _customerService;
        private readonly ShopifyOptions _shopifyOptions;

        public HomeController(ILogger<HomeController> logger, IMemoryCache cache, IShopService shopService,
            IOptions<ShopifyOptions> shopifyOptions, ICustomerService customerService)
        {
            _logger = logger;
            _cache = cache;
            _shopService = shopService;
            _customerService = customerService;
            _shopifyOptions = shopifyOptions.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GenerateStorefrontToken()
        {
            var response = await _shopService.GenerateStorefrontAccessToken();
            return Content(response);
        }

        public async Task<IActionResult> CreateCustomer()
        {
            var response = await _customerService.CreateCustomer($"{Guid.NewGuid():D}@gmail.com", Guid.NewGuid().ToString("D"));

            return Content(response);
        }

        public async Task<IActionResult> UpdateCustomer(string id)
        {
            var response = await _customerService.UpdateCustomer(id, "a", "b");

            return Content(response);
        }

        public async Task<IActionResult> ShopId()
        {
            var response = await _shopService.GetId();
            return Content(response);
        }
        
        public async Task<IActionResult> AppId()
        {
            var response = await _shopService.GetAppId();
            return Content(response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}