using System;
using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Tsp;

namespace iText.Bouncycastle.Asn1.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>.
    /// </summary>
    public class TSTInfoBC : ASN1EncodableBC, ITSTInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>.
        /// </summary>
        /// <param name="tstInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>
        /// to be wrapped
        /// </param>
        public TSTInfoBC(TstInfo tstInfo)
            : base(tstInfo) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>.
        /// </returns>
        public virtual TstInfo GetTstInfo() {
            return (TstInfo)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IMessageImprint GetMessageImprint() {
            return new MessageImprintBC(GetTstInfo().MessageImprint);
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetGenTime() {
            return GetTstInfo().GenTime.ToDateTime();
        }
    }
}
