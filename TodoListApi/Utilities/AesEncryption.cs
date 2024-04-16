using System.Security.Cryptography;
using System.Text;
using TodoListApi.Config;

namespace TodoListApi.Utilities
{
    /*
        https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
        https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-8.0
    */
    public class AesEncryption
    {
        private readonly ApplicationConfig config;
        public AesEncryption (ApplicationConfig _config){
            config = _config;
        }
        public string Encrypt(string plainText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(config.AesEncryptionConfig.Key);
            aesAlg.IV = Encoding.UTF8.GetBytes(config.AesEncryptionConfig.Iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            byte[] encryptedBytes;
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }
            }
            return Convert.ToBase64String(encryptedBytes);
        }

    public string Decrypt(string encryptedText)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(config.AesEncryptionConfig.Key);
        aesAlg.IV = Encoding.UTF8.GetBytes(config.AesEncryptionConfig.Iv);

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        using (var msDecrypt = new MemoryStream(encryptedBytes))
        {
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
    }
}