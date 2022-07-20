using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.CrlReason"/>.
    /// </summary>
    public class CRLReasonBCFips : ASN1EncodableBCFips, ICRLReason {
        private static readonly iText.Bouncycastlefips.Asn1.X509.CRLReasonBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.CRLReasonBCFips
            (null);

        private const int KEY_COMPROMISE = Org.BouncyCastle.Asn1.X509.CrlReason.KeyCompromise;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlReason"/>.
        /// </summary>
        /// <param name="reason">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlReason"/>
        /// to be wrapped
        /// </param>
        public CRLReasonBCFips(CrlReason reason)
            : base(reason) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="CRLReasonBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Asn1.X509.CRLReasonBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlReason"/>.
        /// </returns>
        public virtual CrlReason GetCRLReason() {
            return (CrlReason)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetKeyCompromise() {
            return KEY_COMPROMISE;
        }
    }
}
