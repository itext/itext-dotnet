using System;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampRequestGenerator that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampRequestGenerator {
        /// <summary>
        /// Calls actual
        /// <c>setCertReq</c>
        /// method for the wrapped TimeStampRequestGenerator object.
        /// </summary>
        /// <param name="var1">the value to be set</param>
        void SetCertReq(bool var1);

        /// <summary>
        /// Calls actual
        /// <c>setReqPolicy</c>
        /// method for the wrapped TimeStampRequestGenerator object.
        /// </summary>
        /// <param name="reqPolicy">the value to be set</param>
        void SetReqPolicy(String reqPolicy);

        /// <summary>
        /// Calls actual
        /// <c>generate</c>
        /// method for the wrapped TimeStampRequestGenerator object.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="imprint">byte array</param>
        /// <param name="nonce">BigInteger</param>
        /// <returns>
        /// 
        /// <see cref="ITimeStampRequest"/>
        /// the wrapper for generated TimeStampRequest object.
        /// </returns>
        ITimeStampRequest Generate(IASN1ObjectIdentifier objectIdentifier, byte[] imprint, IBigInteger nonce);
    }
}
