using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1InputStreamBCFips : IASN1InputStream {
        private readonly Asn1InputStream stream;

        public ASN1InputStreamBCFips(Asn1InputStream asn1InputStream) {
            this.stream = asn1InputStream;
        }

        public ASN1InputStreamBCFips(byte[] bytes) {
            this.stream = new Asn1InputStream(bytes);
        }

        public ASN1InputStreamBCFips(Stream stream) {
            this.stream = new Asn1InputStream(stream);
        }

        public virtual Asn1InputStream GetASN1InputStream() {
            return stream;
        }

        public virtual IASN1Primitive ReadObject() {
            return new ASN1PrimitiveBCFips(stream.ReadObject());
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
            iText.Bouncycastlefips.Asn1.ASN1InputStreamBCFips that = (iText.Bouncycastlefips.Asn1.ASN1InputStreamBCFips
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
