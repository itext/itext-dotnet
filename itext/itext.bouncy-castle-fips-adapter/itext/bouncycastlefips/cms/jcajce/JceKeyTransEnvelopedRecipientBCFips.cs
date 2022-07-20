using System;
using Org.BouncyCastle.Cms.Jcajce;
using iText.Bouncycastlefips.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;

namespace iText.Bouncycastlefips.Cms.Jcajce {
    public class JceKeyTransEnvelopedRecipientBCFips : RecipientBCFips, IJceKeyTransEnvelopedRecipient {
        public JceKeyTransEnvelopedRecipientBCFips(JceKeyTransEnvelopedRecipient jceKeyTransEnvelopedRecipient)
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
