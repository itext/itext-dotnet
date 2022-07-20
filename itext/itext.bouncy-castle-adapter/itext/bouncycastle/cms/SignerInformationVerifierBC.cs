using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.SignerInformationVerifier"/>.
    /// </summary>
    public class SignerInformationVerifierBC : ISignerInformationVerifier {
        private readonly SignerInformationVerifier verifier;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.SignerInformationVerifier"/>.
        /// </summary>
        /// <param name="verifier">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.SignerInformationVerifier"/>
        /// to be wrapped
        /// </param>
        public SignerInformationVerifierBC(SignerInformationVerifier verifier) {
            this.verifier = verifier;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.SignerInformationVerifier"/>.
        /// </returns>
        public virtual SignerInformationVerifier GetVerifier() {
            return verifier;
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
            iText.Bouncycastle.Cms.SignerInformationVerifierBC that = (iText.Bouncycastle.Cms.SignerInformationVerifierBC
                )o;
            return Object.Equals(verifier, that.verifier);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(verifier);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return verifier.ToString();
        }
    }
}
