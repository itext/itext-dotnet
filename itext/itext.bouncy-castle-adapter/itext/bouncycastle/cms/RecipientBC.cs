using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    public class RecipientBC : IRecipient {
        private readonly Recipient recipient;

        public RecipientBC(Recipient recipient) {
            this.recipient = recipient;
        }

        public virtual Recipient GetRecipient() {
            return recipient;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipient);
        }

        public override String ToString() {
            return recipient.ToString();
        }
    }
}
