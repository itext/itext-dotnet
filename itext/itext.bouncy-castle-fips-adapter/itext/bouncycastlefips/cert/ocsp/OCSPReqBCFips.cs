using System;
using Org.BouncyCastle.Cert.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class OCSPReqBCFips : IOCSPReq {
        private readonly OcspReq ocspReq;

        public OCSPReqBCFips(OcspReq ocspReq) {
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
            IReq[] reqsBCFips = new IReq[reqs.Length];
            for (int i = 0; i < reqs.Length; ++i) {
                reqsBCFips[i] = new ReqBCFips(reqs[i]);
            }
            return reqsBCFips;
        }

        public virtual IExtension GetExtension(IASN1ObjectIdentifier objectIdentifier) {
            return new ExtensionBCFips(ocspReq.GetExtension(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier
                ()));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBCFips)o;
            return Object.Equals(ocspReq, that.ocspReq);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspReq);
        }

        public override String ToString() {
            return ocspReq.ToString();
        }
    }
}
