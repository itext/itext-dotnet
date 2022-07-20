using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.Recipient"/>.
    /// </summary>
    public class RecipientBC : IRecipient {
        private readonly Recipient recipient;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Recipient"/>.
        /// </summary>
        /// <param name="recipient">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.Recipient"/>
        /// to be wrapped
        /// </param>
        public RecipientBC(Recipient recipient) {
            this.recipient = recipient;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.Recipient"/>.
        /// </returns>
        public virtual Recipient GetRecipient() {
            return recipient;
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
            iText.Bouncycastle.Cms.RecipientBC that = (iText.Bouncycastle.Cms.RecipientBC)o;
            return Object.Equals(recipient, that.recipient);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipient);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return recipient.ToString();
        }
    }
}
