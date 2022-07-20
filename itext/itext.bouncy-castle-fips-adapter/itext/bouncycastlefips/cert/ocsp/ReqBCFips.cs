using System;
using Org.BouncyCastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class ReqBCFips : IReq {
        public readonly Req req;

        public ReqBCFips(Req req) {
            this.req = req;
        }

        public virtual Req GetReq() {
            return req;
        }

        public virtual ICertificateID GetCertID() {
            return new CertificateIDBCFips(req.GetCertID());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.Ocsp.ReqBCFips reqBCFips = (iText.Bouncycastlefips.Cert.Ocsp.ReqBCFips)o;
            return Object.Equals(req, reqBCFips.req);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(req);
        }

        public override String ToString() {
            return req.ToString();
        }
    }
}
