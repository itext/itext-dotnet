namespace iText.Commons.Bouncycastle.Crypto {
    /// <summary>
    /// This interface represents the wrapper for ISigner that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IISigner {
        /// <summary>
        /// Calls actual
        /// <c>InitVerify</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="publicKey">public key</param>
        void InitVerify(IPublicKey publicKey);
        
        /// <summary>
        /// Calls actual
        /// <c>InitVerify</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="publicKey">public key</param>
        void InitSign(IPrivateKey key);
        
        /// <summary>
        /// Calls actual
        /// <c>Update</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        /// <param name="off">offset</param>
        /// <param name="len">buffer length</param>
        void Update(byte[] buf, int off, int len);
        
        /// <summary>
        /// Calls actual
        /// <c>Update</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        void Update(byte[] digest);
        
        /// <summary>
        /// Calls actual
        /// <c>VerifySignature</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="digest">byte array</param>
        /// <returns>boolean value.</returns>
        bool VerifySignature(byte[] digest);
        
        /// <summary>
        /// Calls actual
        /// <c>GenerateSignature</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <returns>byte array.</returns>
        byte[] GenerateSignature();
        
        /// <summary>
        /// Calls actual
        /// <c>UpdateVerifier</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="digest">byte array</param>
        void UpdateVerifier(byte[] digest);

        /// <summary>
        /// Sets hash and encryption algorithms
        /// for the wrapped ISigner object.
        /// </summary>
        /// <param name="algorithm">digest algorithm</param>
        void SetDigestAlgorithm(string algorithm);
    }
}