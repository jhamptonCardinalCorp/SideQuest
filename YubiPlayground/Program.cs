
// Playground for YubiKey PIV signing using C# and .NET 8.0+

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public static byte[] SignWithYubiKey(byte[] dataToSign, string certSubjectContains)
{
    // 1) Find a smart-card backed cert in the current user's "My" store
    using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

    var cert = store.Certificates
        .Cast<X509Certificate2>()
        .Where(c => c.HasPrivateKey && c.Subject.Contains(certSubjectContains, StringComparison.OrdinalIgnoreCase))
        .OrderByDescending(c => c.NotBefore)
        .FirstOrDefault() ?? throw new InvalidOperationException("Certificate not found.");

    // 2) Get the RSA private key (Smart Card KSP / Base CSP will route to YubiKey)
    using RSA rsa = cert.GetRSAPrivateKey(); // returns RSACng or RSACryptoServiceProvider on Windows
    // If the card requires PIN, Windows will prompt the Smart Card PIN dialog.

    // 3) Sign the data (PKCS#1 v1.5 with SHA-256, adjust as needed)
    return rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
}
