using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>.
    /// </summary>
    public class OtherHashAlgAndValueBCFips : ASN1EncodableBCFips, IOtherHashAlgAndValue {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>.
        /// </summary>
        /// <param name="otherHashAlgAndValue">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>
        /// to be wrapped
        /// </param>
        public OtherHashAlgAndValueBCFips(OtherHashAlgAndValue otherHashAlgAndValue)
            : base(otherHashAlgAndValue) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>.
        /// </summary>
        /// <param name="algorithmIdentifier">AlgorithmIdentifier wrapper</param>
        /// <param name="octetString">ASN1OctetString wrapper</param>
        public OtherHashAlgAndValueBCFips(IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString)
            : this(new OtherHashAlgAndValue(((AlgorithmIdentifierBCFips)algorithmIdentifier).GetAlgorithmIdentifier(), 
                ((ASN1OctetStringBCFips)octetString).GetOctetString())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>.
        /// </returns>
        public virtual OtherHashAlgAndValue GetOtherHashAlgAndValue() {
            return (OtherHashAlgAndValue)GetEncodable();
        }
    }
}
