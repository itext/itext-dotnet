using System;
using iText.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>.
    /// </summary>
    public class SingleRespBC : ISingleResp {
        private readonly SingleResponse singleResp;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>.
        /// </summary>
        /// <param name="singleResp">
        /// 
        /// <see cref="SingleResponse"/>
        /// to be wrapped
        /// </param>
        public SingleRespBC(SingleResponse singleResp) {
            this.singleResp = singleResp;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>.
        /// </summary>
        /// <param name="basicResp">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOCSPResponse"/>
        /// wrapper to get
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>
        /// </param>
        public SingleRespBC(IBasicOCSPResponse basicResp)
            : this(SingleResponse.GetInstance(((BasicOCSPResponseBC)basicResp)
                .GetBasicOCSPResponse().TbsResponseData.Responses[0])) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>.
        /// </returns>
        public virtual SingleResponse GetSingleResp() {
            return singleResp;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertificateID GetCertID() {
            return new CertificateIDBC(singleResp.CertId);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertificateStatus GetCertStatus() {
            return new CertificateStatusBC(singleResp.CertStatus);
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetNextUpdate() {
            return singleResp.NextUpdate.ToDateTime();
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetThisUpdate() {
            return singleResp.ThisUpdate.ToDateTime();
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.SingleRespBC that = (iText.Bouncycastle.Cert.Ocsp.SingleRespBC)o;
            return Object.Equals(singleResp, that.singleResp);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(singleResp);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return singleResp.ToString();
        }
    }
}
