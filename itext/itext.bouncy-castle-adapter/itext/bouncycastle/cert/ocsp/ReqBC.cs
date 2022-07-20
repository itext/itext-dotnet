using System;
using Org.BouncyCastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class ReqBC : IReq {
        public readonly Req req;

        public ReqBC(Req req) {
            this.req = req;
        }

        public virtual Req GetReq() {
            return req;
        }

        public virtual ICertificateID GetCertID() {
            return new CertificateIDBC(req.GetCertID());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.ReqBC reqBC = (iText.Bouncycastle.Cert.Ocsp.ReqBC)o;
            return Object.Equals(req, reqBC.req);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(req);
        }

        public override String ToString() {
            return req.ToString();
        }
    }
}
