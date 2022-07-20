using System;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.X509CertificateHolder"/>.
    /// </summary>
    public class X509CertificateHolderBCFips : IX509CertificateHolder {
        private readonly X509CertificateHolder certificateHolder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.X509CertificateHolder"/>.
        /// </summary>
        /// <param name="certificateHolder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.X509CertificateHolder"/>
        /// to be wrapped
        /// </param>
        public X509CertificateHolderBCFips(X509CertificateHolder certificateHolder) {
            this.certificateHolder = certificateHolder;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.X509CertificateHolder"/>.
        /// </summary>
        /// <param name="bytes">
        /// bytes array to create
        /// <see cref="Org.BouncyCastle.Cert.X509CertificateHolder"/>
        /// to be wrapped
        /// </param>
        public X509CertificateHolderBCFips(byte[] bytes) {
            this.certificateHolder = new X509CertificateHolder(bytes);
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.X509CertificateHolder"/>.
        /// </returns>
        public virtual X509CertificateHolder GetCertificateHolder() {
            return certificateHolder;
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
            iText.Bouncycastlefips.Cert.X509CertificateHolderBCFips that = (iText.Bouncycastlefips.Cert.X509CertificateHolderBCFips
                )o;
            return Object.Equals(certificateHolder, that.certificateHolder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateHolder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificateHolder.ToString();
        }
    }
}
