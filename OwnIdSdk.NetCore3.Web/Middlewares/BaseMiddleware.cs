using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OwnIdSdk.NetCore3.Configuration;
using OwnIdSdk.NetCore3.Store;
using OwnIdSdk.NetCore3.Web.Abstractions;

namespace OwnIdSdk.NetCore3.Web.Middlewares
{
    public abstract class BaseMiddleware
    {
        protected readonly RequestDelegate Next;
        protected readonly Provider Provider;

        protected BaseMiddleware(RequestDelegate next, ICacheStore cacheStore,
            IOptions<OwnIdConfiguration> providerConfiguration)
        {
            Next = next;
            Provider = new Provider(cacheStore, providerConfiguration.Value);
        }

        public abstract Task InvokeAsync(HttpContext context);

        protected async Task Ok<T>(HttpResponse response, T responseBody) where T : class
        {
            Ok(response);
            response.ContentType = "application/json";
            await response.WriteAsync(JsonSerializer.Serialize(responseBody));
        }

        protected void Ok(HttpResponse response)
        {
            response.StatusCode = (int) HttpStatusCode.OK;
        }

        protected async Task Json<T>(HttpResponse response, T responseBody, int statusCode) where T : class
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            await response.WriteAsync(JsonSerializer.Serialize(responseBody));
        }

        protected void NotFound(HttpResponse response)
        {
            response.StatusCode = (int) HttpStatusCode.NotFound;
        }

        protected void BadRequest(HttpResponse response)
        {
            response.StatusCode = (int) HttpStatusCode.BadRequest;
        }
    }
}