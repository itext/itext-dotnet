using System;
using Org.BouncyCastle.Cert.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class OCSPReqBuilderBC : IOCSPReqBuilder {
        private readonly OCSPReqBuilder reqBuilder;

        public OCSPReqBuilderBC(OCSPReqBuilder reqBuilder) {
            this.reqBuilder = reqBuilder;
        }

        public virtual OCSPReqBuilder GetReqBuilder() {
            return reqBuilder;
        }

        public virtual IOCSPReqBuilder SetRequestExtensions(IExtensions extensions) {
            reqBuilder.SetRequestExtensions(((ExtensionsBC)extensions).GetExtensions());
            return this;
        }

        public virtual IOCSPReqBuilder AddRequest(ICertificateID certificateID) {
            reqBuilder.AddRequest(((CertificateIDBC)certificateID).GetCertificateID());
            return this;
        }

        public virtual IOCSPReq Build() {
            try {
                return new OCSPReqBC(reqBuilder.Build());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.OCSPReqBuilderBC that = (iText.Bouncycastle.Cert.Ocsp.OCSPReqBuilderBC)o;
            return Object.Equals(reqBuilder, that.reqBuilder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(reqBuilder);
        }

        public override String ToString() {
            return reqBuilder.ToString();
        }
    }
}
