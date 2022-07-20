using System;
using Org.BouncyCastle.Cert.Ocsp;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class RespIDBCFips : IRespID {
        private readonly RespID respID;

        public RespIDBCFips(RespID respID) {
            this.respID = respID;
        }

        public RespIDBCFips(IX500Name x500Name)
            : this(new RespID(((X500NameBCFips)x500Name).GetX500Name())) {
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
            iText.Bouncycastlefips.Cert.Ocsp.RespIDBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.RespIDBCFips)o;
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
