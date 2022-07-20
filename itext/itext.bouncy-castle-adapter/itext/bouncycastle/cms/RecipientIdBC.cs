using System;
using Org.BouncyCastle.Cms;
using iText.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    public class RecipientIdBC : IRecipientId {
        private readonly RecipientId recipientId;

        public RecipientIdBC(RecipientId recipientId) {
            this.recipientId = recipientId;
        }

        public virtual RecipientId GetRecipientId() {
            return recipientId;
        }

        public virtual bool Match(IX509CertificateHolder holder) {
            return recipientId.Match(((X509CertificateHolderBC)holder).GetCertificateHolder());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cms.RecipientIdBC that = (iText.Bouncycastle.Cms.RecipientIdBC)o;
            return Object.Equals(recipientId, that.recipientId);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientId);
        }

        public override String ToString() {
            return recipientId.ToString();
        }
    }
}
