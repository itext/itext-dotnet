/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
    internal class FloatingHelper {
        private FloatingHelper() {
        }

        internal static void AdjustLineAreaAccordingToFloats(IList<Rectangle> floatRendererAreas, Rectangle layoutBox
            ) {
            AdjustLayoutBoxAccordingToFloats(floatRendererAreas, layoutBox, null, 0, null);
        }

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

        internal static LayoutArea AdjustResultOccupiedAreaForFloatAndClear(IRenderer renderer, IList<Rectangle> floatRendererAreas
            , Rectangle parentBBox, float clearHeightCorrection, bool marginsCollapsingEnabled) {
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
                if (clearHeightCorrection > 0 && !marginsCollapsingEnabled) {
                    editedArea = occupiedArea.Clone();
                    editedArea.GetBBox().IncreaseHeight(clearHeightCorrection);
                }
            }
            return editedArea;
        }

        internal static void IncludeChildFloatsInOccupiedArea(IList<Rectangle> floatRendererAreas, IRenderer renderer
            , ICollection<Rectangle> nonChildFloatingRendererAreas) {
            Rectangle commonRectangle = IncludeChildFloatsInOccupiedArea(floatRendererAreas, renderer.GetOccupiedArea(
                ).GetBBox(), nonChildFloatingRendererAreas);
            renderer.GetOccupiedArea().SetBBox(commonRectangle);
        }

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

        internal static float CalculateClearHeightCorrection(IRenderer renderer, IList<Rectangle> floatRendererAreas
            , Rectangle parentBBox) {
            ClearPropertyValue? clearPropertyValue = renderer.GetProperty<ClearPropertyValue?>(Property.CLEAR);
            float clearHeightCorrection = 0;
            if (clearPropertyValue == null || floatRendererAreas.IsEmpty()) {
                return clearHeightCorrection;
            }
            float currY;
            if (floatRendererAreas[floatRendererAreas.Count - 1].GetTop() < parentBBox.GetTop()) {
                currY = floatRendererAreas[floatRendererAreas.Count - 1].GetTop();
            }
            else {
                currY = parentBBox.GetTop();
            }
            IList<Rectangle> boxesAtYLevel = GetBoxesAtYLevel(floatRendererAreas, currY);
            Rectangle[] lastLeftAndRightBoxes = FindLastLeftAndRightBoxes(parentBBox, boxesAtYLevel);
            float lowestFloatBottom = float.MaxValue;
            bool isBoth = clearPropertyValue.Equals(ClearPropertyValue.BOTH);
            if ((clearPropertyValue.Equals(ClearPropertyValue.LEFT) || isBoth) && lastLeftAndRightBoxes[0] != null) {
                foreach (Rectangle floatBox in floatRendererAreas) {
                    if (floatBox.GetBottom() < lowestFloatBottom && floatBox.GetLeft() <= lastLeftAndRightBoxes[0].GetLeft()) {
                        lowestFloatBottom = floatBox.GetBottom();
                    }
                }
            }
            if ((clearPropertyValue.Equals(ClearPropertyValue.RIGHT) || isBoth) && lastLeftAndRightBoxes[1] != null) {
                foreach (Rectangle floatBox in floatRendererAreas) {
                    if (floatBox.GetBottom() < lowestFloatBottom && floatBox.GetRight() >= lastLeftAndRightBoxes[1].GetRight()
                        ) {
                        lowestFloatBottom = floatBox.GetBottom();
                    }
                }
            }
            if (lowestFloatBottom < float.MaxValue) {
                clearHeightCorrection = parentBBox.GetTop() - lowestFloatBottom + AbstractRenderer.EPS;
            }
            return clearHeightCorrection;
        }

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

        internal static bool IsRendererFloating(IRenderer renderer) {
            return IsRendererFloating(renderer, renderer.GetProperty<FloatPropertyValue?>(Property.FLOAT));
        }

        internal static bool IsRendererFloating(IRenderer renderer, FloatPropertyValue? kidFloatPropertyVal) {
            int? position = renderer.GetProperty<int?>(Property.POSITION);
            bool notAbsolutePos = position == null || position != LayoutPosition.ABSOLUTE;
            return notAbsolutePos && kidFloatPropertyVal != null && !kidFloatPropertyVal.Equals(FloatPropertyValue.NONE
                );
        }

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
    }
}
