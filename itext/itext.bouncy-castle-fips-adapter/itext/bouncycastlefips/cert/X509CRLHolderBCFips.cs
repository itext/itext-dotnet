using System;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    public class X509CRLHolderBCFips : IX509CRLHolder {
        private readonly X509CRLHolder x509CRLHolder;

        public X509CRLHolderBCFips(X509CRLHolder x509CRLHolder) {
            this.x509CRLHolder = x509CRLHolder;
        }

        public virtual X509CRLHolder GetX509CRLHolder() {
            return x509CRLHolder;
        }

        public virtual byte[] GetEncoded() {
            return x509CRLHolder.GetEncoded();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.X509CRLHolderBCFips that = (iText.Bouncycastlefips.Cert.X509CRLHolderBCFips)o;
            return Object.Equals(x509CRLHolder, that.x509CRLHolder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(x509CRLHolder);
        }

        public override String ToString() {
            return x509CRLHolder.ToString();
        }
    }
}
