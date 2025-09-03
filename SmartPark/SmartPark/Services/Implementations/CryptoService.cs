using SmartPark.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SmartPark.Services.Implementations
{
    public class CryptoService : ICryptoService
    {
        private readonly string _password;
        private readonly byte[] _saltBytes;
        public CryptoService(IConfiguration configuration)
        {
            var salt = configuration["CryptoSettings:Salt"]?? "";
            string pass = configuration["CryptoSettings:Password"]?? "";
            _password = pass;
            _saltBytes = Encoding.UTF8.GetBytes(salt);
        }

        public string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (var deriveBytes = new Rfc2898DeriveBytes(_password, _saltBytes, 1000))
            using (var aes = new RijndaelManaged())
            {
                aes.Key = deriveBytes.GetBytes(32); // 256-bit key
                aes.IV = deriveBytes.GetBytes(16);  // 128-bit IV
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public string Encrypt(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using (var deriveBytes = new Rfc2898DeriveBytes(_password, _saltBytes, 1000))
            using (var aes = new RijndaelManaged())
            {
                aes.Key = deriveBytes.GetBytes(32); // 256-bit key
                aes.IV = deriveBytes.GetBytes(16);  // 128-bit IV
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }


        }
    }
}
