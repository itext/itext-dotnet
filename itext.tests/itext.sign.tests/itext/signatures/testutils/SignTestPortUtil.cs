using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;

namespace iText.Signatures.Testutils {
    public class SignTestPortUtil {
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        /// <exception cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        public static CertificateID GenerateCertificateId(X509Certificate issuerCert, BigInteger serialNumber, String hashAlgorithm) {
            return new CertificateID(hashAlgorithm, issuerCert, serialNumber);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        public static OcspReq GenerateOcspRequestWithNonce(CertificateID id) {
            OcspReqGenerator gen = new OcspReqGenerator();
            gen.AddRequest(id);

            // create details for nonce extension
            IDictionary extensions = new Hashtable();

            extensions[OcspObjectIdentifiers.PkixOcspNonce] = new X509Extension(false, new DerOctetString(new DerOctetString(PdfEncryption.GenerateNewDocumentId()).GetEncoded()));

            gen.SetRequestExtensions(new X509Extensions(extensions));
            return gen.Generate();
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        public static IDigest GetMessageDigest(String hashAlgorithm) {
            return DigestUtilities.GetDigest(hashAlgorithm);
        }

        /// <exception cref="Java.Security.Cert.CertificateException"/>
        /// <exception cref="Java.Security.Cert.CRLException"/>
        public static X509Crl ParseCrlFromStream(Stream input) {
            return new X509CrlParser().ReadCrl(input);
        }
    }
}
