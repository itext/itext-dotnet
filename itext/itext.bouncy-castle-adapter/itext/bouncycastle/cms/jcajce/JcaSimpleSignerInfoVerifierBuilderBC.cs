using System;
using Org.BouncyCastle.Cms.Jcajce;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.X509;
using iText.Bouncycastle.Cms;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms.Jcajce {
    public class JcaSimpleSignerInfoVerifierBuilderBC : IJcaSimpleSignerInfoVerifierBuilder {
        private readonly JcaSimpleSignerInfoVerifierBuilder verifierBuilder;

        public JcaSimpleSignerInfoVerifierBuilderBC(JcaSimpleSignerInfoVerifierBuilder verifierBuilder) {
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
                return new SignerInformationVerifierBC(verifierBuilder.Build(certificate));
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
            iText.Bouncycastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilderBC that = (iText.Bouncycastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilderBC
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
