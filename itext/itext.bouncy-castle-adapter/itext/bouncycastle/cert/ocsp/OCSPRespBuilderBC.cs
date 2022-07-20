using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.OCSPRespGenerator"/>.
    /// </summary>
    public class OCSPRespBuilderBC : IOCSPRespBuilder {
        private static readonly iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC INSTANCE = new iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC
            (null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        private readonly OCSPRespGenerator ocspRespBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OCSPRespGenerator"/>.
        /// </summary>
        /// <param name="ocspRespBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.OCSPRespGenerator"/>
        /// to be wrapped
        /// </param>
        public OCSPRespBuilderBC(OCSPRespGenerator ocspRespBuilder) {
            this.ocspRespBuilder = ocspRespBuilder;
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="OCSPRespBuilderBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.OCSPRespGenerator"/>.
        /// </returns>
        public virtual OCSPRespGenerator GetOcspRespBuilder() {
            return ocspRespBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetSuccessful() {
            return SUCCESSFUL;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPResp Build(int i, IBasicOCSPResp basicOCSPResp) {
            try {
                return new OCSPRespBC(ocspRespBuilder.Generate(i, ((BasicOCSPRespBC)basicOCSPResp).GetBasicOCSPResp()));
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
            iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC that = (iText.Bouncycastle.Cert.Ocsp.OCSPRespBuilderBC)o;
            return Object.Equals(ocspRespBuilder, that.ocspRespBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspRespBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return ocspRespBuilder.ToString();
        }
    }
}
