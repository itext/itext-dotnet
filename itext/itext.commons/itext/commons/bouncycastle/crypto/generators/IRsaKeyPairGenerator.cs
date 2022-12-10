namespace iText.Commons.Bouncycastle.Crypto.Generators {
    /// <summary>
    /// This interface represents the wrapper for RsaKeyPairGenerator that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IRsaKeyPairGenerator {
        /// <summary>
        /// Calls actual <c>GenerateKeyPair</c> method
        /// for the wrapped RsaKeyPairGenerator object.
        /// </summary>
        /// <returns>
        /// Asymmetric key pair wrapper.
        /// </returns>
        IAsymmetricCipherKeyPair GenerateKeyPair();
    }
}