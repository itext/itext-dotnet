using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Bouncycastle.Asn1.X500 {
    public class X500NameBC : ASN1EncodableBC, IX500Name {
        public X500NameBC(X509Name x500Name)
            : base(x500Name) {
        }

        public virtual X509Name GetX500Name() {
            return (X509Name)GetEncodable();
        }
    }
}
