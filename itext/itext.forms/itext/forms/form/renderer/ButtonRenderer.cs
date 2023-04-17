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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Logs;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="AbstractOneLineTextFieldRenderer"/>
    /// implementation for buttons.
    /// </summary>
    public class ButtonRenderer : AbstractTextFieldRenderer {
        /// <summary>Default padding Y offset for an input button.</summary>
        private const float DEFAULT_Y_OFFSET = 4;

        /// <summary>Relative value is quite big in order to preserve visible padding on small field sizes.</summary>
        /// <remarks>
        /// Relative value is quite big in order to preserve visible padding on small field sizes.
        /// This constant is taken arbitrary, based on visual similarity to Acrobat behaviour.
        /// </remarks>
        private const float RELATIVE_PADDING_FOR_SMALL_SIZES = 0.15f;

        /// <summary>Indicates if the one line caption was split.</summary>
        private bool isSplit = false;

        /// <summary>
        /// Creates a new
        /// <see cref="ButtonRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public ButtonRenderer(Button modelElement)
            : base(modelElement) {
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="layoutContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        protected internal override void AdjustFieldLayout(LayoutContext layoutContext) {
            if (((Button)modelElement).IsSingleLine()) {
                ParagraphRenderer renderer = (ParagraphRenderer)flatRenderer.GetChildRenderers()[0];
                IList<LineRenderer> flatLines = renderer.GetLines();
                Rectangle buttonBBox = GetOccupiedArea().GetBBox();
                Rectangle flatBBox = flatRenderer.GetOccupiedArea().GetBBox();
                UpdatePdfFont(renderer);
                if (flatLines.IsEmpty() || font == null) {
                    ITextLogManager.GetLogger(GetType()).LogError(MessageFormatUtil.Format(FormsLogMessageConstants.ERROR_WHILE_LAYOUT_OF_FORM_FIELD_WITH_TYPE
                        , "button"));
                    SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                    flatBBox.SetY(flatBBox.GetTop()).SetHeight(0);
                }
                else {
                    if (flatLines.Count != 1) {
                        // Used to check if renderer fit,
                        // happens when caption contains '\n' symbol.
                        isSplit = true;
                    }
                    CropContentLines(flatLines, flatBBox);
                    float? width = RetrieveWidth(layoutContext.GetArea().GetBBox().GetWidth());
                    if (width == null) {
                        LineRenderer drawnLine = flatLines[0];
                        drawnLine.Move(flatBBox.GetX() - drawnLine.GetOccupiedArea().GetBBox().GetX(), 0);
                        flatBBox.SetWidth(drawnLine.GetOccupiedArea().GetBBox().GetWidth());
                        buttonBBox.SetWidth(flatBBox.GetWidth() + 2 * (flatBBox.GetX() - buttonBBox.GetX()));
                    }
                }
            }
            else {
                if (this.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT) == null) {
                    // Apply middle vertical alignment for children including floats.
                    float lowestChildBottom = GetLowestChildBottom(flatRenderer, flatRenderer.GetOccupiedArea().GetBBox().GetBottom
                        ());
                    float deltaY = lowestChildBottom - GetInnerAreaBBox().GetY();
                    if (deltaY > 0) {
                        flatRenderer.Move(0, -deltaY / 2);
                    }
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="drawContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void Draw(DrawContext drawContext) {
            if (flatRenderer != null) {
                if (IsFlatten()) {
                    base.Draw(drawContext);
                }
                else {
                    DrawChildren(drawContext);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override bool IsLayoutBasedOnFlatRenderer() {
            return false;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected override float? GetLastYLineRecursively() {
            return base.GetFirstYLineRecursively();
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override IRenderer CreateFlatRenderer() {
            Div div = new Div();
            foreach (IElement child in ((Button)modelElement).GetChildren()) {
                if (child is Image) {
                    // Renderer for the image with fixed position will be added to positionedRenderers of the root renderer,
                    // so occupiedArea of div renderer can have zero or wrong sizes.
                    div.Add((Image)child);
                }
                else {
                    div.Add((IBlockElement)child);
                }
                child.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            }
            SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            if (((Button)modelElement).IsSingleLine()) {
                SetProperty(Property.NO_SOFT_WRAP_INLINE, true);
            }
            float? height = RetrieveHeight();
            if (height == null) {
                float buttonPadding = DEFAULT_Y_OFFSET;
                if (this.GetProperty<UnitValue>(Property.FONT_SIZE) != null) {
                    float fontSize = (this.GetProperty<UnitValue>(Property.FONT_SIZE)).GetValue();
                    buttonPadding = Math.Min(DEFAULT_Y_OFFSET, RELATIVE_PADDING_FOR_SMALL_SIZES * fontSize);
                }
                // 0 - top, 2 - bottom
                UnitValue[] paddings = GetPaddings();
                if (paddings[0] == null || paddings[0].GetValue() == 0) {
                    SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(buttonPadding));
                }
                if (paddings[2] == null || paddings[2].GetValue() == 0) {
                    SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(buttonPadding));
                }
            }
            return div.CreateRendererSubTree();
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.ButtonRenderer((Button)modelElement);
        }

        /// <summary>Gets the default value of the form field.</summary>
        /// <returns>the default value of the form field.</returns>
        public override String GetDefaultValue() {
            // FormProperty.FORM_FIELD_VALUE is not supported for Button element.
            return "";
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="availableWidth">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="availableHeight">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override bool IsRendererFit(float availableWidth, float availableHeight) {
            return !isSplit && base.IsRendererFit(availableWidth, availableHeight);
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="drawContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        protected internal override void ApplyAcroField(DrawContext drawContext) {
            String name = GetModelId();
            UnitValue fontSize = (UnitValue)this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (!fontSize.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.ButtonRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.FONT_SIZE));
            }
            PdfDocument doc = drawContext.GetDocument();
            Rectangle area = GetOccupiedArea().GetBBox().Clone();
            ApplyMargins(area, false);
            DeleteMargins();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            Background background = this.GetProperty<Background>(Property.BACKGROUND);
            // Background is light gray by default, but can be set to null by user.
            Color backgroundColor = background == null ? null : background.GetColor();
            float fontSizeValue = fontSize.GetValue();
            if (font == null) {
                font = doc.GetDefaultFont();
            }
            // Some properties are set to the HtmlDocumentRenderer, which is root renderer for this ButtonRenderer, but
            // in forms logic root renderer is CanvasRenderer, and these properties will have default values. So
            // we get them from renderer and set these properties to model element, which will be passed to forms logic.
            modelElement.SetProperty(Property.FONT_PROVIDER, this.GetProperty<FontProvider>(Property.FONT_PROVIDER));
            modelElement.SetProperty(Property.RENDERING_MODE, this.GetProperty<RenderingMode?>(Property.RENDERING_MODE
                ));
            PdfButtonFormField button = new PushButtonFormFieldBuilder(doc, name).SetWidgetRectangle(area).CreatePushButton
                ();
            button.SetFont(font).SetFontSize(fontSizeValue);
            button.GetFirstFormAnnotation().SetBackgroundColor(backgroundColor);
            ApplyDefaultFieldProperties(button);
            button.GetFirstFormAnnotation().SetFormFieldElement((Button)modelElement);
            PdfAcroForm forms = PdfAcroForm.GetAcroForm(doc, true);
            // Fields can be already added on split, e.g. when button split into multiple pages. But now we merge fields
            // with the same names (and add all the widgets as kids to that merged field), so we can add it anyway.
            forms.AddField(button, page);
            WriteAcroFormFieldLangAttribute(doc);
        }

        /// <summary>Crops the content lines.</summary>
        /// <param name="lines">a list of lines</param>
        /// <param name="bBox">the bounding box</param>
        private void CropContentLines(IList<LineRenderer> lines, Rectangle bBox) {
            AdjustNumberOfContentLines(lines, bBox, 1);
            float? height = RetrieveHeight();
            float? minHeight = RetrieveMinHeight();
            float? maxHeight = RetrieveMaxHeight();
            float originalHeight = flatRenderer.GetOccupiedArea().GetBBox().GetHeight();
            if (height != null && (float)height > 0) {
                SetContentHeight(flatRenderer, (float)height);
            }
            else {
                if (minHeight != null && (float)minHeight > originalHeight) {
                    SetContentHeight(flatRenderer, (float)minHeight);
                }
                else {
                    if (maxHeight != null && (float)maxHeight > 0 && (float)maxHeight < originalHeight) {
                        SetContentHeight(flatRenderer, (float)maxHeight);
                    }
                }
            }
        }

        /// <summary>Sets the content height.</summary>
        /// <param name="flatRenderer">the flat renderer</param>
        /// <param name="height">the height</param>
        private void SetContentHeight(IRenderer flatRenderer, float height) {
            Rectangle bBox = flatRenderer.GetOccupiedArea().GetBBox();
            Border border = GetBorders()[0];
            if (border != null) {
                height += border.GetWidth() * 2;
            }
            UnitValue[] paddings = GetPaddings();
            if (paddings[0] != null) {
                height += paddings[0].GetValue();
            }
            if (paddings[2] != null) {
                height += paddings[2].GetValue();
            }
            UnitValue[] margins = GetMargins();
            if (margins[0] != null) {
                height += margins[0].GetValue();
            }
            if (margins[2] != null) {
                height += margins[2].GetValue();
            }
            float newY = GetOccupiedArea().GetBBox().GetBottom() + height / 2 - bBox.GetHeight() / 2;
            float dy = bBox.GetBottom() - newY;
            bBox.MoveDown(dy);
            bBox.SetHeight(height);
            flatRenderer.Move(0, -dy);
        }

        /// <summary>Gets the value of the lowest bottom coordinate for all button children recursively.</summary>
        /// <returns>the lowest child bottom.</returns>
        private float GetLowestChildBottom(IRenderer renderer, float value) {
            float lowestChildBottom = value;
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                lowestChildBottom = GetLowestChildBottom(child, lowestChildBottom);
                if (child.GetOccupiedArea() != null && child.GetOccupiedArea().GetBBox().GetBottom() < lowestChildBottom) {
                    lowestChildBottom = child.GetOccupiedArea().GetBBox().GetBottom();
                }
            }
            return lowestChildBottom;
        }
    }
}
