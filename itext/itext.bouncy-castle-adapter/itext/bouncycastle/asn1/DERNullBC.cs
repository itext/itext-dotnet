using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerNull"/>.
    /// </summary>
    public class DERNullBC : ASN1PrimitiveBC, IDERNull {
        /// <summary>
        /// Wrapper for
        /// <see cref="Org.BouncyCastle.Asn1.DerNull"/>
        /// INSTANCE.
        /// </summary>
        public static readonly iText.Bouncycastle.Asn1.DERNullBC INSTANCE = new iText.Bouncycastle.Asn1.DERNullBC(
            );

        private DERNullBC()
            : base(Org.BouncyCastle.Asn1.DerNull.Instance) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerNull"/>.
        /// </summary>
        /// <param name="derNull">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerNull"/>
        /// to be wrapped
        /// </param>
        public DERNullBC(DerNull derNull)
            : base(derNull) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerNull"/>.
        /// </returns>
        public virtual DerNull GetDERNull() {
            return (DerNull)GetPrimitive();
        }
    }
}
