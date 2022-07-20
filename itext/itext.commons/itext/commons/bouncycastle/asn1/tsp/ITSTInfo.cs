using System;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TSTInfo that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITSTInfo : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getMessageImprint</c>
        /// method for the wrapped TSTInfo object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IMessageImprint"/>
        /// wrapper for the received MessageImprint object.
        /// </returns>
        IMessageImprint GetMessageImprint();

        /// <summary>
        /// Calls actual
        /// <c>getGenTime</c>
        /// method for the wrapped TSTInfo object and gets date.
        /// </summary>
        /// <returns>
        /// the received
        /// <see cref="System.DateTime"/>
        /// object.
        /// </returns>
        DateTime GetGenTime();
    }
}
