using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.KeyTransRecipientInfo"/>.
    /// </summary>
    public class KeyTransRecipientInfoBC : ASN1EncodableBC, IKeyTransRecipientInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.KeyTransRecipientInfo"/>.
        /// </summary>
        /// <param name="keyTransRecipientInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.KeyTransRecipientInfo"/>
        /// to be wrapped
        /// </param>
        public KeyTransRecipientInfoBC(KeyTransRecipientInfo keyTransRecipientInfo)
            : base(keyTransRecipientInfo) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.KeyTransRecipientInfo"/>.
        /// </summary>
        /// <param name="recipientIdentifier">RecipientIdentifier wrapper</param>
        /// <param name="algorithmIdentifier">AlgorithmIdentifier wrapper</param>
        /// <param name="octetString">ASN1OctetString wrapper</param>
        public KeyTransRecipientInfoBC(IRecipientIdentifier recipientIdentifier, IAlgorithmIdentifier algorithmIdentifier
            , IASN1OctetString octetString)
            : base(new KeyTransRecipientInfo(((RecipientIdentifierBC)recipientIdentifier).GetRecipientIdentifier(), ((
                AlgorithmIdentifierBC)algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBC)octetString).
                GetASN1OctetString())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.KeyTransRecipientInfo"/>.
        /// </returns>
        public virtual KeyTransRecipientInfo GetKeyTransRecipientInfo() {
            return (KeyTransRecipientInfo)GetEncodable();
        }
    }
}
