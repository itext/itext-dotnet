using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class SubjectKeyIdentifierBCFips : ASN1EncodableBCFips, ISubjectKeyIdentifier {
        public SubjectKeyIdentifierBCFips(SubjectKeyIdentifier keyIdentifier)
            : base(keyIdentifier) {
        }

        public virtual SubjectKeyIdentifier GetSubjectKeyIdentifier() {
            return (SubjectKeyIdentifier)GetEncodable();
        }
    }
}
