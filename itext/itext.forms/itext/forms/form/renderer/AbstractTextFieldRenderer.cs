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
