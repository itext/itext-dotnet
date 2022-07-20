using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Tsp;

namespace iText.Bouncycastlefips.Asn1.Tsp {
    public class TSTInfoBCFips : ASN1EncodableBCFips, ITSTInfo {
        public TSTInfoBCFips(TSTInfo tstInfo)
            : base(tstInfo) {
        }

        public virtual TSTInfo GetTstInfo() {
            return (TSTInfo)GetEncodable();
        }

        public virtual IMessageImprint GetMessageImprint() {
            return new MessageImprintBCFips(GetTstInfo().MessageImprint);
        }
    }
}
