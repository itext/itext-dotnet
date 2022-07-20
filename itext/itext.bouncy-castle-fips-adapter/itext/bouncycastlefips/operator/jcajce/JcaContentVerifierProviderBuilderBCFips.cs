using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaContentVerifierProviderBuilder"/>.
    /// </summary>
    public class JcaContentVerifierProviderBuilderBCFips : IJcaContentVerifierProviderBuilder {
        private readonly JcaContentVerifierProviderBuilder providerBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaContentVerifierProviderBuilder"/>.
        /// </summary>
        /// <param name="providerBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaContentVerifierProviderBuilder"/>
        /// to be wrapped
        /// </param>
        public JcaContentVerifierProviderBuilderBCFips(JcaContentVerifierProviderBuilder providerBuilder) {
            this.providerBuilder = providerBuilder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaContentVerifierProviderBuilder"/>.
        /// </returns>
        public virtual JcaContentVerifierProviderBuilder GetProviderBuilder() {
            return providerBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJcaContentVerifierProviderBuilder SetProvider(String provider) {
            providerBuilder.SetProvider(provider);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IContentVerifierProvider Build(AsymmetricKeyParameter publicKey) {
            try {
                return new ContentVerifierProviderBCFips(providerBuilder.Build(publicKey));
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
            iText.Bouncycastlefips.Operator.Jcajce.JcaContentVerifierProviderBuilderBCFips that = (iText.Bouncycastlefips.Operator.Jcajce.JcaContentVerifierProviderBuilderBCFips
                )o;
            return Object.Equals(providerBuilder, that.providerBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(providerBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return providerBuilder.ToString();
        }
    }
}
