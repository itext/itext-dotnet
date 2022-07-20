using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.Operator.Jcajce;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaContentSignerBuilder"/>.
    /// </summary>
    public class JcaContentSignerBuilderBC : IJcaContentSignerBuilder {
        private readonly JcaContentSignerBuilder jcaContentSignerBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaContentSignerBuilder"/>.
        /// </summary>
        /// <param name="jcaContentSignerBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaContentSignerBuilder"/>
        /// to be wrapped
        /// </param>
        public JcaContentSignerBuilderBC(JcaContentSignerBuilder jcaContentSignerBuilder) {
            this.jcaContentSignerBuilder = jcaContentSignerBuilder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Operator.Jcajce.JcaContentSignerBuilder"/>.
        /// </returns>
        public virtual JcaContentSignerBuilder GetJcaContentSignerBuilder() {
            return jcaContentSignerBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IContentSigner Build(ICipherParameters pk) {
            try {
                return new ContentSignerBC(jcaContentSignerBuilder.Build(pk));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJcaContentSignerBuilder SetProvider(String providerName) {
            jcaContentSignerBuilder.SetProvider(providerName);
            return this;
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
            iText.Bouncycastle.Operator.Jcajce.JcaContentSignerBuilderBC that = (iText.Bouncycastle.Operator.Jcajce.JcaContentSignerBuilderBC
                )o;
            return Object.Equals(jcaContentSignerBuilder, that.jcaContentSignerBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(jcaContentSignerBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return jcaContentSignerBuilder.ToString();
        }
    }
}
