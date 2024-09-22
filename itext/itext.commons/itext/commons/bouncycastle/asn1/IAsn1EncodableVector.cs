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
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1EncodableVector that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IAsn1EncodableVector {
        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object.
        /// </summary>
        /// <param name="primitive">ASN1Primitive wrapper.</param>
        void Add(IAsn1Object primitive);

        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object.
        /// </summary>
        /// <param name="attribute">Attribute wrapper.</param>
        void Add(IAttribute attribute);

        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object.
        /// </summary>
        /// <param name="element">AlgorithmIdentifier wrapper.</param>
        void Add(IAlgorithmIdentifier element);

        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object if the primitive is not null.
        /// </summary>
        /// <param name="primitive">ASN1Primitive wrapper.</param>
        void AddOptional(IAsn1Object primitive);

        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object if the attribute is not null.
        /// </summary>
        /// <param name="attribute">Attribute wrapper.</param>
        void AddOptional(IAttribute attribute);

        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object if the element is not null.
        /// </summary>
        /// <param name="element">AlgorithmIdentifier wrapper.</param>
        void AddOptional(IAlgorithmIdentifier element);

        /// <summary>
        /// Calls actual
        /// <c>size</c>
        /// method for the wrapped ASN1EncodableVector object.
        /// </summary>
        /// <returns>
        /// 
        /// <c>int</c>
        /// representing current vector size
        /// </returns>
        int Size();
    }
}
