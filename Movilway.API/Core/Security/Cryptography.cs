using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Movilway.API.Core.Security
{
    public  class   Cryptography
    {
        //es la llave configurable
        public const  String KEY = "oWy4T0o2XYRzR92rjIt6RwndVsa4WRCa";

        /// <summary>
        /// Encrypta  un texto con la la llave por defecto
        /// </summary>
        /// <param name="plainText">Texto a encriptar</param>
        /// <returns></returns>
        internal static string encrypt(String plainText)
        {
            return encrypt(KEY,  plainText);
        }


        /// <summary>
        /// Encrypta  un texto con la la llave indicada
        /// </summary>
        /// <param name="key">Llave para encriptar</param>
        /// <param name="plainText">Texto a encriptar</param>
        /// <returns>Texto encriptado en base 64</returns>
        public static string encrypt(String key, String plainText)
        {
            // Create a new instance of the AesManaged class.  This generates a new key and initialization vector (IV).
            AesManaged aesManager = new AesManaged();

            // Override the cipher mode, key and IV
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            aesManager.Mode = CipherMode.ECB;
            aesManager.IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            aesManager.Key = encoding.GetBytes(key);
            aesManager.Padding = PaddingMode.PKCS7;

            #if DEBUG
                //Console.WriteLine("Encrypting text: \"" + plainText + "\"");
            #endif
            // Encryption
            String encryptedText = null;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (ICryptoTransform encryptor = aesManager.CreateEncryptor())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                }
                encryptedText = Convert.ToBase64String(msEncrypt.ToArray());
            }
            #if DEBUG
                //Console.WriteLine("Encrypted text: \"" + encryptedText + "\"");
            #endif

            // free resources
            aesManager.Clear();

            return encryptedText;
        }
        /// <summary>
        /// Des encrypta un texto con la la llave por defecto
        /// </summary>
        /// <param name="encryptedText">texto a desencriptar en base 64</param>
        /// <returns>Cadena desencriptada</returns>
        internal static string decrypt(String encryptedText)
        {
            return decrypt(KEY, encryptedText);
        }


        /// <summary>
        /// Desencripta un texto plano con la llave indicada
        /// </summary>
        /// <param name="key">llave para desencriptar</param>
        /// <param name="encryptedText">Texto a desencriptar en base 64</param>
        /// <returns>Cadena desencriptada</returns>
        public static string decrypt(String key, String encryptedText)
        {
            // Create a new instance of the AesManaged class.  This generates a new key and initialization vector (IV).
            AesManaged aesManager = new AesManaged();

            // Override the cipher mode, key and IV
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            aesManager.Mode = CipherMode.ECB;
            aesManager.IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            aesManager.Key = encoding.GetBytes(key);
            aesManager.Padding = PaddingMode.PKCS7;

            #if DEBUG
                //Console.WriteLine("Decrypting text: \"" + encryptedText + "\"");
            #endif
            // Decryption
            String decryptedText = null;
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (ICryptoTransform decryptor = aesManager.CreateDecryptor())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                        {
                            decryptedText = swDecrypt.ReadToEnd();
                        }
                    }
                }
                }
            #if DEBUG
                //Console.WriteLine("Decrypted text: \"" + decryptedText + "\"");
            #endif

            // free resources
            aesManager.Clear();

            return decryptedText;
        }

    }

}