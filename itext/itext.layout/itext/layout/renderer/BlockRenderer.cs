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
using System.Collections.Generic;
using iText.IO;
using iText.IO.Log;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public abstract class BlockRenderer : AbstractRenderer {
        protected internal BlockRenderer(IElement modelElement)
            : base(modelElement) {
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            int pageNumber = layoutContext.GetArea().GetPageNumber();
            bool isPositioned = IsPositioned();
            Rectangle parentBBox = layoutContext.GetArea().GetBBox().Clone();
            if (this.GetProperty<float?>(Property.ROTATION_ANGLE) != null || isPositioned) {
                parentBBox.MoveDown(AbstractRenderer.INF - parentBBox.GetHeight()).SetHeight(AbstractRenderer.INF);
            }
            float? blockHeight = RetrieveHeight();
            if (!IsFixedLayout() && blockHeight != null && blockHeight > parentBBox.GetHeight() && !true.Equals(GetPropertyAsBoolean
                (Property.FORCED_PLACEMENT))) {
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, this);
            }
            float[] margins = GetMargins();
            ApplyMargins(parentBBox, margins, false);
            Border[] borders = GetBorders();
            ApplyBorderBox(parentBBox, borders, false);
            if (isPositioned) {
                float x = (float)this.GetPropertyAsFloat(Property.X);
                float relativeX = IsFixedLayout() ? 0 : parentBBox.GetX();
                parentBBox.SetX(relativeX + x);
            }
            float? blockWidth = RetrieveWidth(parentBBox.GetWidth());
            if (blockWidth != null && (blockWidth < parentBBox.GetWidth() || isPositioned)) {
                parentBBox.SetWidth((float)blockWidth);
            }
            float[] paddings = GetPaddings();
            ApplyPaddings(parentBBox, paddings, false);
            IList<Rectangle> areas;
            if (isPositioned) {
                areas = JavaCollectionsUtil.SingletonList(parentBBox);
            }
            else {
                areas = InitElementAreas(new LayoutArea(pageNumber, parentBBox));
            }
            occupiedArea = new LayoutArea(pageNumber, new Rectangle(parentBBox.GetX(), parentBBox.GetY() + parentBBox.
                GetHeight(), parentBBox.GetWidth(), 0));
            int currentAreaPos = 0;
            Rectangle layoutBox = areas[0].Clone();
            // the first renderer (one of childRenderers or their children) to produce LayoutResult.NOTHING
            IRenderer causeOfNothing = null;
            bool anythingPlaced = false;
            for (int childPos = 0; childPos < childRenderers.Count; childPos++) {
                IRenderer childRenderer = childRenderers[childPos];
                LayoutResult result;
                childRenderer.SetParent(this);
                while ((result = childRenderer.SetParent(this).Layout(new LayoutContext(new LayoutArea(pageNumber, layoutBox
                    )))).GetStatus() != LayoutResult.FULL) {
                    if (result.GetOccupiedArea() != null) {
                        occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), result.GetOccupiedArea().GetBBox
                            ()));
                        layoutBox.SetHeight(layoutBox.GetHeight() - result.GetOccupiedArea().GetBBox().GetHeight());
                    }
                    if (childRenderer.GetProperty<Object>(Property.WIDTH) != null) {
                        AlignChildHorizontally(childRenderer, layoutBox.GetWidth());
                    }
                    // Save the first renderer to produce LayoutResult.NOTHING
                    if (null == causeOfNothing && null != result.GetCauseOfNothing()) {
                        causeOfNothing = result.GetCauseOfNothing();
                    }
                    // have more areas
                    if (currentAreaPos + 1 < areas.Count) {
                        if (result.GetStatus() == LayoutResult.PARTIAL) {
                            childRenderers[childPos] = result.GetSplitRenderer();
                            // TODO linkedList would make it faster
                            childRenderers.Add(childPos + 1, result.GetOverflowRenderer());
                        }
                        else {
                            childRenderers[childPos] = result.GetOverflowRenderer();
                            childPos--;
                        }
                        layoutBox = areas[++currentAreaPos].Clone();
                        break;
                    }
                    else {
                        if (result.GetStatus() == LayoutResult.PARTIAL) {
                            layoutBox.SetHeight(layoutBox.GetHeight() - result.GetOccupiedArea().GetBBox().GetHeight());
                            if (currentAreaPos + 1 == areas.Count) {
                                AbstractRenderer splitRenderer = CreateSplitRenderer(LayoutResult.PARTIAL);
                                splitRenderer.childRenderers = new List<IRenderer>(childRenderers.SubList(0, childPos));
                                splitRenderer.childRenderers.Add(result.GetSplitRenderer());
                                splitRenderer.occupiedArea = occupiedArea;
                                AbstractRenderer overflowRenderer = CreateOverflowRenderer(LayoutResult.PARTIAL);
                                // Apply forced placement only on split renderer
                                overflowRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
                                IList<IRenderer> overflowRendererChildren = new List<IRenderer>();
                                overflowRendererChildren.Add(result.GetOverflowRenderer());
                                overflowRendererChildren.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
                                overflowRenderer.childRenderers = overflowRendererChildren;
                                ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                                ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                                ApplyMargins(occupiedArea.GetBBox(), margins, true);
                                return new LayoutResult(LayoutResult.PARTIAL, occupiedArea, splitRenderer, overflowRenderer, causeOfNothing
                                    );
                            }
                            else {
                                childRenderers[childPos] = result.GetSplitRenderer();
                                childRenderers.Add(childPos + 1, result.GetOverflowRenderer());
                                layoutBox = areas[++currentAreaPos].Clone();
                                break;
                            }
                        }
                        else {
                            if (result.GetStatus() == LayoutResult.NOTHING) {
                                bool keepTogether = IsKeepTogether();
                                int layoutResult = anythingPlaced && !keepTogether ? LayoutResult.PARTIAL : LayoutResult.NOTHING;
                                AbstractRenderer splitRenderer = CreateSplitRenderer(layoutResult);
                                splitRenderer.childRenderers = new List<IRenderer>(childRenderers.SubList(0, childPos));
                                foreach (IRenderer renderer in splitRenderer.childRenderers) {
                                    renderer.SetParent(splitRenderer);
                                }
                                AbstractRenderer overflowRenderer = CreateOverflowRenderer(layoutResult);
                                IList<IRenderer> overflowRendererChildren = new List<IRenderer>();
                                overflowRendererChildren.Add(result.GetOverflowRenderer());
                                overflowRendererChildren.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
                                overflowRenderer.childRenderers = overflowRendererChildren;
                                if (keepTogether) {
                                    splitRenderer = null;
                                    overflowRenderer.childRenderers.Clear();
                                    overflowRenderer.childRenderers = new List<IRenderer>(childRenderers);
                                }
                                ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                                ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                                ApplyMargins(occupiedArea.GetBBox(), margins, true);
                                if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                                    return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null);
                                }
                                else {
                                    return new LayoutResult(layoutResult, occupiedArea, splitRenderer, overflowRenderer, LayoutResult.NOTHING 
                                        == layoutResult ? result.GetCauseOfNothing() : null);
                                }
                            }
                        }
                    }
                }
                anythingPlaced = true;
                occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), result.GetOccupiedArea().GetBBox
                    ()));
                if (result.GetStatus() == LayoutResult.FULL) {
                    layoutBox.SetHeight(layoutBox.GetHeight() - result.GetOccupiedArea().GetBBox().GetHeight());
                    if (childRenderer.GetProperty<Object>(Property.WIDTH) != null) {
                        AlignChildHorizontally(childRenderer, layoutBox.GetWidth());
                    }
                }
                // Save the first renderer to produce LayoutResult.NOTHING
                if (null == causeOfNothing && null != result.GetCauseOfNothing()) {
                    causeOfNothing = result.GetCauseOfNothing();
                }
            }
            ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
            if (blockHeight != null && blockHeight > occupiedArea.GetBBox().GetHeight()) {
                occupiedArea.GetBBox().MoveDown((float)blockHeight - occupiedArea.GetBBox().GetHeight()).SetHeight((float)
                    blockHeight);
            }
            if (isPositioned) {
                float y = (float)this.GetPropertyAsFloat(Property.Y);
                float relativeY = IsFixedLayout() ? 0 : layoutBox.GetY();
                Move(0, relativeY + y - occupiedArea.GetBBox().GetY());
            }
            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
            ApplyMargins(occupiedArea.GetBBox(), margins, true);
            if (this.GetProperty<float?>(Property.ROTATION_ANGLE) != null) {
                ApplyRotationLayout(layoutContext.GetArea().GetBBox().Clone());
                if (IsNotFittingHeight(layoutContext.GetArea())) {
                    if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                        return new LayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this);
                    }
                }
            }
            ApplyVerticalAlignment();
            return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null, causeOfNothing);
        }

        protected internal virtual AbstractRenderer CreateSplitRenderer(int layoutResult) {
            AbstractRenderer splitRenderer = (AbstractRenderer)GetNextRenderer();
            splitRenderer.parent = parent;
            splitRenderer.modelElement = modelElement;
            splitRenderer.occupiedArea = occupiedArea;
            splitRenderer.isLastRendererForModelElement = false;
            return splitRenderer;
        }

        protected internal virtual AbstractRenderer CreateOverflowRenderer(int layoutResult) {
            AbstractRenderer overflowRenderer = (AbstractRenderer)GetNextRenderer();
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            overflowRenderer.properties = new Dictionary<int, Object>(properties);
            return overflowRenderer;
        }

        public override void Draw(DrawContext drawContext) {
            PdfDocument document = drawContext.GetDocument();
            ApplyDestination(document);
            ApplyAction(document);
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            TagTreePointer tagPointer = null;
            IAccessibleElement accessibleElement = null;
            if (isTagged) {
                accessibleElement = (IAccessibleElement)GetModelElement();
                PdfName role = accessibleElement.GetRole();
                if (role != null && !PdfName.Artifact.Equals(role)) {
                    tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                    if (!tagPointer.IsElementConnectedToTag(accessibleElement)) {
                        AccessibleAttributesApplier.ApplyLayoutAttributes(role, this, document);
                        if (role.Equals(PdfName.TD)) {
                            AccessibleAttributesApplier.ApplyTableAttributes(this);
                        }
                        if (role.Equals(PdfName.List)) {
                            AccessibleAttributesApplier.ApplyListAttributes(this);
                        }
                    }
                    tagPointer.AddTag(accessibleElement, true);
                }
                else {
                    isTagged = false;
                }
            }
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyAbsolutePositioningTranslation(false);
            }
            BeginRotationIfApplied(drawContext.GetCanvas());
            DrawBackground(drawContext);
            DrawBorder(drawContext);
            DrawChildren(drawContext);
            EndRotationIfApplied(drawContext.GetCanvas());
            if (isRelativePosition) {
                ApplyAbsolutePositioningTranslation(true);
            }
            if (isTagged) {
                tagPointer.MoveToParent();
                if (isLastRendererForModelElement) {
                    document.GetTagStructureContext().RemoveElementConnectionToTag(accessibleElement);
                }
            }
            flushed = true;
        }

        public override Rectangle GetOccupiedAreaBBox() {
            Rectangle bBox = occupiedArea.GetBBox().Clone();
            float? rotationAngle = this.GetProperty<float?>(Property.ROTATION_ANGLE);
            if (rotationAngle != null) {
                if (!HasOwnProperty(Property.ROTATION_INITIAL_HEIGHT) || !HasOwnProperty(Property.ROTATION_INITIAL_HEIGHT)
                    ) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.BlockRenderer));
                    logger.Error(String.Format(LogMessageConstant.ROTATION_WAS_NOT_CORRECTLY_PROCESSED_FOR_RENDERER, GetType()
                        .Name));
                }
                else {
                    bBox.SetWidth((float)this.GetPropertyAsFloat(Property.ROTATION_INITIAL_WIDTH));
                    bBox.SetHeight((float)this.GetPropertyAsFloat(Property.ROTATION_INITIAL_HEIGHT));
                }
            }
            return bBox;
        }

        protected internal virtual void ApplyVerticalAlignment() {
            VerticalAlignment? verticalAlignment = this.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT);
            if (verticalAlignment != null && verticalAlignment != VerticalAlignment.TOP && childRenderers.Count > 0) {
                float deltaY = childRenderers[childRenderers.Count - 1].GetOccupiedArea().GetBBox().GetY() - GetInnerAreaBBox
                    ().GetY();
                switch (verticalAlignment) {
                    case VerticalAlignment.BOTTOM: {
                        foreach (IRenderer child in childRenderers) {
                            child.Move(0, -deltaY);
                        }
                        break;
                    }

                    case VerticalAlignment.MIDDLE: {
                        foreach (IRenderer child_1 in childRenderers) {
                            child_1.Move(0, -deltaY / 2);
                        }
                        break;
                    }
                }
            }
        }

        protected internal virtual void ApplyRotationLayout(Rectangle layoutBox) {
            float angle = (float)this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            float x = occupiedArea.GetBBox().GetX();
            float y = occupiedArea.GetBBox().GetY();
            float height = occupiedArea.GetBBox().GetHeight();
            float width = occupiedArea.GetBBox().GetWidth();
            SetProperty(Property.ROTATION_INITIAL_WIDTH, width);
            SetProperty(Property.ROTATION_INITIAL_HEIGHT, height);
            AffineTransform rotationTransform = new AffineTransform();
            // here we calculate and set the actual occupied area of the rotated content
            if (IsPositioned()) {
                float? rotationPointX = this.GetPropertyAsFloat(Property.ROTATION_POINT_X);
                float? rotationPointY = this.GetPropertyAsFloat(Property.ROTATION_POINT_Y);
                if (rotationPointX == null || rotationPointY == null) {
                    // if rotation point was not specified, the most bottom-left point is used
                    rotationPointX = x;
                    rotationPointY = y;
                }
                // transforms apply from bottom to top
                rotationTransform.Translate((float)rotationPointX, (float)rotationPointY);
                // move point back at place
                rotationTransform.Rotate(angle);
                // rotate
                rotationTransform.Translate((float)-rotationPointX, (float)-rotationPointY);
                // move rotation point to origin
                IList<Point> rotatedPoints = TransformPoints(RectangleToPointsList(occupiedArea.GetBBox()), rotationTransform
                    );
                Rectangle newBBox = CalculateBBox(rotatedPoints);
                // make occupied area be of size and position of actual content
                occupiedArea.GetBBox().SetWidth(newBBox.GetWidth());
                occupiedArea.GetBBox().SetHeight(newBBox.GetHeight());
                float occupiedAreaShiftX = newBBox.GetX() - x;
                float occupiedAreaShiftY = newBBox.GetY() - y;
                Move(occupiedAreaShiftX, occupiedAreaShiftY);
            }
            else {
                rotationTransform = AffineTransform.GetRotateInstance(angle);
                IList<Point> rotatedPoints = TransformPoints(RectangleToPointsList(occupiedArea.GetBBox()), rotationTransform
                    );
                float[] shift = CalculateShiftToPositionBBoxOfPointsAt(x, y + height, rotatedPoints);
                foreach (Point point in rotatedPoints) {
                    point.SetLocation(point.GetX() + shift[0], point.GetY() + shift[1]);
                }
                // clip bounding box on the right side to make it fit in the layout area width
                Point clipLineBeg = new Point(layoutBox.GetRight(), layoutBox.GetTop());
                Point clipLineEnd = new Point(layoutBox.GetRight(), layoutBox.GetBottom());
                IList<Point> newOccupiedAreaPoints = ClipPolygon(rotatedPoints, clipLineBeg, clipLineEnd);
                Rectangle newBBox = CalculateBBox(newOccupiedAreaPoints);
                occupiedArea.GetBBox().SetWidth(newBBox.GetWidth());
                occupiedArea.GetBBox().SetHeight(newBBox.GetHeight());
                float heightDiff = height - newBBox.GetHeight();
                Move(0, heightDiff);
            }
        }

        [System.ObsoleteAttribute(@"Will be removed in iText 7.1")]
        protected internal virtual float[] ApplyRotation() {
            float[] ctm = new float[6];
            CreateRotationTransformInsideOccupiedArea().GetMatrix(ctm);
            return ctm;
        }

        /// <summary>
        /// This method creates
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// instance that could be used
        /// to rotate content inside the occupied area. Be aware that it should be used only after
        /// layout rendering is finished and correct occupied area for the rotated element is calculated.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// that rotates the content and places it inside occupied area.
        /// </returns>
        protected internal virtual AffineTransform CreateRotationTransformInsideOccupiedArea() {
            float? angle = this.GetProperty<float?>(Property.ROTATION_ANGLE);
            AffineTransform rotationTransform = AffineTransform.GetRotateInstance((float)angle);
            Rectangle contentBox = this.GetOccupiedAreaBBox();
            IList<Point> rotatedContentBoxPoints = TransformPoints(RectangleToPointsList(contentBox), rotationTransform
                );
            // Occupied area for rotated elements is already calculated on layout in such way to enclose rotated content;
            // therefore we can simply rotate content as is and then shift it to the occupied area.
            float[] shift = CalculateShiftToPositionBBoxOfPointsAt(occupiedArea.GetBBox().GetLeft(), occupiedArea.GetBBox
                ().GetTop(), rotatedContentBoxPoints);
            rotationTransform.PreConcatenate(AffineTransform.GetTranslateInstance(shift[0], shift[1]));
            return rotationTransform;
        }

        protected internal virtual void BeginRotationIfApplied(PdfCanvas canvas) {
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (angle != null) {
                if (!HasOwnProperty(Property.ROTATION_INITIAL_HEIGHT)) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.BlockRenderer));
                    logger.Error(String.Format(LogMessageConstant.ROTATION_WAS_NOT_CORRECTLY_PROCESSED_FOR_RENDERER, GetType()
                        .Name));
                }
                else {
                    AffineTransform transform = CreateRotationTransformInsideOccupiedArea();
                    canvas.SaveState().ConcatMatrix(transform);
                }
            }
        }

        protected internal virtual void EndRotationIfApplied(PdfCanvas canvas) {
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (angle != null) {
                canvas.RestoreState();
            }
        }

        /// <summary>
        /// This method calculates the shift needed to be applied to the points in order to position
        /// upper and left borders of their bounding box at the given lines.
        /// </summary>
        /// <param name="left">x coordinate at which points bbox left border is to be aligned</param>
        /// <param name="top">y coordinate at which points bbox upper border is to be aligned</param>
        /// <param name="points">the points, which bbox will be aligned at the given position</param>
        /// <returns>
        /// array of two floats, where first element denotes x-coordinate shift and the second
        /// element denotes y-coordinate shift which are needed to align points bbox at the given lines.
        /// </returns>
        private float[] CalculateShiftToPositionBBoxOfPointsAt(float left, float top, IList<Point> points) {
            double minX = double.MaxValue;
            double maxY = -double.MaxValue;
            foreach (Point point in points) {
                minX = Math.Min(point.GetX(), minX);
                maxY = Math.Max(point.GetY(), maxY);
            }
            float dx = (float)(left - minX);
            float dy = (float)(top - maxY);
            return new float[] { dx, dy };
        }

        private IList<Point> ClipPolygon(IList<Point> points, Point clipLineBeg, Point clipLineEnd) {
            IList<Point> filteredPoints = new List<Point>();
            bool prevOnRightSide = false;
            Point filteringPoint = points[0];
            if (CheckPointSide(filteringPoint, clipLineBeg, clipLineEnd) >= 0) {
                filteredPoints.Add(filteringPoint);
                prevOnRightSide = true;
            }
            Point prevPoint = filteringPoint;
            for (int i = 1; i < points.Count + 1; ++i) {
                filteringPoint = points[i % points.Count];
                if (CheckPointSide(filteringPoint, clipLineBeg, clipLineEnd) >= 0) {
                    if (!prevOnRightSide) {
                        filteredPoints.Add(GetIntersectionPoint(prevPoint, filteringPoint, clipLineBeg, clipLineEnd));
                    }
                    filteredPoints.Add(filteringPoint);
                    prevOnRightSide = true;
                }
                else {
                    if (prevOnRightSide) {
                        filteredPoints.Add(GetIntersectionPoint(prevPoint, filteringPoint, clipLineBeg, clipLineEnd));
                    }
                }
                prevPoint = filteringPoint;
            }
            return filteredPoints;
        }

        private int CheckPointSide(Point filteredPoint, Point clipLineBeg, Point clipLineEnd) {
            double x1;
            double x2;
            double y1;
            double y2;
            x1 = filteredPoint.GetX() - clipLineBeg.GetX();
            y2 = clipLineEnd.GetY() - clipLineBeg.GetY();
            x2 = clipLineEnd.GetX() - clipLineBeg.GetX();
            y1 = filteredPoint.GetY() - clipLineBeg.GetY();
            double sgn = x1 * y2 - x2 * y1;
            if (Math.Abs(sgn) < 0.001) {
                return 0;
            }
            if (sgn > 0) {
                return 1;
            }
            if (sgn < 0) {
                return -1;
            }
            return 0;
        }

        private Point GetIntersectionPoint(Point lineBeg, Point lineEnd, Point clipLineBeg, Point clipLineEnd) {
            double A1 = lineBeg.GetY() - lineEnd.GetY();
            double A2 = clipLineBeg.GetY() - clipLineEnd.GetY();
            double B1 = lineEnd.GetX() - lineBeg.GetX();
            double B2 = clipLineEnd.GetX() - clipLineBeg.GetX();
            double C1 = lineBeg.GetX() * lineEnd.GetY() - lineBeg.GetY() * lineEnd.GetX();
            double C2 = clipLineBeg.GetX() * clipLineEnd.GetY() - clipLineBeg.GetY() * clipLineEnd.GetX();
            double M = B1 * A2 - B2 * A1;
            return new Point((B2 * C1 - B1 * C2) / M, (C2 * A1 - C1 * A2) / M);
        }
    }
}
