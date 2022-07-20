using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class AuthorityKeyIdentifierBCFips : ASN1EncodableBCFips, IAuthorityKeyIdentifier {
        public AuthorityKeyIdentifierBCFips(AuthorityKeyIdentifier authorityKeyIdentifier)
            : base(authorityKeyIdentifier) {
        }

        public virtual AuthorityKeyIdentifier GetAuthorityKeyIdentifier() {
            return (AuthorityKeyIdentifier)GetEncodable();
        }
    }
}
