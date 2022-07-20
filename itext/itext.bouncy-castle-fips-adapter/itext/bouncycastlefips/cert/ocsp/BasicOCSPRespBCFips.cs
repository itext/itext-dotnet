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
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.BasicOcspResp"/>.
    /// </summary>
    public class BasicOCSPRespBCFips : IBasicOCSPResp {
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
        public BasicOCSPRespBCFips(BasicOcspResp basicOCSPResp) {
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
            ISingleResp[] respsBCFips = new ISingleResp[resps.Length];
            for (int i = 0; i < respsBCFips.Length; i++) {
                respsBCFips[i] = new SingleRespBCFips(resps[i]);
            }
            return respsBCFips;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsSignatureValid(IContentVerifierProvider provider) {
            try {
                return basicOCSPResp.IsSignatureValid(((ContentVerifierProviderBCFips)provider).GetProvider());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBCFips(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509CertificateHolder[] GetCerts() {
            X509CertificateHolder[] certs = basicOCSPResp.GetCerts();
            IX509CertificateHolder[] certsBCFips = new IX509CertificateHolder[certs.Length];
            for (int i = 0; i < certs.Length; i++) {
                certsBCFips[i] = new X509CertificateHolderBCFips(certs[i]);
            }
            return certsBCFips;
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
            iText.Bouncycastlefips.Cert.Ocsp.BasicOCSPRespBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.BasicOCSPRespBCFips
                )o;
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
