using iText.Bouncycastlefips.Math;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
    /// </summary>
    public class ASN1IntegerBCFips : ASN1PrimitiveBCFips, IASN1Integer {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
        /// </summary>
        /// <param name="i">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>
        /// to be wrapped
        /// </param>
        public ASN1IntegerBCFips(DerInteger i)
            : base(i) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
        /// </summary>
        /// <param name="i">
        /// int value to create
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>
        /// to be wrapped
        /// </param>
        public ASN1IntegerBCFips(int i)
            : base(new DerInteger(i)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
        /// </summary>
        /// <param name="i">
        /// BigInteger value to create
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>
        /// to be wrapped
        /// </param>
        public ASN1IntegerBCFips(IBigInteger i)
            : base(new DerInteger(((BigIntegerBCFips) i).GetBigInteger())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
        /// </returns>
        public virtual DerInteger GetASN1Integer() {
            return (DerInteger)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBigInteger GetValue() {
            return new BigIntegerBCFips(GetASN1Integer().Value);
        }
    }
}
