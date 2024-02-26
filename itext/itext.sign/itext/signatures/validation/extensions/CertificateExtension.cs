using System;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Validation.Extensions {
    /// <summary>Class representing certificate extension with all the information required for validation.</summary>
    public class CertificateExtension {
        private readonly String extensionOid;

        private readonly IAsn1Object extensionValue;

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
        public CertificateExtension(String extensionOid, IAsn1Object extensionValue) {
            this.extensionOid = extensionOid;
            this.extensionValue = extensionValue;
        }

        /// <summary>Get extension value</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// , which represents extension value
        /// </returns>
        public virtual IAsn1Object GetExtensionValue() {
            return extensionValue;
        }

        /// <summary>Get extension OID</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// , which represents extension OID
        /// </returns>
        public virtual String GetExtensionOid() {
            return extensionOid;
        }

        /// <summary>Check if this extension is present in the provided certificate.</summary>
        /// <remarks>
        /// Check if this extension is present in the provided certificate.
        /// <para />
        /// This method doesn't always require complete extension value equality,
        /// instead whenever possible it checks that this extension is present in the certificate.
        /// </remarks>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// in which this extension shall be present
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if extension if present,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool ExistsInCertificate(IX509Certificate certificate) {
            IAsn1Object providedExtensionValue;
            try {
                providedExtensionValue = CertificateUtil.GetExtensionValue(certificate, extensionOid);
            }
            catch (System.IO.IOException) {
                return false;
            }
            return Object.Equals(providedExtensionValue, extensionValue);
        }
    }
}
