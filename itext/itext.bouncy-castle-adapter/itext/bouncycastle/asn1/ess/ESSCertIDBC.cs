using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastle.Asn1.Ess {
    public class ESSCertIDBC : ASN1EncodableBC, IESSCertID {
        public ESSCertIDBC(EssCertID essCertID)
            : base(essCertID) {
        }

        public virtual EssCertID GetEssCertID() {
            return (EssCertID)GetEncodable();
        }

        public virtual byte[] GetCertHash() {
            return GetEssCertID().GetCertHash();
        }
    }
}
