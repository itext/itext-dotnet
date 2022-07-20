using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
    /// </summary>
    public class ExtendedKeyUsageBCFips : ASN1EncodableBCFips, IExtendedKeyUsage {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </summary>
        /// <param name="extendedKeyUsage">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>
        /// to be wrapped
        /// </param>
        public ExtendedKeyUsageBCFips(ExtendedKeyUsage extendedKeyUsage)
            : base(extendedKeyUsage) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </summary>
        /// <param name="purposeId">KeyPurposeId wrapper</param>
        public ExtendedKeyUsageBCFips(IKeyPurposeId purposeId)
            : base(new ExtendedKeyUsage(((KeyPurposeIdBCFips)purposeId).GetKeyPurposeId())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </returns>
        public virtual ExtendedKeyUsage GetExtendedKeyUsage() {
            return (ExtendedKeyUsage)GetEncodable();
        }
    }
}
