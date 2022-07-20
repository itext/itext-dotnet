using System;
using Org.BouncyCastle;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class DERIA5StringBC : ASN1PrimitiveBC, IDERIA5String {
        public DERIA5StringBC(DerIA5String deria5String)
            : base(deria5String) {
        }

        public DERIA5StringBC(String str)
            : this(new DerIA5String(str)) {
        }

        public virtual DerIA5String GetDerIA5String() {
            return (DerIA5String)GetEncodable();
        }

        public virtual String GetString() {
            return GetDerIA5String().GetString();
        }
    }
}
