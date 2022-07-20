using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier"/>.
    /// </summary>
    public class AuthorityKeyIdentifierBCFips : ASN1EncodableBCFips, IAuthorityKeyIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier"/>.
        /// </summary>
        /// <param name="authorityKeyIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier"/>
        /// to be wrapped
        /// </param>
        public AuthorityKeyIdentifierBCFips(AuthorityKeyIdentifier authorityKeyIdentifier)
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
