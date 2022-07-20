using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    public class CMSEnvelopedDataBC : ICMSEnvelopedData {
        private readonly CMSEnvelopedData cmsEnvelopedData;

        public CMSEnvelopedDataBC(CMSEnvelopedData cmsEnvelopedData) {
            this.cmsEnvelopedData = cmsEnvelopedData;
        }

        public virtual CMSEnvelopedData GetCmsEnvelopedData() {
            return cmsEnvelopedData;
        }

        public virtual IRecipientInformationStore GetRecipientInfos() {
            return new RecipientInformationStoreBC(cmsEnvelopedData.GetRecipientInfos());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cms.CMSEnvelopedDataBC that = (iText.Bouncycastle.Cms.CMSEnvelopedDataBC)o;
            return Object.Equals(cmsEnvelopedData, that.cmsEnvelopedData);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(cmsEnvelopedData);
        }

        public override String ToString() {
            return cmsEnvelopedData.ToString();
        }
    }
}
