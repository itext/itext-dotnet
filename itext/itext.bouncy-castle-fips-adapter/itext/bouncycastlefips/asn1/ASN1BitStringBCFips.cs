using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerBitString"/>.
    /// </summary>
    public class ASN1BitStringBCFips : ASN1PrimitiveBCFips, IASN1BitString {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerBitString"/>.
        /// </summary>
        /// <param name="asn1BitString">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerBitString"/>
        /// to be wrapped
        /// </param>
        public ASN1BitStringBCFips(DerBitString asn1BitString)
            : base(asn1BitString) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerBitString"/>.
        /// </returns>
        public virtual DerBitString GetASN1BitString() {
            return (DerBitString)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetString() {
            return GetASN1BitString().GetString();
        }
    }
}
