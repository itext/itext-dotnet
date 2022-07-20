using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.BasicConstraints"/>.
    /// </summary>
    public class BasicConstraintsBCFips : ASN1EncodableBCFips, IBasicConstraints {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.BasicConstraints"/>.
        /// </summary>
        /// <param name="basicConstraints">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.BasicConstraints"/>
        /// to be wrapped
        /// </param>
        public BasicConstraintsBCFips(BasicConstraints basicConstraints)
            : base(basicConstraints) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.BasicConstraints"/>.
        /// </returns>
        public virtual BasicConstraints GetBasicConstraints() {
            return (BasicConstraints)GetEncodable();
        }
    }
}
