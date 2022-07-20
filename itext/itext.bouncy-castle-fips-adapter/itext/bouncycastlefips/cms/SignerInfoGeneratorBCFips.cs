using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    public class SignerInfoGeneratorBCFips : ISignerInfoGenerator {
        private readonly SignerInfoGenerator signerInfoGenerator;

        public SignerInfoGeneratorBCFips(SignerInfoGenerator signerInfoGenerator) {
            this.signerInfoGenerator = signerInfoGenerator;
        }

        public virtual SignerInfoGenerator GetSignerInfoGenerator() {
            return signerInfoGenerator;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cms.SignerInfoGeneratorBCFips that = (iText.Bouncycastlefips.Cms.SignerInfoGeneratorBCFips
                )o;
            return Object.Equals(signerInfoGenerator, that.signerInfoGenerator);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(signerInfoGenerator);
        }

        public override String ToString() {
            return signerInfoGenerator.ToString();
        }
    }
}
