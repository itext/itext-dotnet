/*
This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

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

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1OutputStream"/>.
    /// </summary>
    public class DerOutputStreamBC : IDerOutputStream {
        private readonly Asn1OutputStream stream;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OutputStream"/>.
        /// </summary>
        /// <param name="stream">
        /// OutputStream to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OutputStream"/>
        /// to be wrapped
        /// </param>
        public DerOutputStreamBC(Stream stream) {
            this.stream = Asn1OutputStream.Create(stream);
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OutputStream"/>.
        /// </summary>
        /// <param name="stream">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OutputStream"/>
        /// to be wrapped
        /// </param>
        public DerOutputStreamBC(Asn1OutputStream stream) {
            this.stream = stream;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OutputStream"/>.
        /// </returns>
        public virtual Asn1OutputStream GetDerOutputStream() {
            return stream;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void WriteObject(IAsn1Object primitive) {
            Asn1ObjectBC primitiveBC = (Asn1ObjectBC)primitive;
            stream.WriteObject(primitiveBC.GetPrimitive());
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
            DerOutputStreamBC that = (DerOutputStreamBC)o;
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
