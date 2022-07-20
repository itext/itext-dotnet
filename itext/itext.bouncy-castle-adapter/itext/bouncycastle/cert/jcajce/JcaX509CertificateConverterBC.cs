using System;
using Java.Security;
using Org.BouncyCastle.Cert.Jcajce;
using Org.BouncyCastle.X509;
using iText.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509CertificateConverter"/>.
    /// </summary>
    public class JcaX509CertificateConverterBC : IJcaX509CertificateConverter {
        private readonly JcaX509CertificateConverter certificateConverter;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509CertificateConverter"/>.
        /// </summary>
        /// <param name="certificateConverter">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509CertificateConverter"/>
        /// to be wrapped
        /// </param>
        public JcaX509CertificateConverterBC(JcaX509CertificateConverter certificateConverter) {
            this.certificateConverter = certificateConverter;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509CertificateConverter"/>.
        /// </returns>
        public virtual JcaX509CertificateConverter GetJsaX509CertificateConverter() {
            return certificateConverter;
        }

        /// <summary><inheritDoc/></summary>
        public virtual X509Certificate GetCertificate(IX509CertificateHolder certificateHolder) {
            return certificateConverter.GetCertificate(((X509CertificateHolderBC)certificateHolder).GetCertificateHolder
                ());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJcaX509CertificateConverter SetProvider(Provider provider) {
            certificateConverter.SetProvider(provider);
            return this;
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
            iText.Bouncycastle.Cert.Jcajce.JcaX509CertificateConverterBC that = (iText.Bouncycastle.Cert.Jcajce.JcaX509CertificateConverterBC
                )o;
            return Object.Equals(certificateConverter, that.certificateConverter);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateConverter);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificateConverter.ToString();
        }
    }
}
