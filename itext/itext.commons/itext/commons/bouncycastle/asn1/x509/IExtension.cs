using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface IExtension : IASN1Encodable {
        IASN1ObjectIdentifier GetCRlDistributionPoints();

        IASN1ObjectIdentifier GetAuthorityInfoAccess();

        IASN1ObjectIdentifier GetBasicConstraints();

        IASN1ObjectIdentifier GetKeyUsage();

        IASN1ObjectIdentifier GetExtendedKeyUsage();

        IASN1ObjectIdentifier GetAuthorityKeyIdentifier();

        IASN1ObjectIdentifier GetSubjectKeyIdentifier();
    }
}
