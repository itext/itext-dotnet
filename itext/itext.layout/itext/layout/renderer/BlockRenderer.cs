/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.IO.Log;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Margincollapse;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public abstract class BlockRenderer : AbstractRenderer {
        protected internal BlockRenderer(IElement modelElement)
            : base(modelElement) {
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            OverrideHeightProperties();
            bool wasHeightClipped = false;
            bool wasParentsHeightClipped = layoutContext.GetArea().IsClippedHeight();
            int pageNumber = layoutContext.GetArea().GetPageNumber();
            bool isPositioned = IsPositioned();
            Rectangle parentBBox = layoutContext.GetArea().GetBBox().Clone();
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            FloatPropertyValue? floatPropertyValue = this.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            float? rotation = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            MarginsCollapseHandler marginsCollapseHandler = null;
            bool marginsCollapsingEnabled = true.Equals(GetPropertyAsBoolean(Property.COLLAPSING_MARGINS));
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler = new MarginsCollapseHandler(this, layoutContext.GetMarginsCollapseInfo());
            }
            float? blockWidth = RetrieveWidth(parentBBox.GetWidth());
            if (rotation != null || IsFixedLayout()) {
                parentBBox.MoveDown(AbstractRenderer.INF - parentBBox.GetHeight()).SetHeight(AbstractRenderer.INF);
            }
            if (rotation != null && !FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                blockWidth = RotationUtils.RetrieveRotatedLayoutWidth(parentBBox.GetWidth(), this);
            }
            float clearHeightCorrection = FloatingHelper.CalculateClearHeightCorrection(this, floatRendererAreas, parentBBox
                );
            FloatingHelper.ApplyClearance(parentBBox, marginsCollapseHandler, clearHeightCorrection, FloatingHelper.IsRendererFloating
                (this));
            if (FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                blockWidth = FloatingHelper.AdjustFloatedBlockLayoutBox(this, parentBBox, blockWidth, floatRendererAreas, 
                    floatPropertyValue);
                floatRendererAreas = new List<Rectangle>();
            }
            bool isCellRenderer = this is CellRenderer;
            if (marginsCollapsingEnabled) {
                if (!isCellRenderer) {
                    marginsCollapseHandler.StartMarginsCollapse(parentBBox);
                }
            }
            Border[] borders = GetBorders();
            float[] paddings = GetPaddings();
            ApplyBordersPaddingsMargins(parentBBox, borders, paddings);
            OverflowPropertyValue? overflowX = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
            float? blockMaxHeight = RetrieveMaxHeight();
            OverflowPropertyValue? overflowY = (null == blockMaxHeight || blockMaxHeight > parentBBox.GetHeight()) && 
                !wasParentsHeightClipped ? OverflowPropertyValue.FIT : this.GetProperty<OverflowPropertyValue?>(Property
                .OVERFLOW_Y);
            if (blockWidth != null && (blockWidth < parentBBox.GetWidth() || isPositioned || rotation != null || (null
                 != overflowX && !OverflowPropertyValue.FIT.Equals(overflowX)))) {
                // TODO DEVSIX-1174
                UnitValue widthVal = this.GetProperty<UnitValue>(Property.WIDTH);
                if (widthVal != null && widthVal.IsPercentValue() && widthVal.GetValue() == 100) {
                }
                else {
                    parentBBox.SetWidth((float)blockWidth);
                }
            }
            if (!IsFixedLayout() && null != blockMaxHeight && (blockMaxHeight < parentBBox.GetHeight() || (null != overflowY
                 && !OverflowPropertyValue.FIT.Equals(overflowY))) && !true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT
                ))) {
                if (blockMaxHeight < parentBBox.GetHeight()) {
                    wasHeightClipped = true;
                }
                float heightDelta = parentBBox.GetHeight() - (float)blockMaxHeight;
                if (marginsCollapsingEnabled && !isCellRenderer) {
                    marginsCollapseHandler.ProcessFixedHeightAdjustment(heightDelta);
                }
                parentBBox.MoveUp(heightDelta).SetHeight((float)blockMaxHeight);
            }
            IList<Rectangle> areas;
            if (isPositioned) {
                areas = JavaCollectionsUtil.SingletonList(parentBBox);
            }
            else {
                areas = InitElementAreas(new LayoutArea(pageNumber, parentBBox));
            }
            occupiedArea = new LayoutArea(pageNumber, new Rectangle(parentBBox.GetX(), parentBBox.GetY() + parentBBox.
                GetHeight(), parentBBox.GetWidth(), 0));
            ShrinkOccupiedAreaForAbsolutePosition();
            int currentAreaPos = 0;
            Rectangle layoutBox = areas[0].Clone();
            // the first renderer (one of childRenderers or their children) to produce LayoutResult.NOTHING
            IRenderer causeOfNothing = null;
            bool anythingPlaced = false;
            for (int childPos = 0; childPos < childRenderers.Count; childPos++) {
                IRenderer childRenderer = childRenderers[childPos];
                LayoutResult result;
                childRenderer.SetParent(this);
                MarginsCollapseInfo childMarginsInfo = null;
                if (marginsCollapsingEnabled) {
                    childMarginsInfo = marginsCollapseHandler.StartChildMarginsHandling(childRenderer, layoutBox);
                }
                while ((result = childRenderer.SetParent(this).Layout(new LayoutContext(new LayoutArea(pageNumber, layoutBox
                    , wasHeightClipped || wasParentsHeightClipped), childMarginsInfo, floatRendererAreas))).GetStatus() !=
                     LayoutResult.FULL) {
                    if (marginsCollapsingEnabled) {
                        if (result.GetStatus() != LayoutResult.NOTHING) {
                            marginsCollapseHandler.EndChildMarginsHandling(layoutBox);
                        }
                        if (!isCellRenderer) {
                            marginsCollapseHandler.EndMarginsCollapse(layoutBox);
                        }
                    }
                    if (true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA_ON_SPLIT)) || true.Equals(GetPropertyAsBoolean
                        (Property.FILL_AVAILABLE_AREA))) {
                        occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), layoutBox));
                    }
                    else {
                        if (result.GetOccupiedArea() != null && result.GetStatus() != LayoutResult.NOTHING) {
                            occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), result.GetOccupiedArea().GetBBox
                                ()));
                            if (occupiedArea.GetBBox().GetWidth() > layoutBox.GetWidth()) {
                                occupiedArea.GetBBox().SetWidth(layoutBox.GetWidth());
                            }
                        }
                    }
                    if (FloatingHelper.IsRendererFloating(this) || isCellRenderer) {
                        FloatingHelper.IncludeChildFloatsInOccupiedArea(floatRendererAreas, this);
                    }
                    if (result.GetSplitRenderer() != null) {
                        // Use occupied area's bbox width so that for absolutely positioned renderers we do not align using full width
                        // in case when parent box should wrap around child boxes.
                        // TODO in the latter case, all elements should be layouted first so that we know maximum width needed to place all children and then apply horizontal alignment
                        AlignChildHorizontally(result.GetSplitRenderer(), occupiedArea.GetBBox());
                    }
                    // Save the first renderer to produce LayoutResult.NOTHING
                    if (null == causeOfNothing && null != result.GetCauseOfNothing()) {
                        causeOfNothing = result.GetCauseOfNothing();
                    }
                    // have more areas
                    if (currentAreaPos + 1 < areas.Count && !(result.GetAreaBreak() != null && result.GetAreaBreak().GetAreaType
                        () == AreaBreakType.NEXT_PAGE)) {
                        if (result.GetStatus() == LayoutResult.PARTIAL) {
                            childRenderers[childPos] = result.GetSplitRenderer();
                            // TODO linkedList would make it faster
                            childRenderers.Add(childPos + 1, result.GetOverflowRenderer());
                        }
                        else {
                            if (result.GetOverflowRenderer() != null) {
                                childRenderers[childPos] = result.GetOverflowRenderer();
                            }
                            else {
                                childRenderers.JRemoveAt(childPos);
                            }
                            childPos--;
                        }
                        layoutBox = areas[++currentAreaPos].Clone();
                        break;
                    }
                    else {
                        if (result.GetStatus() == LayoutResult.PARTIAL) {
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
                                if (HasProperty(Property.MAX_HEIGHT)) {
                                    overflowRenderer.SetProperty(Property.MAX_HEIGHT, RetrieveMaxHeight() - occupiedArea.GetBBox().GetHeight()
                                        );
                                }
                                if (HasProperty(Property.MIN_HEIGHT)) {
                                    overflowRenderer.SetProperty(Property.MIN_HEIGHT, RetrieveMinHeight() - occupiedArea.GetBBox().GetHeight()
                                        );
                                }
                                if (HasProperty(Property.HEIGHT)) {
                                    overflowRenderer.SetProperty(Property.HEIGHT, RetrieveHeight() - occupiedArea.GetBBox().GetHeight());
                                }
                                if (wasHeightClipped) {
                                    ILogger logger = LoggerFactory.GetLogger(typeof(TableRenderer));
                                    logger.Warn(iText.IO.LogMessageConstant.CLIP_ELEMENT);
                                    occupiedArea.GetBBox().MoveDown((float)blockMaxHeight - occupiedArea.GetBBox().GetHeight()).SetHeight((float
                                        )blockMaxHeight);
                                }
                                ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                                ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                                ApplyMargins(occupiedArea.GetBBox(), true);
                                LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                                    (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                                if (wasHeightClipped) {
                                    return new LayoutResult(LayoutResult.FULL, editedArea, splitRenderer, null);
                                }
                                else {
                                    return new LayoutResult(LayoutResult.PARTIAL, editedArea, splitRenderer, overflowRenderer, causeOfNothing);
                                }
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
                                if (result.GetOverflowRenderer() != null) {
                                    overflowRendererChildren.Add(result.GetOverflowRenderer());
                                }
                                overflowRendererChildren.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
                                overflowRenderer.childRenderers = overflowRendererChildren;
                                if (IsRelativePosition() && positionedRenderers.Count > 0) {
                                    overflowRenderer.positionedRenderers = new List<IRenderer>(positionedRenderers);
                                }
                                if (keepTogether) {
                                    splitRenderer = null;
                                    overflowRenderer.childRenderers.Clear();
                                    overflowRenderer.childRenderers = new List<IRenderer>(childRenderers);
                                }
                                float? maxHeight = RetrieveMaxHeight();
                                if (maxHeight != null) {
                                    if (isPositioned) {
                                        CorrectPositionedLayout(layoutBox);
                                    }
                                    overflowRenderer.SetProperty(Property.MAX_HEIGHT, maxHeight - occupiedArea.GetBBox().GetHeight());
                                }
                                float? minHeight = RetrieveMinHeight();
                                if (minHeight != null) {
                                    overflowRenderer.SetProperty(Property.MIN_HEIGHT, minHeight - occupiedArea.GetBBox().GetHeight());
                                }
                                float? height = RetrieveHeight();
                                if (height != null) {
                                    overflowRenderer.SetProperty(Property.HEIGHT, height - occupiedArea.GetBBox().GetHeight());
                                }
                                if (wasHeightClipped) {
                                    occupiedArea.GetBBox().MoveDown((float)blockMaxHeight - occupiedArea.GetBBox().GetHeight()).SetHeight((float
                                        )blockMaxHeight);
                                    ILogger logger = LoggerFactory.GetLogger(typeof(TableRenderer));
                                    logger.Warn(iText.IO.LogMessageConstant.CLIP_ELEMENT);
                                }
                                ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                                ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                                ApplyMargins(occupiedArea.GetBBox(), true);
                                //splitRenderer.occupiedArea = occupiedArea.clone();
                                ApplyAbsolutePositionIfNeeded(layoutContext);
                                LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                                    (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                                if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) || wasHeightClipped) {
                                    return new LayoutResult(LayoutResult.FULL, editedArea, splitRenderer, null, null);
                                }
                                else {
                                    if (layoutResult != LayoutResult.NOTHING) {
                                        return new LayoutResult(layoutResult, editedArea, splitRenderer, overflowRenderer, null).SetAreaBreak(result
                                            .GetAreaBreak());
                                    }
                                    else {
                                        return new LayoutResult(layoutResult, null, null, overflowRenderer, result.GetCauseOfNothing()).SetAreaBreak
                                            (result.GetAreaBreak());
                                    }
                                }
                            }
                        }
                    }
                }
                anythingPlaced = true;
                if (result.GetOccupiedArea() != null) {
                    if (!FloatingHelper.IsRendererFloating(childRenderer)) {
                        // this check is needed only if margins collapsing is enabled
                        occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), result.GetOccupiedArea().GetBBox
                            ()));
                        if (occupiedArea.GetBBox().GetWidth() > layoutBox.GetWidth()) {
                            occupiedArea.GetBBox().SetWidth(layoutBox.GetWidth());
                        }
                    }
                }
                if (marginsCollapsingEnabled) {
                    marginsCollapseHandler.EndChildMarginsHandling(layoutBox);
                }
                if (result.GetStatus() == LayoutResult.FULL) {
                    layoutBox.SetHeight(result.GetOccupiedArea().GetBBox().GetY() - layoutBox.GetY());
                    if (childRenderer.GetOccupiedArea() != null) {
                        // Use occupied area's bbox width so that for absolutely positioned renderers we do not align using full width
                        // in case when parent box should wrap around child boxes.
                        // TODO in the latter case, all elements should be layouted first so that we know maximum width needed to place all children and then apply horizontal alignment
                        AlignChildHorizontally(childRenderer, occupiedArea.GetBBox());
                    }
                }
                // Save the first renderer to produce LayoutResult.NOTHING
                if (null == causeOfNothing && null != result.GetCauseOfNothing()) {
                    causeOfNothing = result.GetCauseOfNothing();
                }
            }
            if (marginsCollapsingEnabled && !isCellRenderer) {
                marginsCollapseHandler.EndMarginsCollapse(layoutBox);
            }
            if (true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA))) {
                occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), layoutBox));
            }
            if (IsAbsolutePosition() || FloatingHelper.IsRendererFloating(this) || isCellRenderer) {
                FloatingHelper.IncludeChildFloatsInOccupiedArea(floatRendererAreas, this);
            }
            IRenderer overflowRenderer_1 = null;
            float? blockMinHeight = RetrieveMinHeight();
            if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) && null != blockMinHeight && blockMinHeight
                 > occupiedArea.GetBBox().GetHeight()) {
                if (IsFixedLayout()) {
                    occupiedArea.GetBBox().MoveDown((float)blockMinHeight - occupiedArea.GetBBox().GetHeight()).SetHeight((float
                        )blockMinHeight);
                }
                else {
                    float blockBottom = occupiedArea.GetBBox().GetBottom() - ((float)blockMinHeight - occupiedArea.GetBBox().GetHeight
                        ());
                    if ((null == overflowY || OverflowPropertyValue.FIT.Equals(overflowY)) && blockBottom < layoutBox.GetBottom
                        ()) {
                        blockBottom = layoutBox.GetBottom();
                    }
                    occupiedArea.GetBBox().IncreaseHeight(occupiedArea.GetBBox().GetBottom() - blockBottom).SetY(blockBottom);
                    blockMinHeight -= occupiedArea.GetBBox().GetHeight();
                    if (!IsFixedLayout() && blockMinHeight > AbstractRenderer.EPS) {
                        if (IsKeepTogether()) {
                            return new LayoutResult(LayoutResult.NOTHING, null, null, this, this);
                        }
                        else {
                            overflowRenderer_1 = CreateOverflowRenderer(LayoutResult.PARTIAL);
                            overflowRenderer_1.SetProperty(Property.MIN_HEIGHT, (float)blockMinHeight);
                            if (HasProperty(Property.HEIGHT)) {
                                overflowRenderer_1.SetProperty(Property.HEIGHT, RetrieveHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                        }
                    }
                }
            }
            if (positionedRenderers.Count > 0) {
                foreach (IRenderer childPositionedRenderer in positionedRenderers) {
                    Rectangle fullBbox = occupiedArea.GetBBox().Clone();
                    // Use that value so that layout is independent of whether we are in the bottom of the page or in the top of the page
                    float layoutMinHeight = 1000;
                    fullBbox.MoveDown(layoutMinHeight).SetHeight(layoutMinHeight + fullBbox.GetHeight());
                    LayoutArea parentArea = new LayoutArea(occupiedArea.GetPageNumber(), occupiedArea.GetBBox().Clone());
                    ApplyPaddings(parentArea.GetBBox(), paddings, true);
                    PreparePositionedRendererAndAreaForLayout(childPositionedRenderer, fullBbox, parentArea.GetBBox());
                    childPositionedRenderer.Layout(new PositionedLayoutContext(new LayoutArea(occupiedArea.GetPageNumber(), fullBbox
                        ), parentArea));
                }
            }
            if (isPositioned) {
                CorrectPositionedLayout(layoutBox);
            }
            float overflowPartHeight = GetOverflowPartHeight(overflowY, layoutBox);
            ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
            if (positionedRenderers.Count > 0) {
                LayoutArea area = new LayoutArea(occupiedArea.GetPageNumber(), occupiedArea.GetBBox().Clone(), wasHeightClipped
                     || wasParentsHeightClipped);
                ApplyBorderBox(area.GetBBox(), false);
                foreach (IRenderer childPositionedRenderer in positionedRenderers) {
                    childPositionedRenderer.SetParent(this).Layout(new LayoutContext(area));
                }
                ApplyBorderBox(area.GetBBox(), true);
            }
            ApplyMargins(occupiedArea.GetBBox(), true);
            ApplyAbsolutePositionIfNeeded(layoutContext);
            if (rotation != null) {
                ApplyRotationLayout(layoutContext.GetArea().GetBBox().Clone());
                if (IsNotFittingLayoutArea(layoutContext.GetArea())) {
                    if (IsNotFittingWidth(layoutContext.GetArea()) && !IsNotFittingHeight(layoutContext.GetArea())) {
                        LoggerFactory.GetLogger(GetType()).Warn(MessageFormatUtil.Format(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA
                            , "It fits by height so it will be forced placed"));
                    }
                    else {
                        if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                            return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, null, null, this, this);
                        }
                    }
                }
            }
            ApplyVerticalAlignment();
            FloatingHelper.RemoveFloatsAboveRendererBottom(floatRendererAreas, this);
            LayoutArea editedArea_1 = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
            if (floatPropertyValue != null && !floatPropertyValue.Equals(FloatPropertyValue.NONE)) {
                // TODO anything like this on any other floated renderer?
                RootRenderer rootRenderer = GetRootRenderer();
                if (rootRenderer is DocumentRenderer) {
                    Document document = (Document)(rootRenderer).GetModelElement();
                    float bottomMargin = document.GetBottomMargin();
                    if (occupiedArea.GetBBox().GetY() < bottomMargin) {
                        layoutContext.GetFloatRendererAreas().Clear();
                        return new LayoutResult(LayoutResult.NOTHING, null, null, this, null);
                    }
                }
            }
            if (wasHeightClipped) {
                editedArea_1.GetBBox().MoveUp(overflowPartHeight).DecreaseHeight(overflowPartHeight);
                occupiedArea.GetBBox().MoveUp(overflowPartHeight).DecreaseHeight(overflowPartHeight);
            }
            if (null == overflowRenderer_1) {
                return new LayoutResult(LayoutResult.FULL, editedArea_1, null, null, causeOfNothing);
            }
            else {
                return new LayoutResult(LayoutResult.PARTIAL, editedArea_1, this, overflowRenderer_1, causeOfNothing);
            }
        }

        protected internal virtual AbstractRenderer CreateSplitRenderer(int layoutResult) {
            AbstractRenderer splitRenderer = (AbstractRenderer)GetNextRenderer();
            splitRenderer.parent = parent;
            splitRenderer.modelElement = modelElement;
            splitRenderer.occupiedArea = occupiedArea;
            splitRenderer.isLastRendererForModelElement = false;
            splitRenderer.properties = new Dictionary<int, Object>(properties);
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
            if (occupiedArea == null) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.BlockRenderer));
                logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, 
                    "Drawing won't be performed."));
                return;
            }
            PdfDocument document = drawContext.GetDocument();
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            TagTreePointer tagPointer = null;
            IAccessibleElement accessibleElement = null;
            if (isTagged) {
                accessibleElement = (IAccessibleElement)GetModelElement();
                PdfName role = accessibleElement.GetRole();
                if (role != null && !PdfName.Artifact.Equals(role)) {
                    tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                    bool alreadyCreated = tagPointer.IsElementConnectedToTag(accessibleElement);
                    tagPointer.AddTag(accessibleElement, true);
                    if (!alreadyCreated) {
                        if (role.Equals(PdfName.L)) {
                            PdfDictionary listAttributes = AccessibleAttributesApplier.GetListAttributes(this, tagPointer);
                            ApplyGeneratedAccessibleAttributes(tagPointer, listAttributes);
                        }
                        if (role.Equals(PdfName.TD) || role.Equals(PdfName.TH)) {
                            PdfDictionary tableAttributes = AccessibleAttributesApplier.GetTableAttributes(this, tagPointer);
                            ApplyGeneratedAccessibleAttributes(tagPointer, tableAttributes);
                        }
                        PdfDictionary layoutAttributes = AccessibleAttributesApplier.GetLayoutAttributes(role, this, tagPointer);
                        ApplyGeneratedAccessibleAttributes(tagPointer, layoutAttributes);
                    }
                }
                else {
                    isTagged = false;
                }
            }
            ApplyDestinationsAndAnnotation(drawContext);
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            BeginElementOpacityApplying(drawContext);
            BeginRotationIfApplied(drawContext.GetCanvas());
            DrawBackground(drawContext);
            DrawBorder(drawContext);
            OverflowPropertyValue? overflowX = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
            OverflowPropertyValue? overflowY = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_Y);
            bool processOverflow = OverflowPropertyValue.HIDDEN.Equals(overflowX) || OverflowPropertyValue.HIDDEN.Equals
                (overflowY);
            if (processOverflow) {
                drawContext.GetCanvas().SaveState();
                Rectangle clippedArea = drawContext.GetDocument().GetPage(occupiedArea.GetPageNumber()).GetPageSize();
                Rectangle area = GetBorderAreaBBox();
                if (OverflowPropertyValue.HIDDEN.Equals(overflowX)) {
                    clippedArea.SetX(area.GetX()).SetWidth(area.GetWidth());
                }
                if (OverflowPropertyValue.HIDDEN.Equals(overflowY)) {
                    clippedArea.SetY(area.GetY()).SetHeight(area.GetHeight());
                }
                drawContext.GetCanvas().Rectangle(clippedArea).Clip().NewPath();
            }
            DrawChildren(drawContext);
            DrawPositionedChildren(drawContext);
            if (processOverflow) {
                drawContext.GetCanvas().RestoreState();
            }
            EndRotationIfApplied(drawContext.GetCanvas());
            EndElementOpacityApplying(drawContext);
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(true);
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
                if (!HasOwnProperty(Property.ROTATION_INITIAL_WIDTH) || !HasOwnProperty(Property.ROTATION_INITIAL_HEIGHT)) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.BlockRenderer));
                    logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.ROTATION_WAS_NOT_CORRECTLY_PROCESSED_FOR_RENDERER
                        , GetType().Name));
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
            if (verticalAlignment == null || verticalAlignment == VerticalAlignment.TOP || childRenderers.IsEmpty()) {
                return;
            }
            float lowestChildBottom = float.MaxValue;
            if (FloatingHelper.IsRendererFloating(this) || this is CellRenderer) {
                // include floats in vertical alignment
                foreach (IRenderer child in childRenderers) {
                    if (child.GetOccupiedArea().GetBBox().GetBottom() < lowestChildBottom) {
                        lowestChildBottom = child.GetOccupiedArea().GetBBox().GetBottom();
                    }
                }
            }
            else {
                int lastChildIndex = childRenderers.Count - 1;
                while (lastChildIndex >= 0) {
                    IRenderer child = childRenderers[lastChildIndex--];
                    if (!FloatingHelper.IsRendererFloating(child)) {
                        lowestChildBottom = child.GetOccupiedArea().GetBBox().GetBottom();
                        break;
                    }
                }
            }
            if (lowestChildBottom == float.MaxValue) {
                return;
            }
            float deltaY = lowestChildBottom - GetInnerAreaBBox().GetY();
            switch (verticalAlignment) {
                case VerticalAlignment.BOTTOM: {
                    foreach (IRenderer child in childRenderers) {
                        child.Move(0, -deltaY);
                    }
                    break;
                }

                case VerticalAlignment.MIDDLE: {
                    foreach (IRenderer child in childRenderers) {
                        child.Move(0, -deltaY / 2);
                    }
                    break;
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
                Rectangle newBBox = CalculateBBox(rotatedPoints);
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
                    logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.ROTATION_WAS_NOT_CORRECTLY_PROCESSED_FOR_RENDERER
                        , GetType().Name));
                }
                else {
                    AffineTransform transform = CreateRotationTransformInsideOccupiedArea();
                    canvas.SaveState().ConcatMatrix(transform);
                }
            }
        }

        protected internal virtual void EndRotationIfApplied(PdfCanvas canvas) {
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (angle != null && HasOwnProperty(Property.ROTATION_INITIAL_HEIGHT)) {
                canvas.RestoreState();
            }
        }

        [Obsolete]
        protected internal virtual void CorrectPositionedLayout(Rectangle layoutBox) {
            if (IsFixedLayout()) {
                float y = (float)this.GetPropertyAsFloat(Property.Y);
                float relativeY = IsFixedLayout() ? 0 : layoutBox.GetY();
                Move(0, relativeY + y - occupiedArea.GetBBox().GetY());
            }
        }

        protected internal virtual float GetOverflowPartHeight(OverflowPropertyValue? overflowY, Rectangle parentBox
            ) {
            float difference = 0;
            if (null != overflowY && OverflowPropertyValue.FIT != overflowY) {
                if (occupiedArea.GetBBox().GetBottom() < parentBox.GetBottom()) {
                    difference = parentBox.GetBottom() - occupiedArea.GetBBox().GetBottom();
                }
            }
            return difference;
        }

        protected internal virtual float ApplyBordersPaddingsMargins(Rectangle parentBBox, Border[] borders, float
            [] paddings) {
            float parentWidth = parentBBox.GetWidth();
            ApplyMargins(parentBBox, false);
            ApplyBorderBox(parentBBox, borders, false);
            if (IsPositioned()) {
                if (IsFixedLayout()) {
                    float x = (float)this.GetPropertyAsFloat(Property.X);
                    float relativeX = IsFixedLayout() ? 0 : parentBBox.GetX();
                    parentBBox.SetX(relativeX + x);
                }
            }
            ApplyPaddings(parentBBox, paddings, false);
            return parentWidth - parentBBox.GetWidth();
        }

        protected internal override MinMaxWidth GetMinMaxWidth(float availableWidth) {
            Rectangle area = new Rectangle(availableWidth, AbstractRenderer.INF);
            float additionalWidth = ApplyBordersPaddingsMargins(area, GetBorders(), GetPaddings());
            MinMaxWidth minMaxWidth = new MinMaxWidth(additionalWidth, availableWidth);
            AbstractWidthHandler handler = new MaxMaxWidthHandler(minMaxWidth);
            foreach (IRenderer childRenderer in childRenderers) {
                MinMaxWidth childMinMaxWidth;
                childRenderer.SetParent(this);
                if (childRenderer is AbstractRenderer) {
                    childMinMaxWidth = ((AbstractRenderer)childRenderer).GetMinMaxWidth(area.GetWidth());
                }
                else {
                    childMinMaxWidth = MinMaxWidthUtils.CountDefaultMinMaxWidth(childRenderer, area.GetWidth());
                }
                handler.UpdateMaxChildWidth(childMinMaxWidth.GetMaxWidth());
                handler.UpdateMinChildWidth(childMinMaxWidth.GetMinWidth());
            }
            if (this.GetPropertyAsFloat(Property.ROTATION_ANGLE) != null) {
                return RotationUtils.CountRotationMinMaxWidth(CorrectMinMaxWidth(minMaxWidth), this);
            }
            else {
                return CorrectMinMaxWidth(minMaxWidth);
            }
        }

        internal virtual MinMaxWidth CorrectMinMaxWidth(MinMaxWidth minMaxWidth) {
            float? width = RetrieveWidth(-1);
            OverflowPropertyValue? overflowX = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
            if (width != null && width >= 0 && (width >= minMaxWidth.GetChildrenMinWidth() || !OverflowPropertyValue.FIT
                .Equals(overflowX))) {
                minMaxWidth.SetChildrenMaxWidth((float)width);
                minMaxWidth.SetChildrenMinWidth((float)width);
            }
            return minMaxWidth;
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
