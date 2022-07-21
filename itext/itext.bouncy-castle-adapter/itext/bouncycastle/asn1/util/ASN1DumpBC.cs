using System;
using iText.Commons.Bouncycastle.Asn1;
using Org.BouncyCastle.Asn1.Utilities;
using iText.Commons.Bouncycastle.Asn1.Util;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;

namespace iText.Bouncycastle.Asn1.Util {
    public class ASN1DumpBC : IASN1Dump {
        private static readonly ASN1DumpBC INSTANCE = new ASN1DumpBC
            (null);

        private readonly Asn1Dump asn1Dump;

        public ASN1DumpBC(Asn1Dump asn1Dump) {
            this.asn1Dump = asn1Dump;
        }

        public static ASN1DumpBC GetInstance() {
            return INSTANCE;
        }

        public virtual Asn1Dump GetAsn1Dump() {
            return asn1Dump;
        }

        public virtual String DumpAsString(Object obj, bool b) {
            if (obj is IASN1Encodable) {
                obj = ((ASN1EncodableBC)obj).GetEncodable();
            }
            return Asn1Dump.DumpAsString((Asn1Encodable) obj, b);
        }

        public virtual String DumpAsString(Object obj) {
            if (obj is IASN1Encodable) {
                obj = ((ASN1EncodableBC)obj).GetEncodable();
            }
            return Asn1Dump.DumpAsString((Asn1Encodable) obj);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            ASN1DumpBC that = (ASN1DumpBC)o;
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
