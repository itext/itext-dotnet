using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// This interface represents the wrapper for ContentInfo that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IContentInfo : IASN1Encodable {
    }
}
