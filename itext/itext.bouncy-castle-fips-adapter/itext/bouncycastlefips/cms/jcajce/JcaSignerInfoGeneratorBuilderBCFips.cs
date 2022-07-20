using System;
using Org.BouncyCastle.Cms.Jcajce;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.X509;
using iText.Bouncycastlefips.Cms;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms.Jcajce {
    public class JcaSignerInfoGeneratorBuilderBCFips : IJcaSignerInfoGeneratorBuilder {
        private readonly JcaSignerInfoGeneratorBuilder jcaSignerInfoGeneratorBuilder;

        public JcaSignerInfoGeneratorBuilderBCFips(JcaSignerInfoGeneratorBuilder jcaSignerInfoGeneratorBuilder) {
            this.jcaSignerInfoGeneratorBuilder = jcaSignerInfoGeneratorBuilder;
        }

        public JcaSignerInfoGeneratorBuilderBCFips(IDigestCalculatorProvider digestCalcProviderProvider)
            : this(new JcaSignerInfoGeneratorBuilder(((DigestCalculatorProviderBCFips)digestCalcProviderProvider).GetCalculatorProvider
                ())) {
        }

        public virtual JcaSignerInfoGeneratorBuilder GetJcaSignerInfoGeneratorBuilder() {
            return jcaSignerInfoGeneratorBuilder;
        }

        public virtual ISignerInfoGenerator Build(IContentSigner signer, X509Certificate cert) {
            try {
                return new SignerInfoGeneratorBCFips(jcaSignerInfoGeneratorBuilder.Build(((ContentSignerBCFips)signer).GetContentSigner
                    (), cert));
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
            iText.Bouncycastlefips.Cms.Jcajce.JcaSignerInfoGeneratorBuilderBCFips that = (iText.Bouncycastlefips.Cms.Jcajce.JcaSignerInfoGeneratorBuilderBCFips
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
