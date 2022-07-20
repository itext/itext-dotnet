using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>.
    /// </summary>
    public class ASN1EncodingBC : IASN1Encoding {
        private static readonly iText.Bouncycastle.Asn1.ASN1EncodingBC INSTANCE = new iText.Bouncycastle.Asn1.ASN1EncodingBC
            (null);

        private readonly Asn1Encodable asn1Encoding;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>.
        /// </summary>
        /// <param name="asn1Encoding">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>
        /// to be wrapped
        /// </param>
        public ASN1EncodingBC(Asn1Encodable asn1Encoding) {
            this.asn1Encoding = asn1Encoding;
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="ASN1EncodingBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.ASN1EncodingBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>.
        /// </returns>
        public virtual Asn1Encodable GetASN1Encoding() {
            return asn1Encoding;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetDer() {
            return Org.BouncyCastle.Asn1.Asn1Encodable.Der;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetBer() {
            return Org.BouncyCastle.Asn1.Asn1Encodable.Ber;
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Asn1.ASN1EncodingBC that = (iText.Bouncycastle.Asn1.ASN1EncodingBC)o;
            return Object.Equals(asn1Encoding, that.asn1Encoding);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(asn1Encoding);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return asn1Encoding.ToString();
        }
    }
}
