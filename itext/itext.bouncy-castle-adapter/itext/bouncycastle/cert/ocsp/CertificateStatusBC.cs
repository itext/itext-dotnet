using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class CertificateStatusBC : ICertificateStatus {
        private static readonly iText.Bouncycastle.Cert.Ocsp.CertificateStatusBC INSTANCE = new iText.Bouncycastle.Cert.Ocsp.CertificateStatusBC
            (null);

        private static readonly iText.Bouncycastle.Cert.Ocsp.CertificateStatusBC GOOD = new iText.Bouncycastle.Cert.Ocsp.CertificateStatusBC
            (CertificateStatus.Good);

        private readonly CertificateStatus certificateStatus;

        public CertificateStatusBC(CertificateStatus certificateStatus) {
            this.certificateStatus = certificateStatus;
        }

        public static iText.Bouncycastle.Cert.Ocsp.CertificateStatusBC GetInstance() {
            return INSTANCE;
        }

        public virtual CertificateStatus GetCertificateStatus() {
            return certificateStatus;
        }

        public virtual ICertificateStatus GetGood() {
            return GOOD;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.CertificateStatusBC that = (iText.Bouncycastle.Cert.Ocsp.CertificateStatusBC)
                o;
            return Object.Equals(certificateStatus, that.certificateStatus);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateStatus);
        }

        public override String ToString() {
            return certificateStatus.ToString();
        }
    }
}
