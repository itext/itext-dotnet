using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using Org.BouncyCastle.Cert;

namespace iText.Bouncycastlefips.Cert {
    public class X509CrlBCFips : IX509Crl {
        private readonly X509Crl x509Crl;

        public X509CrlBCFips(X509Crl x509Crl) {
            this.x509Crl = x509Crl;
        }

        public virtual X509Crl GetX509Crl() {
            return x509Crl;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            X509CrlBCFips that = (X509CrlBCFips)o;
            return Object.Equals(x509Crl, that.x509Crl);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(x509Crl);
        }

        public override String ToString() {
            return x509Crl.ToString();
        }
    }
}