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
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilder"/>.
    /// </summary>
    public class JcaSimpleSignerInfoVerifierBuilderBCFips : IJcaSimpleSignerInfoVerifierBuilder {
        private readonly JcaSimpleSignerInfoVerifierBuilder verifierBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilder"/>.
        /// </summary>
        /// <param name="verifierBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilder"/>
        /// to be wrapped
        /// </param>
        public JcaSimpleSignerInfoVerifierBuilderBCFips(JcaSimpleSignerInfoVerifierBuilder verifierBuilder) {
            this.verifierBuilder = verifierBuilder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilder"/>.
        /// </returns>
        public virtual JcaSimpleSignerInfoVerifierBuilder GetVerifierBuilder() {
            return verifierBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJcaSimpleSignerInfoVerifierBuilder SetProvider(String provider) {
            verifierBuilder.SetProvider(provider);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignerInformationVerifier Build(X509Certificate certificate) {
            try {
                return new SignerInformationVerifierBCFips(verifierBuilder.Build(certificate));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBCFips(e);
            }
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(verifierBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return verifierBuilder.ToString();
        }
    }
}
