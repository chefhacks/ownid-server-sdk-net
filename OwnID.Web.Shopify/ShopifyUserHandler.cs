using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OwnID.Extensibility.Flow;
using OwnID.Extensibility.Flow.Abstractions;
using OwnID.Extensibility.Flow.Contracts;
using OwnID.Extensibility.Flow.Contracts.Fido2;
using OwnID.Extensibility.Flow.Contracts.Internal;
using OwnID.Web.Shopify.Services;

namespace OwnID.Web.Shopify
{
    public class ShopifyUserHandler : IUserHandler<ShopifyUserProfile>
    {
        private readonly ICustomerService _customerService;

        public ShopifyUserHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }


        public Task CreateProfileAsync(IUserProfileFormContext<ShopifyUserProfile> context, string recoveryToken = null,
            string recoveryData = null)
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

        public Task<ConnectionRecoveryResult<ShopifyUserProfile>> GetConnectionRecoveryDataAsync(string recoveryToken,
            bool includingProfile = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserIdByEmail(string email)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserSettings> GetUserSettingsAsync(string publicKey)
        {
            return Task.FromResult(new UserSettings());
        }

        public Task UpgradeConnectionAsync(string did, OwnIdConnection newConnection)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserNameAsync(string did)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> RegisterUserAsync(string email, string key, string publicKey)
        {
            //key = "12345678901234567890";
            key = key.Substring(0, 16);

            var password = Guid.NewGuid().ToString("N"); //"superPassword";
            var id = await _customerService.CreateCustomer(email, password);

            // Encrypt password
            var encryptedPassword = EncryptString(password, key);

            var response = await _customerService.UpdateCustomer(id, publicKey, encryptedPassword);


            // await Task.Delay(2000);
            //
            // var encryptedPasswordFromShopify = await _customerService.FindCustomerPasswordAsync(email);
            // var decryptedPassword = DecryptString(encryptedPasswordFromShopify, key);

            return password;
        }

        public async Task<string> FindUserPassword(string email, string key)
        {
            //key = "12345678901234567890";
            key = key.Substring(0, 16);

            var encryptedPasswordFromShopify = await _customerService.FindCustomerPasswordAsync(email);
            var decryptedPassword = DecryptString(encryptedPasswordFromShopify, key);

            return decryptedPassword;
        }

        private static string EncryptString(string text, string keyString)
        {
            return AesOperation.EncryptString(keyString, text);
            
            
            
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        private static string DecryptString(string cipherText, string keyString)
        {
            return AesOperation.DecryptString(keyString, cipherText);
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        result = srDecrypt.ReadToEnd();
                    }
                }
            }

            return result;
        }
    }

    public class ShopifyUserProfile
    {
        public string Email { get; set; }
    }
    
   public static class AesOperation  
    {  
        public static string EncryptString(string key, string plainText)  
        {  
            byte[] iv = new byte[16];  
            byte[] array;  
  
            using (Aes aes = Aes.Create())  
            {  
                aes.Key = Encoding.UTF8.GetBytes(key);  
                aes.IV = iv;  
  
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);  
  
                using (MemoryStream memoryStream = new MemoryStream())  
                {  
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))  
                    {  
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))  
                        {  
                            streamWriter.Write(plainText);  
                        }  
  
                        array = memoryStream.ToArray();  
                    }  
                }  
            }  
  
            return Convert.ToBase64String(array);  
        }  
  
        public static string DecryptString(string key, string cipherText)  
        {  
            byte[] iv = new byte[16];  
            byte[] buffer = Convert.FromBase64String(cipherText);  
  
            using (Aes aes = Aes.Create())  
            {  
                aes.Key = Encoding.UTF8.GetBytes(key);  
                aes.IV = iv;  
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);  
  
                using (MemoryStream memoryStream = new MemoryStream(buffer))  
                {  
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))  
                    {  
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))  
                        {  
                            return streamReader.ReadToEnd();  
                        }  
                    }  
                }  
            }  
        }  
    }  
}