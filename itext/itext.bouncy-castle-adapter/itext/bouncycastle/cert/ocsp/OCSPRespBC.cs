using System;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class OCSPRespBC : IOCSPResp {
        private static readonly iText.Bouncycastle.Cert.Ocsp.OCSPRespBC INSTANCE = new iText.Bouncycastle.Cert.Ocsp.OCSPRespBC
            ((OcspResp)null);

        private readonly OcspResp ocspResp;

        public OCSPRespBC(OcspResp ocspResp) {
            this.ocspResp = ocspResp;
        }

        public OCSPRespBC(IOCSPResponse ocspResponse)
            : this(new OcspResp(((OCSPResponseBC)ocspResponse).GetOcspResponse())) {
        }

        public static iText.Bouncycastle.Cert.Ocsp.OCSPRespBC GetInstance() {
            return INSTANCE;
        }

        public virtual OcspResp GetOcspResp() {
            return ocspResp;
        }

        public virtual byte[] GetEncoded() {
            return ocspResp.GetEncoded();
        }

        public virtual int GetStatus() {
            return ocspResp.Status;
        }

        public virtual Object GetResponseObject() {
            try {
                return ocspResp.GetResponseObject();
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
        }

        public virtual int GetSuccessful() {
            return Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspResp);
        }

        public override String ToString() {
            return ocspResp.ToString();
        }
    }
}
