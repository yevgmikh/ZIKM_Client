using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using ZIKM.Client.Infrastructure.Interfaces;

namespace ZIKM.Client.Services.Protectors {
    /// <summary>
    /// Encrypts the sending data of the <see cref="IProvider"/> by AES symmetric algorithm, 
    /// but gets encrypted by <see cref="RSA"/> algorithm key from server
    /// </summary>
    class AesRsaProtector : IProtector {
        private byte[] key;
        private byte[] vector;

        public void ExchangeKey(ReadData read, SendData send) {
            RSACryptoServiceProvider clientProvider = new RSACryptoServiceProvider(1024);

            send(JsonSerializer.SerializeToUtf8Bytes(clientProvider.ToXmlString(false)));

            var data = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(read());

            key = clientProvider.Decrypt(data["key"], false);
            vector = clientProvider.Decrypt(data["vector"], false);
        }

        public ReadOnlySpan<byte> Decrypt(byte[] data) {
            using AesManaged aesAlg = new AesManaged {
                Key = key,
                IV = vector
            };
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(data);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            byte[] buffer = new byte[1024];
            using MemoryStream ms = new MemoryStream();
            int numBytesRead;
            while ((numBytesRead = csDecrypt.Read(buffer, 0, buffer.Length)) > 0) {
                ms.Write(buffer, 0, numBytesRead);
            }
            return ms.ToArray();
        }

        public ReadOnlySpan<byte> Encrypt(byte[] data) {
            using AesManaged aesAlg = new AesManaged {
                Key = key,
                IV = vector
            };
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            csEncrypt.Write(data, 0, data.Length);
            csEncrypt.FlushFinalBlock();
            return msEncrypt.ToArray();
        }
    }
}
