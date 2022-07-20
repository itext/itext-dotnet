using System;
using iText.Commons.Bouncycastle.Cms;

namespace iText.Commons.Bouncycastle.Cms.Jcajce {
    public interface IJceKeyTransEnvelopedRecipient : IRecipient {
        IJceKeyTransEnvelopedRecipient SetProvider(String provider);
    }
}
