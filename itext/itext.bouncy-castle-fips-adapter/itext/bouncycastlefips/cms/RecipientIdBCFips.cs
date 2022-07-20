using System;
using Org.BouncyCastle.Cms;
using iText.Bouncycastlefips.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    public class RecipientIdBCFips : IRecipientId {
        private readonly RecipientId recipientId;

        public RecipientIdBCFips(RecipientId recipientId) {
            this.recipientId = recipientId;
        }

        public virtual RecipientId GetRecipientId() {
            return recipientId;
        }

        public virtual bool Match(IX509CertificateHolder holder) {
            return recipientId.Match(((X509CertificateHolderBCFips)holder).GetCertificateHolder());
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientId);
        }

        public override String ToString() {
            return recipientId.ToString();
        }
    }
}
