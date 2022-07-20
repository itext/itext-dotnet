using System;
using Org.BouncyCastle.X509;
using iText.Commons.Bouncycastle.Cms;

namespace iText.Commons.Bouncycastle.Cms.Jcajce {
    public interface IJcaSimpleSignerInfoVerifierBuilder {
        IJcaSimpleSignerInfoVerifierBuilder SetProvider(String provider);

        ISignerInformationVerifier Build(X509Certificate certificate);
    }
}
