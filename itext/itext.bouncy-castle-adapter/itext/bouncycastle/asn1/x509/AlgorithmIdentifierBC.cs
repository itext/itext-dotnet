using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class AlgorithmIdentifierBC : ASN1EncodableBC, IAlgorithmIdentifier {
        public AlgorithmIdentifierBC(AlgorithmIdentifier algorithmIdentifier)
            : base(algorithmIdentifier) {
        }

        public virtual AlgorithmIdentifier GetAlgorithmIdentifier() {
            return (AlgorithmIdentifier)GetEncodable();
        }

        public virtual IASN1ObjectIdentifier GetAlgorithm() {
            return new ASN1ObjectIdentifierBC(GetAlgorithmIdentifier().ObjectID);
        }
    }
}
