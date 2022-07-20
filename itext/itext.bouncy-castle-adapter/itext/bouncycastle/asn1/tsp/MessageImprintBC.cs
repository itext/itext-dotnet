using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Tsp;

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
    }
}
