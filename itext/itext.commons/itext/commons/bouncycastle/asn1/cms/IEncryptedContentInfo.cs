using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// This interface represents the wrapper for EncryptedContentInfo that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IEncryptedContentInfo : IASN1Encodable {
    }
}
