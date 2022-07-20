using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>.
    /// </summary>
    public class OCSPResponseStatusBCFips : ASN1EncodableBCFips, IOCSPResponseStatus {
        private static readonly iText.Bouncycastlefips.Asn1.Ocsp.OCSPResponseStatusBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.Ocsp.OCSPResponseStatusBCFips
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
        public OCSPResponseStatusBCFips(OcspResponseStatus ocspResponseStatus)
            : base(ocspResponseStatus) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="OCSPResponseStatusBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Asn1.Ocsp.OCSPResponseStatusBCFips GetInstance() {
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
