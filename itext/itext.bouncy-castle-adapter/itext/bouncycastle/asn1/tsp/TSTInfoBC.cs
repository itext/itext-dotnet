using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Tsp;

namespace iText.Bouncycastle.Asn1.Tsp {
    public class TSTInfoBC : ASN1EncodableBC, ITSTInfo {
        public TSTInfoBC(TSTInfo tstInfo)
            : base(tstInfo) {
        }

        public virtual TSTInfo GetTstInfo() {
            return (TSTInfo)GetEncodable();
        }

        public virtual IMessageImprint GetMessageImprint() {
            return new MessageImprintBC(GetTstInfo().MessageImprint);
        }
    }
}
