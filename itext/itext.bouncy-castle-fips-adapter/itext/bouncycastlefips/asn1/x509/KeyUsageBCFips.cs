using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class KeyUsageBCFips : ASN1EncodableBCFips, IKeyUsage {
        private static readonly iText.Bouncycastlefips.Asn1.X509.KeyUsageBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.KeyUsageBCFips
            (null);

        public KeyUsageBCFips(KeyUsage keyUsage)
            : base(keyUsage) {
        }

        public static iText.Bouncycastlefips.Asn1.X509.KeyUsageBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual KeyUsage GetKeyUsage() {
            return (KeyUsage)GetEncodable();
        }

        public virtual int GetDigitalSignature() {
            return KeyUsage.digitalSignature;
        }

        public virtual int GetNonRepudiation() {
            return KeyUsage.nonRepudiation;
        }
    }
}
