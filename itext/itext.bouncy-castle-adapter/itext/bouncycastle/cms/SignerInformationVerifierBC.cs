using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    public class SignerInformationVerifierBC : ISignerInformationVerifier {
        private readonly SignerInformationVerifier verifier;

        public SignerInformationVerifierBC(SignerInformationVerifier verifier) {
            this.verifier = verifier;
        }

        public virtual SignerInformationVerifier GetVerifier() {
            return verifier;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(verifier);
        }

        public override String ToString() {
            return verifier.ToString();
        }
    }
}
