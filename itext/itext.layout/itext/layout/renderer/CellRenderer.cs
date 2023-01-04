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
