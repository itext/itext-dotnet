using System;
using Org.BouncyCastle.Cms.Jcajce;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.X509;
using iText.Bouncycastle.Cms;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms.Jcajce {
    public class JcaSignerInfoGeneratorBuilderBC : IJcaSignerInfoGeneratorBuilder {
        private readonly JcaSignerInfoGeneratorBuilder jcaSignerInfoGeneratorBuilder;

        public JcaSignerInfoGeneratorBuilderBC(JcaSignerInfoGeneratorBuilder jcaSignerInfoGeneratorBuilder) {
            this.jcaSignerInfoGeneratorBuilder = jcaSignerInfoGeneratorBuilder;
        }

        public JcaSignerInfoGeneratorBuilderBC(IDigestCalculatorProvider digestCalcProviderProvider)
            : this(new JcaSignerInfoGeneratorBuilder(((DigestCalculatorProviderBC)digestCalcProviderProvider).GetCalculatorProvider
                ())) {
        }

        public virtual JcaSignerInfoGeneratorBuilder GetJcaSignerInfoGeneratorBuilder() {
            return jcaSignerInfoGeneratorBuilder;
        }

        public virtual ISignerInfoGenerator Build(IContentSigner signer, X509Certificate cert) {
            try {
                return new SignerInfoGeneratorBC(jcaSignerInfoGeneratorBuilder.Build(((ContentSignerBC)signer).GetContentSigner
                    (), cert));
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
            iText.Bouncycastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilderBC that = (iText.Bouncycastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilderBC
                )o;
            return Object.Equals(jcaSignerInfoGeneratorBuilder, that.jcaSignerInfoGeneratorBuilder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(jcaSignerInfoGeneratorBuilder);
        }

        public override String ToString() {
            return jcaSignerInfoGeneratorBuilder.ToString();
        }
    }
}
