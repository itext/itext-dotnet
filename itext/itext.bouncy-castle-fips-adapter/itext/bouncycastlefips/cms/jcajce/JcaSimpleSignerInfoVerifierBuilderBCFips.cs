using System;
using Org.BouncyCastle.Cms.Jcajce;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.X509;
using iText.Bouncycastlefips.Cms;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms.Jcajce {
    public class JcaSimpleSignerInfoVerifierBuilderBCFips : IJcaSimpleSignerInfoVerifierBuilder {
        private readonly JcaSimpleSignerInfoVerifierBuilder verifierBuilder;

        public JcaSimpleSignerInfoVerifierBuilderBCFips(JcaSimpleSignerInfoVerifierBuilder verifierBuilder) {
            this.verifierBuilder = verifierBuilder;
        }

        public virtual JcaSimpleSignerInfoVerifierBuilder GetVerifierBuilder() {
            return verifierBuilder;
        }

        public virtual IJcaSimpleSignerInfoVerifierBuilder SetProvider(String provider) {
            verifierBuilder.SetProvider(provider);
            return this;
        }

        public virtual ISignerInformationVerifier Build(X509Certificate certificate) {
            try {
                return new SignerInformationVerifierBCFips(verifierBuilder.Build(certificate));
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
            iText.Bouncycastlefips.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilderBCFips that = (iText.Bouncycastlefips.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilderBCFips
                )o;
            return Object.Equals(verifierBuilder, that.verifierBuilder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(verifierBuilder);
        }

        public override String ToString() {
            return verifierBuilder.ToString();
        }
    }
}
