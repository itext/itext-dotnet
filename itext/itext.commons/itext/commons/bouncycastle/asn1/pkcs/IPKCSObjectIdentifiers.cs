using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Pkcs {
    public interface IPKCSObjectIdentifiers {
        IASN1ObjectIdentifier GetIdAaSignatureTimeStampToken();

        IASN1ObjectIdentifier GetIdAaEtsSigPolicyId();

        IASN1ObjectIdentifier GetIdSpqEtsUri();

        IASN1ObjectIdentifier GetEnvelopedData();

        IASN1ObjectIdentifier GetData();
    }
}
