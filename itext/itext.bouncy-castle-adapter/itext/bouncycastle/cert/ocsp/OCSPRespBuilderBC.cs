using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class OCSPRespBuilderBC : IOCSPRespBuilder {
        private static readonly iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC INSTANCE = new iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC
            (null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        private readonly OCSPRespGenerator ocspRespBuilder;

        public OCSPRespBuilderBC(OCSPRespGenerator ocspRespBuilder) {
            this.ocspRespBuilder = ocspRespBuilder;
        }

        public static iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC GetInstance() {
            return INSTANCE;
        }

        public virtual OCSPRespGenerator GetOcspRespBuilder() {
            return ocspRespBuilder;
        }

        public virtual int GetSuccessful() {
            return SUCCESSFUL;
        }

        public virtual IOCSPResp Build(int i, IBasicOCSPResp basicOCSPResp) {
            try {
                return new OCSPRespBC(ocspRespBuilder.Generate(i, ((BasicOCSPRespBC)basicOCSPResp).GetBasicOCSPResp()));
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
            iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC that = (iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC)o;
            return Object.Equals(ocspRespBuilder, that.ocspRespBuilder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspRespBuilder);
        }

        public override String ToString() {
            return ocspRespBuilder.ToString();
        }
    }
}
