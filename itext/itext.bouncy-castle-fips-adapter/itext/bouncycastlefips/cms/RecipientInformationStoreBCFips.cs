using System;
using System.Collections.Generic;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.RecipientInformationStore"/>.
    /// </summary>
    public class RecipientInformationStoreBCFips : IRecipientInformationStore {
        private readonly RecipientInformationStore recipientInformationStore;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformationStore"/>.
        /// </summary>
        /// <param name="recipientInformationStore">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformationStore"/>
        /// to be wrapped
        /// </param>
        public RecipientInformationStoreBCFips(RecipientInformationStore recipientInformationStore) {
            this.recipientInformationStore = recipientInformationStore;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformationStore"/>.
        /// </returns>
        public virtual RecipientInformationStore GetRecipientInformationStore() {
            return recipientInformationStore;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICollection<IRecipientInformation> GetRecipients() {
            List<IRecipientInformation> iRecipientInformations = new List<IRecipientInformation>();
            ICollection<RecipientInformation> recipients = recipientInformationStore.GetRecipients();
            foreach (RecipientInformation recipient in recipients) {
                iRecipientInformations.Add(new RecipientInformationBCFips(recipient));
            }
            return iRecipientInformations;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientInformation Get(IRecipientId id) {
            return new RecipientInformationBCFips(recipientInformationStore.Get(((RecipientIdBCFips)id).GetRecipientId
                ()));
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
            iText.Bouncycastlefips.Cms.RecipientInformationStoreBCFips that = (iText.Bouncycastlefips.Cms.RecipientInformationStoreBCFips
                )o;
            return Object.Equals(recipientInformationStore, that.recipientInformationStore);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientInformationStore);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return recipientInformationStore.ToString();
        }
    }
}
