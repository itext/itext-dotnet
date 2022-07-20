using System;
using Org.BouncyCastle.Operator;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator {
    public class DigestCalculatorProviderBC : IDigestCalculatorProvider {
        private readonly DigestCalculatorProvider calculatorProvider;

        public DigestCalculatorProviderBC(DigestCalculatorProvider calculatorProvider) {
            this.calculatorProvider = calculatorProvider;
        }

        public virtual DigestCalculatorProvider GetCalculatorProvider() {
            return calculatorProvider;
        }

        public virtual IDigestCalculator Get(IAlgorithmIdentifier algorithmIdentifier) {
            try {
                return new DigestCalculatorBC(calculatorProvider.Get(((AlgorithmIdentifierBC)algorithmIdentifier).GetAlgorithmIdentifier
                    ()));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBC(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Operator.DigestCalculatorProviderBC that = (iText.Bouncycastle.Operator.DigestCalculatorProviderBC
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
