using System;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Signatures.Validation.V1.Extensions {
    /// <summary>Certificate extension which is populated with additional dynamically changing validation related information.
    ///     </summary>
    public class DynamicCertificateExtension : CertificateExtension {
        private int certificateChainSize;

        /// <summary>
        /// Create new instance of
        /// <see cref="CertificateExtension"/>
        /// using provided extension OID and value.
        /// </summary>
        /// <param name="extensionOid">
        /// 
        /// <see cref="System.String"/>
        /// , which represents extension OID
        /// </param>
        /// <param name="extensionValue">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// , which represents extension value
        /// </param>
        public DynamicCertificateExtension(String extensionOid, IAsn1Object extensionValue)
            : base(extensionOid, extensionValue) {
        }

        /// <summary>Sets amount of certificates currently present in the chain.</summary>
        /// <param name="certificateChainSize">amount of certificates currently present in the chain</param>
        /// <returns>
        /// this
        /// <see cref="DynamicCertificateExtension"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.V1.Extensions.DynamicCertificateExtension WithCertificateChainSize
            (int certificateChainSize) {
            this.certificateChainSize = certificateChainSize;
            return this;
        }

        /// <summary>Gets amount of certificates currently present in the chain.</summary>
        /// <returns>amount of certificates currently present in the chain</returns>
        public virtual int GetCertificateChainSize() {
            return certificateChainSize;
        }
    }
}
