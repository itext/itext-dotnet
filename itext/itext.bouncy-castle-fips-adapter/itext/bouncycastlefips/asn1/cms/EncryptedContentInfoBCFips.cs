using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>.
    /// </summary>
    public class EncryptedContentInfoBCFips : ASN1EncodableBCFips, IEncryptedContentInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>.
        /// </summary>
        /// <param name="encryptedContentInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>
        /// to be wrapped
        /// </param>
        public EncryptedContentInfoBCFips(EncryptedContentInfo encryptedContentInfo)
            : base(encryptedContentInfo) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>.
        /// </summary>
        /// <param name="data">ASN1ObjectIdentifier wrapper</param>
        /// <param name="algorithmIdentifier">AlgorithmIdentifier wrapper</param>
        /// <param name="octetString">ASN1OctetString wrapper</param>
        public EncryptedContentInfoBCFips(IASN1ObjectIdentifier data, IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString
             octetString)
            : base(new EncryptedContentInfo(((ASN1ObjectIdentifierBCFips)data).GetASN1ObjectIdentifier(), ((AlgorithmIdentifierBCFips
                )algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBCFips)octetString).GetOctetString())
                ) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>.
        /// </returns>
        public virtual EncryptedContentInfo GetEncryptedContentInfo() {
            return (EncryptedContentInfo)GetEncodable();
        }
    }
}
