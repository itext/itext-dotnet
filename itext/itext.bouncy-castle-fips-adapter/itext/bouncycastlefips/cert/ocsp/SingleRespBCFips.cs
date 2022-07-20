using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class SingleRespBCFips : ISingleResp {
        private readonly SingleResp singleResp;

        public SingleRespBCFips(SingleResp singleResp) {
            this.singleResp = singleResp;
        }

        public virtual SingleResp GetSingleResp() {
            return singleResp;
        }

        public virtual ICertificateID GetCertID() {
            return new CertificateIDBCFips(singleResp.GetCertID());
        }

        public virtual ICertificateStatus GetCertStatus() {
            return new CertificateStatusBCFips(singleResp.GetCertStatus());
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
            iText.Bouncycastlefips.Cert.Ocsp.SingleRespBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.SingleRespBCFips
                )o;
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
