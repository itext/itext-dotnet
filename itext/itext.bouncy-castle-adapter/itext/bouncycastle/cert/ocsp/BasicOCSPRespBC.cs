using System;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class BasicOCSPRespBC : IBasicOCSPResp {
        private readonly BasicOcspResp basicOCSPResp;

        public BasicOCSPRespBC(BasicOcspResp basicOCSPRespBC) {
            this.basicOCSPResp = basicOCSPRespBC;
        }

        public virtual BasicOcspResp GetBasicOCSPResp() {
            return basicOCSPResp;
        }

        public virtual ISingleResp[] GetResponses() {
            SingleResp[] resps = basicOCSPResp.Responses;
            ISingleResp[] respsBC = new ISingleResp[resps.Length];
            for (int i = 0; i < respsBC.Length; i++) {
                respsBC[i] = new SingleRespBC(resps[i]);
            }
            return respsBC;
        }

        public virtual bool IsSignatureValid(IContentVerifierProvider provider) {
            try {
                return basicOCSPResp.IsSignatureValid(((ContentVerifierProviderBC)provider).GetContentVerifierProvider());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
        }

        public virtual IX509CertificateHolder[] GetCerts() {
            X509CertificateHolder[] certs = basicOCSPResp.GetCerts();
            IX509CertificateHolder[] certsBC = new IX509CertificateHolder[certs.Length];
            for (int i = 0; i < certs.Length; i++) {
                certsBC[i] = new X509CertificateHolderBC(certs[i]);
            }
            return certsBC;
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
            iText.Bouncycastle.Cert.Ocsp.BasicOCSPRespBC that = (iText.Bouncycastle.Cert.Ocsp.BasicOCSPRespBC)o;
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
