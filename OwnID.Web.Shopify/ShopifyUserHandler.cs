using System.Threading.Tasks;
using OwnID.Extensibility.Flow;
using OwnID.Extensibility.Flow.Abstractions;
using OwnID.Extensibility.Flow.Contracts;
using OwnID.Extensibility.Flow.Contracts.Fido2;
using OwnID.Extensibility.Flow.Contracts.Internal;

namespace OwnID.Web.Shopify
{
    public class ShopifyUserHandler : IUserHandler<ShopifyUserProfile>
    {
        public Task CreateProfileAsync(IUserProfileFormContext<ShopifyUserProfile> context, string recoveryToken = null, string recoveryData = null)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateProfileAsync(IUserProfileFormContext<ShopifyUserProfile> context)
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthResult<object>> OnSuccessLoginAsync(string did, string publicKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthResult<object>> OnSuccessLoginByPublicKeyAsync(string publicKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthResult<object>> OnSuccessLoginByFido2Async(string fido2CredentialId, uint fido2SignCounter)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentitiesCheckResult> CheckUserIdentitiesAsync(string did, string publicKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsUserExists(string publicKey)
        {
            return Task.FromResult(false);
        }

        public Task<bool> IsFido2UserExists(string fido2CredentialId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Fido2Info> FindFido2Info(string fido2CredentialId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ConnectionRecoveryResult<ShopifyUserProfile>> GetConnectionRecoveryDataAsync(string recoveryToken, bool includingProfile = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserIdByEmail(string email)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserSettings> GetUserSettingsAsync(string publicKey)
        {
            throw new System.NotImplementedException();
        }

        public Task UpgradeConnectionAsync(string did, OwnIdConnection newConnection)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserNameAsync(string did)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ShopifyUserProfile
    {
     public   string Email { get; set; }
    }
}