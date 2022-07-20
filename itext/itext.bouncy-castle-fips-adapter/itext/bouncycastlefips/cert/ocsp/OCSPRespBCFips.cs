using System;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastlefips.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class OCSPRespBCFips : IOCSPResp {
        private static readonly iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips INSTANCE = new iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips
            ((OcspResp)null);

        private readonly OcspResp ocspResp;

        public OCSPRespBCFips(OcspResp ocspResp) {
            this.ocspResp = ocspResp;
        }

        public OCSPRespBCFips(IOCSPResponse ocspResponse)
            : this(new OcspResp(((OCSPResponseBCFips)ocspResponse).GetOcspResponse())) {
        }

        public static iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips GetInstance() {
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
                throw new OCSPExceptionBCFips(e);
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
            iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips)o;
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
