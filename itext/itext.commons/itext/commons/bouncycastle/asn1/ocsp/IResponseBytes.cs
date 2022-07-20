using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for ResponseBytes that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IResponseBytes : IASN1Encodable {
    }
}
