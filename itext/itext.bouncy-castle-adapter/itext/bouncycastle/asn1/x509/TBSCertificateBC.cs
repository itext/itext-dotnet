using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class TBSCertificateBC : ASN1EncodableBC, ITBSCertificate {
        public TBSCertificateBC(TbsCertificateStructure tbsCertificate)
            : base(tbsCertificate) {
        }

        public virtual TbsCertificateStructure GetTBSCertificate() {
            return (TbsCertificateStructure)GetEncodable();
        }

        public virtual ISubjectPublicKeyInfo GetSubjectPublicKeyInfo() {
            return new SubjectPublicKeyInfoBC(GetTBSCertificate().SubjectPublicKeyInfo);
        }

        public virtual IX500Name GetIssuer() {
            return new X500NameBC(GetTBSCertificate().Issuer);
        }

        public virtual IASN1Integer GetSerialNumber() {
            return new ASN1IntegerBC(GetTBSCertificate().SerialNumber);
        }
    }
}
