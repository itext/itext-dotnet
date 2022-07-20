using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>.
    /// </summary>
    public class OCSPResponseStatusBC : ASN1EncodableBC, IOCSPResponseStatus {
        private static readonly iText.Bouncycastle.Asn1.Ocsp.OCSPResponseStatusBC INSTANCE = new iText.Bouncycastle.Asn1.Ocsp.OCSPResponseStatusBC
            (null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>.
        /// </summary>
        /// <param name="ocspResponseStatus">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>
        /// to be wrapped
        /// </param>
        public OCSPResponseStatusBC(OcspResponseStatus ocspResponseStatus)
            : base(ocspResponseStatus) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="OCSPResponseStatusBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.Ocsp.OCSPResponseStatusBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>.
        /// </returns>
        public virtual OcspResponseStatus GetOcspResponseStatus() {
            return (OcspResponseStatus)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetSuccessful() {
            return SUCCESSFUL;
        }
    }
}
