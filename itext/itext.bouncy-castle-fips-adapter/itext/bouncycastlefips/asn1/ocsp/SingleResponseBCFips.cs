using System;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp  {
    public class SingleResponseBCFips : ISingleResponse {
        private readonly SingleResponse singleResp;

        public SingleResponseBCFips(SingleResponse singleResp) {
            this.singleResp = singleResp;
        }

        public virtual SingleResponse GetSingleResp() {
            return singleResp;
        }

        public virtual ICertID GetCertID() {
            return new CertIDBCFips(singleResp.CertId);
        }

        public virtual ICertStatus GetCertStatus() {
            return new CertStatusBCFips(singleResp.CertStatus);
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
            SingleResponseBCFips that = (SingleResponseBCFips)o;
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
