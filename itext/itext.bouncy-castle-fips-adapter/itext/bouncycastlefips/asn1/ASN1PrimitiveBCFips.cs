using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>.
    /// </summary>
    public class ASN1PrimitiveBCFips : ASN1EncodableBCFips, IASN1Primitive {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>.
        /// </summary>
        /// <param name="primitive">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>
        /// to be wrapped
        /// </param>
        public ASN1PrimitiveBCFips(Asn1Object primitive)
            : base(primitive) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>.
        /// </summary>
        /// <param name="array">
        /// byte array to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>
        /// to be wrapped
        /// </param>
        public ASN1PrimitiveBCFips(byte[] array)
            : base(Asn1Object.FromByteArray(array)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>.
        /// </returns>
        public virtual Asn1Object GetPrimitive() {
            return (Asn1Object)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return GetPrimitive().GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded(String encoding) {
            return GetPrimitive().GetEncoded(encoding);
        }
    }
}
