using System;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert {
    public class X509CertificateHolderBC : IX509CertificateHolder {
        private readonly X509CertificateHolder certificateHolder;

        public X509CertificateHolderBC(X509CertificateHolder certificateHolder) {
            this.certificateHolder = certificateHolder;
        }

        public X509CertificateHolderBC(byte[] bytes) {
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
            iText.Bouncycastle.Cert.X509CertificateHolderBC that = (iText.Bouncycastle.Cert.X509CertificateHolderBC)o;
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
