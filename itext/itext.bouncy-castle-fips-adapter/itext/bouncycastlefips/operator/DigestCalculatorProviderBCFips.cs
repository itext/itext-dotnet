using System;
using Org.BouncyCastle.Operator;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Operator.DigestCalculatorProvider"/>.
    /// </summary>
    public class DigestCalculatorProviderBCFips : IDigestCalculatorProvider {
        private readonly DigestCalculatorProvider calculatorProvider;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Operator.DigestCalculatorProvider"/>.
        /// </summary>
        /// <param name="calculatorProvider">
        /// 
        /// <see cref="Org.BouncyCastle.Operator.DigestCalculatorProvider"/>
        /// to be wrapped
        /// </param>
        public DigestCalculatorProviderBCFips(DigestCalculatorProvider calculatorProvider) {
            this.calculatorProvider = calculatorProvider;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Operator.DigestCalculatorProvider"/>.
        /// </returns>
        public virtual DigestCalculatorProvider GetCalculatorProvider() {
            return calculatorProvider;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDigestCalculator Get(IAlgorithmIdentifier algorithmIdentifier) {
            try {
                return new DigestCalculatorBCFips(calculatorProvider.Get(((AlgorithmIdentifierBCFips)algorithmIdentifier).
                    GetAlgorithmIdentifier()));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBCFips(e);
            }
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(calculatorProvider);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return calculatorProvider.ToString();
        }
    }
}
