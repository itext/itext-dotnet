using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class ExtendedKeyUsageBC : ASN1EncodableBC, IExtendedKeyUsage {
        public ExtendedKeyUsageBC(ExtendedKeyUsage extendedKeyUsage)
            : base(extendedKeyUsage) {
        }

        public ExtendedKeyUsageBC(IKeyPurposeId purposeId)
            : base(new ExtendedKeyUsage(((KeyPurposeIdBC)purposeId).GetKeyPurposeId())) {
        }

        public virtual ExtendedKeyUsage GetExtendedKeyUsage() {
            return (ExtendedKeyUsage)GetEncodable();
        }
    }
}
