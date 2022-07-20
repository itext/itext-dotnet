using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerSet"/>.
    /// </summary>
    public class DERSetBC : ASN1SetBC, IDERSet {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerSet"/>.
        /// </summary>
        /// <param name="derSet">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerSet"/>
        /// to be wrapped
        /// </param>
        public DERSetBC(DerSet derSet)
            : base(derSet) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerSet"/>.
        /// </summary>
        /// <param name="vector">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>
        /// to create
        /// <see cref="Org.BouncyCastle.Asn1.DerSet"/>
        /// </param>
        public DERSetBC(Asn1EncodableVector vector)
            : base(new DerSet(vector)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerSet"/>.
        /// </summary>
        /// <param name="encodable">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>
        /// to create
        /// <see cref="Org.BouncyCastle.Asn1.DerSet"/>
        /// </param>
        public DERSetBC(Asn1Encodable encodable)
            : base(new DerSet(encodable)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerSet"/>.
        /// </returns>
        public virtual DerSet GetDERSet() {
            return (DerSet)GetEncodable();
        }
    }
}
