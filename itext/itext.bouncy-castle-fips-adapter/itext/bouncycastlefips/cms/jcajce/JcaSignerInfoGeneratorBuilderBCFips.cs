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
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>.
    /// </summary>
    public class JcaSignerInfoGeneratorBuilderBCFips : IJcaSignerInfoGeneratorBuilder {
        private readonly JcaSignerInfoGeneratorBuilder jcaSignerInfoGeneratorBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>.
        /// </summary>
        /// <param name="jcaSignerInfoGeneratorBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>
        /// to be wrapped
        /// </param>
        public JcaSignerInfoGeneratorBuilderBCFips(JcaSignerInfoGeneratorBuilder jcaSignerInfoGeneratorBuilder) {
            this.jcaSignerInfoGeneratorBuilder = jcaSignerInfoGeneratorBuilder;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>.
        /// </summary>
        /// <param name="calculatorProvider">
        /// DigestCalculatorProvider wrapper to create
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>
        /// </param>
        public JcaSignerInfoGeneratorBuilderBCFips(IDigestCalculatorProvider calculatorProvider)
            : this(new JcaSignerInfoGeneratorBuilder(((DigestCalculatorProviderBCFips)calculatorProvider).GetCalculatorProvider
                ())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>.
        /// </returns>
        public virtual JcaSignerInfoGeneratorBuilder GetJcaSignerInfoGeneratorBuilder() {
            return jcaSignerInfoGeneratorBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignerInfoGenerator Build(IContentSigner signer, X509Certificate cert) {
            try {
                return new SignerInfoGeneratorBCFips(jcaSignerInfoGeneratorBuilder.Build(((ContentSignerBCFips)signer).GetContentSigner
                    (), cert));
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
            iText.Bouncycastlefips.Cms.Jcajce.JcaSignerInfoGeneratorBuilderBCFips that = (iText.Bouncycastlefips.Cms.Jcajce.JcaSignerInfoGeneratorBuilderBCFips
                )o;
            return Object.Equals(jcaSignerInfoGeneratorBuilder, that.jcaSignerInfoGeneratorBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(jcaSignerInfoGeneratorBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return jcaSignerInfoGeneratorBuilder.ToString();
        }
    }
}
