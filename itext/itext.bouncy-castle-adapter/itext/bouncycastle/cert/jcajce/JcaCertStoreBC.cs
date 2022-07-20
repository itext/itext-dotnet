using System;
using Org.BouncyCastle.Cert.Jcajce;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Jcajce {
    public class JcaCertStoreBC : IJcaCertStore {
        private readonly JcaCertStore jcaCertStore;

        public JcaCertStoreBC(JcaCertStore jcaCertStore) {
            this.jcaCertStore = jcaCertStore;
        }

        public virtual JcaCertStore GetJcaCertStore() {
            return jcaCertStore;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Jcajce.JcaCertStoreBC that = (iText.Bouncycastle.Cert.Jcajce.JcaCertStoreBC)o;
            return Object.Equals(jcaCertStore, that.jcaCertStore);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(jcaCertStore);
        }

        public override String ToString() {
            return jcaCertStore.ToString();
        }
    }
}
