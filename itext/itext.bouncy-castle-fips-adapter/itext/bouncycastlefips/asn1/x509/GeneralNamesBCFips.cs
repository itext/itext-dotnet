using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class GeneralNamesBCFips : ASN1EncodableBCFips, IGeneralNames {
        public GeneralNamesBCFips(GeneralNames generalNames)
            : base(generalNames) {
        }

        public virtual GeneralNames GetGeneralNames() {
            return (GeneralNames)GetEncodable();
        }

        public virtual IGeneralName[] GetNames() {
            GeneralName[] generalNames = GetGeneralNames().GetNames();
            IGeneralName[] generalNamesBC = new GeneralNameBCFips[generalNames.Length];
            for (int i = 0; i < generalNames.Length; ++i) {
                generalNamesBC[i] = new GeneralNameBCFips(generalNames[i]);
            }
            return generalNamesBC;
        }
    }
}
