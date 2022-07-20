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
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.BasicOcspResp"/>.
    /// </summary>
    public class BasicOCSPRespBC : IBasicOCSPResp {
        private readonly BasicOcspResp basicOCSPResp;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.BasicOcspResp"/>.
        /// </summary>
        /// <param name="basicOCSPResp">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.BasicOcspResp"/>
        /// to be wrapped
        /// </param>
        public BasicOCSPRespBC(BasicOcspResp basicOCSPResp) {
            this.basicOCSPResp = basicOCSPResp;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.BasicOcspResp"/>.
        /// </returns>
        public virtual BasicOcspResp GetBasicOCSPResp() {
            return basicOCSPResp;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISingleResp[] GetResponses() {
            SingleResp[] resps = basicOCSPResp.Responses;
            ISingleResp[] respsBC = new ISingleResp[resps.Length];
            for (int i = 0; i < respsBC.Length; i++) {
                respsBC[i] = new SingleRespBC(resps[i]);
            }
            return respsBC;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsSignatureValid(IContentVerifierProvider provider) {
            try {
                return basicOCSPResp.IsSignatureValid(((ContentVerifierProviderBC)provider).GetContentVerifierProvider());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509CertificateHolder[] GetCerts() {
            X509CertificateHolder[] certs = basicOCSPResp.GetCerts();
            IX509CertificateHolder[] certsBC = new IX509CertificateHolder[certs.Length];
            for (int i = 0; i < certs.Length; i++) {
                certsBC[i] = new X509CertificateHolderBC(certs[i]);
            }
            return certsBC;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return basicOCSPResp.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetProducedAt() {
            return basicOCSPResp.ProducedAt;
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(basicOCSPResp);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return basicOCSPResp.ToString();
        }
    }
}
