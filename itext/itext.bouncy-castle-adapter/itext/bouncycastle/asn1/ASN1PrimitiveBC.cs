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
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>.
    /// </summary>
    public class ASN1PrimitiveBC : ASN1EncodableBC, IASN1Primitive {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>.
        /// </summary>
        /// <param name="primitive">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>
        /// to be wrapped
        /// </param>
        public ASN1PrimitiveBC(Asn1Object primitive)
            : base(primitive) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>.
        /// </summary>
        /// <param name="array">
        /// byte array to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>
        /// to be wrapped
        /// </param>
        public ASN1PrimitiveBC(byte[] array)
            : base(Asn1Object.FromByteArray(array)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Object"/>.
        /// </returns>
        public virtual Asn1Object GetPrimitive() {
            return (Asn1Object)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return GetPrimitive().GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded(String encoding) {
            return GetPrimitive().GetEncoded(encoding);
        }
    }
}
