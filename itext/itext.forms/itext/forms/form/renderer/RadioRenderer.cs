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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// The
    /// <see cref="AbstractOneLineTextFieldRenderer"/>
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

        /* (non-Javadoc)
        * @see com.itextpdf.layout.renderer.IRenderer#getNextRenderer()
        */
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
            paragraph.SetBorder(this.GetProperty<Border>(Property.BORDER));
            paragraph.SetProperty(Property.BACKGROUND, this.GetProperty<Background>(Property.BACKGROUND));
            paragraph.SetBorderRadius(new BorderRadius(UnitValue.CreatePercentValue(50)));
            return new RadioRenderer.FlatParagraphRenderer(this, paragraph);
        }

        /* (non-Javadoc)
        * @see AbstractFormFieldRenderer#adjustFieldLayout()
        */
        protected internal override void AdjustFieldLayout(LayoutContext layoutContext) {
        }

        /// <summary>Defines whether the radio is checked or not.</summary>
        /// <returns>the default value of the radio field</returns>
        public virtual bool IsBoxChecked() {
            return true.Equals(this.GetProperty<bool?>(FormProperty.FORM_FIELD_CHECKED));
        }

        /* (non-Javadoc)
        * @see AbstractFormFieldRenderer#applyAcroField(com.itextpdf.layout.renderer.DrawContext)
        */
        protected internal override void ApplyAcroField(DrawContext drawContext) {
            PdfDocument doc = drawContext.GetDocument();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            Rectangle area = flatRenderer.GetOccupiedArea().GetBBox().Clone();
            PdfPage page = doc.GetPage(occupiedArea.GetPageNumber());
            String groupName = this.GetProperty<String>(FormProperty.FORM_FIELD_RADIO_GROUP_NAME);
            if (groupName == null || String.IsNullOrEmpty(groupName)) {
                throw new PdfException(FormsExceptionMessageConstant.EMPTY_RADIO_GROUP_NAME);
            }
            PdfButtonFormField radioGroup = (PdfButtonFormField)form.GetField(groupName);
            bool addNew = false;
            if (null == radioGroup) {
                radioGroup = new RadioFormFieldBuilder(doc, groupName).CreateRadioGroup();
                radioGroup.SetValue(PdfFormAnnotation.OFF_STATE_VALUE);
                addNew = true;
            }
            if (IsBoxChecked()) {
                radioGroup.SetValue(GetModelId());
            }
            PdfFormAnnotation radio = new RadioFormFieldBuilder(doc, null).CreateRadioButton(GetModelId(), area);
            // Set background color
            Background background = this.GetProperty<Background>(Property.BACKGROUND);
            if (background != null) {
                radio.SetBackgroundColor(background.GetColor());
            }
            // Set border color and border width
            Border border = this.GetProperty<Border>(Property.BORDER);
            if (border != null) {
                radio.SetBorderColor(border.GetColor());
                radio.SetBorderWidth(border.GetWidth());
            }
            radioGroup.AddKid(radio);
            if (addNew) {
                form.AddField(radioGroup, page);
            }
            else {
                form.ReplaceField(groupName, radioGroup);
            }
            WriteAcroFormFieldLangAttribute(doc);
        }

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
                Rectangle rectangle = this.GetOccupiedArea().GetBBox().Clone();
                Border border = this.GetProperty<Border>(Property.BORDER);
                if (border != null) {
                    rectangle.ApplyMargins(border.GetWidth(), border.GetWidth(), border.GetWidth(), border.GetWidth(), false);
                }
                float radius = Math.Min(rectangle.GetWidth(), rectangle.GetHeight()) / 2;
                canvas.SaveState();
                canvas.SetFillColor(RadioRenderer.DEFAULT_CHECKED_COLOR);
                DrawingUtil.DrawCircle(canvas, rectangle.GetLeft() + radius, rectangle.GetBottom() + radius, radius / 2);
                canvas.RestoreState();
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
                    drawContext.GetCanvas().SetStrokeColor(border.GetColor()).SetLineWidth(borderWidth).Circle(cx, cy, r).Stroke
                        ();
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
                    drawContext.GetCanvas().SetFillColor(backgroundColor).Circle(cx, cy, r).Fill();
                }
            }

            private readonly RadioRenderer _enclosing;
        }
    }
}
