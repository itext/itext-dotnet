using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.CrlReason"/>.
    /// </summary>
    public class CRLReasonBC : ASN1EncodableBC, ICRLReason {
        private static readonly iText.Bouncycastle.Asn1.X509.CRLReasonBC INSTANCE = new iText.Bouncycastle.Asn1.X509.CRLReasonBC
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
        public CRLReasonBC(CrlReason reason)
            : base(reason) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="CRLReasonBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.X509.CRLReasonBC GetInstance() {
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
