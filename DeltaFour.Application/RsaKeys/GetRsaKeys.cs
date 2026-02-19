using System.Security.Cryptography;

namespace DeltaFour.Application.RsaKeys
{
    ///<summary>
    ///Operation get private or public keys
    ///</summary>
    public static class GetRsaKeys
    {
        public static RSA GetPrivateKey(string caminho)
        {
            var rsa = RSA.Create();
            var primaryKey = File.ReadAllText(caminho);
            rsa.ImportFromPem(primaryKey);
            return rsa;
        }
        
        public static RSA GetPublicKey(string caminho)
        {
            var rsa = RSA.Create();
            var publicKey = File.ReadAllText(caminho);
            rsa.ImportFromPem(publicKey);
            return rsa;
        }
    }
}
