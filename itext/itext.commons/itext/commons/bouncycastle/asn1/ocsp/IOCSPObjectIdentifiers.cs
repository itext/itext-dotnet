using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    public interface IOCSPObjectIdentifiers {
        IASN1ObjectIdentifier GetIdPkixOcspBasic();

        IASN1ObjectIdentifier GetIdPkixOcspNonce();

        IASN1ObjectIdentifier GetIdPkixOcspNoCheck();
    }
}
