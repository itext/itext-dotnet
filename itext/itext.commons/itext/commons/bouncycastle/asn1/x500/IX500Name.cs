using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X500 {
    /// <summary>
    /// This interface represents the wrapper for X500Name that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX500Name : IASN1Encodable {
    }
}
