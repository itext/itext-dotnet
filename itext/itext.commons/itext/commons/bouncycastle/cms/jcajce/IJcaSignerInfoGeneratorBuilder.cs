using Org.BouncyCastle.X509;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cms.Jcajce {
    public interface IJcaSignerInfoGeneratorBuilder {
        ISignerInfoGenerator Build(IContentSigner signer, X509Certificate cert);
    }
}
