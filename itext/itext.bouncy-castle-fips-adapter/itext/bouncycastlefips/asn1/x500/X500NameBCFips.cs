using iText.Commons.Bouncycastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X500;

namespace iText.Bouncycastlefips.Asn1.X500 {
    public class X500NameBCFips : ASN1EncodableBCFips, IX500Name {
        public X500NameBCFips(X500Name x500Name)
            : base(x500Name) {
        }

        public virtual X500Name GetX500Name() {
            return (X500Name)GetEncodable();
        }
    }
}
