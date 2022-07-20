using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator.Jcajce {
    public class JcaContentVerifierProviderBuilderBC : IJcaContentVerifierProviderBuilder {
        private readonly JcaContentVerifierProviderBuilder providerBuilder;

        public JcaContentVerifierProviderBuilderBC(JcaContentVerifierProviderBuilder providerBuilder) {
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
                return new ContentVerifierProviderBC(providerBuilder.Build(publicKey));
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
            iText.Bouncycastle.Operator.Jcajce.JcaContentVerifierProviderBuilderBC that = (iText.Bouncycastle.Operator.Jcajce.JcaContentVerifierProviderBuilderBC
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
