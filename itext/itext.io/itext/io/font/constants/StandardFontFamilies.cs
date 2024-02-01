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

namespace iText.IO.Font.Constants {
    /// <summary>
    /// Class containing families for
    /// <see cref="StandardFonts"/>.
    /// </summary>
    /// <remarks>
    /// Class containing families for
    /// <see cref="StandardFonts"/>.
    /// This class was made for com.itextpdf.io.font.FontRegisterProvider.
    /// </remarks>
    public sealed class StandardFontFamilies {
        private StandardFontFamilies() {
        }

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.COURIER"/>
        /// ,
        /// <see cref="StandardFonts.COURIER_BOLD"/>
        /// ,
        /// <see cref="StandardFonts.COURIER_OBLIQUE"/>
        /// and
        /// <see cref="StandardFonts.COURIER_BOLDOBLIQUE"/>.
        /// </summary>
        public const String COURIER = "Courier";

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.HELVETICA"/>
        /// ,
        /// <see cref="StandardFonts.HELVETICA_BOLD"/>
        /// ,
        /// <see cref="StandardFonts.HELVETICA_OBLIQUE"/>
        /// and
        /// <see cref="StandardFonts.HELVETICA_BOLDOBLIQUE"/>.
        /// </summary>
        public const String HELVETICA = "Helvetica";

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.SYMBOL"/>.
        /// </summary>
        public const String SYMBOL = "Symbol";

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.ZAPFDINGBATS"/>.
        /// </summary>
        public const String ZAPFDINGBATS = "ZapfDingbats";

        /// <summary>
        /// Font family for
        /// <see cref="StandardFonts.TIMES_ROMAN"/>
        /// ,
        /// <see cref="StandardFonts.TIMES_BOLD"/>
        /// ,
        /// <see cref="StandardFonts.TIMES_ITALIC"/>
        /// and
        /// <see cref="StandardFonts.TIMES_BOLDITALIC"/>.
        /// </summary>
        public const String TIMES = "Times";
    }
}
