using System;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampResponseGenerator that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampResponseGenerator {
        /// <summary>
        /// Calls actual
        /// <c>generate</c>
        /// method for the wrapped TimeStampResponseGenerator object.
        /// </summary>
        /// <param name="request">the wrapper for request this response is for</param>
        /// <param name="bigInteger">serial number for the response token</param>
        /// <param name="date">generation time for the response token</param>
        /// <returns>
        /// 
        /// <see cref="ITimeStampResponse"/>
        /// the wrapper for the generated TimeStampResponse object.
        /// </returns>
        ITimeStampResponse Generate(ITimeStampRequest request, IBigInteger bigInteger, DateTime date);
    }
}
