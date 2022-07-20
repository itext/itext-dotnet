using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.SignerInfoGenerator"/>.
    /// </summary>
    public class SignerInfoGeneratorBC : ISignerInfoGenerator {
        private readonly SignerInfoGenerator signerInfoGenerator;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.SignerInfoGenerator"/>.
        /// </summary>
        /// <param name="signerInfoGenerator">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.SignerInfoGenerator"/>
        /// to be wrapped
        /// </param>
        public SignerInfoGeneratorBC(SignerInfoGenerator signerInfoGenerator) {
            this.signerInfoGenerator = signerInfoGenerator;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.SignerInfoGenerator"/>.
        /// </returns>
        public virtual SignerInfoGenerator GetSignerInfoGenerator() {
            return signerInfoGenerator;
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
            iText.Bouncycastle.Cms.SignerInfoGeneratorBC that = (iText.Bouncycastle.Cms.SignerInfoGeneratorBC)o;
            return Object.Equals(signerInfoGenerator, that.signerInfoGenerator);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(signerInfoGenerator);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return signerInfoGenerator.ToString();
        }
    }
}
