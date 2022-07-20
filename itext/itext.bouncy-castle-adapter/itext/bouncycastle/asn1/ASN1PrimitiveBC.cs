using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1PrimitiveBC : ASN1EncodableBC, IASN1Primitive {
        public ASN1PrimitiveBC(Asn1Object primitive)
            : base(primitive) {
        }

        public ASN1PrimitiveBC(byte[] array)
            : base(Asn1Object.FromByteArray(array)) {
        }

        public virtual Asn1Object GetPrimitive() {
            return (Asn1Object)GetEncodable();
        }

        public virtual byte[] GetEncoded() {
            return GetPrimitive().GetEncoded();
        }

        public virtual byte[] GetEncoded(String encoding) {
            return GetPrimitive().GetEncoded(encoding);
        }
    }
}
