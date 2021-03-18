using Microsoft.Extensions.Logging;
using OwnID.Cryptography;
using OwnID.Extensibility.Flow;
using OwnID.Extensibility.Flow.Abstractions;
using OwnID.Extensibility.Flow.Contracts;
using OwnID.Extensibility.Flow.Contracts.Fido2;
using OwnID.Extensibility.Flow.Contracts.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwnID.Web.IAS.Handlers
{
    public class IASUserHandler<TProfile> : IUserHandler<TProfile> where TProfile : class, IIASUserProfile
    {

        private readonly IASConfiguration _configuration;
        private readonly ILogger<IASUserHandler<TProfile>> _logger;
        private readonly IJwtService _jwtService;

        public IASUserHandler(IASConfiguration configuration, ILogger<IASUserHandler<TProfile>> logger, IJwtService jwtService)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtService = jwtService;

        }

        public Task<IdentitiesCheckResult> CheckUserIdentitiesAsync(string did, string publicKey)
        {
            throw new NotImplementedException();
        }

        public Task CreateProfileAsync(IUserProfileFormContext<TProfile> context, string recoveryToken = null, string recoveryData = null)
        {
            throw new NotImplementedException();
        }

        public Task<Fido2Info> FindFido2Info(string fido2CredentialId)
        {
            throw new NotImplementedException();
        }

        public Task<ConnectionRecoveryResult<TProfile>> GetConnectionRecoveryDataAsync(string recoveryToken, bool includingProfile = false)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(string did)
        {
            throw new NotImplementedException();
        }

        public async Task<UserSettings> GetUserSettingsAsync(string publicKey)
        {
            // First method used
            UserSettings userSettings = await Task.FromResult(new UserSettings
            {
                EnforceTFA = false
            });
            
            return userSettings;
        }

        public Task<bool> IsFido2UserExists(string fido2CredentialId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserExists(string publicKey)
        {
            return await Task.FromResult(true);
        }

        public Task<AuthResult<object>> OnSuccessLoginAsync(string did, string publicKey)
        {
            // mign be never called
            throw new NotImplementedException();
        }

        public async Task<AuthResult<object>> OnSuccessLoginByFido2Async(string fido2CredentialId, uint fido2SignCounter)
        {
            return await OnSuccessLoginInternalAsync(fido2CredentialId);
        }

        public async Task<AuthResult<object>> OnSuccessLoginByPublicKeyAsync(string publicKey)
        {
            return await OnSuccessLoginInternalAsync(publicKey);
        }

        public Task UpdateProfileAsync(IUserProfileFormContext<TProfile> context)
        {
            throw new NotImplementedException();
        }

        public Task UpgradeConnectionAsync(string did, OwnIdConnection newConnection)
        {

            // FOID2 and Apple products specific
            throw new NotImplementedException();
        }

        private async Task<AuthResult<object>> OnSuccessLoginInternalAsync(string fido2CredentialId = null, string publicKey = null)
        {
            return await Task.FromResult(new AuthResult<object>(new
            {
                idToken = _jwtService.GenerateDataJwt(new Dictionary<string, object>
                    {
                        {
                            "data", new
                            {
                                pubKey = publicKey,
                                fido2CredentialId = fido2CredentialId
                            }
                        }
                    })
            }));
        }
    }
}
