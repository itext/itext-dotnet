using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator.Jcajce {
    public class JcaContentVerifierProviderBuilderBCFips : IJcaContentVerifierProviderBuilder {
        private readonly JcaContentVerifierProviderBuilder providerBuilder;

        public JcaContentVerifierProviderBuilderBCFips(JcaContentVerifierProviderBuilder providerBuilder) {
            this.providerBuilder = providerBuilder;
        }

        public virtual JcaContentVerifierProviderBuilder GetProviderBuilder() {
            return providerBuilder;
        }

        public virtual IJcaContentVerifierProviderBuilder SetProvider(String provider) {
            providerBuilder.SetProvider(provider);
            return this;
        }

        public virtual IContentVerifierProvider Build(AsymmetricKeyParameter publicKey) {
            try {
                return new ContentVerifierProviderBCFips(providerBuilder.Build(publicKey));
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
            iText.Bouncycastlefips.Operator.Jcajce.JcaContentVerifierProviderBuilderBCFips that = (iText.Bouncycastlefips.Operator.Jcajce.JcaContentVerifierProviderBuilderBCFips
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
