using System;
using iText.Commons.Bouncycastle.Asn1.Cmp;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampResponse that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampResponse {
        /// <summary>
        /// Calls actual
        /// <c>validate</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <param name="request">TimeStampRequest wrapper</param>
        void Validate(ITimeStampRequest request);

        /// <summary>
        /// Calls actual
        /// <c>getFailInfo</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Cmp.IPKIFailureInfo"/>
        /// the wrapper for the received PKIFailureInfo object.
        /// </returns>
        IPKIFailureInfo GetFailInfo();

        /// <summary>
        /// Calls actual
        /// <c>getTimeStampToken</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ITimeStampToken"/>
        /// the wrapper for the received TimeStampToken object.
        /// </returns>
        ITimeStampToken GetTimeStampToken();

        /// <summary>
        /// Calls actual
        /// <c>getStatusString</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <returns>status string.</returns>
        String GetStatusString();

        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();
    }
}
