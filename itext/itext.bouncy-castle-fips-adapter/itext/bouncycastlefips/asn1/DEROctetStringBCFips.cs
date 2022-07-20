using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class DEROctetStringBCFips : ASN1OctetStringBCFips, IDEROctetString {
        public DEROctetStringBCFips(byte[] bytes)
            : base(new DerOctetString(bytes)) {
        }

        public DEROctetStringBCFips(DerOctetString octetString)
            : base(octetString) {
        }

        public virtual DerOctetString GetDEROctetString() {
            return (DerOctetString)GetPrimitive();
        }
    }
}
