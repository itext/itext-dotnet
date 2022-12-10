namespace iText.Commons.Bouncycastle.Crypto {
    /// <summary>
    /// This interface represents the wrapper for ICipher that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ICipher {
        /// <summary>
        /// Continues processing another data part
        /// in wrapped ICipher object.
        /// </summary>
        /// <param name="inp">byte array</param>
        /// <param name="inpOff">offset</param>
        /// <param name="inpLen">length</param>
        /// <returns>
        /// byte array.
        /// </returns>
        byte[] Update(byte[] inp, int inpOff, int inpLen);
        
        /// <summary>
        /// Process the last block in the buffer
        /// of the wrapped ICipher object.
        /// </summary>
        /// <returns>
        /// byte array.
        /// </returns>
        byte[] DoFinal();
    }
}