using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class OCSPRespBuilderBCFips : IOCSPRespBuilder {
        private static readonly iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBuilderBCFips INSTANCE = new iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBuilderBCFips
            (null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        private readonly OCSPRespGenerator ocspRespBuilder;

        public OCSPRespBuilderBCFips(OCSPRespGenerator ocspRespBuilder) {
            this.ocspRespBuilder = ocspRespBuilder;
        }

        public static iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBuilderBCFips GetInstance() {
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
                return new OCSPRespBCFips(ocspRespBuilder.Generate(i, ((BasicOCSPRespBCFips)basicOCSPResp).GetBasicOCSPResp
                    ()));
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
            iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBuilderBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBuilderBCFips
                )o;
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
