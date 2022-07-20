using System;
using Java.Security;
using Org.BouncyCastle.Cert.Jcajce;
using Org.BouncyCastle.X509;
using iText.Bouncycastlefips.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Jcajce {
    public class JcaX509CertificateConverterBCFips : IJcaX509CertificateConverter {
        private readonly JcaX509CertificateConverter certificateConverter;

        public JcaX509CertificateConverterBCFips(JcaX509CertificateConverter certificateConverter) {
            this.certificateConverter = certificateConverter;
        }

        public virtual JcaX509CertificateConverter GetCertificateConverter() {
            return certificateConverter;
        }

        public virtual X509Certificate GetCertificate(IX509CertificateHolder certificateHolder) {
            return certificateConverter.GetCertificate(((X509CertificateHolderBCFips)certificateHolder).GetCertificateHolder
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
            iText.Bouncycastlefips.Cert.Jcajce.JcaX509CertificateConverterBCFips that = (iText.Bouncycastlefips.Cert.Jcajce.JcaX509CertificateConverterBCFips
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
