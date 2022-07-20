using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle.Cms {
    public interface IRecipientId {
        bool Match(IX509CertificateHolder holder);
    }
}
