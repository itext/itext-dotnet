using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1PrimitiveBCFips : ASN1EncodableBCFips, IASN1Primitive {
        public ASN1PrimitiveBCFips(Asn1Object primitive)
            : base(primitive) {
        }

        public ASN1PrimitiveBCFips(byte[] array)
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
