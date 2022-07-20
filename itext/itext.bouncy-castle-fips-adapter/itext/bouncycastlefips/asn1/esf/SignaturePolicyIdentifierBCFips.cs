using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastlefips.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>.
    /// </summary>
    public class SignaturePolicyIdentifierBCFips : ASN1EncodableBCFips, ISignaturePolicyIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>.
        /// </summary>
        /// <param name="signaturePolicyIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>
        /// to be wrapped
        /// </param>
        public SignaturePolicyIdentifierBCFips(SignaturePolicyIdentifier signaturePolicyIdentifier)
            : base(signaturePolicyIdentifier) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>.
        /// </summary>
        /// <param name="signaturePolicyId">SignaturePolicyId wrapper</param>
        public SignaturePolicyIdentifierBCFips(ISignaturePolicyId signaturePolicyId)
            : this(new SignaturePolicyIdentifier(((SignaturePolicyIdBCFips)signaturePolicyId).GetSignaturePolicyId())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>.
        /// </returns>
        public virtual SignaturePolicyIdentifier GetSignaturePolicyIdentifier() {
            return (SignaturePolicyIdentifier)GetEncodable();
        }
    }
}
