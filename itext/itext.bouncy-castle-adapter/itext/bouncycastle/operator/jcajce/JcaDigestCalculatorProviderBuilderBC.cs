using System;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaDigestCalculatorProviderBuilder"/>.
    /// </summary>
    public class JcaDigestCalculatorProviderBuilderBC : IJcaDigestCalculatorProviderBuilder {
        private readonly JcaDigestCalculatorProviderBuilder providerBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaDigestCalculatorProviderBuilder"/>.
        /// </summary>
        /// <param name="providerBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaDigestCalculatorProviderBuilder"/>
        /// to be wrapped
        /// </param>
        public JcaDigestCalculatorProviderBuilderBC(JcaDigestCalculatorProviderBuilder providerBuilder) {
            this.providerBuilder = providerBuilder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaDigestCalculatorProviderBuilder"/>.
        /// </returns>
        public virtual JcaDigestCalculatorProviderBuilder GetProviderBuilder() {
            return providerBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDigestCalculatorProvider Build() {
            try {
                return new DigestCalculatorProviderBC(providerBuilder.Build());
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBC(e);
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
            iText.Bouncycastle.Operator.Jcajce.JcaDigestCalculatorProviderBuilderBC that = (iText.Bouncycastle.Operator.Jcajce.JcaDigestCalculatorProviderBuilderBC
                )o;
            return Object.Equals(providerBuilder, that.providerBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(providerBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return providerBuilder.ToString();
        }
    }
}
