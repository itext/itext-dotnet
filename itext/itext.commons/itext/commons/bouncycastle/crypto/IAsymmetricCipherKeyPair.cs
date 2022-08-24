namespace iText.Commons.Bouncycastle.Crypto {
    /// <summary>
    /// This interface represents the wrapper for AsymmetricCipherKeyPair that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IAsymmetricCipherKeyPair {
        /// <summary>
        /// Gets actual private key for the wrapped AsymmetricCipherKeyPair object.
        /// </summary>
        /// <returns>Wrapped private key.</returns>
        IPrivateKey GetPrivateKey();
        
        /// <summary>
        /// Gets actual public key for the wrapped AsymmetricCipherKeyPair object.
        /// </summary>
        /// <returns>Wrapped public key.</returns>
        IPublicKey GetPublicKey();
    }
}