using System;
using Org.BouncyCastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.Tsp;

namespace iText.Bouncycastlefips.Asn1.Tsp {
    public class TSTInfoBCFips : ASN1EncodableBCFips, ITSTInfo {
        public TSTInfoBCFips(TstInfo tstInfo)
            : base(tstInfo) {
        }

        public virtual TstInfo GetTstInfo() {
            return (TstInfo)GetEncodable();
        }

        public virtual IMessageImprint GetMessageImprint() {
            return new MessageImprintBCFips(GetTstInfo().MessageImprint);
        }

        public DateTime GetGenTime() {
            return GetTstInfo().GenTime.ToDateTime();
        }
    }
}
