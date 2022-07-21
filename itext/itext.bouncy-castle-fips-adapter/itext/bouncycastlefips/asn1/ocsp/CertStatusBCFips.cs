using System;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class CertStatusBCFips : ICertStatus {
        private static readonly CertStatusBCFips INSTANCE = new CertStatusBCFips(null);

        private static readonly CertStatusBCFips GOOD = new CertStatusBCFips(new CertStatus());

        private readonly CertStatus certStatus;

        public CertStatusBCFips(CertStatus certStatus) {
            this.certStatus = certStatus;
        }

        public static CertStatusBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual CertStatus GetCertStatus() {
            return certStatus;
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
            CertStatusBCFips that = (CertStatusBCFips
                )o;
            return Object.Equals(certStatus, that.certStatus);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certStatus);
        }

        public override String ToString() {
            return certStatus.ToString();
        }
    }
}
