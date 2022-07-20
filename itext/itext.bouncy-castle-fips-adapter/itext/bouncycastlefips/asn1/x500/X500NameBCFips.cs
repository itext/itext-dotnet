using iText.Commons.Bouncycastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X500;

namespace iText.Bouncycastlefips.Asn1.X500 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X500.X500Name"/>.
    /// </summary>
    public class X500NameBCFips : ASN1EncodableBCFips, IX500Name {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X500.X500Name"/>.
        /// </summary>
        /// <param name="x500Name">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X500.X500Name"/>
        /// to be wrapped
        /// </param>
        public X500NameBCFips(X500Name x500Name)
            : base(x500Name) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X500.X500Name"/>.
        /// </returns>
        public virtual X500Name GetX500Name() {
            return (X500Name)GetEncodable();
        }
    }
}
