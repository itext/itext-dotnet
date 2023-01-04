/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
