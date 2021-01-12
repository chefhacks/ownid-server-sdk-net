using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using OwnID.Extensibility.Json;
using OwnID.Web.Gigya.Contracts;
using OwnID.Web.Gigya.Contracts.Accounts;
using OwnID.Web.Gigya.Contracts.Jwt;
using OwnID.Web.Gigya.Contracts.Login;

namespace OwnID.Web.Gigya.ApiClient
{
    public class GigyaRestApiClient<TProfile> where TProfile : class, IGigyaUserProfile
    {
        private readonly GigyaConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public GigyaRestApiClient(GigyaConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<GetAccountInfoResponse<TProfile>> GetUserInfoByUid(string uid)
        {
            return await GetUserProfile(uid);
        }

        public async Task<GetAccountInfoResponse<TProfile>> GetUserInfoByToken(string regToken)
        {
            return await GetUserProfile(regToken: regToken);
        }

        public async Task<BaseGigyaResponse> SetAccountInfo<T>(string did, T profile = null,
            AccountData data = null) where T : class, IGigyaUserProfile
        {
            var parameters = ParametersFactory.CreateAuthParameters(_configuration)
                .AddParameter("UID", did);

            if (profile != null)
                parameters.AddParameter("profile", profile);

            if (data != null)
            {
                foreach (var connection in data.Connections.Where(connection => string.IsNullOrEmpty(connection.Hash)))
                    connection.Hash = connection.PublicKey.GetSha256();

                parameters.AddParameter("data", data);
            }

            var setAccountDataMessage = await _httpClient.PostAsync(
                new Uri($"https://accounts.{_configuration.DataCenter}/accounts.setAccountInfo"),
                new FormUrlEncodedContent(parameters));

            var setAccountResponse = await OwnIdSerializer.DeserializeAsync<BaseGigyaResponse>(
                await setAccountDataMessage.Content
                    .ReadAsStreamAsync());

            return setAccountResponse;
        }

        public async Task<LoginResponse> NotifyLogin(string did, string targetEnvironment = null)
        {
            var parameters = ParametersFactory.CreateAuthParameters(_configuration)
                .AddParameter("siteUID", did)
                .AddParameter("skipValidation", bool.TrueString);

            if (!string.IsNullOrEmpty(targetEnvironment))
                parameters.AddParameter("targetEnv", targetEnvironment);

            var responseMessage = await _httpClient.PostAsync(
                new Uri($"https://accounts.{_configuration.DataCenter}/accounts.notifyLogin"),
                new FormUrlEncodedContent(parameters));

            return await OwnIdSerializer.DeserializeAsync<LoginResponse>(
                await responseMessage.Content.ReadAsStreamAsync());
        }

        public async Task<JsonWebKey> GetPublicKey()
        {
            var parameters = ParametersFactory.CreateApiKeyParameter(_configuration);

            var responseMessage = await _httpClient.PostAsync(
                new Uri($"https://accounts.{_configuration.DataCenter}/accounts.getJWTPublicKey"),
                new FormUrlEncodedContent(parameters));
            return new JsonWebKey(await responseMessage.Content.ReadAsStringAsync());
        }

        public async Task<GetJwtResponse> GetJwt(string did)
        {
            var parameters = ParametersFactory.CreateAuthParameters(_configuration)
                .AddParameter("targetUID", did);

            var responseMessage = await _httpClient.PostAsync(
                new Uri($"https://accounts.{_configuration.DataCenter}/accounts.getJWT"),
                new FormUrlEncodedContent(parameters));
            return await OwnIdSerializer.DeserializeAsync<GetJwtResponse>(
                await responseMessage.Content.ReadAsStreamAsync());
        }

        /// <summary>
        ///     Reset account password with resetToken generated by Gigya
        /// </summary>
        /// <param name="resetToken">reset token</param>
        /// <param name="newPassword">new password to set</param>
        /// <returns>
        ///     A task that represents the asynchronous reset password operation.
        ///     The task result contains the <see cref="ResetPasswordResponse" />
        /// </returns>
        /// <remarks>
        ///     Documentation: https://developers.gigya.com/display/GD/accounts.resetPassword+REST
        /// </remarks>
        public async Task<ResetPasswordResponse> ResetPasswordAsync(string resetToken, string newPassword)
        {
            var parameters = ParametersFactory.CreateAuthParameters(_configuration)
                .AddParameter("newPassword", newPassword)
                .AddParameter("passwordResetToken", resetToken);

            var responseMessage = await _httpClient.PostAsync(
                new Uri($"https://accounts.{_configuration.DataCenter}/accounts.resetPassword"),
                new FormUrlEncodedContent(parameters));

            return await OwnIdSerializer.DeserializeAsync<ResetPasswordResponse>(
                await responseMessage.Content.ReadAsStreamAsync());
        }

        public async Task<BaseGigyaResponse> DeleteAccountAsync(string did)
        {
            var parameters = ParametersFactory.CreateAuthParameters(_configuration)
                .AddParameter("UID", did);

            var responseMessage = await _httpClient.PostAsync(
                new Uri($"https://accounts.{_configuration.DataCenter}/accounts.deleteAccount"),
                new FormUrlEncodedContent(parameters));

            return await OwnIdSerializer.DeserializeAsync<BaseGigyaResponse>(
                await responseMessage.Content.ReadAsStreamAsync());
        }

        public async Task<UidContainer> SearchByPublicKey(string publicKey,
            GigyaProfileFields fields = GigyaProfileFields.Default)
        {
            var objectsToGet = GetGigyaProfileFields(fields | GigyaProfileFields.ConnectionPublicKeys);
            var result =
                await SearchAsync<UidResponse>("data.ownIdConnections.keyHsh", publicKey.GetSha256(), objectsToGet);
            var user = result.Results?.FirstOrDefault();

            if (result.ErrorCode != 0 || (user?.Data?.Connections?.All(x => x.PublicKey != publicKey) ?? true))
                return null;

            return user;
        }

        public async Task<UidContainer> SearchByFido2CredentialId(string fido2CredentialId)
        {
            var result = await SearchAsync<UidResponse>("data.ownIdConnections.fido2CredentialId", fido2CredentialId);
            var user = result.Results?.FirstOrDefault();

            if (result.ErrorCode != 0
                || (user?.Data?.Connections?.All(x => x.Fido2CredentialId != fido2CredentialId) ?? true))
                return null;

            return user;
        }

        public async Task<GetAccountInfoResponse<TProfile>> SearchByRecoveryTokenAsync(string recoveryToken)
        {
            var result = await SearchAsync<GetAccountInfoResponseList<TProfile>>("data.ownIdConnections.recoveryId",
                recoveryToken, new[] {"UID", "data.ownIdConnections", "profile"});
            return result.Results?.FirstOrDefault();
        }

        public async Task<UidContainer> SearchByEmailAsync(string email)
        {
            var result = await SearchAsync<UidResponse>("profile.email", email, new[] {"UID"});
            return result.Results?.FirstOrDefault();
        }

        private async Task<GetAccountInfoResponse<TProfile>> GetUserProfile(string uid = null, string regToken = null)
        {
            var parameters = ParametersFactory.CreateAuthParameters(_configuration);

            if (!string.IsNullOrEmpty(uid))
                parameters.AddParameter("UID", uid);
            else
                parameters.AddParameter("regToken", regToken);

            var getAccountMessage = await _httpClient.PostAsync(
                new Uri($"https://accounts.{_configuration.DataCenter}/accounts.getAccountInfo"),
                new FormUrlEncodedContent(parameters));

            return
                await OwnIdSerializer.DeserializeAsync<GetAccountInfoResponse<TProfile>>(await getAccountMessage.Content
                    .ReadAsStreamAsync());
        }

        private async Task<TResult> SearchAsync<TResult>(string searchKey, string searchValue,
            string[] objectsToGet = null)
            where TResult : BaseGigyaResponse
        {
            objectsToGet ??= new[] {"UID", "data.ownIdConnections"};

            objectsToGet = objectsToGet.Distinct().ToArray();

            var parameters = ParametersFactory.CreateAuthParameters(_configuration).AddParameter("query",
                $"SELECT {string.Join(", ", objectsToGet)} FROM accounts WHERE {searchKey} = \"{searchValue}\" LIMIT 1");
            var responseMessage = await _httpClient.PostAsync(
                new Uri($"https://accounts.{_configuration.DataCenter}/accounts.search"),
                new FormUrlEncodedContent(parameters));

            var result = await OwnIdSerializer.DeserializeAsync<TResult>(
                await responseMessage.Content.ReadAsStreamAsync());

            return result;
        }

        private readonly ConcurrentDictionary<GigyaProfileFields, string[]> _profileFieldsCache = new();

        private string[] GetGigyaProfileFields(GigyaProfileFields fields)
        {
            if (_profileFieldsCache.TryGetValue(fields, out var result))
                return result;

            var resultFields = new List<string>();

            var enumType = typeof(GigyaProfileFields);
            foreach (var value in Enum.GetValues<GigyaProfileFields>())
            {
                if (!fields.HasFlag(value))
                    continue;

                var memberInfos = enumType.GetMember(value.ToString());

                var enumValueMemberInfo = memberInfos.First(m => m.DeclaringType == enumType);
                var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(GigyaFieldsAttribute), false);

                if (valueAttributes.Length == 0)
                    throw new Exception($"`{nameof(GigyaProfileFields)}.{value}` doesn't has `GigyaFields` attribute");

                var queriedFields = ((GigyaFieldsAttribute) valueAttributes[0]).Fields.Split(",");

                resultFields.AddRange(queriedFields);
            }

            result = resultFields
                .Select(x => x.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            _profileFieldsCache.TryAdd(fields, result);

            return result;
        }
    }
}