using System;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    public class X509CertificateHolderBCFips : IX509CertificateHolder {
        private readonly X509CertificateHolder certificateHolder;

        public X509CertificateHolderBCFips(X509CertificateHolder certificateHolder) {
            this.certificateHolder = certificateHolder;
        }

        public X509CertificateHolderBCFips(byte[] bytes) {
            this.certificateHolder = new X509CertificateHolder(bytes);
        }

        public virtual X509CertificateHolder GetCertificateHolder() {
            return certificateHolder;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateHolder);
        }

        public override String ToString() {
            return certificateHolder.ToString();
        }
    }
}
