using System;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class CertStatusBC : ICertStatus {
        private static readonly CertStatusBC INSTANCE = new CertStatusBC(null);

        private static readonly CertStatusBC GOOD = new CertStatusBC(new CertStatus());

        private readonly CertStatus certificateStatus;

        public CertStatusBC(CertStatus certificateStatus) {
            this.certificateStatus = certificateStatus;
        }

        public static CertStatusBC GetInstance() {
            return INSTANCE;
        }

        public virtual CertStatus GetCertStatus() {
            return certificateStatus;
        }

        public virtual ICertStatus GetGood() {
            return GOOD;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            CertStatusBC that = (CertStatusBC)
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
