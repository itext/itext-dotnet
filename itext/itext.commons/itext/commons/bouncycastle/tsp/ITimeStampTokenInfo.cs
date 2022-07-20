using System;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampTokenInfo that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampTokenInfo {
        /// <summary>
        /// Calls actual
        /// <c>getHashAlgorithm</c>
        /// method for the wrapped TimeStampTokenInfo object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAlgorithmIdentifier"/>
        /// the wrapper for the received AlgorithmIdentifier object.
        /// </returns>
        IAlgorithmIdentifier GetHashAlgorithm();

        /// <summary>
        /// Calls actual
        /// <c>toASN1Structure</c>
        /// method for the wrapped TimeStampTokenInfo object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Tsp.ITSTInfo"/>
        /// TSTInfo wrapper.
        /// </returns>
        ITSTInfo ToASN1Structure();

        /// <summary>
        /// Calls actual
        /// <c>getGenTime</c>
        /// method for the wrapped TimeStampTokenInfo object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.DateTime"/>
        /// the received genTime.
        /// </returns>
        DateTime GetGenTime();

        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped TimeStampTokenInfo object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();
    }
}
