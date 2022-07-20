using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Math;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
    /// </summary>
    public class IssuerAndSerialNumberBC : ASN1EncodableBC, IIssuerAndSerialNumber {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
        /// </summary>
        /// <param name="issuerAndSerialNumber">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>
        /// to be wrapped
        /// </param>
        public IssuerAndSerialNumberBC(IssuerAndSerialNumber issuerAndSerialNumber)
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
        public IssuerAndSerialNumberBC(IX500Name issuer, BigInteger value)
            : base(new IssuerAndSerialNumber(((X500NameBC)issuer).GetX500Name(), value)) {
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
