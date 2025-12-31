Short answer: **Yes.** Your YubiKey 5C NFC can hold keys/certificates and perform **hardware-backed signing and decryption** that you can call directly from C#—most commonly via the **PIV (smart card)** application on the key. You have two primary integration paths:

***

## 1) Windows-native smart card (PIV) with .NET (recommended on Windows)

If you install the **YubiKey Smart Card Minidriver**, Windows sees the YubiKey as a standard PIV smart card. Your C# app can then use the usual .NET crypto APIs (`X509Certificate2`, `GetRSAPrivateKey`, `RSACng`, `SignedCms`, `SignedXml`, etc.) and Windows will route the signing operation to the YubiKey—**the private key never leaves the device**. [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/minidriver/md_introduction.html), [\[learn.microsoft.com\]](https://learn.microsoft.com/en-us/windows-hardware/drivers/smartcard/smart-card-minidrivers)

**What you need**

*   **YubiKey Smart Card Minidriver** (lets Windows enroll/manage certs directly on the key; supports PIN, certificate enrollment and smart-card auth). [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/minidriver/md_introduction.html), [\[yubico.com\]](https://www.yubico.com/support/download/smart-card-drivers-tools/)
*   A certificate/key pair **in a PIV slot** (e.g., 9a for authentication, 9c for signing). You can enroll via Windows (with the Minidriver) or load with Yubico tools. [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/minidriver/md_introduction.html), [\[yubico.com\]](https://www.yubico.com/support/download/smart-card-drivers-tools/)

> If you’re in an AD domain, Yubico has step-by-step docs for standing up a CA, enabling ECC logon, and self-enrolling certificates onto YubiKeys. [\[support.yubico.com\]](https://support.yubico.com/s/article/Setting-up-Windows-Server-for-YubiKey-PIV-authentication), [\[yubico.zendesk.com\]](https://yubico.zendesk.com/hc/en-us/articles/360015654500-Setting-up-Windows-Server-for-YubiKey-PIV-authentication)

**How to sign data in C#:**

```csharp
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
```

*   `GetRSAPrivateKey()` is the idiomatic way to access a certificate’s signing key in modern .NET. If the certificate is “smart card” backed, the call uses Windows’ Smart Card KSP/CSP under the hood. [\[learn.microsoft.com\]](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.rsacertificateextensions.getrsaprivatekey?view=net-10.0), [\[learn.microsoft.com\]](https://learn.microsoft.com/en-us/windows-hardware/drivers/smartcard/smart-card-minidrivers)
*   If you need to **pre-supply the PIN programmatically** (avoid the UI), you can set the **CNG key property** `SmartCardPin` on `RSACng` before calling `SignData`. [\[stackoverflow.com\]](https://stackoverflow.com/questions/61462796/setting-smartcard-pin-programmatically-using-getrsaprivatekey-and-net-core)

> Yubico provides a sample solution that demonstrates signing data with **YubiKey PIV** through the Microsoft Base Smart Card crypto provider (C#). Handy as a reference. [\[github.com\]](https://github.com/YubicoLabs/Microsoft.Net-Crypto-YubiKey)

**Where the cert comes from**

*   With the **Minidriver**, Windows can enroll a certificate directly **onto the YubiKey**; certificates then appear in the user’s cert store and are bound to smart-card key containers. Your app doesn’t need to know about “PIV slots”—Windows abstracts that for you. [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/minidriver/md_introduction.html), [\[stackoverflow.com\]](https://stackoverflow.com/questions/61462176/uwp-application-piv-example-needed)

***

## 2) PKCS#11 (cross‑platform) via YKCS11 and .NET wrappers

If you prefer a **vendor-agnostic** approach or need to run on macOS/Linux, use **PKCS#11**. Yubico ships **YKCS11**, a PKCS#11 module for the PIV application. In .NET, you can load that module with **Pkcs11Interop** and call `C_Sign` (RSA/ECDSA) directly against keys in the YubiKey’s PIV slots. [\[developers...yubico.com\]](https://developers.yubico.com/yubico-piv-tool/YKCS11/), [\[github.com\]](https://github.com/Pkcs11Interop/Pkcs11Interop)

*   **YKCS11** is included with the Yubico PIV Tool install; on Windows you’ll reference `libykcs11.dll` and ensure its dependencies are in `PATH` (or next to your app). [\[developers...yubico.com\]](https://developers.yubico.com/yubico-piv-tool/YKCS11/)
*   **Pkcs11Interop** is a mature .NET wrapper for PKCS#11 and specifically supports **YubiKey PIV**. [\[github.com\]](https://github.com/Pkcs11Interop/Pkcs11Interop)

This route is ideal if you need fine-grained control, non-Windows platforms, or want to avoid the Windows certificate store. (Many document-signing libraries show how to wire PKCS#11 tokens into their signing flow.) [\[blog.groupdocs.com\]](https://blog.groupdocs.com/signature/sign-documents-with-pkcs11-dotnet/), [\[bitmiracle.com\]](https://bitmiracle.com/pdf-library/signatures/external-signing)

***

## What about FIDO2, OTP, and OpenPGP?

*   **FIDO2 / Security Key**: Designed for **authentication** (passkeys, WebAuthn). It **does not expose a general-purpose signing API** for applications like document signing. Use **PIV** for that. [\[docs.yubico.com\]](https://docs.yubico.com/)
*   **OTP**: Emits one-time codes/strings; **not** suitable for signing. [\[docs.yubico.com\]](https://docs.yubico.com/)
*   **OpenPGP**: The YubiKey’s OpenPGP app can sign content (e.g., Git commits) via GPG tooling, but typical C# apps integrate with OpenPGP through command-line or native bindings—not as seamless as PIV on Windows. (Yubico’s docs group OpenPGP under “apps,” while the Minidriver/PIV path is best documented for Windows.) [\[docs.yubico.com\]](https://docs.yubico.com/)

***

## Practical setup steps for you (Josh)

Since you’re on Windows and already tested **Yubico Login for Windows**, It’s suggest we use the **Windows-native PIV route** for C#:

1.  **Install the YubiKey Smart Card Minidriver** on your workstation(s).
    *   Download from Yubico “Smart card drivers and tools” and install the MSI; reboot if prompted. [\[yubico.com\]](https://www.yubico.com/support/download/smart-card-drivers-tools/), [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/minidriver/md_manual_install.html)

2.  **Provision a signing certificate on the YubiKey (PIV)**
    *   In a domain: follow Yubico’s “Setting up Windows Server for YubiKey PIV authentication” to enroll certs (supports RSA or ECC).
    *   Standalone: use **Yubico PIV Tool** or **Yubico Authenticator (PIV Certificates)** to generate/import keys and certificates, then they’ll surface in Windows via certificate propagation. [\[support.yubico.com\]](https://support.yubico.com/s/article/Setting-up-Windows-Server-for-YubiKey-PIV-authentication), [\[yubico.zendesk.com\]](https://yubico.zendesk.com/hc/en-us/articles/360015654500-Setting-up-Windows-Server-for-YubiKey-PIV-authentication), [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/authenticator/auth-guide/piv-certificates.html)

3.  **Sign from C#** using the snippet above (`X509Store` → `GetRSAPrivateKey` → `SignData`).
    *   If PIN prompts are undesirable in a service context, set `SmartCardPin` on the `RSACng.Key`. [\[stackoverflow.com\]](https://stackoverflow.com/questions/61462796/setting-smartcard-pin-programmatically-using-getrsaprivatekey-and-net-core)

4.  (Optional) **Use SignedCms / SignedXml for CMS/XML signatures**, or third-party PDF APIs that support external signing—Windows will still route to the YubiKey. [\[learn.microsoft.com\]](https://learn.microsoft.com/en-us/windows-hardware/drivers/smartcard/smart-card-minidrivers)

***

## A slightly fuller example: CMS (PKCS#7) signing with the YubiKey

```csharp
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

public static byte[] CreatePkcs7Signature(byte[] content, string certSubjectContains)
{
    // Find cert
    using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
    store.Open(OpenFlags.ReadOnly);
    var cert = store.Certificates
        .Find(X509FindType.FindBySubjectName, certSubjectContains, validOnly: false)
        .OfType<X509Certificate2>()
        .FirstOrDefault(c => c.HasPrivateKey)
        ?? throw new InvalidOperationException("Signing cert not found.");

    // Prepare content
    var cms = new SignedCms(new ContentInfo(content), detached: true);
    var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, cert)
    {
        IncludeOption = X509IncludeOption.EndCertOnly
    };

    // Windows will prompt for the smart-card PIN here if required
    cms.ComputeSignature(signer);
    return cms.Encode();
}
```

Under the covers, this uses the Windows Smart Card Base CSP/KSP + Minidriver to talk to the YubiKey. [\[learn.microsoft.com\]](https://learn.microsoft.com/en-us/windows-hardware/drivers/smartcard/smart-card-minidrivers), [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/minidriver/md_introduction.html)

***

## Gotchas & tips

*   **PIN / PUK / Management Key**: For PIV, keep track of your PIN/PUK and management key; defaults exist but should be changed. (Yubico Authenticator’s PIV Certificates guide explains each and when they’re required.) [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/authenticator/auth-guide/piv-certificates.html)
*   **Driver installation details**: For servers/remote scenarios, the Minidriver MSI offers `INSTALL_LEGACY_NODE=1` to avoid RDP key-container errors; see the manual install page. [\[docs.yubico.com\]](https://docs.yubico.com/software/yubikey/tools/minidriver/md_manual_install.html)
*   **ECC vs RSA**: If you enroll ECC certs for logon/signing, enable ECC cert enumeration/logon via GPO/registry as per Yubico’s guide. [\[support.yubico.com\]](https://support.yubico.com/s/article/Setting-up-Windows-Server-for-YubiKey-PIV-authentication)
*   **FIDO2 coexistence**: You can keep Security Key (FIDO2) for account login while using **PIV** for app-level signing—they’re independent functions on the same key. [\[docs.yubico.com\]](https://docs.yubico.com/)

***

If you tell me **what you want to sign** (e.g., PDFs, XML, JWTs, code-signing), I can tailor an example—either **Windows-native (PIV)** or **PKCS#11**—to your exact use case and drop in a ready-to-run C# snippet.
