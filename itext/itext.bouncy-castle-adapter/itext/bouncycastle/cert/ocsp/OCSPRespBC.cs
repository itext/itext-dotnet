using System;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>.
    /// </summary>
    public class OCSPRespBC : IOCSPResp {
        private static readonly iText.Bouncycastle.Cert.Ocsp.OCSPRespBC INSTANCE = new iText.Bouncycastle.Cert.Ocsp.OCSPRespBC
            ((OcspResp)null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        private readonly OcspResp ocspResp;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>.
        /// </summary>
        /// <param name="ocspResp">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>
        /// to be wrapped
        /// </param>
        public OCSPRespBC(OcspResp ocspResp) {
            this.ocspResp = ocspResp;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>.
        /// </summary>
        /// <param name="ocspResponse">OCSPResponse wrapper</param>
        public OCSPRespBC(IOCSPResponse ocspResponse)
            : this(new OcspResp(((OCSPResponseBC)ocspResponse).GetOcspResponse())) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="OCSPRespBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Cert.Ocsp.OCSPRespBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>.
        /// </returns>
        public virtual OcspResp GetOcspResp() {
            return ocspResp;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return ocspResp.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetStatus() {
            return ocspResp.Status;
        }

        /// <summary><inheritDoc/></summary>
        public virtual Object GetResponseObject() {
            try {
                return ocspResp.GetResponseObject();
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetSuccessful() {
            return SUCCESSFUL;
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
            iText.Bouncycastle.Cert.Ocsp.OCSPRespBC that = (iText.Bouncycastle.Cert.Ocsp.OCSPRespBC)o;
            return Object.Equals(ocspResp, that.ocspResp);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspResp);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return ocspResp.ToString();
        }
    }
}
