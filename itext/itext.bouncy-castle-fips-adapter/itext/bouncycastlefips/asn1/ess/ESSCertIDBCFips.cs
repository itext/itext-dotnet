using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastlefips.Asn1.Ess {
    public class ESSCertIDBCFips : ASN1EncodableBCFips, IESSCertID {
        public ESSCertIDBCFips(EssCertID essCertID)
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
