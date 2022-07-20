using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Math;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Bouncycastlefips.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
    /// </summary>
    public class IssuerAndSerialNumberBCFips : ASN1EncodableBCFips, IIssuerAndSerialNumber {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
        /// </summary>
        /// <param name="issuerAndSerialNumber">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>
        /// to be wrapped
        /// </param>
        public IssuerAndSerialNumberBCFips(IssuerAndSerialNumber issuerAndSerialNumber)
            : base(issuerAndSerialNumber) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
        /// </summary>
        /// <param name="issuer">
        /// X500Name wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>
        /// </param>
        /// <param name="value">
        /// BigInteger to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>
        /// </param>
        public IssuerAndSerialNumberBCFips(IX500Name issuer, BigInteger value)
            : base(new IssuerAndSerialNumber(((X500NameBCFips)issuer).GetX500Name(), value)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
        /// </returns>
        public virtual IssuerAndSerialNumber GetIssuerAndSerialNumber() {
            return (IssuerAndSerialNumber)GetEncodable();
        }
    }
}
