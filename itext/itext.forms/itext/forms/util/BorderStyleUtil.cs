/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Properties;

namespace iText.Forms.Util {
    /// <summary>This file is a helper class for the internal usage only.</summary>
    /// <remarks>
    /// This file is a helper class for the internal usage only.
    /// Be aware that its API and functionality may be changed in the future.
    /// </remarks>
    public sealed class BorderStyleUtil {
        /// <summary>Applies the border property to the annotation.</summary>
        /// <param name="container">property container to apply border properties from.</param>
        /// <param name="annotation">the annotation to set border characteristics to.</param>
        public static void ApplyBorderProperty(IPropertyContainer container, PdfFormAnnotation annotation) {
            // For now, we set left border to an annotation, but appropriate borders for an element will be drawn.
            Border border = container.GetProperty<Border>(Property.BORDER_LEFT);
            if (border != null) {
                annotation.SetBorderStyle(TransformBorderTypeToBorderStyleDictionary(border.GetBorderType()));
                annotation.SetBorderColor(border.GetColor());
                annotation.SetBorderWidth(border.GetWidth());
            }
        }

        private static PdfDictionary TransformBorderTypeToBorderStyleDictionary(int borderType) {
            PdfDictionary bs = new PdfDictionary();
            PdfName style;
            switch (borderType) {
                case 1001: {
                    style = PdfAnnotation.STYLE_UNDERLINE;
                    break;
                }

                case 1002: {
                    style = PdfAnnotation.STYLE_BEVELED;
                    break;
                }

                case 1003: {
                    style = PdfAnnotation.STYLE_INSET;
                    break;
                }

                case Border.DASHED_FIXED:
                case Border.DASHED:
                case Border.DOTTED: {
                    // Default dash array will be used.
                    style = PdfAnnotation.STYLE_DASHED;
                    break;
                }

                default: {
                    style = PdfAnnotation.STYLE_SOLID;
                    break;
                }
            }
            bs.Put(PdfName.S, style);
            return bs;
        }

        private BorderStyleUtil() {
        }
        // Private constructor.
    }
}
