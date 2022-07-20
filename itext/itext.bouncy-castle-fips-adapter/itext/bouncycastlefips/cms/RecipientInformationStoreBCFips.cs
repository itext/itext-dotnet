using System;
using System.Collections.Generic;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    public class RecipientInformationStoreBCFips : IRecipientInformationStore {
        internal RecipientInformationStore recipientInformationStore;

        public RecipientInformationStoreBCFips(RecipientInformationStore recipientInformationStore) {
            this.recipientInformationStore = recipientInformationStore;
        }

        public virtual RecipientInformationStore GetRecipientInformationStore() {
            return recipientInformationStore;
        }

        public virtual ICollection<IRecipientInformation> GetRecipients() {
            List<IRecipientInformation> iRecipientInformations = new List<IRecipientInformation>();
            ICollection<RecipientInformation> recipients = recipientInformationStore.GetRecipients();
            foreach (RecipientInformation recipient in recipients) {
                iRecipientInformations.Add(new RecipientInformationBCFips(recipient));
            }
            return iRecipientInformations;
        }

        public virtual IRecipientInformation Get(IRecipientId id) {
            return new RecipientInformationBCFips(recipientInformationStore.Get(((RecipientIdBCFips)id).GetRecipientId
                ()));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cms.RecipientInformationStoreBCFips that = (iText.Bouncycastlefips.Cms.RecipientInformationStoreBCFips
                )o;
            return Object.Equals(recipientInformationStore, that.recipientInformationStore);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientInformationStore);
        }

        public override String ToString() {
            return recipientInformationStore.ToString();
        }
    }
}
