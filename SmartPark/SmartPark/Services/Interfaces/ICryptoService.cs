namespace SmartPark.Services.Interfaces
{
    public interface ICryptoService
    {
        public string Encrypt(string plainText);
        public string Decrypt(string cipherText);
    }
}
