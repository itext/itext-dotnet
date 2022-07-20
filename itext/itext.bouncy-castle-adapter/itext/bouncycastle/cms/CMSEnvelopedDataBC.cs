using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.CMSEnvelopedData"/>.
    /// </summary>
    public class CMSEnvelopedDataBC : ICMSEnvelopedData {
        private readonly CMSEnvelopedData cmsEnvelopedData;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.CMSEnvelopedData"/>.
        /// </summary>
        /// <param name="cmsEnvelopedData">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.CMSEnvelopedData"/>
        /// to be wrapped
        /// </param>
        public CMSEnvelopedDataBC(CMSEnvelopedData cmsEnvelopedData) {
            this.cmsEnvelopedData = cmsEnvelopedData;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.CMSEnvelopedData"/>.
        /// </returns>
        public virtual CMSEnvelopedData GetCmsEnvelopedData() {
            return cmsEnvelopedData;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientInformationStore GetRecipientInfos() {
            return new RecipientInformationStoreBC(cmsEnvelopedData.GetRecipientInfos());
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(cmsEnvelopedData);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return cmsEnvelopedData.ToString();
        }
    }
}
