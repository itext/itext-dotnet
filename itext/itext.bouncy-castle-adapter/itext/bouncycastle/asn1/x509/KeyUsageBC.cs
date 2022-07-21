using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class KeyUsageBC : ASN1EncodableBC, IKeyUsage {
        private static readonly iText.Bouncycastle.Asn1.X509.KeyUsageBC INSTANCE = new iText.Bouncycastle.Asn1.X509.KeyUsageBC
            (null);

        public KeyUsageBC(KeyUsage keyUsage)
            : base(keyUsage) {
        }

        public static iText.Bouncycastle.Asn1.X509.KeyUsageBC GetInstance() {
            return INSTANCE;
        }

        public virtual KeyUsage GetKeyUsage() {
            return (KeyUsage)GetEncodable();
        }

        public virtual int GetDigitalSignature() {
            return KeyUsage.DigitalSignature;
        }

        public virtual int GetNonRepudiation() {
            return KeyUsage.NonRepudiation;
        }
    }
}
