using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace letsdocoding_cognitologin.CognitoAccess
{
    /// <summary>
    /// cognito User Helper
    /// </summary>
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AmazonCognitoIdentityProviderClient client;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public UserHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            client = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), RegionEndpoint.APSouth1);
        }

        /// <inheritdoc />
        public async Task<string> GenerateSecret()
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var secretResponse = await client.AssociateSoftwareTokenAsync(new AssociateSoftwareTokenRequest()
            {
                AccessToken = token
            });
            return secretResponse.SecretCode;
        }

        /// <inheritdoc />
        public async Task<bool> VerifyToken(string userCode, string deviceName)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            try
            {
                var verificationResult = await client.VerifySoftwareTokenAsync(new VerifySoftwareTokenRequest
                {
                    UserCode = userCode,
                    AccessToken = token,
                    FriendlyDeviceName = deviceName
                });
                if (verificationResult.Status.Value == "SUCCESS")
                {
                    await client.SetUserMFAPreferenceAsync(new SetUserMFAPreferenceRequest()
                    {
                        AccessToken = token,
                        SoftwareTokenMfaSettings = new SoftwareTokenMfaSettingsType()
                        { Enabled = true, PreferredMfa = true }
                    });
                    return true;
                }
            }
            catch (CodeMismatchException) { }
            catch (EnableSoftwareTokenMFAException) { }
            return false;
        }
    }
}
