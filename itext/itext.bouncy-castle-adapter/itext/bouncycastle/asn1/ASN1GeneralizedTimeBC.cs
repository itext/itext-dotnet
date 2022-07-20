using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerGeneralizedTime"/>.
    /// </summary>
    public class ASN1GeneralizedTimeBC : ASN1PrimitiveBC, IASN1GeneralizedTime {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerGeneralizedTime"/>.
        /// </summary>
        /// <param name="asn1GeneralizedTime">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerGeneralizedTime"/>
        /// to be wrapped
        /// </param>
        public ASN1GeneralizedTimeBC(DerGeneralizedTime asn1GeneralizedTime)
            : base(asn1GeneralizedTime) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerGeneralizedTime"/>.
        /// </returns>
        public virtual DerGeneralizedTime GetASN1GeneralizedTime() {
            return (DerGeneralizedTime)GetEncodable();
        }
    }
}
