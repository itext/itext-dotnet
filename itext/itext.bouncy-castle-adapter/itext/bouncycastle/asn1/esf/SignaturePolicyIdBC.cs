using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastle.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
    /// </summary>
    public class SignaturePolicyIdBC : ASN1EncodableBC, ISignaturePolicyId {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="signaturePolicyId">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>
        /// to be wrapped
        /// </param>
        public SignaturePolicyIdBC(SignaturePolicyId signaturePolicyId)
            : base(signaturePolicyId) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="algAndValue">OtherHashAlgAndValue wrapper</param>
        /// <param name="policyQualifiers">SigPolicyQualifiers wrapper</param>
        public SignaturePolicyIdBC(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue, ISigPolicyQualifiers
             policyQualifiers)
            : this(new SignaturePolicyId(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(), ((OtherHashAlgAndValueBC
                )algAndValue).GetOtherHashAlgAndValue(), ((SigPolicyQualifiersBC)policyQualifiers).GetSigPolityQualifiers
                ())) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="algAndValue">OtherHashAlgAndValue wrapper</param>
        public SignaturePolicyIdBC(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue)
            : this(new SignaturePolicyId(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(), ((OtherHashAlgAndValueBC
                )algAndValue).GetOtherHashAlgAndValue())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </returns>
        public virtual SignaturePolicyId GetSignaturePolicyId() {
            return (SignaturePolicyId)GetEncodable();
        }
    }
}
