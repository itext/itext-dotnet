using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class AlgorithmIdentifierBCFips : ASN1EncodableBCFips, IAlgorithmIdentifier {
        public AlgorithmIdentifierBCFips(AlgorithmIdentifier algorithmIdentifier)
            : base(algorithmIdentifier) {
        }

        public virtual AlgorithmIdentifier GetAlgorithmIdentifier() {
            return (AlgorithmIdentifier)GetEncodable();
        }

        public virtual IASN1ObjectIdentifier GetAlgorithm() {
            return new ASN1ObjectIdentifierBCFips(GetAlgorithmIdentifier().ObjectID);
        }
    }
}
