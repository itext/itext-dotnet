using System;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator.Jcajce {
    public class JcaDigestCalculatorProviderBuilderBC : IJcaDigestCalculatorProviderBuilder {
        private readonly JcaDigestCalculatorProviderBuilder providerBuilder;

        public JcaDigestCalculatorProviderBuilderBC(JcaDigestCalculatorProviderBuilder providerBuilder) {
            this.providerBuilder = providerBuilder;
        }

        public virtual JcaDigestCalculatorProviderBuilder GetProviderBuilder() {
            return providerBuilder;
        }

        public virtual IDigestCalculatorProvider Build() {
            try {
                return new DigestCalculatorProviderBC(providerBuilder.Build());
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
            iText.Bouncycastle.Operator.Jcajce.JcaDigestCalculatorProviderBuilderBC that = (iText.Bouncycastle.Operator.Jcajce.JcaDigestCalculatorProviderBuilderBC
                )o;
            return Object.Equals(providerBuilder, that.providerBuilder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(providerBuilder);
        }

        public override String ToString() {
            return providerBuilder.ToString();
        }
    }
}
