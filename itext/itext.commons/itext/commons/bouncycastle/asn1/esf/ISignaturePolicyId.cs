using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Esf {
    /// <summary>
    /// This interface represents the wrapper for SignaturePolicyId that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ISignaturePolicyId : IASN1Encodable {
    }
}
