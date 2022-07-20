using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastle.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>.
    /// </summary>
    public class SignaturePolicyIdentifierBC : ASN1EncodableBC, ISignaturePolicyIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>.
        /// </summary>
        /// <param name="signaturePolicyIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>
        /// to be wrapped
        /// </param>
        public SignaturePolicyIdentifierBC(SignaturePolicyIdentifier signaturePolicyIdentifier)
            : base(signaturePolicyIdentifier) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyIdentifier"/>.
        /// </summary>
        /// <param name="signaturePolicyId">SignaturePolicyId wrapper</param>
        public SignaturePolicyIdentifierBC(ISignaturePolicyId signaturePolicyId)
            : this(new SignaturePolicyIdentifier(((SignaturePolicyIdBC)signaturePolicyId).GetSignaturePolicyId())) {
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
