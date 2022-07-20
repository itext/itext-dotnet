using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class ExtensionsBC : ASN1EncodableBC, IExtensions {
        public ExtensionsBC(Extensions extensions)
            : base(extensions) {
        }

        public ExtensionsBC(IExtension extensions)
            : base(new Extensions(((ExtensionBC)extensions).GetExtension())) {
        }

        public virtual Extensions GetExtensions() {
            return (Extensions)GetEncodable();
        }
    }
}
