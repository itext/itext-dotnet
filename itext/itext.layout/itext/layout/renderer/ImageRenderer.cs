/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class ImageRenderer : AbstractRenderer {
        private float height;

        private float? width;

        protected internal float? fixedXPosition;

        protected internal float? fixedYPosition;

        protected internal float pivotY;

        protected internal float deltaX;

        protected internal float imageWidth;

        protected internal float imageHeight;

        internal float[] matrix = new float[6];

        /// <summary>Creates an ImageRenderer from its corresponding layout object.</summary>
        /// <param name="image">
        /// the
        /// <see cref="iText.Layout.Element.Image"/>
        /// which this object should manage
        /// </param>
        public ImageRenderer(Image image)
            : base(image) {
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            LayoutArea area = layoutContext.GetArea().Clone();
            Rectangle layoutBox = area.GetBBox();
            ApplyMargins(layoutBox, false);
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), 0, 0));
            width = RetrieveWidth(layoutBox.GetWidth());
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            PdfXObject xObject = ((Image)(GetModelElement())).GetXObject();
            imageWidth = xObject.GetWidth();
            imageHeight = xObject.GetHeight();
            width = width == null ? imageWidth : width;
            height = (float)width / imageWidth * imageHeight;
            fixedXPosition = this.GetPropertyAsFloat(Property.X);
            fixedYPosition = this.GetPropertyAsFloat(Property.Y);
            float? horizontalScaling = this.GetPropertyAsFloat(Property.HORIZONTAL_SCALING, 1f);
            float? verticalScaling = this.GetPropertyAsFloat(Property.VERTICAL_SCALING, 1f);
            AffineTransform t = new AffineTransform();
            if (xObject is PdfFormXObject && width != imageWidth) {
                horizontalScaling *= width / imageWidth;
                verticalScaling *= height / imageHeight;
            }
            if (horizontalScaling != 1) {
                if (xObject is PdfFormXObject) {
                    t.Scale((float)horizontalScaling, 1);
                }
                width *= (float)horizontalScaling;
            }
            if (verticalScaling != 1) {
                if (xObject is PdfFormXObject) {
                    t.Scale(1, (float)verticalScaling);
                }
                height *= (float)verticalScaling;
            }
            float imageItselfScaledWidth = (float)width;
            float imageItselfScaledHeight = (float)height;
            // See in adjustPositionAfterRotation why angle = 0 is necessary
            if (null == angle) {
                angle = 0f;
            }
            t.Rotate((float)angle);
            float scaleCoef = AdjustPositionAfterRotation((float)angle, layoutBox.GetWidth(), layoutBox.GetHeight());
            imageItselfScaledHeight *= scaleCoef;
            imageItselfScaledWidth *= scaleCoef;
            if (xObject is PdfFormXObject) {
                t.Scale(scaleCoef, scaleCoef);
            }
            GetMatrix(t, imageItselfScaledWidth, imageItselfScaledHeight);
            // indicates whether the placement is forced
            bool isPlacingForced = false;
            if (width > layoutBox.GetWidth() || height > layoutBox.GetHeight()) {
                if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                    isPlacingForced = true;
                }
                else {
                    return new LayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this);
                }
            }
            occupiedArea.GetBBox().MoveDown(height);
            occupiedArea.GetBBox().SetHeight(height);
            occupiedArea.GetBBox().SetWidth((float)width);
            float leftMargin = (float)this.GetPropertyAsFloat(Property.MARGIN_LEFT);
            float topMargin = (float)this.GetPropertyAsFloat(Property.MARGIN_TOP);
            if (leftMargin != 0 || topMargin != 0) {
                TranslateImage(leftMargin, topMargin, t);
                GetMatrix(t, imageItselfScaledWidth, imageItselfScaledHeight);
            }
            //Add properties after calculation
            AddLayoutProperties();
            ApplyMargins(occupiedArea.GetBBox(), true);
            return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null, isPlacingForced ? this : null);
        }

        private void AddLayoutProperties() {
            //Add properties after calculation
            SetProperty(Property.HEIGHT, height);
            SetProperty(Property.WIDTH, width);
        }

        public override void Draw(DrawContext drawContext) {
            base.Draw(drawContext);
            PdfDocument document = drawContext.GetDocument();
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            bool isArtifact = false;
            TagTreePointer tagPointer = null;
            if (isTagged) {
                tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                IAccessibleElement accessibleElement = (IAccessibleElement)GetModelElement();
                PdfName role = accessibleElement.GetRole();
                if (role != null && !PdfName.Artifact.Equals(role)) {
                    AccessibleAttributesApplier.ApplyLayoutAttributes(accessibleElement.GetRole(), this, document);
                    tagPointer.AddTag(accessibleElement);
                }
                else {
                    isTagged = false;
                    if (PdfName.Artifact.Equals(role)) {
                        isArtifact = true;
                    }
                }
            }
            ApplyMargins(occupiedArea.GetBBox(), false);
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyAbsolutePositioningTranslation(false);
            }
            if (fixedYPosition == null) {
                fixedYPosition = occupiedArea.GetBBox().GetY() + pivotY;
            }
            if (fixedXPosition == null) {
                fixedXPosition = occupiedArea.GetBBox().GetX();
            }
            PdfCanvas canvas = drawContext.GetCanvas();
            if (isTagged) {
                canvas.OpenTag(tagPointer.GetTagReference());
            }
            else {
                if (isArtifact) {
                    canvas.OpenTag(new CanvasArtifact());
                }
            }
            PdfXObject xObject = ((Image)(GetModelElement())).GetXObject();
            canvas.AddXObject(xObject, matrix[0], matrix[1], matrix[2], matrix[3], (float)fixedXPosition + deltaX, (float
                )fixedYPosition);
            if (true.Equals(GetPropertyAsBoolean(Property.FLUSH_ON_DRAW))) {
                xObject.Flush();
            }
            if (isTagged || isArtifact) {
                canvas.CloseTag();
            }
            if (isRelativePosition) {
                ApplyAbsolutePositioningTranslation(true);
            }
            ApplyMargins(occupiedArea.GetBBox(), true);
            if (isTagged) {
                tagPointer.MoveToParent();
            }
        }

        public override IRenderer GetNextRenderer() {
            return null;
        }

        protected internal virtual iText.Layout.Renderer.ImageRenderer AutoScale(LayoutArea area) {
            if (width > area.GetBBox().GetWidth()) {
                SetProperty(Property.HEIGHT, area.GetBBox().GetWidth() / width * imageHeight);
                SetProperty(Property.WIDTH, UnitValue.CreatePointValue(area.GetBBox().GetWidth()));
                // if still image is not scaled properly
                if (this.GetPropertyAsFloat(Property.HEIGHT) > area.GetBBox().GetHeight()) {
                    SetProperty(Property.WIDTH, UnitValue.CreatePointValue(area.GetBBox().GetHeight() / (float)this.GetPropertyAsFloat
                        (Property.HEIGHT) * (this.GetProperty<UnitValue>(Property.WIDTH)).GetValue()));
                    SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(area.GetBBox().GetHeight()));
                }
            }
            return this;
        }

        private void GetMatrix(AffineTransform t, float imageItselfScaledWidth, float imageItselfScaledHeight) {
            t.GetMatrix(matrix);
            PdfXObject xObject = ((Image)(GetModelElement())).GetXObject();
            if (xObject is PdfImageXObject) {
                matrix[0] *= imageItselfScaledWidth;
                matrix[1] *= imageItselfScaledWidth;
                matrix[2] *= imageItselfScaledHeight;
                matrix[3] *= imageItselfScaledHeight;
            }
        }

        private float AdjustPositionAfterRotation(float angle, float maxWidth, float maxHeight) {
            if (angle != 0) {
                AffineTransform t = AffineTransform.GetRotateInstance(angle);
                Point p00 = t.Transform(new Point(0, 0), new Point());
                Point p01 = t.Transform(new Point(0, height), new Point());
                Point p10 = t.Transform(new Point((float)width, 0), new Point());
                Point p11 = t.Transform(new Point((float)width, height), new Point());
                double[] xValues = new double[] { p01.GetX(), p10.GetX(), p11.GetX() };
                double[] yValues = new double[] { p01.GetY(), p10.GetY(), p11.GetY() };
                double minX = p00.GetX();
                double minY = p00.GetY();
                double maxX = minX;
                double maxY = minY;
                foreach (double x in xValues) {
                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x);
                }
                foreach (double y in yValues) {
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, y);
                }
                height = (float)(maxY - minY);
                width = (float)(maxX - minX);
                pivotY = (float)(p00.GetY() - minY);
                deltaX = -(float)minX;
            }
            // Rotating image can cause fitting into area problems.
            // So let's find scaling coefficient
            // TODO
            float scaleCoeff = 1;
            // hasProperty(Property) checks only properties field, cannot use it
            if (true.Equals(GetPropertyAsBoolean(Property.AUTO_SCALE))) {
                scaleCoeff = Math.Min(maxWidth / (float)width, maxHeight / height);
                height *= scaleCoeff;
                width *= scaleCoeff;
            }
            else {
                if (null != GetPropertyAsBoolean(Property.AUTO_SCALE_WIDTH) && (bool)GetPropertyAsBoolean(Property.AUTO_SCALE_WIDTH
                    )) {
                    scaleCoeff = maxWidth / (float)width;
                    height *= scaleCoeff;
                    width = maxWidth;
                }
                else {
                    if (null != GetPropertyAsBoolean(Property.AUTO_SCALE_HEIGHT) && (bool)GetPropertyAsBoolean(Property.AUTO_SCALE_HEIGHT
                        )) {
                        scaleCoeff = maxHeight / height;
                        height = maxHeight;
                        width *= scaleCoeff;
                    }
                }
            }
            pivotY *= scaleCoeff;
            return scaleCoeff;
        }

        private void TranslateImage(float xDistance, float yDistance, AffineTransform t) {
            t.Translate(xDistance, yDistance);
            t.GetMatrix(matrix);
            if (fixedXPosition != null) {
                fixedXPosition += (float)t.GetTranslateX();
            }
            if (fixedYPosition != null) {
                fixedYPosition += (float)t.GetTranslateY();
            }
        }
    }
}
