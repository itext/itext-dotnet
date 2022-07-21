using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1OutputStreamBC : IASN1OutputStream {
        private readonly DerOutputStream stream;

        public ASN1OutputStreamBC(Stream stream) {
            this.stream = new Org.BouncyCastle.Asn1.Asn1OutputStream(stream);
        }

        public ASN1OutputStreamBC(DerOutputStream stream) {
            this.stream = stream;
        }

        public virtual DerOutputStream GetASN1OutputStream() {
            return stream;
        }

        public virtual void WriteObject(IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            stream.WriteObject(primitiveBC.GetPrimitive());
        }

        public virtual void Close() {
            stream.Dispose();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Asn1.ASN1OutputStreamBC that = (iText.Bouncycastle.Asn1.ASN1OutputStreamBC)o;
            return Object.Equals(stream, that.stream);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(stream);
        }

        public void Dispose() {
            Close();
        }

        public override String ToString() {
            return stream.ToString();
        }
    }
}
