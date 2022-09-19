using System;

namespace iText.Commons.Bouncycastle.Openssl {
    /// <summary>
    /// This interface represents the wrapper for PEMParser that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IPEMParser {
        /// <summary>
        /// Calls actual
        /// <c>readObject</c>
        /// method for the wrapped PEMParser object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.Object"/>
        /// which represents read object.
        /// </returns>
        Object ReadObject();
    }
}
