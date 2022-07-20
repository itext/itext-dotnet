using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>.
    /// </summary>
    public class RecipientInformationBCFips : IRecipientInformation {
        private readonly RecipientInformation recipientInformation;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>.
        /// </summary>
        /// <param name="recipientInformation">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>
        /// to be wrapped
        /// </param>
        public RecipientInformationBCFips(RecipientInformation recipientInformation) {
            this.recipientInformation = recipientInformation;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>.
        /// </returns>
        public virtual RecipientInformation GetRecipientInformation() {
            return recipientInformation;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetContent(IRecipient recipient) {
            try {
                return recipientInformation.GetContent(((RecipientBCFips)recipient).GetRecipient());
            }
            catch (CmsException e) {
                throw new CMSExceptionBCFips(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientId GetRID() {
            return new RecipientIdBCFips(recipientInformation.GetRID());
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
            iText.Bouncycastlefips.Cms.RecipientInformationBCFips that = (iText.Bouncycastlefips.Cms.RecipientInformationBCFips
                )o;
            return Object.Equals(recipientInformation, that.recipientInformation);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientInformation);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return recipientInformation.ToString();
        }
    }
}
