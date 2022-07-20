using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier"/>.
    /// </summary>
    public class AuthorityKeyIdentifierBC : ASN1EncodableBC, IAuthorityKeyIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier"/>.
        /// </summary>
        /// <param name="authorityKeyIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier"/>
        /// to be wrapped
        /// </param>
        public AuthorityKeyIdentifierBC(AuthorityKeyIdentifier authorityKeyIdentifier)
            : base(authorityKeyIdentifier) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier"/>.
        /// </returns>
        public virtual AuthorityKeyIdentifier GetAuthorityKeyIdentifier() {
            return (AuthorityKeyIdentifier)GetEncodable();
        }
    }
}
