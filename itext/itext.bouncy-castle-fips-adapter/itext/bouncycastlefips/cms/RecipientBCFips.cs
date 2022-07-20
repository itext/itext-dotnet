using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    public class RecipientBCFips : IRecipient {
        private readonly Recipient recipient;

        public RecipientBCFips(Recipient recipient) {
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
            iText.Bouncycastlefips.Cms.RecipientBCFips that = (iText.Bouncycastlefips.Cms.RecipientBCFips)o;
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
