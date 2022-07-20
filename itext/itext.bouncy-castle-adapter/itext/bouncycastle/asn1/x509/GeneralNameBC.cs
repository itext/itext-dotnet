using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class GeneralNameBC : ASN1EncodableBC, IGeneralName {
        private static readonly iText.Bouncycastle.Asn1.X509.GeneralNameBC INSTANCE = new iText.Bouncycastle.Asn1.X509.GeneralNameBC
            (null);

        private const int UNIFORM_RESOURCE_IDENTIFIER = GeneralName.UniformResourceIdentifier;

        public GeneralNameBC(GeneralName generalName)
            : base(generalName) {
        }

        public static iText.Bouncycastle.Asn1.X509.GeneralNameBC GetInstance() {
            return INSTANCE;
        }

        public virtual GeneralName GetGeneralName() {
            return (GeneralName)GetEncodable();
        }

        public virtual int GetTagNo() {
            return GetGeneralName().TagNo;
        }

        public virtual int GetUniformResourceIdentifier() {
            return UNIFORM_RESOURCE_IDENTIFIER;
        }
    }
}
