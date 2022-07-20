using System;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.X509CRLHolder"/>.
    /// </summary>
    public class X509CRLHolderBC : IX509CRLHolder {
        private readonly X509CRLHolder x509CRLHolder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.X509CRLHolder"/>.
        /// </summary>
        /// <param name="x509CRLHolder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.X509CRLHolder"/>
        /// to be wrapped
        /// </param>
        public X509CRLHolderBC(X509CRLHolder x509CRLHolder) {
            this.x509CRLHolder = x509CRLHolder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.X509CRLHolder"/>.
        /// </returns>
        public virtual X509CRLHolder GetX509CRLHolder() {
            return x509CRLHolder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return x509CRLHolder.GetEncoded();
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
            iText.Bouncycastle.Cert.X509CRLHolderBC that = (iText.Bouncycastle.Cert.X509CRLHolderBC)o;
            return Object.Equals(x509CRLHolder, that.x509CRLHolder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(x509CRLHolder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return x509CRLHolder.ToString();
        }
    }
}
