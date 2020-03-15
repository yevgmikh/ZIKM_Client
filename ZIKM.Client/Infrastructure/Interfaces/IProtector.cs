using System;

namespace ZIKM.Client.Infrastructure.Interfaces {
    /// <summary>
    /// Encrypts the sending data of the <see cref="IProvider"/>
    /// </summary>
    interface IProtector {
        /// <summary>
        /// Exchange cryptography keys with the server
        /// </summary>
        /// <param name="read">Reader server data</param>
        /// <param name="send">Sender data to server</param>
        void ExchangeKey(ReadData read, SendData send);
        /// <summary>
        /// Encrypt data
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <returns>Encrypt data</returns>
        ReadOnlySpan<byte> Encrypt(byte[] data);
        /// <summary>
        /// Decrypt data
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <returns>Decrypt data</returns>
        ReadOnlySpan<byte> Decrypt(byte[] data);
    }

    delegate byte[] ReadData();
    delegate void SendData(ReadOnlySpan<byte> data);
}
