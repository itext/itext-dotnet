using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaX509v3CertificateBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaX509v3CertificateBuilder {
        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped JcaX509v3CertificateBuilder object.
        /// </summary>
        /// <param name="contentSigner">ContentSigner wrapper</param>
        /// <returns>{IX509CertificateHolder} wrapper for built X509CertificateHolder object.</returns>
        IX509CertificateHolder Build(IContentSigner contentSigner);

        /// <summary>
        /// Calls actual
        /// <c>addExtension</c>
        /// method for the wrapped JcaX509v3CertificateBuilder object.
        /// </summary>
        /// <param name="extensionOID">wrapper for the OID defining the extension type</param>
        /// <param name="critical">true if the extension is critical, false otherwise</param>
        /// <param name="extensionValue">wrapped ASN.1 structure that forms the extension's value</param>
        /// <returns>
        /// 
        /// <see cref="IJcaX509v3CertificateBuilder"/>
        /// this wrapper object.
        /// </returns>
        IJcaX509v3CertificateBuilder AddExtension(IASN1ObjectIdentifier extensionOID, bool critical, IASN1Encodable
             extensionValue);
    }
}
