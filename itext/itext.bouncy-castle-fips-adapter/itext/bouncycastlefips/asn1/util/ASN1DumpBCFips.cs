using System;
using Org.BouncyCastle.Asn1.Utilities;
using iText.Commons.Bouncycastle.Asn1.Util;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1.Util {
    public class ASN1DumpBCFips : IASN1Dump {
        private static readonly iText.Bouncycastlefips.Asn1.Util.ASN1DumpBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.Util.ASN1DumpBCFips
            (null);

        private readonly Asn1Dump asn1Dump;

        public ASN1DumpBCFips(Asn1Dump asn1Dump) {
            this.asn1Dump = asn1Dump;
        }

        public static iText.Bouncycastlefips.Asn1.Util.ASN1DumpBCFips GetInstance() {
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
            iText.Bouncycastlefips.Asn1.Util.ASN1DumpBCFips that = (iText.Bouncycastlefips.Asn1.Util.ASN1DumpBCFips)o;
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
