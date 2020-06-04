using System;
namespace BiometricAuthentication.NetCore
{
    public struct AuthenticationResult
    {
        /// <summary>
        /// Whether the Authentication Result is successful
        /// </summary>
        public bool Success;

        /// <summary>
        /// Any message associated with the authentication result. Typically used for error messages.
        /// </summary>
        public string Message;

        public AuthenticationResult(bool success, string message = null)
        {
            Success = success;
            Message = message;
        }
    }
}
