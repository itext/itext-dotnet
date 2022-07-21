using System;
using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Tsp;

namespace iText.Bouncycastle.Asn1.Tsp {
    public class TSTInfoBC : ASN1EncodableBC, ITSTInfo {
        public TSTInfoBC(TstInfo tstInfo)
            : base(tstInfo) {
        }

        public virtual TstInfo GetTstInfo() {
            return (TstInfo)GetEncodable();
        }

        public virtual IMessageImprint GetMessageImprint() {
            return new MessageImprintBC(GetTstInfo().MessageImprint);
        }
        
        public DateTime GetGenTime() {
            return GetTstInfo().GenTime.ToDateTime();
        }
    }
}
