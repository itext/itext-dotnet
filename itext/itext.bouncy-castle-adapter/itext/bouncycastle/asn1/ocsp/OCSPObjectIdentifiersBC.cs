using System;
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1.Ocsp {
    public class OCSPObjectIdentifiersBC : IOCSPObjectIdentifiers {
        private static readonly iText.Bouncycastle.Asn1.Ocsp.OCSPObjectIdentifiersBC INSTANCE = new iText.Bouncycastle.Asn1.Ocsp.OCSPObjectIdentifiersBC
            (null);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_BASIC = new ASN1ObjectIdentifierBC(OcspObjectIdentifiers
            .PkixOcspBasic);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_NONCE = new ASN1ObjectIdentifierBC(OcspObjectIdentifiers
            .id_pkix_ocsp_nonce);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_NOCHECK = new ASN1ObjectIdentifierBC(OcspObjectIdentifiers
            .PkixOcspNocheck);

        private readonly OcspObjectIdentifiers ocspObjectIdentifiers;

        public OCSPObjectIdentifiersBC(OcspObjectIdentifiers ocspObjectIdentifiers) {
            this.ocspObjectIdentifiers = ocspObjectIdentifiers;
        }

        public static iText.Bouncycastle.Asn1.Ocsp.OCSPObjectIdentifiersBC GetInstance() {
            return INSTANCE;
        }

        public virtual OcspObjectIdentifiers GetOCSPObjectIdentifiers() {
            return ocspObjectIdentifiers;
        }

        public virtual IASN1ObjectIdentifier GetIdPkixOcspBasic() {
            return ID_PKIX_OCSP_BASIC;
        }

        public virtual IASN1ObjectIdentifier GetIdPkixOcspNonce() {
            return ID_PKIX_OCSP_NONCE;
        }

        public virtual IASN1ObjectIdentifier GetIdPkixOcspNoCheck() {
            return ID_PKIX_OCSP_NOCHECK;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Asn1.Ocsp.OCSPObjectIdentifiersBC that = (iText.Bouncycastle.Asn1.Ocsp.OCSPObjectIdentifiersBC
                )o;
            return Object.Equals(ocspObjectIdentifiers, that.ocspObjectIdentifiers);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspObjectIdentifiers);
        }

        public override String ToString() {
            return ocspObjectIdentifiers.ToString();
        }
    }
}
