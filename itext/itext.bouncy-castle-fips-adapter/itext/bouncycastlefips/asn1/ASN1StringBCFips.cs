using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1StringBCFips : IASN1String {
        private readonly DerStringBase asn1String;

        public ASN1StringBCFips(DerStringBase asn1String) {
            this.asn1String = asn1String;
        }

        public virtual DerStringBase GetAsn1String() {
            return asn1String;
        }

        public virtual String GetString() {
            return asn1String.GetString();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Asn1.ASN1StringBCFips that = (iText.Bouncycastlefips.Asn1.ASN1StringBCFips)o;
            return Object.Equals(asn1String, that.asn1String);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(asn1String);
        }

        public override String ToString() {
            return asn1String.ToString();
        }
    }
}
