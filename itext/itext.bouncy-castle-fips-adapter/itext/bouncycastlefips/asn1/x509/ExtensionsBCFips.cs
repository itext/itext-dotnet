using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
    /// </summary>
    public class ExtensionsBCFips : ASN1EncodableBCFips, IExtensions {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
        /// </summary>
        /// <param name="extensions">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>
        /// to be wrapped
        /// </param>
        public ExtensionsBCFips(Extensions extensions)
            : base(extensions) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
        /// </summary>
        /// <param name="extensions">Extension wrapper</param>
        public ExtensionsBCFips(IExtension extensions)
            : base(new Extensions(((ExtensionBCFips)extensions).GetExtension())) {
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
