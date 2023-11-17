using System.Collections.Generic;
using System.IO;
using iText.Bouncycastlefips.Cert;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.OpenSsl;

namespace iText.Bouncycastlefips {
    internal class BouncyCastleFipsUtil : IBouncyCastleUtil {
        
        internal BouncyCastleFipsUtil() {
            // Empty constructor.
        }

        /// <summary><inheritDoc/></summary>
        public virtual List<IX509Certificate> ReadPkcs7Certs(Stream data) {
            using (TextReader file = new StreamReader(data)) {
                OpenSslPemReader reader = new OpenSslPemReader(file);
                object obj = reader.ReadObject();
                if (!(obj is ContentInfo)) {
                    return new List<IX509Certificate>();
                }
                CmsSignedData cmsSignedData = new CmsSignedData(((ContentInfo)obj).GetEncoded());
                ICollection<X509Certificate> certs = cmsSignedData.GetCertificates().GetMatches(null);
                List<IX509Certificate> certsWrappers = new List<IX509Certificate>();
                foreach (X509Certificate certificate in certs) {
                    certsWrappers.Add(new X509CertificateBCFips(certificate));
                }

                return certsWrappers;
            }
        }
    }
}