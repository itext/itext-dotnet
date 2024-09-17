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
using System.Collections.Generic;
using iText.Forms;
using iText.Forms.Exceptions;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="AbstractFormFieldRenderer"/>
    /// implementation for radio buttons.
    /// </summary>
    public class RadioRenderer : AbstractFormFieldRenderer {
        private static readonly Color DEFAULT_CHECKED_COLOR = ColorConstants.BLACK;

        private const float DEFAULT_SIZE = 8.25f;

        // 11px
        private static readonly HorizontalAlignment? DEFAULT_HORIZONTAL_ALIGNMENT = HorizontalAlignment.CENTER;

        private static readonly VerticalAlignment? DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.MIDDLE;

        /// <summary>
        /// Creates a new
        /// <see cref="RadioRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        public RadioRenderer(Radio modelElement)
            : base(modelElement) {
            SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.MIDDLE);
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.RadioRenderer((Radio)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="drawContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void DrawBorder(DrawContext drawContext) {
        }

        // Do not draw borders here, they will be drawn in flat renderer
        /// <summary><inheritDoc/></summary>
        /// <param name="drawContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void DrawBackground(DrawContext drawContext) {
        }

        // Do not draw a background here, it will be drawn in flat renderer
        /// <summary><inheritDoc/></summary>
        /// <param name="rect">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="borders">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="reverse">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected override Rectangle ApplyBorderBox(Rectangle rect, Border[] borders, bool reverse) {
            // Do not apply borders here, they will be applied in flat renderer
            return rect;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IRenderer CreateFlatRenderer() {
            UnitValue heightUV = GetPropertyAsUnitValue(Property.HEIGHT);
            UnitValue widthUV = GetPropertyAsUnitValue(Property.WIDTH);
            float height = null == heightUV ? DEFAULT_SIZE : heightUV.GetValue();
            float width = null == widthUV ? DEFAULT_SIZE : widthUV.GetValue();
            float size = Math.Min(height, width);
            // Set size to current renderer
            SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(height));
            SetProperty(Property.WIDTH, UnitValue.CreatePointValue(width));
            Paragraph paragraph = new Paragraph().SetWidth(size).SetHeight(size).SetHorizontalAlignment(DEFAULT_HORIZONTAL_ALIGNMENT
                ).SetVerticalAlignment(DEFAULT_VERTICAL_ALIGNMENT).SetMargin(0);
            paragraph.SetProperty(Property.BOX_SIZING, this.GetProperty<BoxSizingPropertyValue?>(Property.BOX_SIZING));
            paragraph.SetBorderTop(this.GetProperty<Border>(Property.BORDER_TOP));
            paragraph.SetBorderRight(this.GetProperty<Border>(Property.BORDER_RIGHT));
            paragraph.SetBorderBottom(this.GetProperty<Border>(Property.BORDER_BOTTOM));
            paragraph.SetBorderLeft(this.GetProperty<Border>(Property.BORDER_LEFT));
            paragraph.SetProperty(Property.BACKGROUND, this.GetProperty<Background>(Property.BACKGROUND));
            paragraph.SetBorderRadius(new BorderRadius(UnitValue.CreatePercentValue(50)));
            return new RadioRenderer.FlatParagraphRenderer(this, paragraph);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void AdjustFieldLayout(LayoutContext layoutContext) {
        }

        // We don't need any adjustments (even default ones) for radio button.
        /// <summary>Defines whether the radio is checked or not.</summary>
        /// <returns>the default value of the radio field</returns>
        public virtual bool IsBoxChecked() {
            return true.Equals(this.GetProperty<bool?>(FormProperty.FORM_FIELD_CHECKED));
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void ApplyAcroField(DrawContext drawContext) {
            PdfDocument doc = drawContext.GetDocument();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(doc, true);
            Rectangle area = flatRenderer.GetOccupiedArea().GetBBox().Clone();
            IDictionary<int, Object> properties = FormFieldRendererUtil.RemoveProperties(this.modelElement);
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            String groupName = this.GetProperty<String>(FormProperty.FORM_FIELD_RADIO_GROUP_NAME);
            if (groupName == null || String.IsNullOrEmpty(groupName)) {
                throw new PdfException(FormsExceptionMessageConstant.EMPTY_RADIO_GROUP_NAME);
            }
            PdfButtonFormField radioGroup = (PdfButtonFormField)form.GetField(groupName);
            if (null == radioGroup) {
                radioGroup = new RadioFormFieldBuilder(doc, groupName).SetConformanceLevel(GetConformanceLevel(doc)).CreateRadioGroup
                    ();
                radioGroup.DisableFieldRegeneration();
                radioGroup.SetValue(PdfFormAnnotation.OFF_STATE_VALUE);
            }
            else {
                radioGroup.DisableFieldRegeneration();
            }
            if (IsBoxChecked()) {
                radioGroup.SetValue(GetModelId());
            }
            PdfFormAnnotation radio = new RadioFormFieldBuilder(doc, null).SetConformanceLevel(GetConformanceLevel(doc
                )).CreateRadioButton(GetModelId(), area);
            radio.DisableFieldRegeneration();
            Background background = this.GetProperty<Background>(Property.BACKGROUND);
            if (background != null) {
                radio.SetBackgroundColor(background.GetColor());
            }
            BorderStyleUtil.ApplyBorderProperty(this, radio);
            radio.SetFormFieldElement((Radio)modelElement);
            radioGroup.AddKid(radio);
            radioGroup.EnableFieldRegeneration();
            ApplyAccessibilityProperties(radioGroup, doc);
            form.AddField(radioGroup, page);
            FormFieldRendererUtil.ReapplyProperties(this.modelElement, properties);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsLayoutBasedOnFlatRenderer() {
            return false;
        }

        private bool IsDrawCircledBorder() {
            return true.Equals(this.GetProperty<bool?>(FormProperty.FORM_FIELD_RADIO_BORDER_CIRCLE));
        }

        private class FlatParagraphRenderer : ParagraphRenderer {
            public FlatParagraphRenderer(RadioRenderer _enclosing, Paragraph modelElement)
                : base(modelElement) {
                this._enclosing = _enclosing;
            }

            public override void DrawChildren(DrawContext drawContext) {
                if (!this._enclosing.IsBoxChecked()) {
                    // Nothing to draw
                    return;
                }
                PdfCanvas canvas = drawContext.GetCanvas();
                bool isTaggingEnabled = drawContext.IsTaggingEnabled();
                if (isTaggingEnabled) {
                    TagTreePointer tp = drawContext.GetDocument().GetTagStructureContext().GetAutoTaggingPointer();
                    canvas.OpenTag(tp.GetTagReference());
                }
                Rectangle rectangle = this.GetOccupiedArea().GetBBox().Clone();
                Border borderTop = this.GetProperty<Border>(Property.BORDER_TOP);
                Border borderRight = this.GetProperty<Border>(Property.BORDER_RIGHT);
                Border borderBottom = this.GetProperty<Border>(Property.BORDER_BOTTOM);
                Border borderLeft = this.GetProperty<Border>(Property.BORDER_LEFT);
                rectangle.ApplyMargins(borderTop == null ? 0 : borderTop.GetWidth(), borderRight == null ? 0 : borderRight
                    .GetWidth(), borderBottom == null ? 0 : borderBottom.GetWidth(), borderLeft == null ? 0 : borderLeft.GetWidth
                    (), false);
                float radius = Math.Min(rectangle.GetWidth(), rectangle.GetHeight()) / 2;
                canvas.SaveState();
                canvas.SetFillColor(RadioRenderer.DEFAULT_CHECKED_COLOR);
                DrawingUtil.DrawCircle(canvas, rectangle.GetLeft() + radius, rectangle.GetBottom() + radius, radius / 2);
                canvas.RestoreState();
                if (isTaggingEnabled) {
                    canvas.CloseTag();
                }
            }

            /// <summary><inheritDoc/></summary>
            /// <param name="drawContext">
            /// 
            /// <inheritDoc/>
            /// </param>
            public override void DrawBorder(DrawContext drawContext) {
                Border border = this.GetBorders()[0];
                if (border == null || !this._enclosing.IsDrawCircledBorder()) {
                    base.DrawBorder(drawContext);
                    return;
                }
                // TODO: DEVSIX-7425 - Remove the following workaround once the ticket is fixed.
                // The rounded border/background is drawn lousy. It's not an exact circle for border radius 50%.
                // That is why we draw a real circle here by default
                float borderWidth = border.GetWidth();
                if (borderWidth > 0 && border.GetColor() != null) {
                    Rectangle rectangle = this.GetOccupiedArea().GetBBox().Clone();
                    rectangle.ApplyMargins(borderWidth, borderWidth, borderWidth, borderWidth, false);
                    float cx = rectangle.GetX() + rectangle.GetWidth() / 2;
                    float cy = rectangle.GetY() + rectangle.GetHeight() / 2;
                    float r = (Math.Min(rectangle.GetWidth(), rectangle.GetHeight()) + borderWidth) / 2;
                    bool isTaggingEnabled = drawContext.IsTaggingEnabled();
                    PdfCanvas canvas = drawContext.GetCanvas();
                    if (isTaggingEnabled) {
                        canvas.OpenTag(new CanvasArtifact());
                    }
                    canvas.SetStrokeColor(border.GetColor()).SetLineWidth(borderWidth).Circle(cx, cy, r).Stroke();
                    if (isTaggingEnabled) {
                        canvas.CloseTag();
                    }
                }
            }

            /// <summary><inheritDoc/></summary>
            /// <param name="drawContext">
            /// 
            /// <inheritDoc/>
            /// </param>
            public override void DrawBackground(DrawContext drawContext) {
                Border border = this.GetBorders()[0];
                if (border == null || !this._enclosing.IsDrawCircledBorder()) {
                    base.DrawBackground(drawContext);
                    return;
                }
                // TODO: DEVSIX-7425 - Remove the following workaround once the ticket is fixed.
                // The rounded border/background is drawn lousy. It's not an exact circle for border radius 50%.
                // That is why we draw a real circle here by default
                // Draw a circle
                float borderWidth = border.GetWidth();
                Background background = this.GetProperty<Background>(Property.BACKGROUND);
                Color backgroundColor = background == null ? null : background.GetColor();
                if (backgroundColor != null) {
                    Rectangle rectangle = this.GetOccupiedArea().GetBBox().Clone();
                    rectangle.ApplyMargins(borderWidth, borderWidth, borderWidth, borderWidth, false);
                    float cx = rectangle.GetX() + rectangle.GetWidth() / 2;
                    float cy = rectangle.GetY() + rectangle.GetHeight() / 2;
                    float r = (Math.Min(rectangle.GetWidth(), rectangle.GetHeight()) + borderWidth) / 2;
                    bool isTaggingEnabled = drawContext.IsTaggingEnabled();
                    PdfCanvas canvas = drawContext.GetCanvas();
                    if (isTaggingEnabled) {
                        canvas.OpenTag(new CanvasArtifact());
                    }
                    canvas.SetFillColor(backgroundColor).Circle(cx, cy, r).Fill();
                    if (isTaggingEnabled) {
                        canvas.CloseTag();
                    }
                }
            }

            private readonly RadioRenderer _enclosing;
        }
    }
}
