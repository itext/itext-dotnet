using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Operator.ContentSigner"/>.
    /// </summary>
    public class ContentSignerBCFips : IContentSigner {
        private readonly ContentSigner contentSigner;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Operator.ContentSigner"/>.
        /// </summary>
        /// <param name="contentSigner">
        /// 
        /// <see cref="Org.BouncyCastle.Operator.ContentSigner"/>
        /// to be wrapped
        /// </param>
        public ContentSignerBCFips(ContentSigner contentSigner) {
            this.contentSigner = contentSigner;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Operator.ContentSigner"/>.
        /// </returns>
        public virtual ContentSigner GetContentSigner() {
            return contentSigner;
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
            iText.Bouncycastlefips.Operator.ContentSignerBCFips that = (iText.Bouncycastlefips.Operator.ContentSignerBCFips
                )o;
            return Object.Equals(contentSigner, that.contentSigner);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(contentSigner);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return contentSigner.ToString();
        }
    }
}
