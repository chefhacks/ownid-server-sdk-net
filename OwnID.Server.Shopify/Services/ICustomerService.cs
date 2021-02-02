using System.Threading.Tasks;
using ShopifySharp;

namespace OwnID.Server.Shopify.Services
{
    public interface ICustomerService
    {
        public Task CreateCustomer(string email, string password)
        {
            return Task.CompletedTask;
        }
    }
}