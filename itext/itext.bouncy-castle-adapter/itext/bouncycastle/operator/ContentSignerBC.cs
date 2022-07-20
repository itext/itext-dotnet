using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator {
    public class ContentSignerBC : IContentSigner {
        private readonly ContentSigner contentSigner;

        public ContentSignerBC(ContentSigner contentSigner) {
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
            iText.Bouncycastle.Operator.ContentSignerBC that = (iText.Bouncycastle.Operator.ContentSignerBC)o;
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
