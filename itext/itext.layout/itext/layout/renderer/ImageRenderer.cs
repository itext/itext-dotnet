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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer.Objectfit;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    public class ImageRenderer : AbstractRenderer, ILeafElementRenderer {
        protected internal float? fixedXPosition;

        protected internal float? fixedYPosition;

        protected internal float pivotY;

        protected internal float deltaX;

        protected internal float imageWidth;

        protected internal float imageHeight;

        internal float[] matrix = new float[6];

        private float? height;

        private float? width;

        private float renderedImageHeight;

        private float renderedImageWidth;

        private bool doesObjectFitRequireCutting;

        private Rectangle initialOccupiedAreaBBox;

        private float rotatedDeltaX;

        private float rotatedDeltaY;

        /// <summary>Creates an ImageRenderer from its corresponding layout object.</summary>
        /// <param name="image">
        /// the
        /// <see cref="iText.Layout.Element.Image"/>
        /// which this object should manage
        /// </param>
        public ImageRenderer(Image image)
            : base(image) {
            imageWidth = image.GetImageWidth();
            imageHeight = image.GetImageHeight();
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            LayoutArea area = layoutContext.GetArea().Clone();
            Rectangle layoutBox = area.GetBBox().Clone();
            AffineTransform t = new AffineTransform();
            Image modelElement = (Image)(GetModelElement());
            PdfXObject xObject = modelElement.GetXObject();
            CalculateImageDimensions(layoutBox, t, xObject);
            OverflowPropertyValue? overflowX = null != parent ? parent.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X
                ) : OverflowPropertyValue.FIT;
            bool nowrap = false;
            if (parent is LineRenderer) {
                nowrap = true.Equals(this.parent.GetOwnProperty<bool?>(Property.NO_SOFT_WRAP_INLINE));
            }
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            float clearHeightCorrection = FloatingHelper.CalculateClearHeightCorrection(this, floatRendererAreas, layoutBox
                );
            FloatPropertyValue? floatPropertyValue = this.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            if (FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                layoutBox.DecreaseHeight(clearHeightCorrection);
                FloatingHelper.AdjustFloatedBlockLayoutBox(this, layoutBox, width, floatRendererAreas, floatPropertyValue, 
                    overflowX);
            }
            else {
                clearHeightCorrection = FloatingHelper.AdjustLayoutBoxAccordingToFloats(floatRendererAreas, layoutBox, width
                    , clearHeightCorrection, null);
            }
            ApplyMargins(layoutBox, false);
            Border[] borders = GetBorders();
            ApplyBorderBox(layoutBox, borders, false);
            float? declaredMaxHeight = RetrieveMaxHeight();
            OverflowPropertyValue? overflowY = null == parent || ((null == declaredMaxHeight || declaredMaxHeight > layoutBox
                .GetHeight()) && !layoutContext.IsClippedHeight()) ? OverflowPropertyValue.FIT : parent.GetProperty<OverflowPropertyValue?
                >(Property.OVERFLOW_Y);
            bool processOverflowX = !IsOverflowFit(overflowX) || nowrap;
            bool processOverflowY = !IsOverflowFit(overflowY);
            if (IsAbsolutePosition()) {
                ApplyAbsolutePosition(layoutBox);
            }
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), 0, 0));
            TargetCounterHandler.AddPageByID(this);
            float imageContainerWidth = (float)width;
            float imageContainerHeight = (float)height;
            if (IsFixedLayout()) {
                fixedXPosition = this.GetPropertyAsFloat(Property.LEFT);
                fixedYPosition = this.GetPropertyAsFloat(Property.BOTTOM);
            }
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            // See in adjustPositionAfterRotation why angle = 0 is necessary
            if (null == angle) {
                angle = 0f;
            }
            t.Rotate((float)angle);
            initialOccupiedAreaBBox = GetOccupiedAreaBBox().Clone();
            float scaleCoef = AdjustPositionAfterRotation((float)angle, layoutBox.GetWidth(), layoutBox.GetHeight());
            imageContainerHeight *= scaleCoef;
            imageContainerWidth *= scaleCoef;
            initialOccupiedAreaBBox.MoveDown(imageContainerHeight);
            initialOccupiedAreaBBox.SetHeight(imageContainerHeight);
            initialOccupiedAreaBBox.SetWidth(imageContainerWidth);
            if (xObject is PdfFormXObject) {
                t.Scale(scaleCoef, scaleCoef);
            }
            float imageItselfWidth;
            float imageItselfHeight;
            ApplyObjectFit(modelElement.GetObjectFit(), imageWidth, imageHeight);
            if (modelElement.GetObjectFit() == ObjectFit.FILL) {
                imageItselfWidth = imageContainerWidth;
                imageItselfHeight = imageContainerHeight;
            }
            else {
                imageItselfWidth = renderedImageWidth;
                imageItselfHeight = renderedImageHeight;
            }
            GetMatrix(t, imageItselfWidth, imageItselfHeight);
            // indicates whether the placement is forced
            bool isPlacingForced = false;
            if (width > layoutBox.GetWidth() + EPS || height > layoutBox.GetHeight() + EPS) {
                if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                    isPlacingForced = true;
                }
                else {
                    isPlacingForced = true;
                    if (width > layoutBox.GetWidth() + EPS) {
                        isPlacingForced &= processOverflowX;
                    }
                    if (height > layoutBox.GetHeight() + EPS) {
                        isPlacingForced &= processOverflowY;
                    }
                }
                if (!isPlacingForced) {
                    ApplyMargins(initialOccupiedAreaBBox, true);
                    ApplyBorderBox(initialOccupiedAreaBBox, true);
                    occupiedArea.GetBBox().SetHeight(initialOccupiedAreaBBox.GetHeight());
                    return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this);
                }
            }
            occupiedArea.GetBBox().MoveDown((float)height);
            if (borders[3] != null) {
                float delta = (float)Math.Sin((float)angle) * borders[3].GetWidth();
                float renderScaling = renderedImageHeight / (float)height;
                height += delta;
                renderedImageHeight += delta * renderScaling;
            }
            occupiedArea.GetBBox().SetHeight((float)height);
            occupiedArea.GetBBox().SetWidth((float)width);
            UnitValue leftMargin = this.GetPropertyAsUnitValue(Property.MARGIN_LEFT);
            if (!leftMargin.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ImageRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_LEFT));
            }
            UnitValue topMargin = this.GetPropertyAsUnitValue(Property.MARGIN_TOP);
            if (!topMargin.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ImageRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_TOP));
            }
            if (0 != leftMargin.GetValue() || 0 != topMargin.GetValue()) {
                TranslateImage(leftMargin.GetValue(), topMargin.GetValue(), t);
                GetMatrix(t, imageContainerWidth, imageContainerHeight);
            }
            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
            ApplyMargins(occupiedArea.GetBBox(), true);
            if (angle != 0) {
                ApplyRotationLayout((float)angle);
            }
            float unscaledWidth = occupiedArea.GetBBox().GetWidth() / scaleCoef;
            MinMaxWidth minMaxWidth = new MinMaxWidth(unscaledWidth, unscaledWidth, 0);
            UnitValue rendererWidth = this.GetProperty<UnitValue>(Property.WIDTH);
            if (rendererWidth != null && rendererWidth.IsPercentValue()) {
                minMaxWidth.SetChildrenMinWidth(0);
                float coeff = imageWidth / (float)RetrieveWidth(area.GetBBox().GetWidth());
                minMaxWidth.SetChildrenMaxWidth(unscaledWidth * coeff);
            }
            else {
                bool autoScale = HasProperty(Property.AUTO_SCALE) && (bool)this.GetProperty<bool?>(Property.AUTO_SCALE);
                bool autoScaleWidth = HasProperty(Property.AUTO_SCALE_WIDTH) && (bool)this.GetProperty<bool?>(Property.AUTO_SCALE_WIDTH
                    );
                if (autoScale || autoScaleWidth) {
                    minMaxWidth.SetChildrenMinWidth(0);
                }
            }
            FloatingHelper.RemoveFloatsAboveRendererBottom(floatRendererAreas, this);
            LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, floatRendererAreas, 
                layoutContext.GetArea().GetBBox(), clearHeightCorrection, false);
            ApplyAbsolutePositionIfNeeded(layoutContext);
            return new MinMaxWidthLayoutResult(LayoutResult.FULL, editedArea, null, null, isPlacingForced ? this : null
                ).SetMinMaxWidth(minMaxWidth);
        }

        public override void Draw(DrawContext drawContext) {
            if (occupiedArea == null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ImageRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Drawing won't be performed."));
                return;
            }
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            bool isTagged = drawContext.IsTaggingEnabled();
            LayoutTaggingHelper taggingHelper = null;
            bool isArtifact = false;
            TagTreePointer tagPointer = null;
            if (isTagged) {
                taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper == null) {
                    isArtifact = true;
                }
                else {
                    isArtifact = taggingHelper.IsArtifact(this);
                    if (!isArtifact) {
                        tagPointer = taggingHelper.UseAutoTaggingPointerAndRememberItsPosition(this);
                        if (taggingHelper.CreateTag(this, tagPointer)) {
                            tagPointer.GetProperties().AddAttributes(0, AccessibleAttributesApplier.GetLayoutAttributes(this, tagPointer
                                ));
                        }
                    }
                }
            }
            BeginTransformationIfApplied(drawContext.GetCanvas());
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (angle != null) {
                drawContext.GetCanvas().SaveState();
                ApplyConcatMatrix(drawContext, angle);
            }
            base.Draw(drawContext);
            bool clipImageInAViewOfBorderRadius = ClipBackgroundArea(drawContext, ApplyMargins(GetOccupiedAreaBBox(), 
                false), true);
            ApplyMargins(occupiedArea.GetBBox(), false);
            ApplyBorderBox(occupiedArea.GetBBox(), GetBorders(), false);
            if (fixedYPosition == null) {
                fixedYPosition = occupiedArea.GetBBox().GetY() + pivotY;
            }
            if (fixedXPosition == null) {
                fixedXPosition = occupiedArea.GetBBox().GetX();
            }
            if (angle != null) {
                fixedXPosition += rotatedDeltaX;
                fixedYPosition -= rotatedDeltaY;
                drawContext.GetCanvas().RestoreState();
            }
            PdfCanvas canvas = drawContext.GetCanvas();
            if (isTagged) {
                if (isArtifact) {
                    canvas.OpenTag(new CanvasArtifact());
                }
                else {
                    canvas.OpenTag(tagPointer.GetTagReference());
                }
            }
            BeginObjectFitImageClipping(canvas);
            PdfXObject xObject = ((Image)(GetModelElement())).GetXObject();
            BeginElementOpacityApplying(drawContext);
            float renderedImageShiftX = ((float)width - renderedImageWidth) / 2;
            float renderedImageShiftY = ((float)height - renderedImageHeight) / 2;
            canvas.AddXObjectWithTransformationMatrix(xObject, matrix[0], matrix[1], matrix[2], matrix[3], (float)fixedXPosition
                 + deltaX + renderedImageShiftX, (float)fixedYPosition + renderedImageShiftY);
            EndElementOpacityApplying(drawContext);
            EndObjectFitImageClipping(canvas);
            EndTransformationIfApplied(drawContext.GetCanvas());
            if (true.Equals(GetPropertyAsBoolean(Property.FLUSH_ON_DRAW))) {
                xObject.Flush();
            }
            if (isTagged) {
                canvas.CloseTag();
            }
            if (clipImageInAViewOfBorderRadius) {
                canvas.RestoreState();
            }
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(true);
            }
            ApplyBorderBox(occupiedArea.GetBBox(), GetBorders(), true);
            ApplyMargins(occupiedArea.GetBBox(), true);
            if (isTagged && !isArtifact) {
                taggingHelper.FinishTaggingHint(this);
                taggingHelper.RestoreAutoTaggingPointerPosition(this);
            }
        }

        public override IRenderer GetNextRenderer() {
            return null;
        }

        public override Rectangle GetBorderAreaBBox() {
            ApplyMargins(initialOccupiedAreaBBox, false);
            ApplyBorderBox(initialOccupiedAreaBBox, GetBorders(), false);
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            ApplyMargins(initialOccupiedAreaBBox, true);
            ApplyBorderBox(initialOccupiedAreaBBox, true);
            return initialOccupiedAreaBBox;
        }

        /// <summary><inheritDoc/></summary>
        internal override bool HasAspectRatio() {
            return true;
        }

        /// <summary><inheritDoc/></summary>
        internal override float? GetAspectRatio() {
            return imageWidth / imageHeight;
        }

        /// <summary>
        /// Gets original width of the image, not the width set by
        /// <see cref="iText.Layout.Element.Image.SetWidth(float)"/>
        /// method.
        /// </summary>
        /// <returns>original image width</returns>
        public virtual float GetImageWidth() {
            return imageWidth;
        }

        /// <summary>
        /// Gets original height of the image, not the height set by
        /// <see cref="iText.Layout.Element.Image.SetHeight(float)"/>
        /// method.
        /// </summary>
        /// <returns>original image height</returns>
        public virtual float GetImageHeight() {
            return imageHeight;
        }

        protected internal override Rectangle ApplyPaddings(Rectangle rect, UnitValue[] paddings, bool reverse) {
            return rect;
        }

        public override void Move(float dxRight, float dyUp) {
            base.Move(dxRight, dyUp);
            if (initialOccupiedAreaBBox != null) {
                initialOccupiedAreaBBox.MoveRight(dxRight);
                initialOccupiedAreaBBox.MoveUp(dyUp);
            }
            if (fixedXPosition != null) {
                fixedXPosition += dxRight;
            }
            if (fixedYPosition != null) {
                fixedYPosition += dyUp;
            }
        }

        public override MinMaxWidth GetMinMaxWidth() {
            return ((MinMaxWidthLayoutResult)Layout(new LayoutContext(new LayoutArea(1, new Rectangle(MinMaxWidthUtils
                .GetInfWidth(), AbstractRenderer.INF))))).GetMinMaxWidth();
        }

        protected internal virtual iText.Layout.Renderer.ImageRenderer AutoScale(LayoutArea layoutArea) {
            Rectangle area = layoutArea.GetBBox().Clone();
            ApplyMargins(area, false);
            ApplyBorderBox(area, false);
            // if rotation was applied, width would be equal to the width of rectangle bounding the rotated image
            float angleScaleCoef = imageWidth / (float)width;
            if (width > angleScaleCoef * area.GetWidth()) {
                UpdateHeight(UnitValue.CreatePointValue(area.GetWidth() / (float)width * imageHeight));
                UpdateWidth(UnitValue.CreatePointValue(angleScaleCoef * area.GetWidth()));
            }
            return this;
        }

        private void ApplyObjectFit(ObjectFit objectFit, float imageWidth, float imageHeight) {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(objectFit, imageWidth, imageHeight
                , (float)width, (float)height);
            renderedImageWidth = (float)result.GetRenderedImageWidth();
            renderedImageHeight = (float)result.GetRenderedImageHeight();
            doesObjectFitRequireCutting = result.IsImageCuttingRequired();
        }

        private void BeginObjectFitImageClipping(PdfCanvas canvas) {
            if (doesObjectFitRequireCutting) {
                canvas.SaveState();
                Rectangle clippedArea = new Rectangle((float)fixedXPosition, (float)fixedYPosition, (float)width, (float)height
                    );
                canvas.Rectangle(clippedArea).Clip().EndPath();
            }
        }

        private void EndObjectFitImageClipping(PdfCanvas canvas) {
            if (doesObjectFitRequireCutting) {
                canvas.RestoreState();
            }
        }

        private void CalculateImageDimensions(Rectangle layoutBox, AffineTransform t, PdfXObject xObject) {
            width = this.GetProperty<UnitValue>(Property.WIDTH) != null ? RetrieveWidth(layoutBox.GetWidth()) : null;
            float? declaredHeight = RetrieveHeight();
            height = declaredHeight;
            if (width == null && height == null) {
                width = imageWidth;
                height = (float)width / imageWidth * imageHeight;
            }
            else {
                if (width == null) {
                    width = (float)height / imageHeight * imageWidth;
                }
                else {
                    if (height == null) {
                        height = (float)width / imageWidth * imageHeight;
                    }
                }
            }
            float? horizontalScaling = this.GetPropertyAsFloat(Property.HORIZONTAL_SCALING, 1f);
            float? verticalScaling = this.GetPropertyAsFloat(Property.VERTICAL_SCALING, 1f);
            if (xObject is PdfFormXObject && width != imageWidth) {
                horizontalScaling *= width / imageWidth;
                verticalScaling *= height / imageHeight;
            }
            if (horizontalScaling != 1) {
                if (xObject is PdfFormXObject) {
                    t.Scale((float)horizontalScaling, 1);
                    width = imageWidth * (float)horizontalScaling;
                }
                else {
                    width *= (float)horizontalScaling;
                }
            }
            if (verticalScaling != 1) {
                if (xObject is PdfFormXObject) {
                    t.Scale(1, (float)verticalScaling);
                    height = imageHeight * (float)verticalScaling;
                }
                else {
                    height *= (float)verticalScaling;
                }
            }
            // Constrain width and height according to min/max width
            float? minWidth = RetrieveMinWidth(layoutBox.GetWidth());
            float? maxWidth = RetrieveMaxWidth(layoutBox.GetWidth());
            if (null != minWidth && width < minWidth) {
                height *= minWidth / width;
                width = minWidth;
            }
            else {
                if (null != maxWidth && width > maxWidth) {
                    height *= maxWidth / width;
                    width = maxWidth;
                }
            }
            // Constrain width and height according to min/max height, which has precedence over width settings
            float? minHeight = RetrieveMinHeight();
            float? maxHeight = RetrieveMaxHeight();
            if (null != minHeight && height < minHeight) {
                width *= minHeight / height;
                height = minHeight;
            }
            else {
                if (null != maxHeight && height > maxHeight) {
                    width *= maxHeight / height;
                    this.height = maxHeight;
                }
                else {
                    if (null != declaredHeight && !height.Equals(declaredHeight)) {
                        width *= declaredHeight / height;
                        height = declaredHeight;
                    }
                }
            }
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
                Point p01 = t.Transform(new Point(0, (float)height), new Point());
                Point p10 = t.Transform(new Point((float)width, 0), new Point());
                Point p11 = t.Transform(new Point((float)width, (float)height), new Point());
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
            float scaleCoeff = 1;
            if (true.Equals(GetPropertyAsBoolean(Property.AUTO_SCALE))) {
                if (maxWidth / (float)width < maxHeight / (float)height) {
                    scaleCoeff = maxWidth / (float)width;
                    height *= maxWidth / (float)width;
                    width = maxWidth;
                }
                else {
                    scaleCoeff = maxHeight / (float)height;
                    width *= maxHeight / (float)height;
                    height = maxHeight;
                }
            }
            else {
                if (true.Equals(GetPropertyAsBoolean(Property.AUTO_SCALE_WIDTH))) {
                    scaleCoeff = maxWidth / (float)width;
                    height *= scaleCoeff;
                    width = maxWidth;
                }
                else {
                    if (true.Equals(GetPropertyAsBoolean(Property.AUTO_SCALE_HEIGHT))) {
                        scaleCoeff = maxHeight / (float)height;
                        height = maxHeight;
                        width *= scaleCoeff;
                    }
                }
            }
            pivotY *= scaleCoeff;
            deltaX *= scaleCoeff;
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

        private void ApplyConcatMatrix(DrawContext drawContext, float? angle) {
            AffineTransform rotationTransform = AffineTransform.GetRotateInstance((float)angle);
            Rectangle rect = GetBorderAreaBBox();
            IList<Point> rotatedPoints = TransformPoints(RectangleToPointsList(rect), rotationTransform);
            float[] shift = CalculateShiftToPositionBBoxOfPointsAt(rect.GetX(), rect.GetY() + rect.GetHeight(), rotatedPoints
                );
            double[] matrix = new double[6];
            rotationTransform.GetMatrix(matrix);
            drawContext.GetCanvas().ConcatMatrix(matrix[0], matrix[1], matrix[2], matrix[3], shift[0], shift[1]);
        }

        private void ApplyRotationLayout(float angle) {
            Border[] borders = GetBorders();
            Rectangle rect = GetBorderAreaBBox();
            float leftBorderWidth = borders[3] == null ? 0 : borders[3].GetWidth();
            float rightBorderWidth = borders[1] == null ? 0 : borders[1].GetWidth();
            float topBorderWidth = borders[0] == null ? 0 : borders[0].GetWidth();
            if (leftBorderWidth != 0) {
                float gip = (float)Math.Sqrt(Math.Pow(topBorderWidth, 2) + Math.Pow(leftBorderWidth, 2));
                double atan = Math.Atan(topBorderWidth / leftBorderWidth);
                if (angle < 0) {
                    atan = -atan;
                }
                rotatedDeltaX = Math.Abs((float)(gip * Math.Cos(angle - atan) - leftBorderWidth));
            }
            else {
                rotatedDeltaX = 0;
            }
            rect.MoveRight(rotatedDeltaX);
            occupiedArea.GetBBox().SetWidth(occupiedArea.GetBBox().GetWidth() + rotatedDeltaX);
            if (rightBorderWidth != 0) {
                float gip = (float)Math.Sqrt(Math.Pow(topBorderWidth, 2) + Math.Pow(leftBorderWidth, 2));
                double atan = Math.Atan(rightBorderWidth / topBorderWidth);
                if (angle < 0) {
                    atan = -atan;
                }
                rotatedDeltaY = Math.Abs((float)(gip * Math.Cos(angle - atan) - topBorderWidth));
            }
            else {
                rotatedDeltaY = 0;
            }
            rect.MoveDown(rotatedDeltaY);
            if (angle < 0) {
                rotatedDeltaY += rightBorderWidth;
            }
            occupiedArea.GetBBox().IncreaseHeight(rotatedDeltaY);
        }

        public virtual float GetAscent() {
            return occupiedArea.GetBBox().GetHeight();
        }

        public virtual float GetDescent() {
            return 0;
        }
    }
}
