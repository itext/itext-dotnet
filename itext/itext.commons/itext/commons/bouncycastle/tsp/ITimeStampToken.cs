using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampToken that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampToken {
        /// <summary>
        /// Calls actual
        /// <c>getTimeStampInfo</c>
        /// method for the wrapped TimeStampToken object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ITimeStampTokenInfo"/>
        /// the wrapper for the received TimeStampInfo object.
        /// </returns>
        ITSTInfo GetTimeStampInfo();

        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped TimeStampToken object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>validate</c>
        /// method for the wrapped TimeStampToken object.
        /// </summary>
        /// <param name="cert">X509Certificate wrapper</param>
        void Validate(IX509Certificate cert);
    }
}
