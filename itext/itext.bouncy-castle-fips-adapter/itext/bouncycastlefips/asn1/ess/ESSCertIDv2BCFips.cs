using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Ess;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Ess {
    public class ESSCertIDv2BCFips : ASN1EncodableBCFips, IESSCertIDv2 {
        public ESSCertIDv2BCFips(EssCertIDv2 essCertIDv2)
            : base(essCertIDv2) {
        }

        public virtual EssCertIDv2 GetEssCertIDv2() {
            return (EssCertIDv2)GetEncodable();
        }

        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBCFips(GetEssCertIDv2().HashAlgorithm);
        }

        public virtual byte[] GetCertHash() {
            return GetEssCertIDv2().GetCertHash();
        }
    }
}
