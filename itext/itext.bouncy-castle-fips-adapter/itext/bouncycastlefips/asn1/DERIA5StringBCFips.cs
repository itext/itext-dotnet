using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>.
    /// </summary>
    public class DERIA5StringBCFips : ASN1PrimitiveBCFips, IDERIA5String {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>.
        /// </summary>
        /// <param name="deria5String">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>
        /// to be wrapped
        /// </param>
        public DERIA5StringBCFips(DerIA5String deria5String)
            : base(deria5String) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>.
        /// </summary>
        /// <param name="str">
        /// string to create
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>
        /// to be wrapped
        /// </param>
        public DERIA5StringBCFips(String str)
            : this(new DerIA5String(str)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>.
        /// </returns>
        public virtual DerIA5String GetDerIA5String() {
            return (DerIA5String)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetString() {
            return GetDerIA5String().GetString();
        }
    }
}
