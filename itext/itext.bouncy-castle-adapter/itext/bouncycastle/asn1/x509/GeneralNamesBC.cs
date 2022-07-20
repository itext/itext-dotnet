using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class GeneralNamesBC : ASN1EncodableBC, IGeneralNames {
        public GeneralNamesBC(GeneralNames generalNames)
            : base(generalNames) {
        }

        public virtual GeneralNames GetGeneralNames() {
            return (GeneralNames)GetEncodable();
        }

        public virtual IGeneralName[] GetNames() {
            GeneralName[] generalNames = GetGeneralNames().GetNames();
            IGeneralName[] generalNamesBC = new GeneralNameBC[generalNames.Length];
            for (int i = 0; i < generalNames.Length; ++i) {
                generalNamesBC[i] = new GeneralNameBC(generalNames[i]);
            }
            return generalNamesBC;
        }
    }
}
