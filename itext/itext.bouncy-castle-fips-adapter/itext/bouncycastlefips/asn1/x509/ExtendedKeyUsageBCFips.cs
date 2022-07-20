using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class ExtendedKeyUsageBCFips : ASN1EncodableBCFips, IExtendedKeyUsage {
        public ExtendedKeyUsageBCFips(ExtendedKeyUsage extendedKeyUsage)
            : base(extendedKeyUsage) {
        }

        public ExtendedKeyUsageBCFips(IKeyPurposeId purposeId)
            : base(new ExtendedKeyUsage(((KeyPurposeIdBCFips)purposeId).GetKeyPurposeId())) {
        }

        public virtual ExtendedKeyUsage GetExtendedKeyUsage() {
            return (ExtendedKeyUsage)GetEncodable();
        }
    }
}
