using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    public class SignerInfoGeneratorBC : ISignerInfoGenerator {
        private readonly SignerInfoGenerator signerInfoGenerator;

        public SignerInfoGeneratorBC(SignerInfoGenerator signerInfoGenerator) {
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
            iText.Bouncycastle.Cms.SignerInfoGeneratorBC that = (iText.Bouncycastle.Cms.SignerInfoGeneratorBC)o;
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
