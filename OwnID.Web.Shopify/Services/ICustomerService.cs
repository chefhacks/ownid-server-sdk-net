using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Options;
using OwnID.Web.Shopify.Configuration;

namespace OwnID.Web.Shopify.Services
{
    public interface ICustomerService
    {
        public Task<string> CreateCustomer(string email, string password);
        Task SetMetadataAsync(string customerId, string someFieldValue);
        Task<string> UpdateCustomer(string id, string publicKey, string password);
        Task<string> FindCustomerPasswordAsync(string email);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ShopifyOptions _options;

        public CustomerService(IOptions<ShopifyOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> CreateCustomer(string email, string password)
        {
            var url = new Uri($"https://{_options.Shop}/api/2021-01/graphql.json");
            using var client = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = url
            }, new NewtonsoftJsonSerializer());

            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Storefront-Access-Token",
                (_options.StoreFrontAccessToken));

            var request = new GraphQLRequest
            {
                Query =
                    @"mutation CreateCustomer ($input: CustomerCreateInput!) { 
                         customerCreate(input: $input) { 
                             customer { 
                                 id
                                email
                             }
                             customerUserErrors {
                                 code
                                 field
                                 message
                             } 
                        }
                    }",
                Variables = new
                {
                    input = new
                    {
                        email,
                        password
                    }
                }
            };

            try
            {
                var response = await client.SendQueryAsync<CustomerCreateResult>(request);
                Console.Write(response);
                return response.Data.CustomerCreate.Customer.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public async Task SetMetadataAsync(string customerId, string someFieldValue)
        {
            var url = new Uri($"https://{_options.Shop}/admin/api/2021-01/graphql.json");
            using var client = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = url
            }, new NewtonsoftJsonSerializer());

            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _options.AccessToken);

            var request = new GraphQLRequest
            {
                Query =
                    @"mutation customerUpdate($input: CustomerInput!) {
                      customerUpdate(input: $input) {
                        customer {
                            id
                        }
                        userErrors {
                            field
                            message
                        }
                      }
                    }",
                Variables = new
                {
                    input = new
                    {
                        id = customerId,
                        privateMetafields = new[]
                        {
                            new
                            {
                                key = "someKey",
                                @namespace = "ownId",
                                owner = "gid://shopify/App/4788263",
                                valueInput = new
                                {
                                    value = someFieldValue,
                                    valueType = "STRING"
                                }
                            }
                        }
                    }
                }
            };

            try
            {
                var response = await client.SendQueryAsync<object>(request);
                Console.Write(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> UpdateCustomer(string id, string publicKey, string password)
        {
            var url = new Uri($"https://{_options.Shop}/admin/api/2021-01/graphql.json");
            using var client = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = url
            }, new NewtonsoftJsonSerializer());

            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _options.AccessToken);

            var request = new GraphQLRequest
            {
                Query =
                    @"mutation customerUpdate($input: CustomerInput!) {
                      customerUpdate(input: $input) {
                        customer {
                            id
                            tags
                        }
                        userErrors {
                            field
                            message
                        }
                      }
                    }",
                Variables = new
                {
                    input = new
                    {
                        id,
                        tags = new[] {"tag1", "tag2"},
                        privateMetafields = new[]
                        {
                            new
                            {
                                key = "publicKey",
                                @namespace = "ownId",
                                valueInput = new
                                {
                                    valueType = "STRING",
                                    value = publicKey
                                }
                            },
                            new
                            {
                                key = "password",
                                @namespace = "ownId",
                                valueInput = new
                                {
                                    valueType = "STRING",
                                    value = password
                                }
                            }
                        }
                    }
                }
            };

            try
            {
                var response = await client.SendQueryAsync<object>(request);
                Console.Write(response);
                return response.Data.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> FindCustomerPasswordAsync(string email)
        {
            var url = new Uri($"https://{_options.Shop}/admin/api/2021-01/graphql.json");
            using var client = new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = url
            }, new NewtonsoftJsonSerializer());

            client.HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _options.AccessToken);

            var request = new GraphQLRequest
            {
                Query =
                    @"query FindCustomerPassword($query: String) {
                        customers(first: 1, query: $query) {
                            edges {
                                node {
                                    id
                                    email
                                    password: privateMetafield(namespace: ""ownId"", key: ""password"") {
                                        value
                                    }
                                }
                            }
                        }
                    }",
                Variables = new
                {
                    query = $"email:{email}"
                }
            };

            try
            {
                var response = await client.SendQueryAsync<CustomersQueryResult>(request);
                return response.Data.Customers?.Edges.First().Node.Password.Value;
                Console.Write(response);
                return response.Data.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public class CustomerCreateResult
    {
        [JsonPropertyName("customerCreate")]
        public CustomerCreate CustomerCreate { get; set; }
    }

    public class CustomerCreate
    {
        [JsonPropertyName("customer")]
        public Customer Customer { get; set; }
    }

    public class Customer
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}