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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Layout;
using iText.Layout.Margincollapse;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal class FloatingHelper {
        private FloatingHelper() {
        }

//\cond DO_NOT_DOCUMENT
        internal static void AdjustLineAreaAccordingToFloats(IList<Rectangle> floatRendererAreas, Rectangle layoutBox
            ) {
            AdjustLayoutBoxAccordingToFloats(floatRendererAreas, layoutBox, null, 0, null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float AdjustLayoutBoxAccordingToFloats(IList<Rectangle> floatRendererAreas, Rectangle layoutBox
            , float? boxWidth, float clearHeightCorrection, MarginsCollapseHandler marginsCollapseHandler) {
            float topShift = clearHeightCorrection;
            float left;
            float right;
            Rectangle[] lastLeftAndRightBoxes = null;
            do {
                if (lastLeftAndRightBoxes != null) {
                    float bottomLeft = lastLeftAndRightBoxes[0] != null ? lastLeftAndRightBoxes[0].GetBottom() : float.MaxValue;
                    float bottomRight = lastLeftAndRightBoxes[1] != null ? lastLeftAndRightBoxes[1].GetBottom() : float.MaxValue;
                    float updatedHeight = Math.Min(bottomLeft, bottomRight) - layoutBox.GetY();
                    topShift = layoutBox.GetHeight() - updatedHeight;
                }
                IList<Rectangle> boxesAtYLevel = GetBoxesAtYLevel(floatRendererAreas, layoutBox.GetTop() - topShift);
                if (boxesAtYLevel.IsEmpty()) {
                    ApplyClearance(layoutBox, marginsCollapseHandler, topShift, false);
                    return topShift;
                }
                lastLeftAndRightBoxes = FindLastLeftAndRightBoxes(layoutBox, boxesAtYLevel);
                left = lastLeftAndRightBoxes[0] != null ? lastLeftAndRightBoxes[0].GetRight() : float.Epsilon;
                right = lastLeftAndRightBoxes[1] != null ? lastLeftAndRightBoxes[1].GetLeft() : float.MaxValue;
                if (left > right || left > layoutBox.GetRight() || right < layoutBox.GetLeft()) {
                    left = layoutBox.GetLeft();
                    right = left;
                }
                else {
                    if (right > layoutBox.GetRight()) {
                        right = layoutBox.GetRight();
                    }
                    if (left < layoutBox.GetLeft()) {
                        left = layoutBox.GetLeft();
                    }
                }
            }
            while (boxWidth != null && boxWidth > right - left);
            if (layoutBox.GetWidth() > right - left) {
                layoutBox.SetX(left).SetWidth(right - left);
            }
            ApplyClearance(layoutBox, marginsCollapseHandler, topShift, false);
            return topShift;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float? CalculateLineShiftUnderFloats(IList<Rectangle> floatRendererAreas, Rectangle layoutBox
            ) {
            IList<Rectangle> boxesAtYLevel = GetBoxesAtYLevel(floatRendererAreas, layoutBox.GetTop());
            if (boxesAtYLevel.IsEmpty()) {
                return null;
            }
            Rectangle[] lastLeftAndRightBoxes = FindLastLeftAndRightBoxes(layoutBox, boxesAtYLevel);
            float left = lastLeftAndRightBoxes[0] != null ? lastLeftAndRightBoxes[0].GetRight() : layoutBox.GetLeft();
            float right = lastLeftAndRightBoxes[1] != null ? lastLeftAndRightBoxes[1].GetLeft() : layoutBox.GetRight();
            if (layoutBox.GetLeft() < left || layoutBox.GetRight() > right) {
                float maxLastFloatBottom;
                if (lastLeftAndRightBoxes[0] != null && lastLeftAndRightBoxes[1] != null) {
                    maxLastFloatBottom = Math.Max(lastLeftAndRightBoxes[0].GetBottom(), lastLeftAndRightBoxes[1].GetBottom());
                }
                else {
                    if (lastLeftAndRightBoxes[0] != null) {
                        maxLastFloatBottom = lastLeftAndRightBoxes[0].GetBottom();
                    }
                    else {
                        maxLastFloatBottom = lastLeftAndRightBoxes[1].GetBottom();
                    }
                }
                return layoutBox.GetTop() - maxLastFloatBottom + AbstractRenderer.EPS;
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void AdjustFloatedTableLayoutBox(TableRenderer tableRenderer, Rectangle layoutBox, float tableWidth
            , IList<Rectangle> floatRendererAreas, FloatPropertyValue? floatPropertyValue) {
            tableRenderer.SetProperty(Property.HORIZONTAL_ALIGNMENT, null);
            UnitValue[] margins = tableRenderer.GetMargins();
            if (!margins[1].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.FloatingHelper));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_RIGHT));
            }
            if (!margins[3].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.FloatingHelper));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_LEFT));
            }
            AdjustBlockAreaAccordingToFloatRenderers(floatRendererAreas, layoutBox, tableWidth + margins[1].GetValue()
                 + margins[3].GetValue(), FloatPropertyValue.LEFT.Equals(floatPropertyValue));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float? AdjustFloatedBlockLayoutBox(AbstractRenderer renderer, Rectangle parentBBox, float?
             blockWidth, IList<Rectangle> floatRendererAreas, FloatPropertyValue? floatPropertyValue, OverflowPropertyValue?
             overflowX) {
            renderer.SetProperty(Property.HORIZONTAL_ALIGNMENT, null);
            float floatElemWidth;
            bool overflowFit = AbstractRenderer.IsOverflowFit(overflowX);
            if (blockWidth != null) {
                floatElemWidth = (float)blockWidth + AbstractRenderer.CalculateAdditionalWidth(renderer);
                if (overflowFit && floatElemWidth > parentBBox.GetWidth()) {
                    floatElemWidth = parentBBox.GetWidth();
                }
            }
            else {
                MinMaxWidth minMaxWidth = CalculateMinMaxWidthForFloat(renderer, floatPropertyValue);
                float maxWidth = minMaxWidth.GetMaxWidth();
                if (maxWidth > parentBBox.GetWidth()) {
                    maxWidth = parentBBox.GetWidth();
                }
                if (!overflowFit && minMaxWidth.GetMinWidth() > parentBBox.GetWidth()) {
                    maxWidth = minMaxWidth.GetMinWidth();
                }
                floatElemWidth = maxWidth + AbstractRenderer.EPS;
                blockWidth = maxWidth - minMaxWidth.GetAdditionalWidth() + AbstractRenderer.EPS;
            }
            AdjustBlockAreaAccordingToFloatRenderers(floatRendererAreas, parentBBox, floatElemWidth, FloatPropertyValue
                .LEFT.Equals(floatPropertyValue));
            return blockWidth;
        }
