using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Operator.ContentVerifierProvider"/>.
    /// </summary>
    public class ContentVerifierProviderBCFips : IContentVerifierProvider {
        private readonly ContentVerifierProvider provider;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Operator.ContentVerifierProvider"/>.
        /// </summary>
        /// <param name="provider">
        /// 
        /// <see cref="Org.BouncyCastle.Operator.ContentVerifierProvider"/>
        /// to be wrapped
        /// </param>
        public ContentVerifierProviderBCFips(ContentVerifierProvider provider) {
            this.provider = provider;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Operator.ContentVerifierProvider"/>.
        /// </returns>
        public virtual ContentVerifierProvider GetProvider() {
            return provider;
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
            iText.Bouncycastlefips.Operator.ContentVerifierProviderBCFips that = (iText.Bouncycastlefips.Operator.ContentVerifierProviderBCFips
                )o;
            return Object.Equals(provider, that.provider);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(provider);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return provider.ToString();
        }
    }
}
