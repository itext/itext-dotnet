using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Bouncycastlefips.Math;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Math;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class IssuerAndSerialNumberBCFips : ASN1EncodableBCFips, IIssuerAndSerialNumber {
        public IssuerAndSerialNumberBCFips(IssuerAndSerialNumber issuerAndSerialNumber)
            : base(issuerAndSerialNumber) {
        }

        public IssuerAndSerialNumberBCFips(IX500Name issuer, IBigInteger value)
            : base(new IssuerAndSerialNumber(((X500NameBCFips)issuer).GetX500Name(), 
                ((BigIntegerBCFips)value).GetBigInteger())) {
        }

        public virtual IssuerAndSerialNumber GetIssuerAndSerialNumber() {
            return (IssuerAndSerialNumber)GetEncodable();
        }
    }
}
