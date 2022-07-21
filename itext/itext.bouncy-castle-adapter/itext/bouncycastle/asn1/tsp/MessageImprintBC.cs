using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Tsp {
    public class MessageImprintBC : ASN1EncodableBC, IMessageImprint {
        public MessageImprintBC(MessageImprint messageImprint)
            : base(messageImprint) {
        }

        public virtual MessageImprint GetMessageImprint() {
            return (MessageImprint)GetEncodable();
        }

        public virtual byte[] GetHashedMessage() {
            return GetMessageImprint().GetHashedMessage();
        }
        
        public IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBC(GetMessageImprint().HashAlgorithm);
        }
    }
}
