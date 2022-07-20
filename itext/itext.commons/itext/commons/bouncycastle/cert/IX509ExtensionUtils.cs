using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509ExtensionUtils that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509ExtensionUtils {
        /// <summary>
        /// Calls actual
        /// <c>createAuthorityKeyIdentifier</c>
        /// method for the wrapped X509ExtensionUtils object.
        /// </summary>
        /// <param name="publicKeyInfo">SubjectPublicKeyInfo wrapper</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAuthorityKeyIdentifier"/>
        /// wrapper for the created AuthorityKeyIdentifier.
        /// </returns>
        IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo);

        /// <summary>
        /// Calls actual
        /// <c>createSubjectKeyIdentifier</c>
        /// method for the wrapped X509ExtensionUtils object.
        /// </summary>
        /// <param name="publicKeyInfo">SubjectPublicKeyInfo wrapper</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.ISubjectKeyIdentifier"/>
        /// wrapper for the created SubjectKeyIdentifier.
        /// </returns>
        ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo);
    }
}
