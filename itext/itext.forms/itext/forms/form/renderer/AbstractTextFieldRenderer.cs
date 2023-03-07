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
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.Forms.Form.Element;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// Abstract
    /// <see cref="iText.Layout.Renderer.BlockRenderer"/>
    /// for form fields with text content.
    /// </summary>
    public abstract class AbstractTextFieldRenderer : AbstractFormFieldRenderer {
        /// <summary>The font to be used for the text.</summary>
        protected internal PdfFont font;

        /// <summary>
        /// Creates a new
        /// <see cref="AbstractTextFieldRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        internal AbstractTextFieldRenderer(IFormField modelElement)
            : base(modelElement) {
        }

        /// <summary>Creates a paragraph renderer.</summary>
        /// <param name="defaultValue">the default value</param>
        /// <returns>the renderer</returns>
        internal virtual IRenderer CreateParagraphRenderer(String defaultValue) {
            if (String.IsNullOrEmpty(defaultValue.Trim())) {
                defaultValue = "\u00A0";
            }
            Paragraph paragraph = new Paragraph(defaultValue).SetMargin(0);
            return paragraph.CreateRendererSubTree();
        }

        /// <summary>Applies the default field properties.</summary>
        /// <param name="inputField">the input field</param>
        internal virtual void ApplyDefaultFieldProperties(PdfFormField inputField) {
            inputField.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_NONE);
            inputField.GetFirstFormAnnotation().SetBorderWidth(0);
            TransparentColor color = GetPropertyAsTransparentColor(Property.FONT_COLOR);
            if (color != null) {
                inputField.SetColor(color.GetColor());
            }
        }

        internal virtual float GetHeightRowsBased(IList<LineRenderer> lines, Rectangle bBox, int rows) {
            float averageLineHeight = bBox.GetHeight() / lines.Count;
            return averageLineHeight * rows;
        }

        /// <summary>Updates the font.</summary>
        /// <param name="renderer">the renderer</param>
        internal virtual void UpdatePdfFont(ParagraphRenderer renderer) {
            Object retrievedFont;
            if (renderer != null) {
                IList<LineRenderer> lines = renderer.GetLines();
                if (lines != null) {
                    foreach (LineRenderer line in lines) {
                        foreach (IRenderer child in line.GetChildRenderers()) {
                            retrievedFont = child.GetProperty<Object>(Property.FONT);
                            if (retrievedFont is PdfFont) {
                                font = (PdfFont)retrievedFont;
                                return;
                            }
                        }
                    }
                }
                retrievedFont = renderer.GetProperty<Object>(Property.FONT);
                if (retrievedFont is PdfFont) {
                    font = (PdfFont)retrievedFont;
                }
            }
        }

        //The width based on cols of textarea and size of input doesn't affected by box sizing, so we emulate it here
        internal virtual float UpdateHtmlColsSizeBasedWidth(float width) {
            if (BoxSizingPropertyValue.BORDER_BOX == this.GetProperty<BoxSizingPropertyValue?>(Property.BOX_SIZING)) {
                Rectangle dummy = new Rectangle(width, 0);
                ApplyBorderBox(dummy, true);
                ApplyPaddings(dummy, true);
                return dummy.GetWidth();
            }
            return width;
        }

        /// <summary>Adjust number of content lines.</summary>
        /// <param name="lines">the lines that need to be rendered</param>
        /// <param name="bBox">the bounding box</param>
        /// <param name="rows">the desired number of lines</param>
        internal virtual void AdjustNumberOfContentLines(IList<LineRenderer> lines, Rectangle bBox, int rows) {
            if (lines.Count != rows) {
                float rowsHeight = GetHeightRowsBased(lines, bBox, rows);
                AdjustNumberOfContentLines(lines, bBox, rows, rowsHeight);
            }
        }

        /// <summary>Adjust number of content lines.</summary>
        /// <param name="lines">the lines that need to be rendered</param>
        /// <param name="bBox">the bounding box</param>
        /// <param name="height">the desired height of content</param>
        internal virtual void AdjustNumberOfContentLines(IList<LineRenderer> lines, Rectangle bBox, float height) {
            float averageLineHeight = bBox.GetHeight() / lines.Count;
            int visibleLinesNumber = (int)Math.Ceiling(height / averageLineHeight);
            AdjustNumberOfContentLines(lines, bBox, visibleLinesNumber, height);
        }

        private static void AdjustNumberOfContentLines(IList<LineRenderer> lines, Rectangle bBox, int linesNumber, 
            float height) {
            bBox.MoveUp(bBox.GetHeight() - height);
            bBox.SetHeight(height);
            if (lines.Count > linesNumber) {
                IList<LineRenderer> subList = new List<LineRenderer>(lines.SubList(0, linesNumber));
                lines.Clear();
                lines.AddAll(subList);
            }
        }
    }
}
