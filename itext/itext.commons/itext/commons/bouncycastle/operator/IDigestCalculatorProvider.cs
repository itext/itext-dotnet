using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Operator {
    public interface IDigestCalculatorProvider {
        IDigestCalculator Get(IAlgorithmIdentifier algorithmIdentifier);
    }
}
