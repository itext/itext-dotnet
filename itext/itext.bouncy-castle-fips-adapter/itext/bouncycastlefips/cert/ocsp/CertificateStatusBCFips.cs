using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class CertificateStatusBCFips : ICertificateStatus {
        private static readonly iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips INSTANCE = new iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips
            (null);

        private static readonly iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips GOOD = new iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips
            (CertificateStatus.Good);

        private readonly CertificateStatus certificateStatus;

        public CertificateStatusBCFips(CertificateStatus certificateStatus) {
            this.certificateStatus = certificateStatus;
        }

        public static iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips GetInstance() {
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
            iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.CertificateStatusBCFips
                )o;
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
