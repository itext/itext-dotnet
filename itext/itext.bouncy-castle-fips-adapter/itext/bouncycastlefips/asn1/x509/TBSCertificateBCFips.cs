using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class TBSCertificateBCFips : ASN1EncodableBCFips, ITBSCertificate {
        public TBSCertificateBCFips(TBSCertificate tbsCertificate)
            : base(tbsCertificate) {
        }

        public virtual TBSCertificate GetTBSCertificate() {
            return (TBSCertificate)GetEncodable();
        }

        public virtual ISubjectPublicKeyInfo GetSubjectPublicKeyInfo() {
            return new SubjectPublicKeyInfoBCFips(GetTBSCertificate().GetSubjectPublicKeyInfo());
        }

        public virtual IX500Name GetIssuer() {
            return new X500NameBCFips(GetTBSCertificate().GetIssuer());
        }

        public virtual IASN1Integer GetSerialNumber() {
            return new ASN1IntegerBCFips(GetTBSCertificate().GetSerialNumber());
        }
    }
}
