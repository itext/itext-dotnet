using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Tsp {
    public class MessageImprintBCFips : ASN1EncodableBCFips, IMessageImprint {
        public MessageImprintBCFips(MessageImprint messageImprint)
            : base(messageImprint) {
        }

        public virtual MessageImprint GetMessageImprint() {
            return (MessageImprint)GetEncodable();
        }

        public virtual byte[] GetHashedMessage() {
            return GetMessageImprint().GetHashedMessage();
        }

        public IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBCFips(GetMessageImprint().HashAlgorithm);
        }
    }
}
