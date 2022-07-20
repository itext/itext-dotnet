using System;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
    /// </summary>
    public class OCSPReqBCFips : IOCSPReq {
        private readonly OcspRequest ocspReq;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>.
        /// </summary>
        /// <param name="ocspReq">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>
        /// to be wrapped
        /// </param>
        public OCSPReqBCFips(OcspRequest ocspReq) {
            this.ocspReq = ocspReq;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>.
        /// </summary>
        /// <param name="ocspReq">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>
        /// to be wrapped
        /// </param>
        public OCSPReqBCFips(ICertificateID certId, byte[] documentId)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>.
        /// </returns>
        public virtual OcspRequest GetOcspReq() {
            return ocspReq;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return ocspReq.GetEncoded();
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
            iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBCFips)o;
            return Object.Equals(ocspReq, that.ocspReq);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspReq);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return ocspReq.ToString();
        }
    }
}
