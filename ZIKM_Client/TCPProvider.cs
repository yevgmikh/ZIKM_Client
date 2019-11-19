using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ZIKM_Client.Infrastructure;
using ZIKM_Client.Interfaces;

namespace ZIKM_Client
{
    public class TCPProvider : IProvider
    {
        private readonly string _address;
        private readonly int _port;
        private TcpClient _client;
        private NetworkStream _stream;

        /// <summary>
        /// Log provider data
        /// </summary>
        public event Action<string> ProviderLog = (string name) => { };

        public TCPProvider(string address, int port)
        {
            _address = address;
            _port = port;
            _client = new TcpClient(_address, _port);
            _stream = _client.GetStream();
        }

        /// <summary>
        /// Send Json data to server
        /// </summary>
        /// <param name="json">Json string with data</param>
        private void SendData(string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            _stream.Write(data, 0, data.Length);
            ProviderLog(json);
        }

        /// <summary>
        /// Send login-request to server
        /// </summary>
        /// <param name="login">Request data</param>
        /// <returns>Data of login response</returns>
        public ResponseData SendLoginRequest(LoginData login)
        {
            SendData(JsonConvert.SerializeObject(login));
            ResponseData response = GetResponse();
            if (response.Code == -2)
            {
                // Reconnecting because server close connection if code -2
                _stream.Close();
                _client.Close();
                _client = new TcpClient(_address, _port);
                _stream = _client.GetStream();
            }
            return response;
        }

        /// <summary>
        /// Send request to server
        /// </summary>
        /// <param name="request">Request data</param>
        public void SendRequest(RequestData request)
        {
            SendData(JsonConvert.SerializeObject(request));
        }

        /// <summary>
        /// Get data from server response
        /// </summary>
        /// <returns>Data of server's response</returns>
        public ResponseData GetResponse()
        {
            var data = new byte[100000];
            int bytes = _stream.Read(data, 0, data.Length);

            string json = Encoding.UTF8.GetString(data, 0, bytes);
            ProviderLog(json);
            return JsonConvert.DeserializeObject<ResponseData>(json);
        }

        /// <summary>
        /// Get captcha from server 
        /// </summary>
        /// <returns>Image of aptcha</returns>
        public IBitmap GetCaptcha()
        {
            byte[] data = new byte[5000];
            _stream.Read(data, 0, data.Length);
            return new Bitmap(new MemoryStream(data));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                SendData("Close");
                _stream.Close();
                _client.Close();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~TCPProvider()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
