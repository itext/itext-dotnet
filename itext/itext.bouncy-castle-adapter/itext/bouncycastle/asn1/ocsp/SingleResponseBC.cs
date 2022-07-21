using System;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class SingleResponseBC : ISingleResponse {
        private readonly SingleResponse singleResp;

        public SingleResponseBC(SingleResponse singleResp) {
            this.singleResp = singleResp;
        }

        public virtual SingleResponse GetSingleResp() {
            return singleResp;
        }

        public virtual ICertID GetCertID() {
            return new CertIDBC(singleResp.CertId);
        }

        public virtual ICertStatus GetCertStatus() {
            return new CertStatusBC(singleResp.CertStatus);
        }

        public virtual DateTime GetNextUpdate() {
            return singleResp.NextUpdate.ToDateTime();
        }

        public virtual DateTime GetThisUpdate() {
            return singleResp.ThisUpdate.ToDateTime();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            SingleResponseBC that = (SingleResponseBC)o;
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
