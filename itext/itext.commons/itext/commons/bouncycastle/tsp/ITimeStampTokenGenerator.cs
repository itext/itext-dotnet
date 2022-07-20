using System;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampTokenGenerator that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampTokenGenerator {
        /// <summary>
        /// Calls actual
        /// <c>setAccuracySeconds</c>
        /// method for the wrapped TimeStampTokenGenerator object.
        /// </summary>
        /// <param name="i">accuracy seconds to set</param>
        void SetAccuracySeconds(int i);

        /// <summary>
        /// Calls actual
        /// <c>addCertificates</c>
        /// method for the wrapped TimeStampTokenGenerator object.
        /// </summary>
        /// <param name="jcaCertStore">the wrapper for the JcaCertStore to add</param>
        void AddCertificates(IJcaCertStore jcaCertStore);

        /// <summary>
        /// Calls actual
        /// <c>generate</c>
        /// method for the wrapped TimeStampTokenGenerator object.
        /// </summary>
        /// <param name="request">the originating TimeStampRequest wrapper</param>
        /// <param name="bigInteger">serial number for the TimeStampToken</param>
        /// <param name="date">token generation time</param>
        /// <returns>
        /// 
        /// <see cref="ITimeStampToken"/>
        /// the wrapper for the generated TimeStampToken object.
        /// </returns>
        ITimeStampToken Generate(ITimeStampRequest request, IBigInteger bigInteger, DateTime date);
    }
}
