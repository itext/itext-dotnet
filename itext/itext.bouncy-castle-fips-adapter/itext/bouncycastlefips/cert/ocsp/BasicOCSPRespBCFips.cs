using System;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastlefips.Cert;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class BasicOCSPRespBCFips : IBasicOCSPResp {
        private readonly BasicOcspResp basicOCSPResp;

        public BasicOCSPRespBCFips(BasicOcspResp basicOCSPResp) {
            this.basicOCSPResp = basicOCSPResp;
        }

        public virtual BasicOcspResp GetBasicOCSPResp() {
            return basicOCSPResp;
        }

        public virtual ISingleResp[] GetResponses() {
            SingleResp[] resps = basicOCSPResp.Responses;
            ISingleResp[] respsBCFips = new ISingleResp[resps.Length];
            for (int i = 0; i < respsBCFips.Length; i++) {
                respsBCFips[i] = new SingleRespBCFips(resps[i]);
            }
            return respsBCFips;
        }

        public virtual bool IsSignatureValid(IContentVerifierProvider provider) {
            try {
                return basicOCSPResp.IsSignatureValid(((ContentVerifierProviderBCFips)provider).GetProvider());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBCFips(e);
            }
        }

        public virtual IX509CertificateHolder[] GetCerts() {
            X509CertificateHolder[] certs = basicOCSPResp.GetCerts();
            IX509CertificateHolder[] certsBCFips = new IX509CertificateHolder[certs.Length];
            for (int i = 0; i < certs.Length; i++) {
                certsBCFips[i] = new X509CertificateHolderBCFips(certs[i]);
            }
            return certsBCFips;
        }

        public virtual byte[] GetEncoded() {
            return basicOCSPResp.GetEncoded();
        }

        public virtual DateTime GetProducedAt() {
            return basicOCSPResp.ProducedAt;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.Ocsp.BasicOCSPRespBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.BasicOCSPRespBCFips
                )o;
            return Object.Equals(basicOCSPResp, that.basicOCSPResp);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(basicOCSPResp);
        }

        public override String ToString() {
            return basicOCSPResp.ToString();
        }
    }
}
