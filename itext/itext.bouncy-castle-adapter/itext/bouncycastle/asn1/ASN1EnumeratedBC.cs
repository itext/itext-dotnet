using  Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref=" Org.BouncyCastle.Asn1.DerEnumerated"/>.
    /// </summary>
    public class ASN1EnumeratedBC : ASN1PrimitiveBC, IASN1Enumerated {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerEnumerated"/>.
        /// </summary>
        /// <param name="asn1Enumerated">
        /// 
        /// <see cref=" Org.BouncyCastle.Asn1.DerEnumerated"/>
        /// to be wrapped
        /// </param>
        public ASN1EnumeratedBC(DerEnumerated asn1Enumerated)
            : base(asn1Enumerated) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerEnumerated"/>.
        /// </summary>
        /// <param name="i">
        /// int value to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerEnumerated"/>
        /// to be wrapped
        /// </param>
        public ASN1EnumeratedBC(int i)
            : base(new DerEnumerated(i)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref=" Org.BouncyCastle.Asn1.DerEnumerated"/>.
        /// </returns>
        public virtual DerEnumerated GetASN1Enumerated() {
            return (DerEnumerated)GetEncodable();
        }
    }
}
