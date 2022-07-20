using System;
using Org.BouncyCastle.Crypto;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Operator.Jcajce {
    public interface IJcaContentVerifierProviderBuilder {
        IJcaContentVerifierProviderBuilder SetProvider(String provider);

        IContentVerifierProvider Build(AsymmetricKeyParameter publicKey);
    }
}
