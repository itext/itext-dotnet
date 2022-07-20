using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Math;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class IssuerAndSerialNumberBCFips : ASN1EncodableBCFips, IIssuerAndSerialNumber {
        public IssuerAndSerialNumberBCFips(IssuerAndSerialNumber issuerAndSerialNumber)
            : base(issuerAndSerialNumber) {
        }

        public IssuerAndSerialNumberBCFips(IX500Name issuer, BigInteger value)
            : base(new IssuerAndSerialNumber(((X500NameBCFips)issuer).GetX500Name(), value)) {
        }

        public virtual IssuerAndSerialNumber GetIssuerAndSerialNumber() {
            return (IssuerAndSerialNumber)GetEncodable();
        }
    }
}
