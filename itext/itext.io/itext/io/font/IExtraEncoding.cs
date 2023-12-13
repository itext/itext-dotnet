/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.IO.Font {
    /// <summary>
    /// Classes implementing this interface can create custom encodings or
    /// replace existing ones.
    /// </summary>
    /// <remarks>
    /// Classes implementing this interface can create custom encodings or
    /// replace existing ones. It is used in the context of <c>PdfEncoding</c>.
    /// </remarks>
    /// <author>Paulo Soares</author>
    public interface IExtraEncoding {
        /// <summary>Converts an Unicode string to a byte array according to some encoding.</summary>
        /// <param name="text">the Unicode string</param>
        /// <param name="encoding">
        /// the requested encoding. It's mainly of use if the same class
        /// supports more than one encoding.
        /// </param>
        /// <returns>the conversion or <c>null</c> if no conversion is supported</returns>
        byte[] CharToByte(String text, String encoding);

        /// <summary>Converts an Unicode char to a byte array according to some encoding.</summary>
        /// <param name="char1">the Unicode char</param>
        /// <param name="encoding">
        /// the requested encoding. It's mainly of use if the same class
        /// supports more than one encoding.
        /// </param>
        /// <returns>the conversion or <c>null</c> if no conversion is supported</returns>
        byte[] CharToByte(char char1, String encoding);

        /// <summary>Converts a byte array to an Unicode string according to some encoding.</summary>
        /// <param name="b">the input byte array</param>
        /// <param name="encoding">
        /// the requested encoding. It's mainly of use if the same class
        /// supports more than one encoding.
        /// </param>
        /// <returns>the conversion or <c>null</c> if no conversion is supported</returns>
        String ByteToChar(byte[] b, String encoding);
    }
}
