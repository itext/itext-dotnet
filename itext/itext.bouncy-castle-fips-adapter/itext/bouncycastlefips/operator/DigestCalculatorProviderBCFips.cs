using System;
using Org.BouncyCastle.Operator;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator {
    public class DigestCalculatorProviderBCFips : IDigestCalculatorProvider {
        private readonly DigestCalculatorProvider calculatorProvider;

        public DigestCalculatorProviderBCFips(DigestCalculatorProvider calculatorProvider) {
            this.calculatorProvider = calculatorProvider;
        }

        public virtual DigestCalculatorProvider GetCalculatorProvider() {
            return calculatorProvider;
        }

        public virtual IDigestCalculator Get(IAlgorithmIdentifier algorithmIdentifier) {
            try {
                return new DigestCalculatorBCFips(calculatorProvider.Get(((AlgorithmIdentifierBCFips)algorithmIdentifier).
                    GetAlgorithmIdentifier()));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBCFips(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Operator.DigestCalculatorProviderBCFips that = (iText.Bouncycastlefips.Operator.DigestCalculatorProviderBCFips
                )o;
            return Object.Equals(calculatorProvider, that.calculatorProvider);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(calculatorProvider);
        }

        public override String ToString() {
            return calculatorProvider.ToString();
        }
    }
}
