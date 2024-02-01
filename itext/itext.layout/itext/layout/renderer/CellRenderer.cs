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
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Represents a renderer for the
    /// <see cref="iText.Layout.Element.Cell"/>
    /// layout element.
    /// </summary>
    public class CellRenderer : BlockRenderer {
        /// <summary>Creates a CellRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.Cell"/>
        /// which this object should manage
        /// </param>
        public CellRenderer(Cell modelElement)
            : base(modelElement) {
            System.Diagnostics.Debug.Assert(modelElement != null);
            SetProperty(Property.ROWSPAN, modelElement.GetRowspan());
            SetProperty(Property.COLSPAN, modelElement.GetColspan());
        }

        /// <summary><inheritDoc/></summary>
        public override IPropertyContainer GetModelElement() {
            return base.GetModelElement();
        }

        protected internal override float? RetrieveWidth(float parentBoxWidth) {
            return null;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override AbstractRenderer CreateSplitRenderer(int layoutResult) {
            iText.Layout.Renderer.CellRenderer splitRenderer = (iText.Layout.Renderer.CellRenderer)GetNextRenderer();
            splitRenderer.parent = parent;
            splitRenderer.modelElement = modelElement;
            splitRenderer.occupiedArea = occupiedArea;
            splitRenderer.isLastRendererForModelElement = false;
            splitRenderer.AddAllProperties(GetOwnProperties());
            return splitRenderer;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override AbstractRenderer CreateOverflowRenderer(int layoutResult) {
            iText.Layout.Renderer.CellRenderer overflowRenderer = (iText.Layout.Renderer.CellRenderer)GetNextRenderer(
                );
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            return overflowRenderer;
        }

        public override void DrawBackground(DrawContext drawContext) {
            PdfCanvas canvas = drawContext.GetCanvas();
            Matrix ctm = canvas.GetGraphicsState().GetCtm();
            // Avoid rotation
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            bool avoidRotation = null != angle && HasProperty(Property.BACKGROUND);
            bool restoreRotation = HasOwnProperty(Property.ROTATION_ANGLE);
            if (avoidRotation) {
                AffineTransform transform = new AffineTransform(ctm.Get(0), ctm.Get(1), ctm.Get(3), ctm.Get(4), ctm.Get(6)
                    , ctm.Get(7));
                try {
                    transform = transform.CreateInverse();
                }
                catch (NoninvertibleTransformException e) {
                    throw new PdfException(LayoutExceptionMessageConstant.NONINVERTIBLE_MATRIX_CANNOT_BE_PROCESSED, e);
                }
                transform.Concatenate(new AffineTransform());
                canvas.ConcatMatrix(transform);
                SetProperty(Property.ROTATION_ANGLE, null);
            }
            base.DrawBackground(drawContext);
            // restore concat matrix and rotation angle
            if (avoidRotation) {
                if (restoreRotation) {
                    SetProperty(Property.ROTATION_ANGLE, angle);
                }
                else {
                    DeleteOwnProperty(Property.ROTATION_ANGLE);
                }
                canvas.ConcatMatrix(new AffineTransform(ctm.Get(0), ctm.Get(1), ctm.Get(3), ctm.Get(4), ctm.Get(6), ctm.Get
                    (7)));
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawBorder(DrawContext drawContext) {
            if (BorderCollapsePropertyValue.SEPARATE.Equals(parent.GetProperty<BorderCollapsePropertyValue?>(Property.
                BORDER_COLLAPSE))) {
                base.DrawBorder(drawContext);
            }
        }

        // Do nothing here. Border drawing for cells is done on TableRenderer.
        protected internal override Rectangle ApplyBorderBox(Rectangle rect, Border[] borders, bool reverse) {
            if (BorderCollapsePropertyValue.SEPARATE.Equals(parent.GetProperty<BorderCollapsePropertyValue?>(Property.
                BORDER_COLLAPSE))) {
                base.ApplyBorderBox(rect, borders, reverse);
            }
            // Do nothing here. Borders are processed on TableRenderer level.
            return rect;
        }

        protected internal override Rectangle ApplyMargins(Rectangle rect, UnitValue[] margins, bool reverse) {
            // If borders are separated, process border's spacing here.
            if (BorderCollapsePropertyValue.SEPARATE.Equals(parent.GetProperty<BorderCollapsePropertyValue?>(Property.
                BORDER_COLLAPSE))) {
                ApplySpacings(rect, reverse);
            }
            return rect;
        }

        /// <summary>Applies spacings on the given rectangle.</summary>
        /// <param name="rect">a rectangle spacings will be applied on</param>
        /// <param name="reverse">
        /// indicates whether spacings will be applied
        /// inside (in case of false) or outside (in case of true) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        protected internal virtual Rectangle ApplySpacings(Rectangle rect, bool reverse) {
            if (BorderCollapsePropertyValue.SEPARATE.Equals(parent.GetProperty<BorderCollapsePropertyValue?>(Property.
                BORDER_COLLAPSE))) {
                float? verticalBorderSpacing = this.parent.GetProperty<float?>(Property.VERTICAL_BORDER_SPACING);
                float? horizontalBorderSpacing = this.parent.GetProperty<float?>(Property.HORIZONTAL_BORDER_SPACING);
                float[] cellSpacings = new float[4];
                for (int i = 0; i < cellSpacings.Length; i++) {
                    cellSpacings[i] = 0 == i % 2 ? null != verticalBorderSpacing ? (float)verticalBorderSpacing : 0f : null !=
                         horizontalBorderSpacing ? (float)horizontalBorderSpacing : 0f;
                }
                ApplySpacings(rect, cellSpacings, reverse);
            }
            // Do nothing here. Spacings are meaningless if borders are collapsed.
            return rect;
        }

        /// <summary>Applies given spacings on the given rectangle.</summary>
        /// <param name="rect">a rectangle spacings will be applied on</param>
        /// <param name="spacings">the spacings to be applied on the given rectangle</param>
        /// <param name="reverse">
        /// indicates whether spacings will be applied
        /// inside (in case of false) or outside (in case of true) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        protected internal virtual Rectangle ApplySpacings(Rectangle rect, float[] spacings, bool reverse) {
            if (BorderCollapsePropertyValue.SEPARATE.Equals(parent.GetProperty<BorderCollapsePropertyValue?>(Property.
                BORDER_COLLAPSE))) {
                rect.ApplyMargins(spacings[0] / 2, spacings[1] / 2, spacings[2] / 2, spacings[3] / 2, reverse);
            }
            // Do nothing here. Spacings are meaningless if borders are collapsed.
            return rect;
        }

        /// <summary>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="BlockRenderer.Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// </summary>
        /// <remarks>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="BlockRenderer.Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// <para />
        /// If a renderer overflows to the next area, iText uses this method to create a renderer
        /// for the overflow part. So if one wants to extend
        /// <see cref="CellRenderer"/>
        /// , one should override
        /// this method: otherwise the default method will be used and thus the default rather than the custom
        /// renderer will be created.
        /// </remarks>
        /// <returns>new renderer instance</returns>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.CellRenderer), this.GetType());
            return new iText.Layout.Renderer.CellRenderer((Cell)GetModelElement());
        }
    }
}
