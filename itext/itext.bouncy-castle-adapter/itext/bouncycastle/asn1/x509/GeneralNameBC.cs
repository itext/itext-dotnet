using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralName"/>.
    /// </summary>
    public class GeneralNameBC : ASN1EncodableBC, IGeneralName {
        private static readonly iText.Bouncycastle.Asn1.X509.GeneralNameBC INSTANCE = new iText.Bouncycastle.Asn1.X509.GeneralNameBC
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
        public GeneralNameBC(GeneralName generalName)
            : base(generalName) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="GeneralNameBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.X509.GeneralNameBC GetInstance() {
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
