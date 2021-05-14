using System.Threading.Tasks;

namespace letsdocoding_cognitologin.CognitoAccess
{
    /// <summary>
    /// User Helper
    /// </summary>
    public interface IUserHelper
    {
        
        /// <summary>
        /// Generate Secret for the logged in User
        /// </summary>
        /// <returns></returns>
        Task<string> GenerateSecret();
        /// <summary>
        /// Verify Generated opt against the token for the logged in user
        /// </summary>
        /// <param name="userCode">Generated OTP</param>
        /// <param name="deviceName">device name </param>
        /// <returns></returns>
        Task<bool> VerifyToken(string userCode, string deviceName);
    }
}
