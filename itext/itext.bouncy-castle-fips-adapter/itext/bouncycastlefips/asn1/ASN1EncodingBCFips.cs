using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1EncodingBCFips : IASN1Encoding {
        private static readonly iText.Bouncycastlefips.Asn1.ASN1EncodingBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.ASN1EncodingBCFips
            (null);

        private readonly Asn1Encodable asn1Encoding;

        public ASN1EncodingBCFips(Asn1Encodable asn1Encoding) {
            this.asn1Encoding = asn1Encoding;
        }

        public static iText.Bouncycastlefips.Asn1.ASN1EncodingBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual Asn1Encodable GetAsn1Encoding() {
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
            iText.Bouncycastlefips.Asn1.ASN1EncodingBCFips that = (iText.Bouncycastlefips.Asn1.ASN1EncodingBCFips)o;
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
