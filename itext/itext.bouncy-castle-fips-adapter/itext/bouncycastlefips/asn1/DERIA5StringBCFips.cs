using System;
using Org.BouncyCastle;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class DERIA5StringBCFips : ASN1PrimitiveBCFips, IDERIA5String {
        public DERIA5StringBCFips(DerIA5String deria5String)
            : base(deria5String) {
        }

        public DERIA5StringBCFips(String str)
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
