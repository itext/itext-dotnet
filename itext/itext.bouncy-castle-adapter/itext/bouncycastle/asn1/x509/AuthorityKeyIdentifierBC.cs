using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class AuthorityKeyIdentifierBC : ASN1EncodableBC, IAuthorityKeyIdentifier {
        public AuthorityKeyIdentifierBC(AuthorityKeyIdentifier authorityKeyIdentifier)
            : base(authorityKeyIdentifier) {
        }

        public virtual AuthorityKeyIdentifier GetAuthorityKeyIdentifier() {
            return (AuthorityKeyIdentifier)GetEncodable();
        }
    }
}
