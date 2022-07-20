using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Esf {
    public class OtherHashAlgAndValueBCFips : ASN1EncodableBCFips, IOtherHashAlgAndValue {
        public OtherHashAlgAndValueBCFips(OtherHashAlgAndValue otherHashAlgAndValue)
            : base(otherHashAlgAndValue) {
        }

        public OtherHashAlgAndValueBCFips(IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString)
            : this(new OtherHashAlgAndValue(((AlgorithmIdentifierBCFips)algorithmIdentifier).GetAlgorithmIdentifier(), 
                ((ASN1OctetStringBCFips)octetString).GetOctetString())) {
        }

        public virtual OtherHashAlgAndValue GetOtherHashAlgAndValue() {
            return (OtherHashAlgAndValue)GetEncodable();
        }
    }
}
