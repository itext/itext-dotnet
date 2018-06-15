/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: iText Software.

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
using System.Collections.Generic;
using iText.Kernel.Font;
using iText.Kernel.Pdf;

namespace iText.Forms.Fields {
    /// <summary>AppearanceResources allows to register font names that will be used as resource name.</summary>
    /// <remarks>
    /// AppearanceResources allows to register font names that will be used as resource name.
    /// Preserving existed font names in default resources of AcroForm is the only goal of this class.
    /// <p>
    /// Shall be used only in
    /// <see cref="PdfFormField"/>
    /// .
    /// </remarks>
    /// <seealso cref="AppearanceXObject"/>
    internal class AppearanceResources : PdfResources {
        private IDictionary<PdfIndirectReference, PdfName> drFonts = new Dictionary<PdfIndirectReference, PdfName>
            ();

        internal AppearanceResources()
            : base() {
        }

        internal AppearanceResources(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        internal virtual iText.Forms.Fields.AppearanceResources AddFontFromDefaultResources(PdfName name, PdfFont 
            font) {
            if (name != null && font != null && font.GetPdfObject().GetIndirectReference() != null) {
                //So, most likely it's a document PdfFont
                drFonts.Put(font.GetPdfObject().GetIndirectReference(), name);
            }
            return this;
        }

        public override PdfName AddFont(PdfDocument pdfDocument, PdfFont font) {
            PdfName fontName = null;
            if (font != null && font.GetPdfObject().GetIndirectReference() != null) {
                fontName = drFonts.Get(font.GetPdfObject().GetIndirectReference());
            }
            if (fontName != null) {
                AddResource(font.GetPdfObject(), PdfName.Font, fontName);
                return fontName;
            }
            else {
                return base.AddFont(pdfDocument, font);
            }
        }
    }
}
