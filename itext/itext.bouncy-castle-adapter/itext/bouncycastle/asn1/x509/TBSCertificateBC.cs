using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.TbsCertificateStructure"/>.
    /// </summary>
    public class TBSCertificateBC : ASN1EncodableBC, ITBSCertificate {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.TbsCertificateStructure"/>.
        /// </summary>
        /// <param name="tbsCertificate">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.TbsCertificateStructure"/>
        /// to be wrapped
        /// </param>
        public TBSCertificateBC(TbsCertificateStructure tbsCertificate)
            : base(tbsCertificate) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.TbsCertificateStructure"/>.
        /// </returns>
        public virtual TbsCertificateStructure GetTBSCertificate() {
            return (TbsCertificateStructure)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISubjectPublicKeyInfo GetSubjectPublicKeyInfo() {
            return new SubjectPublicKeyInfoBC(GetTBSCertificate().GetSubjectPublicKeyInfo());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX500Name GetIssuer() {
            return new X500NameBC(GetTBSCertificate().Issuer);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Integer GetSerialNumber() {
            return new ASN1IntegerBC(GetTBSCertificate().SerialNumber);
        }
    }
}
