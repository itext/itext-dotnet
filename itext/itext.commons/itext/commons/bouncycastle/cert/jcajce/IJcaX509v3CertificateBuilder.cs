using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Jcajce {
    public interface IJcaX509v3CertificateBuilder {
        IX509CertificateHolder Build(IContentSigner contentSigner);

        IJcaX509v3CertificateBuilder AddExtension(IASN1ObjectIdentifier extensionOID, bool critical, IASN1Encodable
             extensionValue);
    }
}
