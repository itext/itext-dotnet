using System;
using Java.Security;
using Org.BouncyCastle.Cert.Jcajce;
using Org.BouncyCastle.X509;
using iText.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Jcajce {
    public class JcaX509CertificateConverterBC : IJcaX509CertificateConverter {
        private readonly JcaX509CertificateConverter certificateConverter;

        public JcaX509CertificateConverterBC(JcaX509CertificateConverter certificateConverter) {
            this.certificateConverter = certificateConverter;
        }

        public virtual JcaX509CertificateConverter GetJsaX509CertificateConverter() {
            return certificateConverter;
        }

        public virtual X509Certificate GetCertificate(IX509CertificateHolder certificateHolder) {
            return certificateConverter.GetCertificate(((X509CertificateHolderBC)certificateHolder).GetCertificateHolder
                ());
        }

        public virtual IJcaX509CertificateConverter SetProvider(Provider provider) {
            certificateConverter.SetProvider(provider);
            return this;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateConverter);
        }

        public override String ToString() {
            return certificateConverter.ToString();
        }
    }
}
