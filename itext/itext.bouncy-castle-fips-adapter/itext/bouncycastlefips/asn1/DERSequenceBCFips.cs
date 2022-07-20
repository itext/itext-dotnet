using  Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
    /// </summary>
    public class DERSequenceBCFips : ASN1SequenceBCFips, IDERSequence {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
        /// </summary>
        /// <param name="derSequence">
        /// 
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>
        /// to be wrapped
        /// </param>
        public DERSequenceBCFips(DerSequence derSequence)
            : base(derSequence) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
        /// </summary>
        /// <param name="vector">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>
        /// to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>
        /// </param>
        public DERSequenceBCFips(Asn1EncodableVector vector)
            : base(new DerSequence(vector)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
        /// </summary>
        /// <param name="encodable">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>
        /// to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>
        /// </param>
        public DERSequenceBCFips(Asn1Encodable encodable)
            : base(new DerSequence(encodable)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
        /// </returns>
        public virtual DerSequence GetDERSequence() {
            return (DerSequence)GetEncodable();
        }
    }
}
