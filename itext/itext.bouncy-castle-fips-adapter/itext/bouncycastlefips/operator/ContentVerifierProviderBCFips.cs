using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator {
    public class ContentVerifierProviderBCFips : IContentVerifierProvider {
        private readonly ContentVerifierProvider provider;

        public ContentVerifierProviderBCFips(ContentVerifierProvider provider) {
            this.provider = provider;
        }

        public virtual ContentVerifierProvider GetProvider() {
            return provider;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Operator.ContentVerifierProviderBCFips that = (iText.Bouncycastlefips.Operator.ContentVerifierProviderBCFips
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
