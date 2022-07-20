using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// This interface represents the wrapper for RecipientInfo that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IRecipientInfo : IASN1Encodable {
    }
}
