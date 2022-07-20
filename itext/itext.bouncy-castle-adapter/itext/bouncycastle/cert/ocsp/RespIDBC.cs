using System;
using Org.BouncyCastle.Cert.Ocsp;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class RespIDBC : IRespID {
        private readonly RespID respID;

        public RespIDBC(RespID respID) {
            this.respID = respID;
        }

        public RespIDBC(IX500Name x500Name)
            : this(new RespID(((X500NameBC)x500Name).GetX500Name())) {
        }

        public virtual RespID GetRespID() {
            return respID;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.RespIDBC that = (iText.Bouncycastle.Cert.Ocsp.RespIDBC)o;
            return Object.Equals(respID, that.respID);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(respID);
        }

        public override String ToString() {
            return respID.ToString();
        }
    }
}
