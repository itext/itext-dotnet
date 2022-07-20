using System;
using Org.BouncyCastle.Crypto;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Operator.Jcajce {
    public interface IJcaContentSignerBuilder {
        IContentSigner Build(ICipherParameters pk);

        IJcaContentSignerBuilder SetProvider(String providerName);
    }
}
