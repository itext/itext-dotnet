using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
    /// </summary>
    public class ExtendedKeyUsageBC : ASN1EncodableBC, IExtendedKeyUsage {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </summary>
        /// <param name="extendedKeyUsage">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>
        /// to be wrapped
        /// </param>
        public ExtendedKeyUsageBC(ExtendedKeyUsage extendedKeyUsage)
            : base(extendedKeyUsage) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </summary>
        /// <param name="purposeId">KeyPurposeId wrapper</param>
        public ExtendedKeyUsageBC(IKeyPurposeId purposeId)
            : base(new ExtendedKeyUsage(((KeyPurposeIdBC)purposeId).GetKeyPurposeId())) {
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
