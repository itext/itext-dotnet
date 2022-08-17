namespace iText.Commons.Bouncycastle.Crypto {
    /// <summary>
    /// This interface represents the wrapper for IDigest that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IIDigest {
        /// <summary>
        /// Calls actual
        /// <c>Digest</c>
        /// method for the wrapped IDigest object.
        /// </summary>
        /// <param name="enc2">byte array</param>
        /// <returns>
        /// byte array.
        /// </returns>
        byte[] Digest(byte[] enc2);
        
        /// <summary>
        /// Calls actual
        /// <c>Digest</c>
        /// method for the wrapped IDigest object.
        /// Leaves the digest reset.
        /// </summary>
        /// <returns>
        /// byte array.
        /// </returns>
        byte[] Digest();
        
        /// <summary>
        /// Calls actual
        /// <c>Update</c>
        /// method for the wrapped IDigest object.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        /// <param name="off">offset</param>
        /// <param name="len">buffer length</param>
        void Update(byte[] buf, int off, int len);
        
        /// <summary>
        /// Calls actual
        /// <c>Update</c>
        /// method for the wrapped IDigest object.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        void Update(byte[] buf);

        /// <summary>
        /// Calls actual
        /// <c>Reset</c>
        /// method for the wrapped IDigest object.
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets actual
        /// <c>AlgorithmName</c>
        /// for the wrapped IDigest object.
        /// </summary>
        /// <returns>algorithm name.</returns>
        string GetAlgorithmName();
    }
}