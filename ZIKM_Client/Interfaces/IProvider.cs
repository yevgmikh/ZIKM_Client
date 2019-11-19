using Avalonia.Media.Imaging;
using System;
using ZIKM_Client.Infrastructure;

namespace ZIKM_Client.Interfaces
{
    public interface IProvider : IDisposable
    {
        /// <summary>
        /// Send login-request to server
        /// </summary>
        /// <param name="login">Request data</param>
        /// <returns>Data of login response</returns>
        ResponseData SendLoginRequest(LoginData login);
        /// <summary>
        /// Send request to server
        /// </summary>
        /// <param name="request">Request data</param>
        void SendRequest(RequestData request);
        /// <summary>
        /// Get data from server response
        /// </summary>
        /// <returns>Data of server's response</returns>
        ResponseData GetResponse();
        /// <summary>
        /// Get captcha from server 
        /// </summary>
        /// <returns>Image of aptcha</returns>
        IBitmap GetCaptcha();
    }
}
