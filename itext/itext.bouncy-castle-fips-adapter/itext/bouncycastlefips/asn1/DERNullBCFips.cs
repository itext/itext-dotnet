using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerNull"/>.
    /// </summary>
    public class DERNullBCFips : ASN1PrimitiveBCFips, IDERNull {
        /// <summary>
        /// Wrapper for
        /// <see cref="Org.BouncyCastle.Asn1.DerNull"/>
        /// INSTANCE.
        /// </summary>
        public static readonly iText.Bouncycastlefips.Asn1.DERNullBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.DERNullBCFips
            ();

        private DERNullBCFips()
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
        public DERNullBCFips(DerNull derNull)
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
