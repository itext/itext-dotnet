using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralName"/>.
    /// </summary>
    public class GeneralNameBCFips : ASN1EncodableBCFips, IGeneralName {
        private static readonly iText.Bouncycastlefips.Asn1.X509.GeneralNameBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.GeneralNameBCFips
            (null);

        private const int UNIFORM_RESOURCE_IDENTIFIER = GeneralName.UniformResourceIdentifier;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralName"/>.
        /// </summary>
        /// <param name="generalName">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralName"/>
        /// to be wrapped
        /// </param>
        public GeneralNameBCFips(GeneralName generalName)
            : base(generalName) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="GeneralNameBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Asn1.X509.GeneralNameBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralName"/>.
        /// </returns>
        public virtual GeneralName GetGeneralName() {
            return (GeneralName)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetTagNo() {
            return GetGeneralName().TagNo;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetUniformResourceIdentifier() {
            return UNIFORM_RESOURCE_IDENTIFIER;
        }
    }
}
