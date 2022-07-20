using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Cert {
    public interface IX509ExtensionUtils {
        IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo);

        ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo);
    }
}
