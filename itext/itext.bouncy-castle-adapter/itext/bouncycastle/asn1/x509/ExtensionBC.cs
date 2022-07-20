using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class ExtensionBC : ASN1EncodableBC, IExtension {
        private static readonly iText.Bouncycastle.Asn1.X509.ExtensionBC INSTANCE = new iText.Bouncycastle.Asn1.X509.ExtensionBC
            (null);

        private static readonly ASN1ObjectIdentifierBC C_RL_DISTRIBUTION_POINTS = new ASN1ObjectIdentifierBC(X509Extensions.CrlDistributionPoints
            );

        private static readonly ASN1ObjectIdentifierBC AUTHORITY_INFO_ACCESS = new ASN1ObjectIdentifierBC(X509Extensions.AuthorityInfoAccess
            );

        private static readonly ASN1ObjectIdentifierBC BASIC_CONSTRAINTS = new ASN1ObjectIdentifierBC(X509Extensions
            .basicConstraints);

        private static readonly ASN1ObjectIdentifierBC KEY_USAGE = new ASN1ObjectIdentifierBC(X509Extensions.keyUsage
            );

        private static readonly ASN1ObjectIdentifierBC EXTENDED_KEY_USAGE = new ASN1ObjectIdentifierBC(X509Extensions
            .extendedKeyUsage);

        private static readonly ASN1ObjectIdentifierBC AUTHORITY_KEY_IDENTIFIER = new ASN1ObjectIdentifierBC(X509Extensions
            .authorityKeyIdentifier);

        private static readonly ASN1ObjectIdentifierBC SUBJECT_KEY_IDENTIFIER = new ASN1ObjectIdentifierBC(X509Extensions
            .subjectKeyIdentifier);

        public ExtensionBC(X509Extensions extension)
            : base(extension) {
        }

        public ExtensionBC(IASN1ObjectIdentifier objectIdentifier, bool critical, IASN1OctetString octetString)
            : base(new X509Extensions(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(), critical, 
                ((ASN1OctetStringBC)octetString).GetASN1OctetString())) {
        }

        public static iText.Bouncycastle.Asn1.X509.ExtensionBC GetInstance() {
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
