using Avalonia.Media.Imaging;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using ZIKM.Client.Infrastructure.Interfaces;
using ZIKM.Client.Services.Protectors;
using ZIKM.Infrastructure.DataStructures;

namespace ZIKM.Client.Services.Providers {
    class TcpProvider : IProvider {
        private readonly string _address;
        private readonly int _port;
        private TcpClient _client;
        private NetworkStream _stream;
        private Guid _guid;
        private readonly IProtector protector = new AesRsaProtector();

        public TcpProvider(string address, int port) {
            _address = address;
            _port = port;
            _client = new TcpClient(_address, _port);
            _stream = _client.GetStream();
        }

        /// <summary>
        /// Read server response
        /// </summary>
        /// <returns>Data of server response</returns>
        private byte[] ReadResponse() {
            // Read int value (contains 4 bytes)
            byte[] bufferSize = new byte[4];
            _stream.Read(bufferSize, 0, 4);

            byte[] data = new byte[BitConverter.ToInt32(bufferSize, 0)];
            _stream.Read(data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// Send data to server
        /// </summary>
        /// <param name="data">Sent data</param>
        private void SendData(ReadOnlySpan<byte> data) {
            _stream.Write(BitConverter.GetBytes(data.Length));
            _stream.Write(data);
        }

        public void PrepareProtecting() {
            protector.ExchangeKey(ReadResponse, SendData);
        }

        public ResponseData SendLoginRequest(LoginData login) {
            SendData(protector.Encrypt(JsonSerializer.SerializeToUtf8Bytes(login)));

            var response = GetResponse();
            if (response.Code == 0 && response.SessionId != Guid.Empty) {
                _guid = response.SessionId;
            }
            return response;
        }

        public void SendRequest(RequestData request) {
            request.SessionId = _guid;
            SendData(protector.Encrypt(JsonSerializer.SerializeToUtf8Bytes(request)));
        }

        public ResponseData GetResponse() {
            var data = protector.Decrypt(ReadResponse());
            return JsonSerializer.Deserialize<ResponseData>(data);
        }

        public IBitmap GetCaptcha() {
            var data = protector.Decrypt(ReadResponse());
            return new Bitmap(new MemoryStream(data.ToArray()));
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) { }

                _stream?.Close();
                _client?.Close();

                disposedValue = true;
            }
        }

        ~TcpProvider() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
