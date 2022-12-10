using System.Collections.Generic;
using System.IO;
using iText.Bouncycastlefips.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Utilities.IO;

namespace iText.Bouncycastlefips.X509 {
    /// <summary>
    /// Class to mimic X509CertificateParser logic.
    /// </summary>
    public class X509CertificateParserBCFips : IX509CertificateParser {

        private Asn1Set sData;
        
        private int sDataObjectCount;
        
        /// <summary>
        /// Creates new parser instance.
        /// </summary>
        public X509CertificateParserBCFips() {
            // Empty constructor.
        }
        
        /// <summary><inheritDoc/></summary>
        public List<IX509Certificate> ReadAllCerts(byte[] contentsKey) {
            List<IX509Certificate> certs = new List<IX509Certificate>();
            sData = null;
            sDataObjectCount = 0;
            X509Certificate cert;
            MemoryStream stream = new MemoryStream(contentsKey);
            while ((cert = ReadCertificate(stream)) != null) {
                certs.Add(new X509CertificateBCFips(cert));
            }
            return certs;
        }
        
        private X509Certificate ReadCertificate(MemoryStream stream) {
            if (sData != null) {
                if (sDataObjectCount != sData.Count) {
                    return GetCertificate();
                }

                sData = null;
                sDataObjectCount = 0;
                return null;
            }
            
            PushbackStream pushbackStream = new PushbackStream(stream);
            int tag = pushbackStream.ReadByte();
            if (tag <= 0) {
                return null;
            }
            pushbackStream.Unread(tag);
            Asn1Sequence seq = (Asn1Sequence)(new Asn1InputStream(pushbackStream).ReadObject());
            if (seq.Count > 1 && seq[0] is DerObjectIdentifier) {
                if (seq[0].Equals(PkcsObjectIdentifiers.SignedData)) {
                    sData = SignedData.GetInstance(
                        Asn1Sequence.GetInstance((Asn1TaggedObject) seq[1], true)).Certificates;
                    return GetCertificate();
                }
            }
            return new X509Certificate(X509CertificateStructure.GetInstance(seq));
        }

        private X509Certificate GetCertificate() {
            if (sData != null) {
                while (sDataObjectCount < sData.Count) {
                    object obj = sData[sDataObjectCount++];
                    if (obj is Asn1Sequence) {
                        return new X509Certificate(X509CertificateStructure.GetInstance(obj));
                    }
                }
            }

            return null;
        }
    }
}