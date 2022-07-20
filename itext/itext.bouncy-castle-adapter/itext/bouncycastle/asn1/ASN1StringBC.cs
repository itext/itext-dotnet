using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1StringBC : IASN1String {
        private readonly DerStringBase asn1String;

        public ASN1StringBC(DerStringBase asn1String) {
            this.asn1String = asn1String;
        }

        public virtual DerStringBase GetASN1String() {
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
            iText.Bouncycastle.Asn1.ASN1StringBC that = (iText.Bouncycastle.Asn1.ASN1StringBC)o;
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
