using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    public class RecipientInformationBCFips : IRecipientInformation {
        private readonly RecipientInformation recipientInformation;

        public RecipientInformationBCFips(RecipientInformation recipientInformation) {
            this.recipientInformation = recipientInformation;
        }

        public virtual RecipientInformation GetRecipientInformation() {
            return recipientInformation;
        }

        public virtual byte[] GetContent(IRecipient recipient) {
            try {
                return recipientInformation.GetContent(((RecipientBCFips)recipient).GetRecipient());
            }
            catch (CMSException e) {
                throw new CMSExceptionBCFips(e);
            }
        }

        public virtual IRecipientId GetRID() {
            return new RecipientIdBCFips(recipientInformation.GetRID());
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientInformation);
        }

        public override String ToString() {
            return recipientInformation.ToString();
        }
    }
}
