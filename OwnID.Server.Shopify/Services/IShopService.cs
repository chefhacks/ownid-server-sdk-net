using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace OwnID.Server.Shopify.Services
{
    public interface IShopService
    {
        Task<string> GetId(string token, string shopName);
    }

    public class ShopService : IShopService
    {
        public async Task<string> GetId(string token, string shopName)
        {
            var url = new Uri($"https://{shopName}/admin/api/2021-01/graphql.json");
            using var client = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = url
            }, new NewtonsoftJsonSerializer());
        
            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", token);
        
            var request = new GraphQLRequest
            {
                Query = @"{ shop { id } }"
            };
        
            try
            {
                var response = await client.SendQueryAsync<ShopResponse>(request);
                return response.Data.Shop.Id;
                //return response.Data.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public class ShopResponse
    {
        public Shop Shop { get; set; }
    }

    public class Shop
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}