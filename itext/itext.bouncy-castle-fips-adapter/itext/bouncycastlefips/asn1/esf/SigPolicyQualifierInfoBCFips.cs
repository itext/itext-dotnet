using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastlefips.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifierInfo"/>.
    /// </summary>
    public class SigPolicyQualifierInfoBCFips : ASN1EncodableBCFips, ISigPolicyQualifierInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifierInfo"/>.
        /// </summary>
        /// <param name="qualifierInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifierInfo"/>
        /// to be wrapped
        /// </param>
        public SigPolicyQualifierInfoBCFips(SigPolicyQualifierInfo qualifierInfo)
            : base(qualifierInfo) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifierInfo"/>.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="string">DERIA5String wrapper</param>
        public SigPolicyQualifierInfoBCFips(IASN1ObjectIdentifier objectIdentifier, IDERIA5String @string)
            : this(new SigPolicyQualifierInfo(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier()
                , ((DERIA5StringBCFips)@string).GetDerIA5String())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SigPolicyQualifierInfo"/>.
        /// </returns>
        public virtual SigPolicyQualifierInfo GetQualifierInfo() {
            return (SigPolicyQualifierInfo)GetEncodable();
        }
    }
}
