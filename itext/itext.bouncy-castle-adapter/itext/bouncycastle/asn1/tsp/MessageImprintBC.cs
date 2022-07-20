using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Tsp.MessageImprint"/>.
    /// </summary>
    public class MessageImprintBC : ASN1EncodableBC, IMessageImprint {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.MessageImprint"/>.
        /// </summary>
        /// <param name="messageImprint">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.MessageImprint"/>
        /// to be wrapped
        /// </param>
        public MessageImprintBC(MessageImprint messageImprint)
            : base(messageImprint) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.MessageImprint"/>.
        /// </returns>
        public virtual MessageImprint GetMessageImprint() {
            return (MessageImprint)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetHashedMessage() {
            return GetMessageImprint().GetHashedMessage();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBC(GetMessageImprint().HashAlgorithm);
        }
    }
}
