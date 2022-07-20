using System;
using Org.BouncyCastle.Asn1.Utilities;
using iText.Commons.Bouncycastle.Asn1.Util;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1.Util {
    public class ASN1DumpBC : IASN1Dump {
        private static readonly iText.Bouncycastle.Asn1.Util.ASN1DumpBC INSTANCE = new iText.Bouncycastle.Asn1.Util.ASN1DumpBC
            (null);

        private readonly Asn1Dump asn1Dump;

        public ASN1DumpBC(Asn1Dump asn1Dump) {
            this.asn1Dump = asn1Dump;
        }

        public static iText.Bouncycastle.Asn1.Util.ASN1DumpBC GetInstance() {
            return INSTANCE;
        }

        public virtual Asn1Dump GetAsn1Dump() {
            return asn1Dump;
        }

        public virtual String DumpAsString(Object obj, bool b) {
            return Asn1Dump.DumpAsString(obj, b);
        }

        public virtual String DumpAsString(Object obj) {
            return Asn1Dump.DumpAsString(obj);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Asn1.Util.ASN1DumpBC that = (iText.Bouncycastle.Asn1.Util.ASN1DumpBC)o;
            return Object.Equals(asn1Dump, that.asn1Dump);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(asn1Dump);
        }

        public override String ToString() {
            return asn1Dump.ToString();
        }
    }
}
