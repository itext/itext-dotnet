using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastlefips.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
    /// </summary>
    public class SignaturePolicyIdBCFips : ASN1EncodableBCFips, ISignaturePolicyId {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="signaturePolicyId">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>
        /// to be wrapped
        /// </param>
        public SignaturePolicyIdBCFips(SignaturePolicyId signaturePolicyId)
            : base(signaturePolicyId) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="algAndValue">OtherHashAlgAndValue wrapper</param>
        /// <param name="policyQualifiers">SigPolicyQualifiers wrapper</param>
        public SignaturePolicyIdBCFips(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue, 
            ISigPolicyQualifiers policyQualifiers)
            : this(new SignaturePolicyId(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier(), ((OtherHashAlgAndValueBCFips
                )algAndValue).GetOtherHashAlgAndValue(), ((SigPolicyQualifiersBCFips)policyQualifiers).GetSigPolityQualifiers
                ())) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="algAndValue">OtherHashAlgAndValue wrapper</param>
        public SignaturePolicyIdBCFips(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue)
            : this(new SignaturePolicyId(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier(), ((OtherHashAlgAndValueBCFips
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
