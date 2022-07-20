using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    public class RecipientInformationBC : IRecipientInformation {
        private readonly RecipientInformation recipientInformation;

        public RecipientInformationBC(RecipientInformation recipientInformation) {
            this.recipientInformation = recipientInformation;
        }

        public virtual RecipientInformation GetRecipientInformation() {
            return recipientInformation;
        }

        public virtual byte[] GetContent(IRecipient recipient) {
            try {
                return recipientInformation.GetContent(((RecipientBC)recipient).GetRecipient());
            }
            catch (CMSException e) {
                throw new CMSExceptionBC(e);
            }
        }

        public virtual IRecipientId GetRID() {
            return new RecipientIdBC(recipientInformation.GetRID());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cms.RecipientInformationBC that = (iText.Bouncycastle.Cms.RecipientInformationBC)o;
            return Object.Equals(recipientInformation, that.recipientInformation);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientInformation);
        }

        public override String ToString() {
            return recipientInformation.ToString();
        }
    }
}
