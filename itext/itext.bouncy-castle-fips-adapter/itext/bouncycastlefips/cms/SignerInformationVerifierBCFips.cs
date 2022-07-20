using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    public class SignerInformationVerifierBCFips : ISignerInformationVerifier {
        private readonly SignerInformationVerifier verifier;

        public SignerInformationVerifierBCFips(SignerInformationVerifier verifier) {
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
            iText.Bouncycastlefips.Cms.SignerInformationVerifierBCFips that = (iText.Bouncycastlefips.Cms.SignerInformationVerifierBCFips
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
