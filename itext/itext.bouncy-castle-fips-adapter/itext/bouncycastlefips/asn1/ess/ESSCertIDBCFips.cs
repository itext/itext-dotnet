using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastlefips.Asn1.Ess {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ess.EssCertID"/>.
    /// </summary>
    public class ESSCertIDBCFips : ASN1EncodableBCFips, IESSCertID {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ess.EssCertID"/>.
        /// </summary>
        /// <param name="essCertID">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ess.EssCertID"/>
        /// to be wrapped
        /// </param>
        public ESSCertIDBCFips(EssCertID essCertID)
            : base(essCertID) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ess.EssCertID"/>.
        /// </returns>
        public virtual EssCertID GetEssCertID() {
            return (EssCertID)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetCertHash() {
            return GetEssCertID().GetCertHash();
        }
    }
}
