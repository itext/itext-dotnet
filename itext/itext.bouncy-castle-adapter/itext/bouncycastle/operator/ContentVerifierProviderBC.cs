using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator {
    public class ContentVerifierProviderBC : IContentVerifierProvider {
        private readonly ContentVerifierProvider provider;

        public ContentVerifierProviderBC(ContentVerifierProvider provider) {
            this.provider = provider;
        }

        public virtual ContentVerifierProvider GetContentVerifierProvider() {
            return provider;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Operator.ContentVerifierProviderBC that = (iText.Bouncycastle.Operator.ContentVerifierProviderBC
                )o;
            return Object.Equals(provider, that.provider);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(provider);
        }

        public override String ToString() {
            return provider.ToString();
        }
    }
}
