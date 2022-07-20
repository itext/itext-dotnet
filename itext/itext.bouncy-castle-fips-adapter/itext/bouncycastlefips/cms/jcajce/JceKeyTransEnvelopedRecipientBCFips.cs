using System;
using Org.BouncyCastle.Cms.Jcajce;
using iText.Bouncycastlefips.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;

namespace iText.Bouncycastlefips.Cms.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.Jcajce.JceKeyTransEnvelopedRecipient"/>.
    /// </summary>
    public class JceKeyTransEnvelopedRecipientBCFips : RecipientBCFips, IJceKeyTransEnvelopedRecipient {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JceKeyTransEnvelopedRecipient"/>.
        /// </summary>
        /// <param name="jceKeyTransEnvelopedRecipient">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JceKeyTransEnvelopedRecipient"/>
        /// to be wrapped
        /// </param>
        public JceKeyTransEnvelopedRecipientBCFips(JceKeyTransEnvelopedRecipient jceKeyTransEnvelopedRecipient)
            : base(jceKeyTransEnvelopedRecipient) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JceKeyTransEnvelopedRecipient"/>.
        /// </returns>
        public virtual JceKeyTransEnvelopedRecipient GetJceKeyTransEnvelopedRecipient() {
            return (JceKeyTransEnvelopedRecipient)GetRecipient();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJceKeyTransEnvelopedRecipient SetProvider(String provider) {
            GetJceKeyTransEnvelopedRecipient().SetProvider(provider);
            return this;
        }
    }
}
