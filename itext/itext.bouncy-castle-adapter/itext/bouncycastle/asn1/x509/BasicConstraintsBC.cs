using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class BasicConstraintsBC : ASN1EncodableBC, IBasicConstraints {
        public BasicConstraintsBC(BasicConstraints basicConstraints)
            : base(basicConstraints) {
        }

        public virtual BasicConstraints GetBasicConstraints() {
            return (BasicConstraints)GetEncodable();
        }
    }
}
