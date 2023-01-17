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
using System.Collections;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1Sequence that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1Sequence : IASN1Primitive {
        /// <summary>
        /// Calls actual
        /// <c>getObjectAt</c>
        /// method for the wrapped ASN1Sequence object.
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>
        /// 
        /// <see cref="IASN1Encodable"/>
        /// wrapped ASN1Encodable object.
        /// </returns>
        IASN1Encodable GetObjectAt(int i);

        /// <summary>
        /// Calls actual
        /// <c>getObjects</c>
        /// method for the wrapped ASN1Sequence object.
        /// </summary>
        /// <returns>received objects.</returns>
        IEnumerator GetObjects();

        /// <summary>
        /// Calls actual
        /// <c>size</c>
        /// method for the wrapped ASN1Sequence object.
        /// </summary>
        /// <returns>sequence size.</returns>
        int Size();

        /// <summary>
        /// Calls actual
        /// <c>toArray</c>
        /// method for the wrapped ASN1Sequence object.
        /// </summary>
        /// <returns>array of wrapped ASN1Encodable objects.</returns>
        IASN1Encodable[] ToArray();
    }
}
