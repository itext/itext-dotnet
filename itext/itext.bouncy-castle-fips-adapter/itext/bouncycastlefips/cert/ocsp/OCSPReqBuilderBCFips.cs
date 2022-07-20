using System;
using Org.BouncyCastle.Cert.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class OCSPReqBuilderBCFips : IOCSPReqBuilder {
        private readonly OCSPReqBuilder reqBuilder;

        public OCSPReqBuilderBCFips(OCSPReqBuilder reqBuilder) {
            this.reqBuilder = reqBuilder;
        }

        public virtual OCSPReqBuilder GetReqBuilder() {
            return reqBuilder;
        }

        public virtual IOCSPReqBuilder SetRequestExtensions(IExtensions extensions) {
            reqBuilder.SetRequestExtensions(((ExtensionsBCFips)extensions).GetExtensions());
            return this;
        }

        public virtual IOCSPReqBuilder AddRequest(ICertificateID certificateID) {
            reqBuilder.AddRequest(((CertificateIDBCFips)certificateID).GetCertificateID());
            return this;
        }

        public virtual IOCSPReq Build() {
            try {
                return new OCSPReqBCFips(reqBuilder.Build());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBCFips(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBuilderBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBuilderBCFips
                )o;
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
