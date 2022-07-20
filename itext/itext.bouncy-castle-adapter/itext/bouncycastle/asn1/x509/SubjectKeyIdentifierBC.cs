using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier"/>.
    /// </summary>
    public class SubjectKeyIdentifierBC : ASN1EncodableBC, ISubjectKeyIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier"/>.
        /// </summary>
        /// <param name="keyIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier"/>
        /// to be wrapped
        /// </param>
        public SubjectKeyIdentifierBC(SubjectKeyIdentifier keyIdentifier)
            : base(keyIdentifier) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier"/>.
        /// </returns>
        public virtual SubjectKeyIdentifier GetSubjectKeyIdentifier() {
            return (SubjectKeyIdentifier)GetEncodable();
        }
    }
}
