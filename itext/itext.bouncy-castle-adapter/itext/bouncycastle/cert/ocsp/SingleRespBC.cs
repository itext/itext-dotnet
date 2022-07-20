using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class SingleRespBC : ISingleResp {
        private readonly SingleResp singleResp;

        public SingleRespBC(SingleResp singleResp) {
            this.singleResp = singleResp;
        }

        public virtual SingleResp GetSingleResp() {
            return singleResp;
        }

        public virtual ICertificateID GetCertID() {
            return new CertificateIDBC(singleResp.GetCertID());
        }

        public virtual ICertificateStatus GetCertStatus() {
            return new CertificateStatusBC(singleResp.GetCertStatus());
        }

        public virtual DateTime GetNextUpdate() {
            return singleResp.NextUpdate;
        }

        public virtual DateTime GetThisUpdate() {
            return singleResp.ThisUpdate;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.SingleRespBC that = (iText.Bouncycastle.Cert.Ocsp.SingleRespBC)o;
            return Object.Equals(singleResp, that.singleResp);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(singleResp);
        }

        public override String ToString() {
            return singleResp.ToString();
        }
    }
}
