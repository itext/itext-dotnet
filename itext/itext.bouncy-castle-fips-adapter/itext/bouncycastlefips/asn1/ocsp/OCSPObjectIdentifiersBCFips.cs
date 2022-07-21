using System;
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class OCSPObjectIdentifiersBCFips : IOCSPObjectIdentifiers {
        private static readonly OCSPObjectIdentifiersBCFips INSTANCE = new OCSPObjectIdentifiersBCFips
            (null);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_BASIC = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspBasic);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_NONCE = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspNonce);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_NOCHECK = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspNocheck);

        private readonly OcspObjectIdentifiers ocspObjectIdentifiers;

        public OCSPObjectIdentifiersBCFips(OcspObjectIdentifiers ocspObjectIdentifiers) {
            this.ocspObjectIdentifiers = ocspObjectIdentifiers;
        }

        public static OCSPObjectIdentifiersBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual OcspObjectIdentifiers GetOcspObjectIdentifiers() {
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
            OCSPObjectIdentifiersBCFips that = (OCSPObjectIdentifiersBCFips)o;
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
