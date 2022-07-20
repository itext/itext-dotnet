using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class GeneralNameBCFips : ASN1EncodableBCFips, IGeneralName {
        private static readonly iText.Bouncycastlefips.Asn1.X509.GeneralNameBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.GeneralNameBCFips
            (null);

        private const int UNIFORM_RESOURCE_IDENTIFIER = GeneralName.UniformResourceIdentifier;

        public GeneralNameBCFips(GeneralName generalName)
            : base(generalName) {
        }

        public static iText.Bouncycastlefips.Asn1.X509.GeneralNameBCFips GetInstance() {
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
