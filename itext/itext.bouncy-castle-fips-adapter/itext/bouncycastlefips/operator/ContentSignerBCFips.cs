using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator {
    public class ContentSignerBCFips : IContentSigner {
        private readonly ContentSigner contentSigner;

        public ContentSignerBCFips(ContentSigner contentSigner) {
            this.contentSigner = contentSigner;
        }

        public virtual ContentSigner GetContentSigner() {
            return contentSigner;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Operator.ContentSignerBCFips that = (iText.Bouncycastlefips.Operator.ContentSignerBCFips
                )o;
            return Object.Equals(contentSigner, that.contentSigner);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(contentSigner);
        }

        public override String ToString() {
            return contentSigner.ToString();
        }
    }
}
