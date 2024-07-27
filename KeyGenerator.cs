using System;
using System.Security.Cryptography;

public class KeyGenerator
{
    public static void Main()
    {
        using (var rsa = RSA.Create(2048))
        {
            // Export the public key
            var publicKey = rsa.ExportRSAPublicKey();
            var publicKeyBase64 = Convert.ToBase64String(publicKey);

            // Export the private key
            var privateKey = rsa.ExportRSAPrivateKey();
            var privateKeyBase64 = Convert.ToBase64String(privateKey);

            Console.WriteLine("RSA Public Key (Base64): ");
            Console.WriteLine(publicKeyBase64);

            Console.WriteLine("RSA Private Key (Base64): ");
            Console.WriteLine(privateKeyBase64);
        }
    }
}
