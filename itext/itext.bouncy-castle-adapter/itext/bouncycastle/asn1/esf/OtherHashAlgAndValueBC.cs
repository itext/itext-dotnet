using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Esf {
    public class OtherHashAlgAndValueBC : ASN1EncodableBC, IOtherHashAlgAndValue {
        public OtherHashAlgAndValueBC(OtherHashAlgAndValue otherHashAlgAndValue)
            : base(otherHashAlgAndValue) {
        }

        public OtherHashAlgAndValueBC(IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString)
            : this(new OtherHashAlgAndValue(((AlgorithmIdentifierBC)algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBC
                )octetString).GetASN1OctetString())) {
        }

        public virtual OtherHashAlgAndValue GetOtherHashAlgAndValue() {
            return (OtherHashAlgAndValue)GetEncodable();
        }
    }
}
