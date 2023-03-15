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
using iText.Layout.Font;

namespace iText.StyledXmlParser.Resolver.Font {
    /// <summary>
    /// A basic
    /// <see cref="iText.Layout.Font.FontProvider"/>
    /// that allows configuring in the constructor which fonts are loaded by default.
    /// </summary>
    public class BasicFontProvider : FontProvider {
        private const String DEFAULT_FONT_FAMILY = "Times";

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        public BasicFontProvider()
            : this(true, false) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        /// <param name="registerStandardPdfFonts">use true if you want to register the standard Type 1 fonts (can't be embedded)
        ///     </param>
        /// <param name="registerSystemFonts">use true if you want to register the system fonts (can require quite some resources)
        ///     </param>
        public BasicFontProvider(bool registerStandardPdfFonts, bool registerSystemFonts)
            : this(registerStandardPdfFonts, registerSystemFonts, DEFAULT_FONT_FAMILY) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        /// <param name="registerStandardPdfFonts">use true if you want to register the standard Type 1 fonts (can't be embedded)
        ///     </param>
        /// <param name="registerSystemFonts">use true if you want to register the system fonts (can require quite some resources)
        ///     </param>
        /// <param name="defaultFontFamily">default font family</param>
        public BasicFontProvider(bool registerStandardPdfFonts, bool registerSystemFonts, String defaultFontFamily
            )
            : base(defaultFontFamily) {
            if (registerStandardPdfFonts) {
                AddStandardPdfFonts();
            }
            if (registerSystemFonts) {
                AddSystemFonts();
            }
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        /// <param name="fontSet">predefined set of fonts, could be null.</param>
        /// <param name="defaultFontFamily">default font family.</param>
        public BasicFontProvider(FontSet fontSet, String defaultFontFamily)
            : base(fontSet, defaultFontFamily) {
        }
    }
}
