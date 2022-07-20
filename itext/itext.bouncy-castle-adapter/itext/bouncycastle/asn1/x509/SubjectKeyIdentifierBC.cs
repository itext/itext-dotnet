using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class SubjectKeyIdentifierBC : ASN1EncodableBC, ISubjectKeyIdentifier {
        public SubjectKeyIdentifierBC(SubjectKeyIdentifier keyIdentifier)
            : base(keyIdentifier) {
        }

        public virtual SubjectKeyIdentifier GetSubjectKeyIdentifier() {
            return (SubjectKeyIdentifier)GetEncodable();
        }
    }
}
