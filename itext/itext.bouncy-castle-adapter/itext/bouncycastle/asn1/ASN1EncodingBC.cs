using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1EncodingBC : IASN1Encoding {
        private static readonly iText.Bouncycastle.Asn1.ASN1EncodingBC INSTANCE = new iText.Bouncycastle.Asn1.ASN1EncodingBC
            (null);

        private readonly Asn1Encodable asn1Encoding;

        public ASN1EncodingBC(Asn1Encodable asn1Encoding) {
            this.asn1Encoding = asn1Encoding;
        }

        public static iText.Bouncycastle.Asn1.ASN1EncodingBC GetInstance() {
            return INSTANCE;
        }

        public virtual Asn1Encodable GetASN1Encoding() {
            return asn1Encoding;
        }

        public virtual String GetDer() {
            return Org.BouncyCastle.Asn1.Asn1Encodable.Der;
        }

        public virtual String GetDl() {
            return Asn1Encodable.DL;
        }

        public virtual String GetBer() {
            return Org.BouncyCastle.Asn1.Asn1Encodable.Ber;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(asn1Encoding);
        }

        public override String ToString() {
            return asn1Encoding.ToString();
        }
    }
}
