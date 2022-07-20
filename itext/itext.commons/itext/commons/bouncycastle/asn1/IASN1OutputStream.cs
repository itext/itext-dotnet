using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1OutputStream that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1OutputStream : IDisposable {
        /// <summary>
        /// Calls actual
        /// <c>writeObject</c>
        /// method for the wrapped ASN1OutputStream object.
        /// </summary>
        /// <param name="primitive">wrapped ASN1Primitive object.</param>
        void WriteObject(IASN1Primitive primitive);

        /// <summary>
        /// Delegates
        /// <c>close</c>
        /// method call to the wrapped stream.
        /// </summary>
        void Dispose();
    }
}
