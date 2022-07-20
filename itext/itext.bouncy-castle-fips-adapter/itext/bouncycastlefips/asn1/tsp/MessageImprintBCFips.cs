using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Tsp;

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
    }
}
