using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator.Jcajce {
    public class JcaContentSignerBuilderBCFips : IJcaContentSignerBuilder {
        private readonly JcaContentSignerBuilder jcaContentSignerBuilder;

        public JcaContentSignerBuilderBCFips(JcaContentSignerBuilder jcaContentSignerBuilder) {
            this.jcaContentSignerBuilder = jcaContentSignerBuilder;
        }

        public virtual JcaContentSignerBuilder GetJcaContentSignerBuilder() {
            return jcaContentSignerBuilder;
        }

        public virtual IContentSigner Build(ICipherParameters pk) {
            try {
                return new ContentSignerBCFips(jcaContentSignerBuilder.Build(pk));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBCFips(e);
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
            iText.Bouncycastlefips.Operator.Jcajce.JcaContentSignerBuilderBCFips that = (iText.Bouncycastlefips.Operator.Jcajce.JcaContentSignerBuilderBCFips
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
