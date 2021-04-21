using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OwnID.Cryptography;
using OwnID.Extensibility.Configuration;
using OwnID.Extensibility.Json;
using OwnID.Server.IAS.Certs;
using OwnID.Web.IAS;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OwnID.Server.IAS
{
    public class CertDiscovery
    {
        RequestDelegate _next;
        private readonly IASConfiguration _configuration;
        public CertDiscovery(RequestDelegate next, IASConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            RsaSecurityKey jwtSignCredentials = new RsaSecurityKey(_configuration.jwtSigningCredentials);
            JsonWebKey parsedJwK = JsonWebKeyConverter.ConvertFromRSASecurityKey(jwtSignCredentials);
            
            JwkWrapper key = new JwkWrapper(parsedJwK);
            JwkWrapper[] keys = new JwkWrapper[] { key };
            var result = new Dictionary<string, object>() 
            {
                {"keys", keys}
            };


            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(OwnIdSerializer.Serialize(result));

        }
    }
}
