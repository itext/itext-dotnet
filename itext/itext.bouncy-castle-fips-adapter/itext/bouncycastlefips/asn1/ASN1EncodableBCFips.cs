using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1EncodableBCFips : IASN1Encodable {
        private readonly Asn1Encodable encodable;

        public ASN1EncodableBCFips(Asn1Encodable encodable) {
            this.encodable = encodable;
        }

        public virtual Asn1Encodable GetEncodable() {
            return encodable;
        }

        public virtual IASN1Primitive ToASN1Primitive() {
            return new ASN1PrimitiveBCFips(encodable.ToAsn1Object());
        }

        public virtual bool IsNull() {
            return encodable == null;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Asn1.ASN1EncodableBCFips that = (iText.Bouncycastlefips.Asn1.ASN1EncodableBCFips)o;
            return Object.Equals(encodable, that.encodable);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(encodable);
        }

        public override String ToString() {
            return encodable.ToString();
        }
    }
}
