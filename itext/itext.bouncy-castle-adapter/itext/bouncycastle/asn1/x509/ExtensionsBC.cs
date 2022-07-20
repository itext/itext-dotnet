using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
    /// </summary>
    public class ExtensionsBC : ASN1EncodableBC, IExtensions {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
        /// </summary>
        /// <param name="extensions">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>
        /// to be wrapped
        /// </param>
        public ExtensionsBC(Extensions extensions)
            : base(extensions) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
        /// </summary>
        /// <param name="extensions">Extension wrapper</param>
        public ExtensionsBC(IExtension extensions)
            : base(new Extensions(((ExtensionBC)extensions).GetExtension())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
        /// </returns>
        public virtual Extensions GetExtensions() {
            return (Extensions)GetEncodable();
        }
    }
}