//\endcond

        // Float boxes shall be ordered by addition; No zero-width boxes shall be in the list.
        private static void AdjustBlockAreaAccordingToFloatRenderers(IList<Rectangle> floatRendererAreas, Rectangle
             layoutBox, float blockWidth, bool isFloatLeft) {
            if (floatRendererAreas.IsEmpty()) {
                if (!isFloatLeft) {
                    AdjustBoxForFloatRight(layoutBox, blockWidth);
                }
                return;
            }
            float currY;
            if (floatRendererAreas[floatRendererAreas.Count - 1].GetTop() < layoutBox.GetTop()) {
                currY = floatRendererAreas[floatRendererAreas.Count - 1].GetTop();
            }
            else {
                // e.g. if clear was applied on float and current top of layoutBox is lower than last float renderer
                currY = layoutBox.GetTop();
            }
            Rectangle[] lastLeftAndRightBoxes = null;
            float left = 0;
            float right = 0;
            while (lastLeftAndRightBoxes == null || right - left < blockWidth) {
                if (lastLeftAndRightBoxes != null) {
                    if (isFloatLeft) {
                        currY = lastLeftAndRightBoxes[0] != null ? lastLeftAndRightBoxes[0].GetBottom() : lastLeftAndRightBoxes[1]
                            .GetBottom();
                    }
                    else {
                        currY = lastLeftAndRightBoxes[1] != null ? lastLeftAndRightBoxes[1].GetBottom() : lastLeftAndRightBoxes[0]
                            .GetBottom();
                    }
                }
                layoutBox.SetHeight(currY - layoutBox.GetY());
                IList<Rectangle> yLevelBoxes = GetBoxesAtYLevel(floatRendererAreas, currY);
                if (yLevelBoxes.IsEmpty()) {
                    if (!isFloatLeft) {
                        AdjustBoxForFloatRight(layoutBox, blockWidth);
                    }
                    return;
                }
                lastLeftAndRightBoxes = FindLastLeftAndRightBoxes(layoutBox, yLevelBoxes);
                left = lastLeftAndRightBoxes[0] != null ? lastLeftAndRightBoxes[0].GetRight() : layoutBox.GetLeft();
                right = lastLeftAndRightBoxes[1] != null ? lastLeftAndRightBoxes[1].GetLeft() : layoutBox.GetRight();
            }
            layoutBox.SetX(left);
            layoutBox.SetWidth(right - left);
            if (!isFloatLeft) {
                AdjustBoxForFloatRight(layoutBox, blockWidth);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static void RemoveFloatsAboveRendererBottom(IList<Rectangle> floatRendererAreas, IRenderer renderer
            ) {
            if (!IsRendererFloating(renderer)) {
                float bottom = renderer.GetOccupiedArea().GetBBox().GetBottom();
                for (int i = floatRendererAreas.Count - 1; i >= 0; i--) {
                    if (floatRendererAreas[i].GetBottom() >= bottom) {
                        floatRendererAreas.JRemoveAt(i);
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static LayoutArea AdjustResultOccupiedAreaForFloatAndClear(IRenderer renderer, IList<Rectangle> floatRendererAreas
            , Rectangle parentBBox, float clearHeightCorrection, bool marginsCollapsingEnabled) {
            return AdjustResultOccupiedAreaForFloatAndClear(renderer, floatRendererAreas, parentBBox, clearHeightCorrection
                , 0, marginsCollapsingEnabled);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static LayoutArea AdjustResultOccupiedAreaForFloatAndClear(IRenderer renderer, IList<Rectangle> floatRendererAreas
            , Rectangle parentBBox, float clearHeightCorrection, float bfcHeightCorrection, bool marginsCollapsingEnabled
            ) {
            LayoutArea occupiedArea = renderer.GetOccupiedArea();
            LayoutArea editedArea = occupiedArea;
            if (IsRendererFloating(renderer)) {
                editedArea = occupiedArea.Clone();
                if (occupiedArea.GetBBox().GetWidth() > 0) {
                    floatRendererAreas.Add(occupiedArea.GetBBox());
                }
                editedArea.GetBBox().SetY(parentBBox.GetTop());
                editedArea.GetBBox().SetHeight(0);
            }
            else {
                if (clearHeightCorrection > AbstractRenderer.EPS && !marginsCollapsingEnabled) {
                    editedArea = occupiedArea.Clone();
                    editedArea.GetBBox().IncreaseHeight(clearHeightCorrection);
                }
                else {
                    if (bfcHeightCorrection > AbstractRenderer.EPS) {
                        editedArea = occupiedArea.Clone();
                        editedArea.GetBBox().IncreaseHeight(bfcHeightCorrection);
                    }
                }
            }
            return editedArea;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void IncludeChildFloatsInOccupiedArea(IList<Rectangle> floatRendererAreas, IRenderer renderer
            , ICollection<Rectangle> nonChildFloatingRendererAreas) {
            Rectangle commonRectangle = IncludeChildFloatsInOccupiedArea(floatRendererAreas, renderer.GetOccupiedArea(
                ).GetBBox(), nonChildFloatingRendererAreas);
            renderer.GetOccupiedArea().SetBBox(commonRectangle);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static Rectangle IncludeChildFloatsInOccupiedArea(IList<Rectangle> floatRendererAreas, Rectangle 
            occupiedAreaBbox, ICollection<Rectangle> nonChildFloatingRendererAreas) {
            foreach (Rectangle floatBox in floatRendererAreas) {
                if (nonChildFloatingRendererAreas.Contains(floatBox)) {
                    // Currently there is no other way to distinguish floats that are not descendants of this renderer
                    // except by preserving a set of such.
                    continue;
                }
                occupiedAreaBbox = Rectangle.GetCommonRectangle(occupiedAreaBbox, floatBox);
            }
            return occupiedAreaBbox;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static MinMaxWidth CalculateMinMaxWidthForFloat(AbstractRenderer renderer, FloatPropertyValue? floatPropertyVal
            ) {
            bool floatPropIsRendererOwn = renderer.HasOwnProperty(Property.FLOAT);
            renderer.SetProperty(Property.FLOAT, FloatPropertyValue.NONE);
            MinMaxWidth kidMinMaxWidth = renderer.GetMinMaxWidth();
            if (floatPropIsRendererOwn) {
                renderer.SetProperty(Property.FLOAT, floatPropertyVal);
            }
            else {
                renderer.DeleteOwnProperty(Property.FLOAT);
            }
            return kidMinMaxWidth;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float CalculateClearHeightCorrection(IRenderer renderer, IList<Rectangle> floatRendererAreas
            , Rectangle parentBBox) {
            ClearPropertyValue? clearPropertyValue = renderer.GetProperty<ClearPropertyValue?>(Property.CLEAR);
            float clearHeightCorrection = 0;
            if (clearPropertyValue == null || floatRendererAreas.IsEmpty()) {
                return clearHeightCorrection;
            }
            float currY = Math.Min(floatRendererAreas[floatRendererAreas.Count - 1].GetTop(), parentBBox.GetTop());
            IList<Rectangle> boxesAtYLevel = GetBoxesAtYLevel(floatRendererAreas, currY);
            Rectangle[] lastLeftAndRightBoxes = FindLastLeftAndRightBoxes(parentBBox, boxesAtYLevel);
            bool isBoth = clearPropertyValue.Equals(ClearPropertyValue.BOTH);
            float lowestFloatBottom = CalculateLowestFloatBottom(clearPropertyValue.Equals(ClearPropertyValue.LEFT) ||
                 isBoth, clearPropertyValue.Equals(ClearPropertyValue.RIGHT) || isBoth, float.MaxValue, lastLeftAndRightBoxes
                , floatRendererAreas);
            if (lowestFloatBottom < float.MaxValue) {
                clearHeightCorrection = parentBBox.GetTop() - lowestFloatBottom + AbstractRenderer.EPS;
            }
            return clearHeightCorrection;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float AdjustBlockFormattingContextLayoutBox(BlockRenderer renderer, IList<Rectangle> floatRendererAreas
            , Rectangle parentBBox, float blockWidth, float clearHeightCorrection) {
            if (floatRendererAreas.IsEmpty() || !BlockFormattingContextUtil.IsRendererCreateBfc(renderer) && !(renderer
                 is FlexContainerRenderer)) {
                return 0;
            }
            float currY = Math.Min(floatRendererAreas[floatRendererAreas.Count - 1].GetTop(), parentBBox.GetTop() - clearHeightCorrection
                );
            IList<Rectangle> boxesAtYLevel = GetBoxesAtYLevel(floatRendererAreas, currY);
            Rectangle[] lastLeftAndRightBoxes = FindLastLeftAndRightBoxes(parentBBox, boxesAtYLevel);
            if (lastLeftAndRightBoxes[0] == null && lastLeftAndRightBoxes[1] == null) {
                return 0;
            }
            float leftX = lastLeftAndRightBoxes[0] == null ? parentBBox.GetLeft() : lastLeftAndRightBoxes[0].GetRight(
                );
            float rightX = lastLeftAndRightBoxes[1] == null ? parentBBox.GetRight() : lastLeftAndRightBoxes[1].GetLeft
                ();
            if (Math.Max(blockWidth, renderer.GetMinMaxWidth().GetMinWidth()) <= rightX - leftX) {
                float width = Math.Max(0, leftX - parentBBox.GetLeft()) + Math.Max(0, parentBBox.GetRight() - rightX);
                parentBBox.SetX(Math.Max(parentBBox.GetX(), leftX));
                parentBBox.DecreaseWidth(width);
                return 0;
            }
            float lowestFloatBottom = CalculateLowestFloatBottom(true, true, float.MaxValue, lastLeftAndRightBoxes, floatRendererAreas
                );
            if (lowestFloatBottom < float.MaxValue) {
                float adjustedHeightDelta = parentBBox.GetTop() - lowestFloatBottom + AbstractRenderer.EPS;
                parentBBox.DecreaseHeight(adjustedHeightDelta);
                return adjustedHeightDelta;
            }
            return 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void ApplyClearance(Rectangle layoutBox, MarginsCollapseHandler marginsCollapseHandler, float
             clearHeightAdjustment, bool isFloat) {
            if (clearHeightAdjustment <= 0) {
                return;
            }
            if (marginsCollapseHandler == null || isFloat) {
                layoutBox.DecreaseHeight(clearHeightAdjustment);
            }
            else {
                marginsCollapseHandler.ApplyClearance(clearHeightAdjustment);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static bool IsRendererFloating(IRenderer renderer) {
            return IsRendererFloating(renderer, renderer.GetProperty<FloatPropertyValue?>(Property.FLOAT));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static bool IsRendererFloating(IRenderer renderer, FloatPropertyValue? kidFloatPropertyVal) {
            int? position = renderer.GetProperty<int?>(Property.POSITION);
            bool notAbsolutePos = position == null || position != LayoutPosition.ABSOLUTE;
            return notAbsolutePos && kidFloatPropertyVal != null && !kidFloatPropertyVal.Equals(FloatPropertyValue.NONE
                );
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static bool IsClearanceApplied(IList<IRenderer> floatingRenderers, ClearPropertyValue? clearPropertyValue
            ) {
            if (clearPropertyValue == null || clearPropertyValue.Equals(ClearPropertyValue.NONE)) {
                return false;
            }
            foreach (IRenderer floatingRenderer in floatingRenderers) {
                FloatPropertyValue? floatPropertyValue = floatingRenderer.GetProperty<FloatPropertyValue?>(Property.FLOAT);
                if (clearPropertyValue.Equals(ClearPropertyValue.BOTH) || (floatPropertyValue.Equals(FloatPropertyValue.LEFT
                    ) && clearPropertyValue.Equals(ClearPropertyValue.LEFT)) || (floatPropertyValue.Equals(FloatPropertyValue
                    .RIGHT) && clearPropertyValue.Equals(ClearPropertyValue.RIGHT))) {
                    return true;
                }
            }
            return false;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void RemoveParentArtifactsOnPageSplitIfOnlyFloatsOverflow(IRenderer overflowRenderer) {
            overflowRenderer.SetProperty(Property.BACKGROUND, null);
            overflowRenderer.SetProperty(Property.BACKGROUND_IMAGE, null);
            overflowRenderer.SetProperty(Property.OUTLINE, null);
            Border[] borders = AbstractRenderer.GetBorders(overflowRenderer);
            overflowRenderer.SetProperty(Property.BORDER_TOP, null);
            overflowRenderer.SetProperty(Property.BORDER_BOTTOM, null);
            if (borders[1] != null) {
                overflowRenderer.SetProperty(Property.BORDER_RIGHT, new SolidBorder(ColorConstants.BLACK, borders[1].GetWidth
                    (), 0));
            }
            if (borders[3] != null) {
                overflowRenderer.SetProperty(Property.BORDER_LEFT, new SolidBorder(ColorConstants.BLACK, borders[3].GetWidth
                    (), 0));
            }
            overflowRenderer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(0));
            overflowRenderer.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(0));
            overflowRenderer.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(0));
            overflowRenderer.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(0));
        }
//\endcond

        private static void AdjustBoxForFloatRight(Rectangle layoutBox, float blockWidth) {
            layoutBox.SetX(layoutBox.GetRight() - blockWidth);
            layoutBox.SetWidth(blockWidth);
        }

        private static Rectangle[] FindLastLeftAndRightBoxes(Rectangle layoutBox, IList<Rectangle> yLevelBoxes) {
            Rectangle lastLeftFloatAtY = null;
            Rectangle lastRightFloatAtY = null;
            float left = layoutBox.GetLeft();
            foreach (Rectangle box in yLevelBoxes) {
                if (box.GetLeft() < left) {
                    left = box.GetLeft();
                }
            }
            foreach (Rectangle box in yLevelBoxes) {
                if (left >= box.GetLeft() && left < box.GetRight()) {
                    lastLeftFloatAtY = box;
                    left = box.GetRight();
                }
                else {
                    lastRightFloatAtY = box;
                }
            }
            return new Rectangle[] { lastLeftFloatAtY, lastRightFloatAtY };
        }

        private static IList<Rectangle> GetBoxesAtYLevel(IList<Rectangle> floatRendererAreas, float currY) {
            IList<Rectangle> yLevelBoxes = new List<Rectangle>();
            foreach (Rectangle box in floatRendererAreas) {
                if (box.GetBottom() + AbstractRenderer.EPS < currY && box.GetTop() + AbstractRenderer.EPS >= currY) {
                    yLevelBoxes.Add(box);
                }
            }
            return yLevelBoxes;
        }

        private static float CalculateLowestFloatBottom(bool isLeftOrBoth, bool isRightOrBoth, float lowestFloatBottom
            , Rectangle[] lastLeftAndRightBoxes, IList<Rectangle> floatRendererAreas) {
            if (isLeftOrBoth && lastLeftAndRightBoxes[0] != null) {
                foreach (Rectangle floatBox in floatRendererAreas) {
                    if (floatBox.GetBottom() < lowestFloatBottom && floatBox.GetLeft() <= lastLeftAndRightBoxes[0].GetLeft()) {
                        lowestFloatBottom = floatBox.GetBottom();
                    }
                }
            }
            if (isRightOrBoth && lastLeftAndRightBoxes[1] != null) {
                foreach (Rectangle floatBox in floatRendererAreas) {
                    if (floatBox.GetBottom() < lowestFloatBottom && floatBox.GetRight() >= lastLeftAndRightBoxes[1].GetRight()
                        ) {
                        lowestFloatBottom = floatBox.GetBottom();
                    }
                }
            }
            return lowestFloatBottom;
        }
    }
//\endcond
}
