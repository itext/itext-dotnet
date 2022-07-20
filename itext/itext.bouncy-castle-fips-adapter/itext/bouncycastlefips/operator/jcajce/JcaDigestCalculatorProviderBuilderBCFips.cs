using System;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator.Jcajce {
    public class JcaDigestCalculatorProviderBuilderBCFips : IJcaDigestCalculatorProviderBuilder {
        private readonly JcaDigestCalculatorProviderBuilder providerBuilder;

        public JcaDigestCalculatorProviderBuilderBCFips(JcaDigestCalculatorProviderBuilder providerBuilder) {
            this.providerBuilder = providerBuilder;
        }

        public virtual JcaDigestCalculatorProviderBuilder GetProviderBuilder() {
            return providerBuilder;
        }

        public virtual IDigestCalculatorProvider Build() {
            try {
                return new DigestCalculatorProviderBCFips(providerBuilder.Build());
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
            iText.Bouncycastlefips.Operator.Jcajce.JcaDigestCalculatorProviderBuilderBCFips that = (iText.Bouncycastlefips.Operator.Jcajce.JcaDigestCalculatorProviderBuilderBCFips
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
