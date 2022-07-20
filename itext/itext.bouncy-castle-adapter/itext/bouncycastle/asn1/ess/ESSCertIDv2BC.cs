using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Ess;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Ess {
    public class ESSCertIDv2BC : ASN1EncodableBC, IESSCertIDv2 {
        public ESSCertIDv2BC(EssCertIDv2 essCertIDv2)
            : base(essCertIDv2) {
        }

        public virtual EssCertIDv2 GetEssCertIDv2() {
            return (EssCertIDv2)GetEncodable();
        }

        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBC(GetEssCertIDv2().HashAlgorithm);
        }

        public virtual byte[] GetCertHash() {
            return GetEssCertIDv2().GetCertHash();
        }
    }
}
