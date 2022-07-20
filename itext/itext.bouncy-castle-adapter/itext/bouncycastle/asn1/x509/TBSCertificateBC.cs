using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class TBSCertificateBC : ASN1EncodableBC, ITBSCertificate {
        public TBSCertificateBC(TBSCertificate tbsCertificate)
            : base(tbsCertificate) {
        }

        public virtual TBSCertificate GetTBSCertificate() {
            return (TBSCertificate)GetEncodable();
        }

        public virtual ISubjectPublicKeyInfo GetSubjectPublicKeyInfo() {
            return new SubjectPublicKeyInfoBC(GetTBSCertificate().GetSubjectPublicKeyInfo());
        }

        public virtual IX500Name GetIssuer() {
            return new X500NameBC(GetTBSCertificate().GetIssuer());
        }

        public virtual IASN1Integer GetSerialNumber() {
            return new ASN1IntegerBC(GetTBSCertificate().GetSerialNumber());
        }
    }
}
