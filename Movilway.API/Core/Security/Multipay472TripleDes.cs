// <copyright file="Multipay472TripleDes.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Core.Security
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    /// <summary>
    /// Clase con los métodos de encriptación/desencriptación para Multipay472
    /// </summary>
    public class Multipay472TripleDes
    {
        /// <summary>
        /// Encrypta  un texto con la la llave indicada
        /// </summary>
        /// <param name="key">Llave para encriptar</param>
        /// <param name="plainText">Texto a encriptar</param>
        /// <returns>Texto encriptado en base 64</returns>
        public static string Encrypt(string key, string plainText)
        {
            TripleDES tdesManager = new TripleDESCryptoServiceProvider();
            tdesManager.Mode = CipherMode.ECB;
            tdesManager.Padding = PaddingMode.None;

            byte[] keyBytes = HexStringToBytes(key);

            plainText = plainText.PadLeft(keyBytes.Length, '0');
            byte[] messageBytes = HexStringToBytes(plainText);

            string encryptedText = null;
            byte[] encryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (ICryptoTransform encryptor = tdesManager.CreateEncryptor(keyBytes, keyBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(messageBytes, 0, messageBytes.Length);
                        cs.FlushFinalBlock();
                    }
                }

                encryptedBytes = ms.ToArray();
            }

            tdesManager.Clear();
            encryptedText = ArrayToHexString(encryptedBytes);

            return encryptedText;
        }

        /// <summary>
        /// Desencripta un texto plano con la llave indicada
        /// </summary>
        /// <param name="key">llave para desencriptar</param>
        /// <param name="encryptedText">Texto a desencriptar en base 64</param>
        /// <returns>Cadena desencriptada</returns>
        public static string Decrypt(string key, string encryptedText)
        {
            TripleDESCryptoServiceProvider tdesManager = new TripleDESCryptoServiceProvider();
            tdesManager.Mode = CipherMode.ECB;
            tdesManager.Padding = PaddingMode.None;

            byte[] keyBytes = HexStringToBytes(key);
            byte[] messageBytes = HexStringToBytes(encryptedText);

            string decryptedText = null;
            byte[] decryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (ICryptoTransform decryptor = tdesManager.CreateDecryptor(keyBytes, keyBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(messageBytes, 0, messageBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            tdesManager.Clear();
            decryptedText = ArrayToHexString(decryptedBytes);

            return decryptedText.TrimStart('0');
        }

        /// <summary>
        /// Convierte un string en formato HEX a un array de bytes
        /// </summary>
        /// <param name="str">string en formato HEX</param>
        /// <returns>Cadena de bytes que representan el string</returns>
        private static byte[] HexStringToBytes(string str)
        {
            return Enumerable.Range(0, str.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(str.Substring(x, 2), 16))
                     .ToArray();
        }

        /// <summary>
        /// Convierte una cadena de bytes a su representación en formato HEX
        /// </summary>
        /// <param name="array">Cadena de bytes</param>
        /// <returns>Cadena de bytes que representación en formato HEX</returns>
        private static string ArrayToHexString(byte[] array)
        {
            return string.Concat(Array.ConvertAll(array, x => x.ToString("X2")));
        }
    }
}
