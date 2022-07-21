using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1.X500;
using iText.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Math;

namespace iText.Bouncycastle.Asn1.Cms {
    public class IssuerAndSerialNumberBC : ASN1EncodableBC, IIssuerAndSerialNumber {
        public IssuerAndSerialNumberBC(IssuerAndSerialNumber issuerAndSerialNumber)
            : base(issuerAndSerialNumber) {
        }

        public IssuerAndSerialNumberBC(IX500Name issuer, IBigInteger value)
            : base(new IssuerAndSerialNumber(((X500NameBC)issuer).GetX500Name(), 
                ((BigIntegerBC)value).GetBigInteger())) {
        }

        public virtual IssuerAndSerialNumber GetIssuerAndSerialNumber() {
            return (IssuerAndSerialNumber)GetEncodable();
        }
    }
}
