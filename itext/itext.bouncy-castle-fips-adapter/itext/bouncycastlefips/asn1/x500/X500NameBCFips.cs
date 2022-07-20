using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Bouncycastlefips.Asn1.X500 {
    public class X500NameBCFips : ASN1EncodableBCFips, IX500Name {
        public X500NameBCFips(X509Name x500Name)
            : base(x500Name) {
        }

        public virtual X509Name GetX500Name() {
            return (X509Name)GetEncodable();
        }
    }
}
