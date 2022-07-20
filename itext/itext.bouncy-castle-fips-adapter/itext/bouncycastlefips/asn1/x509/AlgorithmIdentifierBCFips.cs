using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>.
    /// </summary>
    public class AlgorithmIdentifierBCFips : ASN1EncodableBCFips, IAlgorithmIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>.
        /// </summary>
        /// <param name="algorithmIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>
        /// to be wrapped
        /// </param>
        public AlgorithmIdentifierBCFips(AlgorithmIdentifier algorithmIdentifier)
            : base(algorithmIdentifier) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>.
        /// </returns>
        public virtual AlgorithmIdentifier GetAlgorithmIdentifier() {
            return (AlgorithmIdentifier)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetAlgorithm() {
            return new ASN1ObjectIdentifierBCFips(GetAlgorithmIdentifier().Algorithm);
        }
    }
}
