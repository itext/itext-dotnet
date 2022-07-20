using System;
using Org.BouncyCastle.Cert.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class OCSPReqBC : IOCSPReq {
        private readonly OcspReq ocspReq;

        public OCSPReqBC(OcspReq ocspReq) {
            this.ocspReq = ocspReq;
        }

        public virtual OcspReq GetOcspReq() {
            return ocspReq;
        }

        public virtual byte[] GetEncoded() {
            return ocspReq.GetEncoded();
        }

        public virtual IReq[] GetRequestList() {
            Req[] reqs = ocspReq.GetRequestList();
            IReq[] reqsBC = new IReq[reqs.Length];
            for (int i = 0; i < reqs.Length; ++i) {
                reqsBC[i] = new ReqBC(reqs[i]);
            }
            return reqsBC;
        }

        public virtual IExtension GetExtension(IASN1ObjectIdentifier objectIdentifier) {
            return new ExtensionBC(ocspReq.GetExtension(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier
                ()));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.OCSPReqBC ocspReqBC = (iText.Bouncycastle.Cert.Ocsp.OCSPReqBC)o;
            return Object.Equals(ocspReq, ocspReqBC.ocspReq);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspReq);
        }

        public override String ToString() {
            return ocspReq.ToString();
        }
    }
}
