using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerOutputStream"/>.
    /// </summary>
    public class ASN1OutputStreamBCFips : IASN1OutputStream {
        private readonly DerOutputStream stream;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerOutputStream"/>.
        /// </summary>
        /// <param name="stream">
        /// OutputStream to create
        /// <see cref="Org.BouncyCastle.Asn1.DerOutputStream"/>
        /// to be wrapped
        /// </param>
        public ASN1OutputStreamBCFips(Stream stream) {
            this.stream = new DerOutputStream(stream);
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerOutputStream"/>.
        /// </summary>
        /// <param name="stream">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerOutputStream"/>
        /// to be wrapped
        /// </param>
        public ASN1OutputStreamBCFips(DerOutputStream stream) {
            this.stream = stream;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerOutputStream"/>.
        /// </returns>
        public virtual DerOutputStream GetASN1OutputStream() {
            return stream;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void WriteObject(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            stream.WriteObject(primitiveBCFips.GetPrimitive());
        }

        /// <summary>
        /// Delegates
        /// <c>close</c>
        /// method call to the wrapped stream.
        /// </summary>
        public virtual void Dispose() {
            stream.Dispose();
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(stream);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return stream.ToString();
        }
    }
}
