using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator.Jcajce {
    public class JcaContentSignerBuilderBC : IJcaContentSignerBuilder {
        private readonly JcaContentSignerBuilder jcaContentSignerBuilder;

        public JcaContentSignerBuilderBC(JcaContentSignerBuilder jcaContentSignerBuilder) {
            this.jcaContentSignerBuilder = jcaContentSignerBuilder;
        }

        public virtual JcaContentSignerBuilder GetJcaContentSignerBuilder() {
            return jcaContentSignerBuilder;
        }

        public virtual IContentSigner Build(ICipherParameters pk) {
            try {
                return new ContentSignerBC(jcaContentSignerBuilder.Build(pk));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBC(e);
            }
        }

        public virtual IJcaContentSignerBuilder SetProvider(String providerName) {
            jcaContentSignerBuilder.SetProvider(providerName);
            return this;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Operator.Jcajce.JcaContentSignerBuilderBC that = (iText.Bouncycastle.Operator.Jcajce.JcaContentSignerBuilderBC
                )o;
            return Object.Equals(jcaContentSignerBuilder, that.jcaContentSignerBuilder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(jcaContentSignerBuilder);
        }

        public override String ToString() {
            return jcaContentSignerBuilder.ToString();
        }
    }
}
