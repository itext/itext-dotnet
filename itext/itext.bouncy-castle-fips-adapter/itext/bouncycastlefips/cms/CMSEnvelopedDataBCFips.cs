using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms {
    public class CMSEnvelopedDataBCFips : ICMSEnvelopedData {
        private readonly CMSEnvelopedData cmsEnvelopedData;

        public CMSEnvelopedDataBCFips(CMSEnvelopedData cmsEnvelopedData) {
            this.cmsEnvelopedData = cmsEnvelopedData;
        }

        public virtual CMSEnvelopedData GetCmsEnvelopedData() {
            return cmsEnvelopedData;
        }

        public virtual IRecipientInformationStore GetRecipientInfos() {
            return new RecipientInformationStoreBCFips(cmsEnvelopedData.GetRecipientInfos());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cms.CMSEnvelopedDataBCFips that = (iText.Bouncycastlefips.Cms.CMSEnvelopedDataBCFips
                )o;
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
