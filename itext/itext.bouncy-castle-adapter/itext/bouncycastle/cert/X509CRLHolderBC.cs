using System;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert {
    public class X509CRLHolderBC : IX509CRLHolder {
        private readonly X509CRLHolder x509CRLHolder;

        public X509CRLHolderBC(X509CRLHolder x509CRLHolder) {
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
            iText.Bouncycastle.Cert.X509CRLHolderBC that = (iText.Bouncycastle.Cert.X509CRLHolderBC)o;
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
