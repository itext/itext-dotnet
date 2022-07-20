using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Operator.Jcajce {
    public interface IJcaDigestCalculatorProviderBuilder {
        IDigestCalculatorProvider Build();
    }
}
