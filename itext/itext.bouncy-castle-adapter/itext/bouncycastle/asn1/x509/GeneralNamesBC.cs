using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralNames"/>.
    /// </summary>
    public class GeneralNamesBC : ASN1EncodableBC, IGeneralNames {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralNames"/>.
        /// </summary>
        /// <param name="generalNames">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralNames"/>
        /// to be wrapped
        /// </param>
        public GeneralNamesBC(GeneralNames generalNames)
            : base(generalNames) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralNames"/>.
        /// </returns>
        public virtual GeneralNames GetGeneralNames() {
            return (GeneralNames)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
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
