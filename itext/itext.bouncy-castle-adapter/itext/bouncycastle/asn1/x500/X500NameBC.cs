using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Bouncycastle.Asn1.X500 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.X509Name"/>.
    /// </summary>
    public class X500NameBC : ASN1EncodableBC, IX500Name {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Name"/>.
        /// </summary>
        /// <param name="x500Name">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Name"/>
        /// to be wrapped
        /// </param>
        public X500NameBC(X509Name x500Name)
            : base(x500Name) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Name"/>.
        /// </returns>
        public virtual X509Name GetX500Name() {
            return (X509Name)GetEncodable();
        }
    }
}
