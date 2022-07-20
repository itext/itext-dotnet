using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class DEROctetStringBC : ASN1OctetStringBC, IDEROctetString {
        public DEROctetStringBC(byte[] bytes)
            : base(new DerOctetString(bytes)) {
        }

        public DEROctetStringBC(DerOctetString octetString)
            : base(octetString) {
        }

        public virtual DerOctetString GetDEROctetString() {
            return (DerOctetString)GetPrimitive();
        }
    }
}
