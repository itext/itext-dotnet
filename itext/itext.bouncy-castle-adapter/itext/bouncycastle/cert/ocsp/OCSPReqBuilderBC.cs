using System;
using Org.BouncyCastle.Cert.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Ocsp.OCSPReqBuilder"/>.
    /// </summary>
    public class OCSPReqBuilderBC : IOCSPReqBuilder {
        private readonly OCSPReqBuilder reqBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.OCSPReqBuilder"/>.
        /// </summary>
        /// <param name="reqBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.OCSPReqBuilder"/>
        /// to be wrapped
        /// </param>
        public OCSPReqBuilderBC(OCSPReqBuilder reqBuilder) {
            this.reqBuilder = reqBuilder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.OCSPReqBuilder"/>.
        /// </returns>
        public virtual OCSPReqBuilder GetReqBuilder() {
            return reqBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPReqBuilder SetRequestExtensions(IExtensions extensions) {
            reqBuilder.SetRequestExtensions(((ExtensionsBC)extensions).GetExtensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPReqBuilder AddRequest(ICertificateID certificateID) {
            reqBuilder.AddRequest(((CertificateIDBC)certificateID).GetCertificateID());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPReq Build() {
            try {
                return new OCSPReqBC(reqBuilder.Build());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
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
            iText.Bouncycastle.Cert.Ocsp.OCSPReqBuilderBC that = (iText.Bouncycastle.Cert.Ocsp.OCSPReqBuilderBC)o;
            return Object.Equals(reqBuilder, that.reqBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(reqBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return reqBuilder.ToString();
        }
    }
}
