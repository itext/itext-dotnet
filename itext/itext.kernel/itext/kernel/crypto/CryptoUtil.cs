using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace iText.Kernel.Crypto {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that it's API and functionality may be changed in future.
    /// </summary>
    public static class CryptoUtil {

        public static X509Certificate ReadPublicCertificate(Stream s) {
            return new X509CertificateParser().ReadCertificate(s);
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        public static ICipherParameters ReadPrivateKeyFromPkcs12KeyStore(Stream keyStore, String pkAlias, char[] pkPassword) {
            return new Pkcs12Store(keyStore, pkPassword).GetKey(pkAlias).Key;
        }

    }
}