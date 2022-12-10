using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1InputStream that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1InputStream : IDisposable {
        /// <summary>
        /// Calls actual
        /// <c>readObject</c>
        /// method for the wrapped ASN1InputStream object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IASN1Primitive"/>
        /// wrapped ASN1Primitive object.
        /// </returns>
        IASN1Primitive ReadObject();

        /// <summary>
        /// Delegates
        /// <c>close</c>
        /// method call to the wrapped stream.
        /// </summary>
        void Dispose();
    }
}
