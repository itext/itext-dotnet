using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class ExtensionBCFips : ASN1EncodableBCFips, IExtension {
        private static readonly iText.Bouncycastlefips.Asn1.X509.ExtensionBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.ExtensionBCFips
            (null);

        private static readonly ASN1ObjectIdentifierBCFips C_RL_DISTRIBUTION_POINTS = new ASN1ObjectIdentifierBCFips
            (X509Extensions.CrlDistributionPoints);

        private static readonly ASN1ObjectIdentifierBCFips AUTHORITY_INFO_ACCESS = new ASN1ObjectIdentifierBCFips(
            X509Extensions.AuthorityInfoAccess);

        private static readonly ASN1ObjectIdentifierBCFips BASIC_CONSTRAINTS = new ASN1ObjectIdentifierBCFips(X509Extensions
            .BasicConstraints);

        private static readonly ASN1ObjectIdentifierBCFips KEY_USAGE = new ASN1ObjectIdentifierBCFips(X509Extensions
            .KeyUsage);

        private static readonly ASN1ObjectIdentifierBCFips EXTENDED_KEY_USAGE = new ASN1ObjectIdentifierBCFips(X509Extensions
            .ExtendedKeyUsage);

        private static readonly ASN1ObjectIdentifierBCFips AUTHORITY_KEY_IDENTIFIER = new ASN1ObjectIdentifierBCFips
            (X509Extensions.AuthorityKeyIdentifier);

        private static readonly ASN1ObjectIdentifierBCFips SUBJECT_KEY_IDENTIFIER = new ASN1ObjectIdentifierBCFips
            (X509Extensions.SubjectKeyIdentifier);

        public ExtensionBCFips(X509Extensions extension)
            : base(extension) {
        }

        public ExtensionBCFips(IASN1ObjectIdentifier objectIdentifier, bool critical, IASN1OctetString octetString)
            : base(new X509Extensions(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier(), critical
                , ((ASN1OctetStringBCFips)octetString).GetOctetString())) {
        }

        public static iText.Bouncycastlefips.Asn1.X509.ExtensionBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual X509Extensions GetExtension() {
            return (X509Extensions)GetEncodable();
        }

        public virtual IASN1ObjectIdentifier GetCRlDistributionPoints() {
            return C_RL_DISTRIBUTION_POINTS;
        }

        public virtual IASN1ObjectIdentifier GetAuthorityInfoAccess() {
            return AUTHORITY_INFO_ACCESS;
        }

        public virtual IASN1ObjectIdentifier GetBasicConstraints() {
            return BASIC_CONSTRAINTS;
        }

        public virtual IASN1ObjectIdentifier GetKeyUsage() {
            return KEY_USAGE;
        }

        public virtual IASN1ObjectIdentifier GetExtendedKeyUsage() {
            return EXTENDED_KEY_USAGE;
        }

        public virtual IASN1ObjectIdentifier GetAuthorityKeyIdentifier() {
            return AUTHORITY_KEY_IDENTIFIER;
        }

        public virtual IASN1ObjectIdentifier GetSubjectKeyIdentifier() {
            return SUBJECT_KEY_IDENTIFIER;
        }
    }
}
