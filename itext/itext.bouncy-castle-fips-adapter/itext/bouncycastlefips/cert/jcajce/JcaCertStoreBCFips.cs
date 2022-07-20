using System;
using Org.BouncyCastle.Cert.Jcajce;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Jcajce {
    public class JcaCertStoreBCFips : IJcaCertStore {
        private readonly JcaCertStore jcaCertStore;

        public JcaCertStoreBCFips(JcaCertStore jcaCertStore) {
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
            iText.Bouncycastlefips.Cert.Jcajce.JcaCertStoreBCFips that = (iText.Bouncycastlefips.Cert.Jcajce.JcaCertStoreBCFips
                )o;
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
