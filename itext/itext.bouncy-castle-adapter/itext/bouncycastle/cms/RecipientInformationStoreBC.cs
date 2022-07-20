using System;
using System.Collections.Generic;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    public class RecipientInformationStoreBC : IRecipientInformationStore {
        private readonly RecipientInformationStore recipientInformationStore;

        public RecipientInformationStoreBC(RecipientInformationStore recipientInformationStore) {
            this.recipientInformationStore = recipientInformationStore;
        }

        public virtual RecipientInformationStore GetRecipientInformationStore() {
            return recipientInformationStore;
        }

        public virtual ICollection<IRecipientInformation> GetRecipients() {
            List<IRecipientInformation> iRecipientInformations = new List<IRecipientInformation>();
            ICollection<RecipientInformation> recipients = recipientInformationStore.GetRecipients();
            foreach (RecipientInformation recipient in recipients) {
                iRecipientInformations.Add(new RecipientInformationBC(recipient));
            }
            return iRecipientInformations;
        }

        public virtual IRecipientInformation Get(IRecipientId id) {
            return new RecipientInformationBC(recipientInformationStore.Get(((RecipientIdBC)id).GetRecipientId()));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cms.RecipientInformationStoreBC that = (iText.Bouncycastle.Cms.RecipientInformationStoreBC
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
