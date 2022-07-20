using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1BitStringBC : ASN1PrimitiveBC, IASN1BitString {
        public ASN1BitStringBC(DerBitString asn1BitString)
            : base(asn1BitString) {
        }

        public virtual DerBitString GetASN1BitString() {
            return (DerBitString)GetEncodable();
        }

        public virtual String GetString() {
            return GetASN1BitString().GetString();
        }
    }
}
