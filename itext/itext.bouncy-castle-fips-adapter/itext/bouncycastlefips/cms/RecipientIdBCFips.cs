using System;
using Org.BouncyCastle.Cms;
using iText.Bouncycastlefips.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.RecipientId"/>.
    /// </summary>
    public class RecipientIdBCFips : IRecipientId {
        private readonly RecipientId recipientId;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.RecipientId"/>.
        /// </summary>
        /// <param name="recipientId">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.RecipientId"/>
        /// to be wrapped
        /// </param>
        public RecipientIdBCFips(RecipientId recipientId) {
            this.recipientId = recipientId;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.RecipientId"/>.
        /// </returns>
        public virtual RecipientId GetRecipientId() {
            return recipientId;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool Match(IX509CertificateHolder holder) {
            return recipientId.Match(((X509CertificateHolderBCFips)holder).GetCertificateHolder());
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
            iText.Bouncycastlefips.Cms.RecipientIdBCFips that = (iText.Bouncycastlefips.Cms.RecipientIdBCFips)o;
            return Object.Equals(recipientId, that.recipientId);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientId);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return recipientId.ToString();
        }
    }
}
