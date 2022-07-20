using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerOctetString"/>.
    /// </summary>
    public class DEROctetStringBC : ASN1OctetStringBC, IDEROctetString {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerOctetString"/>.
        /// </summary>
        /// <param name="bytes">
        /// byte array to create
        /// <see cref="Org.BouncyCastle.Asn1.DerOctetString"/>
        /// to be wrapped
        /// </param>
        public DEROctetStringBC(byte[] bytes)
            : base(new DerOctetString(bytes)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerOctetString"/>.
        /// </summary>
        /// <param name="octetString">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerOctetString"/>
        /// to be wrapped
        /// </param>
        public DEROctetStringBC(DerOctetString octetString)
            : base(octetString) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerOctetString"/>.
        /// </returns>
        public virtual DerOctetString GetDEROctetString() {
            return (DerOctetString)GetPrimitive();
        }
    }
}
