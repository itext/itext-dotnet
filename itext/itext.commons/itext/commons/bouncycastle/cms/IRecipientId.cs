using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle.Cms {
    /// <summary>
    /// This interface represents the wrapper for RecipientId that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IRecipientId {
        /// <summary>
        /// Calls actual
        /// <c>match</c>
        /// method for the wrapped RecipientId object.
        /// </summary>
        /// <param name="holder">X509CertificateHolder wrapper</param>
        /// <returns>boolean value.</returns>
        bool Match(IX509CertificateHolder holder);
    }
}
