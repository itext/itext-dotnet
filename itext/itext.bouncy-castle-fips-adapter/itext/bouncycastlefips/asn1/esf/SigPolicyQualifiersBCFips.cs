using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastlefips.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifiers"/>.
    /// </summary>
    public class SigPolicyQualifiersBCFips : ASN1EncodableBCFips, ISigPolicyQualifiers {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifiers"/>.
        /// </summary>
        /// <param name="policyQualifiers">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifiers"/>
        /// to be wrapped
        /// </param>
        public SigPolicyQualifiersBCFips(SigPolicyQualifiers policyQualifiers)
            : base(policyQualifiers) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifiers"/>.
        /// </summary>
        /// <param name="qualifierInfo">SigPolicyQualifierInfo array</param>
        public SigPolicyQualifiersBCFips(params SigPolicyQualifierInfo[] qualifierInfo)
            : base(new SigPolicyQualifiers(qualifierInfo)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifiers"/>.
        /// </returns>
        public virtual SigPolicyQualifiers GetSigPolityQualifiers() {
            return (SigPolicyQualifiers)GetEncodable();
        }
    }
}
