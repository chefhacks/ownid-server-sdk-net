using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OwnIdSdk.NetCore3.Contracts;
using OwnIdSdk.NetCore3.Server.Gigya.Gigya;
using OwnIdSdk.NetCore3.Server.Gigya.Gigya.Login;
using OwnIdSdk.NetCore3.Server.Gigya.Gigya.UpdateProfile;
using OwnIdSdk.NetCore3.Web.Abstractions;
using OwnIdSdk.NetCore3.Web.FlowEntries;

namespace OwnIdSdk.NetCore3.Server.Gigya
{
    public class ClientAppChallengeHandler : IChallengeHandler<UserProfile>
    {
        private readonly string _apiKey;
        private readonly string _authSecret;
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public ClientAppChallengeHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            var gigyaSection = configuration.GetSection("gigya");
            _apiKey = gigyaSection["api_key"];
            _secretKey = gigyaSection["secret"];
            _authSecret = configuration["auth_secret"];
        }

        public async Task<LoginResult<object>> OnSuccessLoginAsync(string did, HttpResponse response)
        {
            var responseMessage = await _httpClient.PostAsync(
                new Uri("https://accounts.us1.gigya.com/accounts.notifyLogin"), new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("apiKey", _apiKey),
                        new KeyValuePair<string, string>("secret", _secretKey),
                        new KeyValuePair<string, string>("siteUID", did),
                        new KeyValuePair<string, string>("targetEnv", "browser")
                    }));
            var loginResponse =
                await JsonSerializer.DeserializeAsync<LoginResponse>(await responseMessage.Content.ReadAsStreamAsync());

            if (loginResponse.SessionInfo == null || loginResponse.ErrorCode != 0)
                return new LoginResult<object>
                {
                    HttpCode = (int) HttpStatusCode.Unauthorized,
                    Data = new
                    {
                        status = false,
                        errorMessage = $"Gigya: {loginResponse.ErrorCode}:{loginResponse.ErrorMessage}"
                    }
                };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("did", loginResponse.Identities.FirstOrDefault()?.ProviderUID)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResult<object>
            {
                HttpCode = (int) HttpStatusCode.OK,
                Data = new
                {
                    status = true, jwt = token,
                    sessionInfo = loginResponse.SessionInfo,
                    identities = loginResponse.Identities.FirstOrDefault()
                }
            };
        }

        public async Task UpdateProfileAsync(UserProfileFormContext<UserProfile> context)
        {
            var getAccountMessage = await _httpClient.PostAsync(
                new Uri("https://accounts.us1.gigya.com/accounts.getAccountInfo"), new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("apiKey", _apiKey),
                        new KeyValuePair<string, string>("secret", _secretKey),
                        new KeyValuePair<string, string>("UID", context.DID)
                    }));

            var content =
                await JsonSerializer.DeserializeAsync<GetAccountInfoResponse>(await getAccountMessage.Content
                    .ReadAsStreamAsync());

            if (content.ErrorCode == 0)
            {
                if (content.Data == null || !content.Data.ContainsKey("pubKey"))
                    throw new Exception("Found gigya user without pubKey");

                var key = content.Data["pubKey"];

                if (key != context.PublicKey)
                    throw new Exception("Public key doesn't match gigya user key");


                var exProfileSerializedFields = JsonSerializer.Serialize(context.Profile, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true
                });
                var setAccountResponse = await SetAccountInfo(new[]
                {
                    new KeyValuePair<string, string>("apiKey", _apiKey),
                    new KeyValuePair<string, string>("secret", _secretKey),
                    new KeyValuePair<string, string>("UID", context.DID),
                    new KeyValuePair<string, string>("profile", exProfileSerializedFields)
                });

                if (setAccountResponse.ErrorCode == 403043)
                {
                    context.SetError(x=>x.Email, setAccountResponse.ErrorMessage);
                    return;
                }

                if (setAccountResponse.ErrorCode > 0)
                {
                    await Console.Error.WriteLineAsync(
                        $"did: {context.DID}{Environment.NewLine}" +
                        $"profile: {exProfileSerializedFields}{Environment.NewLine}" +
                        $"Gigya.setAccountInfo for EXISTING user failed with code {setAccountResponse.ErrorCode} : {setAccountResponse.ErrorMessage}");

                    context.SetGeneralError($"{setAccountResponse.ErrorCode}: {setAccountResponse.ErrorMessage}");
                    // throw new Exception(
                    //     $"Gigya.setAccountInfo for EXISTING user failed with code {setAccountResponse.ErrorCode} : {setAccountResponse.ErrorMessage}");
                }

                return;
            }

            // new user
            if (content.ErrorCode != 403005)
            {
                await Console.Error.WriteLineAsync(
                    $"Gigya.getAccountInfo error with code {content.ErrorCode} : {content.ErrorMessage}");
                context.SetGeneralError($"{content.ErrorCode}: {content.ErrorMessage}");
                // throw new Exception(
                //     $"Gigya.getAccountInfo error with code {content.ErrorCode} : {content.ErrorMessage}");
            }

            var loginMessage = await _httpClient.PostAsync(
                new Uri("https://accounts.us1.gigya.com/accounts.notifyLogin"), new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("apiKey", _apiKey),
                        new KeyValuePair<string, string>("secret", _secretKey),
                        new KeyValuePair<string, string>("siteUID", context.DID)
                    }));

            var loginStr = await loginMessage.Content.ReadAsStreamAsync();
            var loginResponse =
                await JsonSerializer.DeserializeAsync<BaseGigyaResponse>(loginStr);

            await Console.Out.WriteLineAsync(await loginMessage.Content.ReadAsStringAsync());

            var setAccountPublicKeyMessage = await SetAccountInfo(new[]
            {
                new KeyValuePair<string, string>("apiKey", _apiKey),
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("UID", context.DID),
                new KeyValuePair<string, string>("data", JsonSerializer.Serialize(new {pubKey = context.PublicKey}))
            });

            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(setAccountPublicKeyMessage));

            // if (setAccountPublicKeyMessage.ErrorCode > 0)
            //     throw new Exception(
            //         $"Gigya.setAccountInfo (public key) for NEW user failed with code {setAccountPublicKeyMessage.ErrorCode} : {setAccountPublicKeyMessage.ErrorMessage}");

            var profileSerializedFields = JsonSerializer.Serialize(context.Profile, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            });
            var setAccountMessage = await SetAccountInfo(new[]
            {
                new KeyValuePair<string, string>("apiKey", _apiKey),
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("UID", context.DID),
                new KeyValuePair<string, string>("profile", profileSerializedFields)
            });

            if (setAccountMessage.ErrorCode == 403043)
            {
                context.SetError(x=>x.Email, setAccountMessage.ErrorMessage);
                return;
            }

            if (setAccountMessage.ErrorCode > 0)
            {
                await Console.Error.WriteLineAsync(
                    $"did: {context.DID}{Environment.NewLine}" +
                    $"profile: {profileSerializedFields}{Environment.NewLine}" +
                    $"Gigya.setAccountInfo (profile) for NEW user failed with code {setAccountMessage.ErrorCode} : {setAccountMessage.ErrorMessage}");

                context.SetGeneralError($"{setAccountMessage.ErrorCode}: {setAccountMessage.ErrorMessage}");
                // throw new Exception(
                //     $"Gigya.setAccountInfo (profile) for NEW user failed with code {setAccountMessage.ErrorCode} : {setAccountMessage.ErrorMessage}");
            }
        }

        private async Task<BaseGigyaResponse> SetAccountInfo(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var setAccountDataMessage = await _httpClient.PostAsync(
                new Uri("https://accounts.us1.gigya.com/accounts.setAccountInfo"), new FormUrlEncodedContent(parameters
                ));

            var setAccountResponse = await JsonSerializer.DeserializeAsync<BaseGigyaResponse>(
                await setAccountDataMessage.Content
                    .ReadAsStreamAsync());

            return setAccountResponse;
        }
    }
}