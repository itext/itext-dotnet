using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class ExtensionsBCFips : ASN1EncodableBCFips, IExtensions {
        public ExtensionsBCFips(Extensions extensions)
            : base(extensions) {
        }

        public ExtensionsBCFips(IExtension extensions)
            : base(new Extensions(((ExtensionBCFips)extensions).GetExtension())) {
        }

        public virtual Extensions GetExtensions() {
            return (Extensions)GetEncodable();
        }
    }
}
