using System;
using Org.BouncyCastle.Cms.Jcajce;
using iText.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;

namespace iText.Bouncycastle.Cms.Jcajce {
    public class JceKeyTransEnvelopedRecipientBC : RecipientBC, IJceKeyTransEnvelopedRecipient {
        public JceKeyTransEnvelopedRecipientBC(JceKeyTransEnvelopedRecipient jceKeyTransEnvelopedRecipient)
            : base(jceKeyTransEnvelopedRecipient) {
        }

        public virtual JceKeyTransEnvelopedRecipient GetJceKeyTransEnvelopedRecipient() {
            return (JceKeyTransEnvelopedRecipient)GetRecipient();
        }

        public virtual IJceKeyTransEnvelopedRecipient SetProvider(String provider) {
            GetJceKeyTransEnvelopedRecipient().SetProvider(provider);
            return this;
        }
    }
}
