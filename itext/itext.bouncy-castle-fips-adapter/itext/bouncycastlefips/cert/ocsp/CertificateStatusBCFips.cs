using System;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>.
    /// </summary>
    public class CertificateStatusBCFips : ICertificateStatus {
        private static readonly CertificateStatusBCFips INSTANCE = new CertificateStatusBCFips(null);

        private static readonly CertificateStatusBCFips GOOD = new CertificateStatusBCFips(new CertStatus());

        private readonly CertStatus certificateStatus;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>.
        /// </summary>
        /// <param name="certificateStatus">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>
        /// to be wrapped
        /// </param>
        public CertificateStatusBCFips(CertStatus certificateStatus) {
            this.certificateStatus = certificateStatus;
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="CertificateStatusBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>.
        /// </returns>
        public virtual CertStatus GetCertificateStatus() {
            return certificateStatus;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertificateStatus GetGood() {
            return GOOD;
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
            iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips
                )o;
            return Object.Equals(certificateStatus, that.certificateStatus);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateStatus);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificateStatus.ToString();
        }
    }
}
