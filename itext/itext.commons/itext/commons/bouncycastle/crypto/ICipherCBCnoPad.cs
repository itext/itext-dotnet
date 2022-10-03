namespace iText.Commons.Bouncycastle.Crypto {
    public interface ICipherCBCnoPad {
        /// <summary>
        /// Processes data block using created cipher.
        /// </summary>
        /// <param name="inp">Input data bytes</param>
        /// <param name="inpOff">Input data offset</param>
        /// <param name="inpLen">Input data length</param>
        /// <returns>Processed bytes</returns>
        byte[] ProcessBlock(byte[] inp, int inpOff, int inpLen);
    }
}