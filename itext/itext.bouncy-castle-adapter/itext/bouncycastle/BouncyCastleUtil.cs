using System.Collections.Generic;
using System.IO;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle {
    internal class BouncyCastleUtil : IBouncyCastleUtil {

        internal BouncyCastleUtil() {
            // Empty constructor.
        }

        /// <summary><inheritDoc/></summary>
        public virtual List<IX509Certificate> ReadPkcs7Certs(Stream data) {
            using (TextReader file = new StreamReader(data)) {
                PemReader reader = new PemReader(file, new BouncyCastleFactory.BouncyCastlePasswordFinder(null));
                object obj = reader.ReadObject();
                if (!(obj is ContentInfo)) {
                    return new List<IX509Certificate>();
                }
                CmsSignedData cmsSignedData = new CmsSignedData((ContentInfo)obj);
                IEnumerable<X509Certificate> certs = cmsSignedData.GetCertificates().EnumerateMatches(null);
                List<IX509Certificate> certsWrappers = new List<IX509Certificate>();
                foreach (X509Certificate certificate in certs) {
                    certsWrappers.Add(new X509CertificateBC(certificate));
                }

                return certsWrappers;
            }
        }
    }
}