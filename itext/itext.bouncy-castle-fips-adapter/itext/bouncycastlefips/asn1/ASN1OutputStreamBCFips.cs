using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1OutputStreamBCFips : IASN1OutputStream {
        private readonly DerOutputStream stream;

        public ASN1OutputStreamBCFips(Stream stream) {
            this.stream = new DerOutputStream(stream);
        }

        public ASN1OutputStreamBCFips(DerOutputStream stream) {
            this.stream = stream;
        }

        public virtual DerOutputStream GetASN1OutputStream() {
            return stream;
        }

        public virtual void WriteObject(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            stream.WriteObject(primitiveBCFips.GetPrimitive());
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
            iText.Bouncycastlefips.Asn1.ASN1OutputStreamBCFips that = (iText.Bouncycastlefips.Asn1.ASN1OutputStreamBCFips
                )o;
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
