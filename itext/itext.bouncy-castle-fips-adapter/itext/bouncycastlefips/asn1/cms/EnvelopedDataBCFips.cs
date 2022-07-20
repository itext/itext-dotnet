using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>.
    /// </summary>
    public class EnvelopedDataBCFips : ASN1EncodableBCFips, IEnvelopedData {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>.
        /// </summary>
        /// <param name="envelopedData">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>
        /// to be wrapped
        /// </param>
        public EnvelopedDataBCFips(EnvelopedData envelopedData)
            : base(envelopedData) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>.
        /// </summary>
        /// <param name="originatorInfo">
        /// OriginatorInfo wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>
        /// </param>
        /// <param name="set">
        /// ASN1Set wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>
        /// </param>
        /// <param name="encryptedContentInfo">
        /// EncryptedContentInfo wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>
        /// </param>
        /// <param name="set1">
        /// ASN1Set wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>
        /// </param>
        public EnvelopedDataBCFips(IOriginatorInfo originatorInfo, IASN1Set set, IEncryptedContentInfo encryptedContentInfo
            , IASN1Set set1)
            : base(new EnvelopedData(((OriginatorInfoBCFips)originatorInfo).GetOriginatorInfo(), ((ASN1SetBCFips)set).
                GetASN1Set(), ((EncryptedContentInfoBCFips)encryptedContentInfo).GetEncryptedContentInfo(), ((ASN1SetBCFips
                )set1).GetASN1Set())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EnvelopedData"/>.
        /// </returns>
        public virtual EnvelopedData GetEnvelopedData() {
            return (EnvelopedData)GetEncodable();
        }
    }
}
