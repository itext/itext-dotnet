using System;
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers"/>.
    /// </summary>
    public class OCSPObjectIdentifiersBCFips : IOCSPObjectIdentifiers {
        private static readonly iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips
            (null);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_BASIC = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspBasic);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_NONCE = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspNonce);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_NOCHECK = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspNocheck);

        private readonly OcspObjectIdentifiers ocspObjectIdentifiers;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers"/>.
        /// </summary>
        /// <param name="ocspObjectIdentifiers">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers"/>
        /// to be wrapped
        /// </param>
        public OCSPObjectIdentifiersBCFips(OcspObjectIdentifiers ocspObjectIdentifiers) {
            this.ocspObjectIdentifiers = ocspObjectIdentifiers;
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="OCSPObjectIdentifiersBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers"/>.
        /// </returns>
        public virtual OcspObjectIdentifiers GetOcspObjectIdentifiers() {
            return ocspObjectIdentifiers;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetIdPkixOcspBasic() {
            return ID_PKIX_OCSP_BASIC;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetIdPkixOcspNonce() {
            return ID_PKIX_OCSP_NONCE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetIdPkixOcspNoCheck() {
            return ID_PKIX_OCSP_NOCHECK;
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips that = (iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips
                )o;
            return Object.Equals(ocspObjectIdentifiers, that.ocspObjectIdentifiers);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspObjectIdentifiers);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return ocspObjectIdentifiers.ToString();
        }
    }
}
