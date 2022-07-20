using System;

namespace iText.Commons.Bouncycastle.Crypto {
    /// <summary>
    /// This interface represents the wrapper for PrivateKey that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IPrivateKey {
        /// <summary>
        /// Gets private key algorithm.
        /// </summary>
        String GetAlgorithm();
    }
}