using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Options;
using OwnID.Web.Shopify.Configuration;

namespace OwnID.Server.Shopify.Services
{
    public interface IShopService
    {
        Task<string> GenerateStorefrontAccessToken();

        Task<string> GetId();
        Task<string> GetAppId();
        Task<string> GetCustomer(string id);
    }

    public class ShopService : IShopService
    {
        private readonly ShopifyOptions _options;

        public ShopService(IOptions<ShopifyOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> GetId()
        {
            // await GenerateStorefrontAccessToken();

            var url = new Uri($"https://{_options.Shop}/admin/api/2021-01/graphql.json");
            using var client = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = url
            }, new NewtonsoftJsonSerializer());

            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _options.AccessToken);

            var request = new GraphQLRequest
            {
                Query = @"{ shop { id } }"
            };

            try
            {
                var response = await client.SendQueryAsync<object>(request);
                return response.Data.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> GetAppId()
        {
            var url = new Uri($"https://{_options.Shop}/admin/api/2021-01/graphql.json");
            using var client = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = url
            }, new NewtonsoftJsonSerializer());

            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _options.AccessToken);

            var request = new GraphQLRequest
            {
                Query = @"
{ 
    app { 
        id
        appStoreAppUrl
        developerName
        embedded
        installUrl
        published
        title
    } 
}"
            };

            try
            {
                var response = await client.SendQueryAsync<object>(request);
                return response.Data.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> GetCustomer(string id)
        {
            var url = new Uri($"https://{_options.Shop}/admin/api/2021-01/graphql.json");
            using var client = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = url
            }, new NewtonsoftJsonSerializer());

            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _options.AccessToken);

            var request = new GraphQLRequest
            {
                Query = @"
{ 
    app { 
        id
        appStoreAppUrl
        developerName
        embedded
        installUrl
        published
        title
    } 
}"
            };

            try
            {
                var response = await client.SendQueryAsync<object>(request);
                return response.Data.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> GenerateStorefrontAccessToken()
        {
            // https://shopify.dev/docs/admin-api/graphql/reference/access/storefrontaccesstokencreate

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _options.AccessToken);

            var response = await client.PostAsJsonAsync(
                $"https://{_options.Shop}/admin/api/2021-01/storefront_access_tokens.json", new
                {
                    storefront_access_token = new
                    {
                        title = "test title"
                    }
                });

            var result = await response.Content.ReadAsStringAsync();
            return result;
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