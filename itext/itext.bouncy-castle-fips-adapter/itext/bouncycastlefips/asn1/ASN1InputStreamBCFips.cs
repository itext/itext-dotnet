/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1InputStream"/>.
    /// </summary>
    public class ASN1InputStreamBCFips : IASN1InputStream {
        private readonly Asn1InputStream stream;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1InputStream"/>.
        /// </summary>
        /// <param name="asn1InputStream">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1InputStream"/>
        /// to be wrapped
        /// </param>
        public ASN1InputStreamBCFips(Asn1InputStream asn1InputStream) {
            this.stream = asn1InputStream;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1InputStream"/>.
        /// </summary>
        /// <param name="bytes">
        /// byte array to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1InputStream"/>
        /// </param>
        public ASN1InputStreamBCFips(byte[] bytes) {
            this.stream = new Asn1InputStream(bytes);
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1InputStream"/>.
        /// </summary>
        /// <param name="stream">
        /// InputStream to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1InputStream"/>
        /// </param>
        public ASN1InputStreamBCFips(Stream stream) {
            this.stream = new Asn1InputStream(stream);
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1InputStream"/>.
        /// </returns>
        public virtual Asn1InputStream GetASN1InputStream() {
            return stream;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Primitive ReadObject() {
            return new ASN1PrimitiveBCFips(stream.ReadObject());
        }

        /// <summary>
        /// Delegates
        /// <c>close</c>
        /// method call to the wrapped stream.
        /// </summary>
        public virtual void Dispose() {
            stream.Close();
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
            iText.Bouncycastlefips.Asn1.ASN1InputStreamBCFips that = (iText.Bouncycastlefips.Asn1.ASN1InputStreamBCFips
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
