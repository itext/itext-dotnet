using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class BasicConstraintsBCFips : ASN1EncodableBCFips, IBasicConstraints {
        public BasicConstraintsBCFips(BasicConstraints basicConstraints)
            : base(basicConstraints) {
        }

        public virtual BasicConstraints GetBasicConstraints() {
            return (BasicConstraints)GetEncodable();
        }
    }
}
