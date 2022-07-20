using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1InputStreamBC : IASN1InputStream {
        private readonly Asn1InputStream stream;

        public ASN1InputStreamBC(Asn1InputStream asn1InputStream) {
            this.stream = asn1InputStream;
        }

        public ASN1InputStreamBC(byte[] bytes) {
            this.stream = new Asn1InputStream(bytes);
        }

        public ASN1InputStreamBC(Stream stream) {
            this.stream = new Asn1InputStream(stream);
        }

        public virtual Asn1InputStream GetASN1InputStream() {
            return stream;
        }

        public virtual IASN1Primitive ReadObject() {
            return new ASN1PrimitiveBC(stream.ReadObject());
        }

        public virtual void Close() {
            stream.Close();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Asn1.ASN1InputStreamBC that = (iText.Bouncycastle.Asn1.ASN1InputStreamBC)o;
            return Object.Equals(stream, that.stream);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(stream);
        }

        public override String ToString() {
            return stream.ToString();
        }
    }
}
