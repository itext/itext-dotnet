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
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// Abstract
    /// <see cref="AbstractFormFieldRenderer"/>
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
            if (String.IsNullOrEmpty(defaultValue)) {
                defaultValue = "\u00a0";
            }
            Text text = new Text(defaultValue);
            FormFieldValueNonTrimmingTextRenderer nextRenderer = new FormFieldValueNonTrimmingTextRenderer(text);
            text.SetNextRenderer(nextRenderer);
            return new Paragraph(text).SetMargin(0).CreateRendererSubTree();
        }

        /// <summary>Applies the default field properties.</summary>
        /// <param name="inputField">the input field</param>
        internal virtual void ApplyDefaultFieldProperties(PdfFormField inputField) {
            inputField.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_NONE);
            TransparentColor color = GetPropertyAsTransparentColor(Property.FONT_COLOR);
            if (color != null) {
                inputField.SetColor(color.GetColor());
            }
            inputField.SetJustification(this.GetProperty<TextAlignment?>(Property.TEXT_ALIGNMENT));
            ApplyBorderProperty(inputField.GetFirstFormAnnotation());
            Background background = this.GetProperty<Background>(Property.BACKGROUND);
            if (background != null) {
                inputField.GetFirstFormAnnotation().SetBackgroundColor(background.GetColor());
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

        /// <summary>Approximates font size to fit occupied area if width anf height are specified.</summary>
        /// <param name="layoutContext">layout context that specifies layout area.</param>
        /// <param name="lFontSize">minimal font size value.</param>
        /// <param name="rFontSize">maximum font size value.</param>
        /// <returns>fitting font size or -1 in case it shouldn't be approximated.</returns>
        internal virtual float ApproximateFontSize(LayoutContext layoutContext, float lFontSize, float rFontSize) {
            IRenderer flatRenderer = CreateFlatRenderer().SetParent(this);
            float? areaWidth = RetrieveWidth(layoutContext.GetArea().GetBBox().GetWidth());
            float? areaHeight = RetrieveHeight();
            if (areaWidth == null || areaHeight == null) {
                return -1;
            }
            flatRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(AbstractPdfFormField.DEFAULT_FONT_SIZE
                ));
            LayoutContext newLayoutContext = new LayoutContext(new LayoutArea(1, new Rectangle((float)areaWidth, (float
                )areaHeight)));
            if (flatRenderer.Layout(newLayoutContext).GetStatus() == LayoutResult.FULL) {
                return -1;
            }
            else {
                int numberOfIterations = 6;
                return CalculateFittingFontSize(flatRenderer, lFontSize, rFontSize, newLayoutContext, numberOfIterations);
            }
        }

        internal virtual float CalculateFittingFontSize(IRenderer renderer, float lFontSize, float rFontSize, LayoutContext
             newLayoutContext, int numberOfIterations) {
            for (int i = 0; i < numberOfIterations; i++) {
                float mFontSize = (lFontSize + rFontSize) / 2;
                renderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(mFontSize));
                LayoutResult result = renderer.Layout(newLayoutContext);
                if (result.GetStatus() == LayoutResult.FULL) {
                    lFontSize = mFontSize;
                }
                else {
                    rFontSize = mFontSize;
                }
            }
            return lFontSize;
        }

        // The width based on cols of textarea and size of input isn't affected by box sizing, so we emulate it here.
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
            if (averageLineHeight > EPS) {
                int visibleLinesNumber = (int)Math.Ceiling(height / averageLineHeight);
                AdjustNumberOfContentLines(lines, bBox, visibleLinesNumber, height);
            }
        }

        /// <summary>Gets the value of the lowest bottom coordinate for all field's children recursively.</summary>
        /// <returns>the lowest child bottom.</returns>
        internal virtual float GetLowestChildBottom(IRenderer renderer, float value) {
            float lowestChildBottom = value;
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                lowestChildBottom = GetLowestChildBottom(child, lowestChildBottom);
                if (child.GetOccupiedArea() != null && child.GetOccupiedArea().GetBBox().GetBottom() < lowestChildBottom) {
                    lowestChildBottom = child.GetOccupiedArea().GetBBox().GetBottom();
                }
            }
            return lowestChildBottom;
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
