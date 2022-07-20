using System;
using Org.BouncyCastle.Cert.Jcajce;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaCertStore"/>.
    /// </summary>
    public class JcaCertStoreBCFips : IJcaCertStore {
        private readonly JcaCertStore jcaCertStore;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaCertStore"/>.
        /// </summary>
        /// <param name="jcaCertStore">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaCertStore"/>
        /// to be wrapped
        /// </param>
        public JcaCertStoreBCFips(JcaCertStore jcaCertStore) {
            this.jcaCertStore = jcaCertStore;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaCertStore"/>.
        /// </returns>
        public virtual JcaCertStore GetJcaCertStore() {
            return jcaCertStore;
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(jcaCertStore);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return jcaCertStore.ToString();
        }
    }
}
