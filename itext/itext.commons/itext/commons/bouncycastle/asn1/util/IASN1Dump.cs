using System;

namespace iText.Commons.Bouncycastle.Asn1.Util {
    /// <summary>
    /// This interface represents the wrapper for ASN1Dump that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1Dump {
        /// <summary>
        /// Calls actual
        /// <c>dumpAsString</c>
        /// method for the wrapped ASN1Dump object.
        /// </summary>
        /// <param name="obj">the ASN1Primitive (or its wrapper) to be dumped out</param>
        /// <param name="b">if true, dump out the contents of octet and bit strings</param>
        /// <returns>the resulting string.</returns>
        String DumpAsString(Object obj, bool b);

        /// <summary>
        /// Calls actual
        /// <c>dumpAsString</c>
        /// method for the wrapped ASN1Dump object.
        /// </summary>
        /// <param name="obj">the ASN1Primitive (or its wrapper) to be dumped out</param>
        /// <returns>the resulting string.</returns>
        String DumpAsString(Object obj);
    }
}
