using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Math;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Bouncycastle.Asn1.Cms {
    public class IssuerAndSerialNumberBC : ASN1EncodableBC, IIssuerAndSerialNumber {
        public IssuerAndSerialNumberBC(IssuerAndSerialNumber issuerAndSerialNumber)
            : base(issuerAndSerialNumber) {
        }

        public IssuerAndSerialNumberBC(IX500Name issuer, BigInteger value)
            : base(new IssuerAndSerialNumber(((X500NameBC)issuer).GetX500Name(), value)) {
        }

        public virtual IssuerAndSerialNumber GetIssuerAndSerialNumber() {
            return (IssuerAndSerialNumber)GetEncodable();
        }
    }
}
