/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Layout.Exceptions;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal sealed class FlexUtil {
        private const float EPSILON = 0.0001F;

        private const float FLEX_GROW_INITIAL_VALUE = 0F;

        private const float FLEX_SHRINK_INITIAL_VALUE = 1F;

        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.FlexUtil));

        private FlexUtil() {
        }

        // Do nothing
        /// <summary>Performs flex layout algorithm.</summary>
        /// <remarks>
        /// Performs flex layout algorithm.
        /// <para />
        /// The algorithm could be found here:
        /// <seealso>https://www.w3.org/TR/css-flexbox-1/#layout-algorithm</seealso>
        /// </remarks>
        /// <param name="flexContainerBBox">bounding box in which flex container should be rendered</param>
        /// <param name="flexContainerRenderer">flex container's renderer</param>
        /// <returns>list of lines</returns>
        public static IList<IList<FlexItemInfo>> CalculateChildrenRectangles(Rectangle flexContainerBBox, FlexContainerRenderer
             flexContainerRenderer) {
            Rectangle layoutBox = flexContainerBBox.Clone();
            flexContainerRenderer.ApplyMarginsBordersPaddings(layoutBox, false);
            // 9.2. Line Length Determination
            // 2. Determine the available main and cross space for the flex items.
            // TODO DEVSIX-5001 min-content and max-content as width are not supported
            // if that dimension of the flex container is being sized under a min or max-content constraint,
            // the available space in that dimension is that constraint;
            float mainSize = GetMainSize(flexContainerRenderer, layoutBox);
            bool isColumnDirection = IsColumnDirection(flexContainerRenderer);
            // We need to have crossSize only if its value is definite.
            float?[] crossSizes = GetCrossSizes(flexContainerRenderer, layoutBox);
            float? crossSize = crossSizes[0];
            if (crossSize == null && isColumnDirection) {
                crossSize = layoutBox.GetWidth();
            }
            float? minCrossSize = crossSizes[1];
            float? maxCrossSize = crossSizes[2];
            float layoutBoxCrossSize = isColumnDirection ? layoutBox.GetWidth() : layoutBox.GetHeight();
            layoutBoxCrossSize = crossSize == null ? layoutBoxCrossSize : Math.Min((float)crossSize, layoutBoxCrossSize
                );
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = CreateFlexItemCalculationInfos(flexContainerRenderer
                , mainSize, layoutBoxCrossSize);
            DetermineFlexBasisAndHypotheticalMainSizeForFlexItems(flexItemCalculationInfos, layoutBoxCrossSize, isColumnDirection
                );
            float? columnGapProp = flexContainerRenderer.GetProperty<float?>(Property.COLUMN_GAP);
            float? rowGapProp = flexContainerRenderer.GetProperty<float?>(Property.ROW_GAP);
            float columnGap = columnGapProp == null ? 0f : (float)columnGapProp;
            float rowGap = rowGapProp == null ? 0f : (float)rowGapProp;
            // 9.3. Main Size Determination
            // 5. Collect flex items into flex lines:
            IList<IList<FlexUtil.FlexItemCalculationInfo>> lines = CollectFlexItemsIntoFlexLines(flexItemCalculationInfos
                , isColumnDirection ? Math.Min(mainSize, layoutBox.GetHeight()) : mainSize, IsSingleLine(flexContainerRenderer
                ), isColumnDirection ? rowGap : columnGap);
            // 6. Resolve the flexible lengths of all the flex items to find their used main size.
            // See §9.7 Resolving Flexible Lengths.
            // 9.7. Resolving Flexible Lengths
            // First, calculate max line size. For column container it should be the default size if width is not set.
            // For row container it is not used currently.
            float maxHypotheticalMainSize = 0;
            foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                float hypotheticalMainSizesSum = 0;
                for (int i = 0; i < line.Count; i++) {
                    if (i != 0) {
                        hypotheticalMainSizesSum += isColumnDirection ? rowGap : columnGap;
                    }
                    FlexUtil.FlexItemCalculationInfo info = line[i];
                    hypotheticalMainSizesSum += info.GetOuterMainSize(info.hypotheticalMainSize);
                }
                maxHypotheticalMainSize = Math.Max(maxHypotheticalMainSize, hypotheticalMainSizesSum);
            }
            float containerMainSize = GetMainSize(flexContainerRenderer, new Rectangle(isColumnDirection ? 0 : maxHypotheticalMainSize
                , isColumnDirection ? maxHypotheticalMainSize : 0));
            if (isColumnDirection) {
                ResolveFlexibleLengths(lines, layoutBox.GetHeight(), containerMainSize, rowGap);
            }
            else {
                ResolveFlexibleLengths(lines, mainSize, columnGap);
            }
            // 9.4. Cross Size Determination
            // 7. Determine the hypothetical cross size of each item by
            // performing layout with the used main size and the available space, treating auto as fit-content.
            DetermineHypotheticalCrossSizeForFlexItems(lines, isColumnDirection, layoutBoxCrossSize);
            // 8. Calculate the cross size of each flex line.
            IList<float> lineCrossSizes = CalculateCrossSizeOfEachFlexLine(lines, minCrossSize, crossSize, maxCrossSize
                );
            // If the flex container is single-line, then clamp the line’s cross-size to be within
            // the container’s computed min and max cross sizes. Note that if CSS 2.1’s definition of min/max-width/height
            // applied more generally, this behavior would fall out automatically.
            // 9. Handle 'align-content: stretch'.
            float? currentCrossSize = isColumnDirection ? layoutBoxCrossSize : crossSize;
            HandleAlignContentStretch(flexContainerRenderer, lines, currentCrossSize, lineCrossSizes, layoutBox, isColumnDirection
                 ? columnGap : rowGap);
            // TODO DEVSIX-2090 visibility-collapse items are not supported
            // 10. Collapse visibility:collapse items.
            // 11. Determine the used cross size of each flex item.
            DetermineUsedCrossSizeOfEachFlexItem(lines, lineCrossSizes, flexContainerRenderer);
            // 9.5. Main-Axis Alignment
            // 12. Align the items along the main-axis per justify-content.
            ApplyJustifyContent(lines, flexContainerRenderer, isColumnDirection ? layoutBox.GetHeight() : mainSize, containerMainSize
                , isColumnDirection ? rowGap : columnGap);
            // 9.6. Cross-Axis Alignment
            // TODO DEVSIX-5002 margin: auto is not supported
            // 13. Resolve cross-axis auto margins
            // 14. Align all flex items along the cross-axis
            ApplyAlignItemsAndAlignSelf(lines, flexContainerRenderer, lineCrossSizes);
            // 15. Determine the flex container’s used cross size
            // 16. Align all flex lines per align-content.
            ApplyAlignContent(lines, flexContainerRenderer, crossSize, layoutBox, isColumnDirection ? columnGap : rowGap
                );
            ApplyGap(lines, columnGap, rowGap, flexContainerRenderer);
            // Convert FlexItemCalculationInfo's into FlexItemInfo's
            IList<IList<FlexItemInfo>> layoutTable = new List<IList<FlexItemInfo>>();
            foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                IList<FlexItemInfo> layoutLine = new List<FlexItemInfo>();
                foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                    layoutLine.Add(new FlexItemInfo(info.renderer, info.ToRectangle()));
                }
                layoutTable.Add(layoutLine);
            }
            return layoutTable;
        }

        private static void ApplyAlignContent(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, FlexContainerRenderer
             renderer, float? crossSize, Rectangle layoutBox, float gap) {
            AlignContentPropertyValue alignContent = (AlignContentPropertyValue)renderer.GetProperty<AlignContentPropertyValue?
                >(Property.ALIGN_CONTENT, AlignContentPropertyValue.NORMAL);
            bool isFirstFlexStart = (bool)renderer.GetProperty<bool?>(Property.FLEX_FORCE_START_ON_TOP, false);
            if (crossSize != null) {
                if (renderer.IsWrapReverse()) {
                    JavaCollectionsUtil.Reverse(lines);
                }
                float boxSize;
                float freeSpace;
                if (IsColumnDirection(renderer)) {
                    boxSize = Math.Min(layoutBox.GetWidth(), (float)crossSize);
                }
                else {
                    boxSize = Math.Min(layoutBox.GetHeight(), (float)crossSize);
                }
                float lineCrossSize = 0;
                int columnsOnPage = 0;
                foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                    float maxItemSize = GetItemMaxCrossSize(line);
                    if (IsColumnDirection(renderer)) {
                        if (lineCrossSize + maxItemSize <= boxSize) {
                            lineCrossSize += maxItemSize;
                            columnsOnPage++;
                        }
                        else {
                            break;
                        }
                    }
                    else {
                        lineCrossSize += maxItemSize;
                    }
                }
                int linesOnPage = IsColumnDirection(renderer) ? columnsOnPage : lines.Count;
                float gapSize = (linesOnPage - 1) * gap;
                freeSpace = boxSize - lineCrossSize - gapSize;
                ApplyAlignContent(lines, linesOnPage, alignContent, freeSpace < 0 ? 0 : freeSpace, IsColumnDirection(renderer
                    ), isFirstFlexStart, renderer.IsWrapReverse());
                if (renderer.IsWrapReverse()) {
                    JavaCollectionsUtil.Reverse(lines);
                }
            }
        }

        private static void ApplyAlignContent(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, int linesOnPage
            , AlignContentPropertyValue alignContent, float freeSpace, bool isColumnDirection, bool isFirstFlexStart
            , bool isWrapReverse) {
            if (!lines.IsEmpty()) {
                switch (alignContent) {
                    case AlignContentPropertyValue.CENTER: {
                        ApplyCenterAlignment(lines, freeSpace, isColumnDirection, isFirstFlexStart);
                        break;
                    }

                    case AlignContentPropertyValue.FLEX_END: {
                        if (!isWrapReverse) {
                            ApplyFlexEndAlignment(lines, freeSpace, isColumnDirection, isFirstFlexStart);
                        }
                        break;
                    }

                    case AlignContentPropertyValue.END: {
                        ApplyFlexEndAlignment(lines, freeSpace, isColumnDirection, isFirstFlexStart);
                        break;
                    }

                    case AlignContentPropertyValue.SPACE_BETWEEN: {
                        ApplySpaceBetweenAlignment(lines, linesOnPage, freeSpace, isColumnDirection);
                        break;
                    }

                    case AlignContentPropertyValue.SPACE_AROUND: {
                        ApplySpaceAroundAlignment(lines, linesOnPage, freeSpace, isColumnDirection, isFirstFlexStart);
                        break;
                    }

                    case AlignContentPropertyValue.SPACE_EVENLY: {
                        ApplySpaceEvenlyAlignment(lines, linesOnPage, freeSpace, isColumnDirection, isFirstFlexStart);
                        break;
                    }

                    case AlignContentPropertyValue.FLEX_START: {
                        if (isWrapReverse) {
                            ApplyFlexEndAlignment(lines, freeSpace, isColumnDirection, isFirstFlexStart);
                        }
                        break;
                    }

                    case AlignContentPropertyValue.START:
                    default: {
                        break;
                    }
                }
            }
        }

        // We don't need to do anything in these cases
        private static void ApplyFlexEndAlignment(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, float freeSpace
            , bool isColumnDirection, bool isFirstFlexStart) {
            if (isFirstFlexStart && lines.Count < 2) {
                return;
            }
            IList<FlexUtil.FlexItemCalculationInfo> targetLine = isFirstFlexStart && lines.Count > 1 ? lines[1] : lines
                [0];
            foreach (FlexUtil.FlexItemCalculationInfo item in targetLine) {
                if (isColumnDirection) {
                    item.xShift = freeSpace;
                }
                else {
                    item.yShift = freeSpace;
                }
            }
        }

        private static void ApplySpaceBetweenAlignment(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, int linesOnPage
            , float freeSpace, bool isColumnDirection) {
            if (linesOnPage > 1) {
                float indentation = freeSpace / (linesOnPage - 1);
                for (int i = 1; i < linesOnPage; i++) {
                    foreach (FlexUtil.FlexItemCalculationInfo item in lines[i]) {
                        if (isColumnDirection) {
                            item.xShift = indentation;
                        }
                        else {
                            item.yShift = indentation;
                        }
                    }
                }
            }
        }

        private static void ApplySpaceEvenlyAlignment(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, int linesOnPage
            , float freeSpace, bool isColumnDirection, bool isFirstFlexStart) {
            float indentation = freeSpace / (linesOnPage + 1);
            int startIndex = isFirstFlexStart ? 1 : 0;
            for (int i = startIndex; i < linesOnPage; i++) {
                foreach (FlexUtil.FlexItemCalculationInfo item in lines[i]) {
                    if (isColumnDirection) {
                        item.xShift = indentation;
                    }
                    else {
                        item.yShift = indentation;
                    }
                }
            }
        }

        private static void ApplySpaceAroundAlignment(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, int linesOnPage
            , float freeSpace, bool isColumnDirection, bool isFirstFlexStart) {
            int startIndex = isFirstFlexStart ? 1 : 0;
            float indentation = freeSpace / linesOnPage;
            for (int i = startIndex; i < linesOnPage; i++) {
                float shift = (i == 0) ? (indentation / 2) : indentation;
                foreach (FlexUtil.FlexItemCalculationInfo item in lines[i]) {
                    if (isColumnDirection) {
                        item.xShift = shift;
                    }
                    else {
                        item.yShift = shift;
                    }
                }
            }
        }

        private static void ApplyCenterAlignment(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, float freeSpace
            , bool isColumnDirection, bool isFirstFlexStart) {
            if (isFirstFlexStart && lines.Count < 2) {
                return;
            }
            float indentation = freeSpace / 2;
            int targetIndex = isFirstFlexStart ? 1 : 0;
            foreach (FlexUtil.FlexItemCalculationInfo item in lines[targetIndex]) {
                if (isColumnDirection) {
                    item.xShift = indentation;
                }
                else {
                    item.yShift = indentation;
                }
            }
        }

        private static float GetItemMaxCrossSize(IList<FlexUtil.FlexItemCalculationInfo> line) {
            float maxItemSize = 0;
            foreach (FlexUtil.FlexItemCalculationInfo item in line) {
                if (item.GetOuterCrossSize(item.crossSize) > maxItemSize) {
                    maxItemSize = item.GetOuterCrossSize(item.crossSize);
                }
            }
            return maxItemSize;
        }

//\cond DO_NOT_DOCUMENT
        internal static bool IsColumnDirection(FlexContainerRenderer renderer) {
            FlexDirectionPropertyValue flexDir = (FlexDirectionPropertyValue)renderer.GetProperty<FlexDirectionPropertyValue?
                >(Property.FLEX_DIRECTION, FlexDirectionPropertyValue.ROW);
            return FlexDirectionPropertyValue.COLUMN == flexDir || FlexDirectionPropertyValue.COLUMN_REVERSE == flexDir;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static bool IsSingleLine(FlexContainerRenderer flexContainerRenderer) {
            return !flexContainerRenderer.HasProperty(Property.FLEX_WRAP) || FlexWrapPropertyValue.NOWRAP == flexContainerRenderer
                .GetProperty<FlexWrapPropertyValue?>(Property.FLEX_WRAP);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static float GetMainSize(FlexContainerRenderer renderer, Rectangle layoutBox) {
            bool isColumnDirection = IsColumnDirection(renderer);
            float layoutBoxMainSize;
            float? mainSize;
            float? maxDimension = null;
            float? minDimension = null;
            if (isColumnDirection) {
                layoutBoxMainSize = layoutBox.GetHeight();
                mainSize = renderer.RetrieveHeight();
                maxDimension = ResolveUnitValue(renderer, Property.MAX_HEIGHT, layoutBoxMainSize);
                minDimension = ResolveUnitValue(renderer, Property.MIN_HEIGHT, layoutBoxMainSize);
            }
            else {
                layoutBoxMainSize = layoutBox.GetWidth();
                mainSize = renderer.RetrieveWidth(layoutBoxMainSize);
                maxDimension = ResolveUnitValue(renderer, Property.MAX_WIDTH, layoutBoxMainSize);
                minDimension = ResolveUnitValue(renderer, Property.MIN_WIDTH, layoutBoxMainSize);
            }
            // TODO DEVSIX-5001 min-content and max-content as width are not supported
            // if that dimension of the flex container is being sized under a min or max-content constraint,
            // the available space in that dimension is that constraint;
            if (mainSize == null) {
                mainSize = layoutBoxMainSize;
            }
            if (minDimension != null && minDimension > mainSize) {
                mainSize = minDimension;
            }
            if (maxDimension != null && (minDimension == null || maxDimension > minDimension) && maxDimension < mainSize
                ) {
                mainSize = maxDimension;
            }
            return (float)mainSize;
        }
//\endcond

        private static float? ResolveUnitValue(FlexContainerRenderer renderer, int property, float baseValue) {
            UnitValue value = renderer.GetPropertyAsUnitValue(property);
            if (value == null) {
                return null;
            }
            if (value.IsPercentValue()) {
                return value.GetValue() / 100 * baseValue;
            }
            return value.GetValue();
        }

        private static float?[] GetCrossSizes(FlexContainerRenderer renderer, Rectangle layoutBox) {
            bool isColumnDirection = IsColumnDirection(renderer);
            return new float?[] { isColumnDirection ? renderer.RetrieveWidth(layoutBox.GetWidth()) : renderer.RetrieveHeight
                (), isColumnDirection ? renderer.RetrieveMinWidth(layoutBox.GetWidth()) : renderer.RetrieveMinHeight()
                , isColumnDirection ? renderer.RetrieveMaxWidth(layoutBox.GetWidth()) : renderer.RetrieveMaxHeight() };
        }

//\cond DO_NOT_DOCUMENT
        internal static void DetermineFlexBasisAndHypotheticalMainSizeForFlexItems(IList<FlexUtil.FlexItemCalculationInfo
            > flexItemCalculationInfos, float crossSize, bool isColumnDirection) {
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                // 3. Determine the flex base size and hypothetical main size of each item:
                AbstractRenderer renderer = info.renderer;
                // TODO DEVSIX-5255 Support aspect-ratio property
                // B. If the flex item has ...
                // an intrinsic aspect ratio,
                // a used flex basis of content, and
                // a definite cross size,
                // then the flex base size is calculated from its inner cross size
                // and the flex item’s intrinsic aspect ratio.
                float? definiteCrossSize = null;
                if (renderer.HasAspectRatio()) {
                    definiteCrossSize = isColumnDirection ? renderer.RetrieveWidth(crossSize) : renderer.RetrieveHeight();
                }
                if (info.flexBasisContent && definiteCrossSize != null) {
                    float aspectRatio = (float)renderer.GetAspectRatio();
                    info.flexBaseSize = isColumnDirection ? (float)definiteCrossSize / aspectRatio : (float)definiteCrossSize 
                        * aspectRatio;
                }
                else {
                    // A. If the item has a definite used flex basis, that’s the flex base size.
                    // TODO DEVSIX-5001 width: max-content is not supported
                    // E. Otherwise, size the item into the available space using its used flex basis in place of its main
                    // size, treating a value of content as max-content (calculated during FlexItemCalculationInfo creation).
                    // If a cross size is needed to determine the main size (e.g. when the flex item’s main size is in its
                    // block axis (column direction), or when it has a preferred aspect ratio) and the flex item’s cross
                    // size is auto and not definite, in this calculation use fit-content as the flex item’s cross size.
                    // The flex base size is the item’s resulting main size.
                    info.flexBaseSize = info.flexBasis;
                }
                // TODO DEVSIX-5001 width: min-content and max-content are not supported, so we can't check whether
                //  the flex container is being sized under a min-content or max-content constraint
                // C. If the used flex basis is content or depends on its available space,
                // and the flex container is being sized under a min-content or max-content constraint
                // (e.g. when performing automatic table layout [CSS21]), size the item under that constraint.
                // The flex base size is the item’s resulting main size. E.g. for flex container parent with
                // width: max-content flex item should be also layouted with width: max-content.
                // TODO DEVSIX-5001 width: min-content and max-content are not supported
                // D. Otherwise, if the used flex basis is content or depends on its available space,
                // the available main size is infinite, and the flex item’s inline axis is parallel to the main axis,
                // lay the item out using the rules for a box in an orthogonal flow [CSS3-WRITING-MODES].
                // The flex base size is the item’s max-content main size.
                // The hypothetical main size is the item’s flex base size clamped
                // according to its used min and max main sizes (and flooring the content box size at zero).
                info.hypotheticalMainSize = Math.Max(0, Math.Min(Math.Max(info.minContent, info.flexBaseSize), info.maxContent
                    ));
                // Each item in the flex line has a target main size, initially set to its flex base size
                info.mainSize = info.hypotheticalMainSize;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // Note: We assume that it was resolved on some upper level
        // 4. Determine the main size of the flex container
        internal static IList<IList<FlexUtil.FlexItemCalculationInfo>> CollectFlexItemsIntoFlexLines(IList<FlexUtil.FlexItemCalculationInfo
            > flexItemCalculationInfos, float mainSize, bool isSingleLine, float gap) {
            IList<IList<FlexUtil.FlexItemCalculationInfo>> lines = new List<IList<FlexUtil.FlexItemCalculationInfo>>();
            IList<FlexUtil.FlexItemCalculationInfo> currentLineInfos = new List<FlexUtil.FlexItemCalculationInfo>();
            if (isSingleLine) {
                currentLineInfos.AddAll(flexItemCalculationInfos);
            }
            else {
                float occupiedLineSpace = 0;
                foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                    occupiedLineSpace += info.GetOuterMainSize(info.hypotheticalMainSize);
                    if (occupiedLineSpace > mainSize + EPSILON) {
                        // If the very first uncollected item wouldn’t fit, collect just it into the line.
                        if (currentLineInfos.IsEmpty()) {
                            currentLineInfos.Add(info);
                            lines.Add(currentLineInfos);
                            currentLineInfos = new List<FlexUtil.FlexItemCalculationInfo>();
                            occupiedLineSpace = 0;
                        }
                        else {
                            lines.Add(currentLineInfos);
                            currentLineInfos = new List<FlexUtil.FlexItemCalculationInfo>();
                            currentLineInfos.Add(info);
                            occupiedLineSpace = info.GetOuterMainSize(info.hypotheticalMainSize);
                            occupiedLineSpace += gap;
                        }
                    }
                    else {
                        currentLineInfos.Add(info);
                        occupiedLineSpace += gap;
                    }
                }
            }
            // the last line should be added
            if (!currentLineInfos.IsEmpty()) {
                lines.Add(currentLineInfos);
            }
            return lines;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void ResolveFlexibleLengths(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, float mainSize
            , float gap) {
            foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                // 1. Determine the used flex factor.
                float hypotheticalMainSizesSum = 0;
                foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                    hypotheticalMainSizesSum += info.GetOuterMainSize(info.hypotheticalMainSize);
                }
                hypotheticalMainSizesSum += gap * (line.Count - 1);
                // if the sum is less than the flex container’s inner main size,
                // use the flex grow factor for the rest of this algorithm; otherwise, use the flex shrink factor.
                bool isFlexGrow = hypotheticalMainSizesSum < mainSize;
                // 2. Size inflexible items.
                foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                    if (isFlexGrow) {
                        if (IsZero(info.flexGrow) || info.flexBaseSize > info.hypotheticalMainSize) {
                            info.mainSize = info.hypotheticalMainSize;
                            info.isFrozen = true;
                        }
                    }
                    else {
                        if (IsZero(info.flexShrink) || info.flexBaseSize < info.hypotheticalMainSize) {
                            info.mainSize = info.hypotheticalMainSize;
                            info.isFrozen = true;
                        }
                    }
                }
                // 3. Calculate initial free space.
                float initialFreeSpace = CalculateFreeSpace(line, mainSize, gap);
                // 4. Loop:
                // a. Check for flexible items
                while (HasFlexibleItems(line)) {
                    // b. Calculate the remaining free space as for initial free space, above.
                    float remainingFreeSpace = CalculateFreeSpace(line, mainSize, gap);
                    float flexFactorSum = 0;
                    foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                        if (!info.isFrozen) {
                            flexFactorSum += isFlexGrow ? info.flexGrow : info.flexShrink;
                        }
                    }
                    // If the sum of the unfrozen flex items’ flex factors is less than one,
                    // multiply the initial free space by this sum.
                    // If the magnitude of this value is less than the magnitude of the remaining free space,
                    // use this as the remaining free space.
                    if (flexFactorSum < 1 && Math.Abs(remainingFreeSpace) > Math.Abs(initialFreeSpace * flexFactorSum)) {
                        remainingFreeSpace = initialFreeSpace * flexFactorSum;
                    }
                    // c. Distribute free space proportional to the flex factors
                    if (!IsZero(remainingFreeSpace)) {
                        float scaledFlexShrinkFactorsSum = 0;
                        foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                            if (!info.isFrozen) {
                                if (isFlexGrow) {
                                    float ratio = info.flexGrow / flexFactorSum;
                                    info.mainSize = info.flexBaseSize + remainingFreeSpace * ratio;
                                }
                                else {
                                    info.scaledFlexShrinkFactor = info.flexShrink * info.flexBaseSize;
                                    scaledFlexShrinkFactorsSum += info.scaledFlexShrinkFactor;
                                }
                            }
                        }
                        if (!IsZero(scaledFlexShrinkFactorsSum)) {
                            foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                                if (!info.isFrozen && !isFlexGrow) {
                                    float ratio = info.scaledFlexShrinkFactor / scaledFlexShrinkFactorsSum;
                                    info.mainSize = info.flexBaseSize - Math.Abs(remainingFreeSpace) * ratio;
                                }
                            }
                        }
                    }
                    else {
                        // This is not mentioned in the algo, however we must initialize main size (target main size)
                        foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                            if (!info.isFrozen) {
                                info.mainSize = info.flexBaseSize;
                            }
                        }
                    }
                    // d. Fix min/max violations.
                    float sum = 0;
                    foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                        if (!info.isFrozen) {
                            // Clamp each non-frozen item’s target main size by its used min and max main sizes
                            // and floor its content-box size at zero.
                            float clampedSize = Math.Min(Math.Max(info.mainSize, info.minContent), info.maxContent);
                            if (info.mainSize > clampedSize) {
                                info.isMaxViolated = true;
                            }
                            else {
                                if (info.mainSize < clampedSize) {
                                    info.isMinViolated = true;
                                }
                            }
                            sum += clampedSize - info.mainSize;
                            info.mainSize = clampedSize;
                        }
                    }
                    foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                        if (!info.isFrozen) {
                            if (IsZero(sum) || (0 < sum && info.isMinViolated) || (0 > sum && info.isMaxViolated)) {
                                info.isFrozen = true;
                            }
                        }
                    }
                }
            }
        }
//\endcond

        // 9.5. Main-Axis Alignment
        // 12. Distribute any remaining free space.
        // Once any of the to-do remarks below is resolved, one should add a corresponding block,
        // which will be triggered if 0 < remaining free space
        // TODO DEVSIX-5002 margin: auto is not supported
        // If the remaining free space is positive and at least one main-axis margin on this line is auto,
        // distribute the free space equally among these margins. Otherwise, set all auto margins to zero.
        private static void ResolveFlexibleLengths(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, float layoutBoxSize
            , float containerSize, float gap) {
            ResolveFlexibleLengths(lines, containerSize, gap);
            if (lines.Count == 1 && layoutBoxSize < containerSize - EPSILON) {
                IList<FlexUtil.FlexItemCalculationInfo> lineToRecalculate = new List<FlexUtil.FlexItemCalculationInfo>();
                float mainSize = 0;
                foreach (FlexUtil.FlexItemCalculationInfo itemInfo in lines[0]) {
                    mainSize += itemInfo.GetOuterMainSize(itemInfo.mainSize);
                    if (mainSize < layoutBoxSize - EPSILON) {
                        itemInfo.isFrozen = false;
                        lineToRecalculate.Add(itemInfo);
                    }
                    else {
                        break;
                    }
                }
                if (!lineToRecalculate.IsEmpty()) {
                    IList<IList<FlexUtil.FlexItemCalculationInfo>> updatedLines = new List<IList<FlexUtil.FlexItemCalculationInfo
                        >>();
                    updatedLines.Add(lineToRecalculate);
                    ResolveFlexibleLengths(updatedLines, layoutBoxSize, gap);
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static void DetermineHypotheticalCrossSizeForFlexItems(IList<IList<FlexUtil.FlexItemCalculationInfo
            >> lines, bool isColumnDirection, float crossSize) {
            foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                    DetermineHypotheticalCrossSizeForFlexItem(info, isColumnDirection, crossSize);
                }
            }
        }
//\endcond

        private static void DetermineHypotheticalCrossSizeForFlexItem(FlexUtil.FlexItemCalculationInfo info, bool 
            isColumnDirection, float crossSize) {
            if (info.renderer is FlexContainerRenderer && ((FlexContainerRenderer)info.renderer).GetHypotheticalCrossSize
                (info.mainSize) != null) {
                // Take from cache
                info.hypotheticalCrossSize = ((FlexContainerRenderer)info.renderer).GetHypotheticalCrossSize(info.mainSize
                    ).Value;
            }
            else {
                if (isColumnDirection) {
                    MinMaxWidth minMaxWidth = info.renderer.GetMinMaxWidth(crossSize);
                    info.hypotheticalCrossSize = info.GetInnerCrossSize(Math.Max(Math.Min(minMaxWidth.GetMaxWidth(), crossSize
                        ), minMaxWidth.GetMinWidth()));
                    // Cache hypotheticalCrossSize for FlexContainerRenderer
                    if (info.renderer is FlexContainerRenderer) {
                        ((FlexContainerRenderer)info.renderer).SetHypotheticalCrossSize(info.mainSize, info.hypotheticalCrossSize);
                    }
                }
                else {
                    UnitValue prevMainSize = info.renderer.ReplaceOwnProperty<UnitValue>(Property.WIDTH, UnitValue.CreatePointValue
                        (info.mainSize));
                    UnitValue prevMinMainSize = info.renderer.ReplaceOwnProperty<UnitValue>(Property.MIN_WIDTH, null);
                    UnitValue prevMaxMainSize = info.renderer.ReplaceOwnProperty<UnitValue>(Property.MAX_WIDTH, null);
                    float? horizontalScaling = (float?)info.renderer.ReplaceOwnProperty<Object>(Property.HORIZONTAL_SCALING, 1f
                        );
                    float? verticalScaling = (float?)info.renderer.ReplaceOwnProperty<Object>(Property.VERTICAL_SCALING, 1f);
                    info.renderer.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, InlineVerticalAlignmentType.BOTTOM);
                    LayoutResult result = info.renderer.Layout(new LayoutContext(new LayoutArea(0, new Rectangle(AbstractRenderer
                        .INF, AbstractRenderer.INF))));
                    info.renderer.ReturnBackOwnProperty(Property.WIDTH, prevMainSize);
                    info.renderer.ReturnBackOwnProperty(Property.MIN_WIDTH, prevMinMainSize);
                    info.renderer.ReturnBackOwnProperty(Property.MAX_WIDTH, prevMaxMainSize);
                    info.renderer.ReturnBackOwnProperty(Property.HORIZONTAL_SCALING, horizontalScaling);
                    info.renderer.ReturnBackOwnProperty(Property.VERTICAL_SCALING, verticalScaling);
                    // Since main size is clamped with min-width, we do expect the result to be full
                    if (result.GetStatus() == LayoutResult.FULL) {
                        info.hypotheticalCrossSize = info.GetInnerCrossSize(result.GetOccupiedArea().GetBBox().GetHeight());
                        // Cache hypotheticalCrossSize for FlexContainerRenderer
                        if (info.renderer is FlexContainerRenderer) {
                            ((FlexContainerRenderer)info.renderer).SetHypotheticalCrossSize(info.mainSize, info.hypotheticalCrossSize);
                        }
                    }
                    else {
                        logger.LogError(iText.IO.Logs.IoLogMessageConstant.FLEX_ITEM_LAYOUT_RESULT_IS_NOT_FULL);
                        info.hypotheticalCrossSize = 0;
                    }
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static IList<float> CalculateColumnDirectionCrossSizes(IList<IList<FlexItemInfo>> lines) {
            IList<float> lineCrossSizes = new List<float>();
            foreach (IList<FlexItemInfo> line in lines) {
                float flexLinesCrossSize = 0;
                float largestCrossSize = 0;
                foreach (FlexItemInfo info in line) {
                    // TODO DEVSIX-5002 Flex items whose cross-axis margins are both auto shouldn't be collected
                    // TODO DEVSIX-5167 Support BASELINE as align-self
                    largestCrossSize = Math.Max(largestCrossSize, info.GetRectangle().GetWidth());
                    flexLinesCrossSize = Math.Max(0, largestCrossSize);
                }
                lineCrossSizes.Add(flexLinesCrossSize);
            }
            return lineCrossSizes;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static IList<float> CalculateCrossSizeOfEachFlexLine(IList<IList<FlexUtil.FlexItemCalculationInfo
            >> lines, float? minCrossSize, float? crossSize, float? maxCrossSize) {
            bool isSingleLine = lines.Count == 1;
            IList<float> lineCrossSizes = new List<float>();
            if (isSingleLine && crossSize != null) {
                lineCrossSizes.Add((float)crossSize);
            }
            else {
                foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                    float flexLinesCrossSize = 0;
                    float largestHypotheticalCrossSize = 0;
                    foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                        // 1. Collect all the flex items whose inline-axis is parallel to the main-axis,
                        // whose align-self is baseline, and whose cross-axis margins are both non-auto.
                        // Find the largest of the distances between each item’s baseline and
                        // its hypothetical outer cross-start edge, and the largest of the distances
                        // between each item’s baseline and its hypothetical outer cross-end edge, and sum these two values.
                        // TODO DEVSIX-5002 margin: auto is not supported => "cross-axis margins are both non-auto" is true
                        // TODO DEVSIX-5167 Support BASELINE as align-self
                        // 2. Among all the items not collected by the previous step,
                        // find the largest outer hypothetical cross size.
                        if (largestHypotheticalCrossSize < info.GetOuterCrossSize(info.hypotheticalCrossSize)) {
                            largestHypotheticalCrossSize = info.GetOuterCrossSize(info.hypotheticalCrossSize);
                        }
                        flexLinesCrossSize = Math.Max(0, largestHypotheticalCrossSize);
                    }
                    // 3. If the flex container is single-line, then clamp the line’s cross-size to be
                    // within the container’s computed min and max cross sizes
                    if (isSingleLine) {
                        if (null != minCrossSize) {
                            flexLinesCrossSize = Math.Max((float)minCrossSize, flexLinesCrossSize);
                        }
                        if (null != maxCrossSize) {
                            flexLinesCrossSize = Math.Min((float)maxCrossSize, flexLinesCrossSize);
                        }
                    }
                    lineCrossSizes.Add(flexLinesCrossSize);
                }
            }
            return lineCrossSizes;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void HandleAlignContentStretch(FlexContainerRenderer flexContainerRenderer, IList<IList<FlexUtil.FlexItemCalculationInfo
            >> lines, float? crossSize, IList<float> lineCrossSizes, Rectangle layoutBox, float gap) {
            AlignContentPropertyValue alignContent = (AlignContentPropertyValue)flexContainerRenderer.GetProperty<AlignContentPropertyValue?
                >(Property.ALIGN_CONTENT, AlignContentPropertyValue.STRETCH);
            if (crossSize != null && (alignContent == AlignContentPropertyValue.STRETCH || alignContent == AlignContentPropertyValue
                .NORMAL)) {
                // Line order becomes important for alignment
                if (flexContainerRenderer.IsWrapReverse()) {
                    JavaCollectionsUtil.Reverse(lineCrossSizes);
                    JavaCollectionsUtil.Reverse(lines);
                }
                IList<float> currentPageLineCrossSizes = RetrieveCurrentPageLineCrossSizes(flexContainerRenderer, lines, lineCrossSizes
                    , crossSize, layoutBox);
                if (!currentPageLineCrossSizes.IsEmpty()) {
                    float flexLinesCrossSizesSum = 0;
                    for (int i = 0; i < currentPageLineCrossSizes.Count; i++) {
                        float size = currentPageLineCrossSizes[i];
                        flexLinesCrossSizesSum += size;
                        if (i != 0) {
                            flexLinesCrossSizesSum += gap;
                        }
                    }
                    if (flexLinesCrossSizesSum < crossSize - EPSILON) {
                        float addition = ((float)crossSize - flexLinesCrossSizesSum) / currentPageLineCrossSizes.Count;
                        for (int i = 0; i < currentPageLineCrossSizes.Count; i++) {
                            lineCrossSizes[i] = lineCrossSizes[i] + addition;
                        }
                    }
                }
                // Reverse back
                if (flexContainerRenderer.IsWrapReverse()) {
                    JavaCollectionsUtil.Reverse(lineCrossSizes);
                    JavaCollectionsUtil.Reverse(lines);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void DetermineUsedCrossSizeOfEachFlexItem(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines
            , IList<float> lineCrossSizes, FlexContainerRenderer flexContainerRenderer) {
            bool isColumnDirection = IsColumnDirection(flexContainerRenderer);
            AlignmentPropertyValue alignItems = (AlignmentPropertyValue)flexContainerRenderer.GetProperty<AlignmentPropertyValue?
                >(Property.ALIGN_ITEMS, AlignmentPropertyValue.STRETCH);
            System.Diagnostics.Debug.Assert(lines.Count == lineCrossSizes.Count);
            for (int i = 0; i < lines.Count; i++) {
                foreach (FlexUtil.FlexItemCalculationInfo info in lines[i]) {
                    // TODO DEVSIX-5002 margin: auto is not supported
                    // If a flex item has align-self: stretch, its computed cross size property is auto,
                    // and neither of its cross-axis margins are auto,
                    // the used outer cross size is the used cross size of its flex line,
                    // clamped according to the item’s used min and max cross sizes.
                    // Otherwise, the used cross size is the item’s hypothetical cross size.
                    // Note that this step doesn't affect the main size of the flex item, even if it has aspect ratio.
                    // Also note that for some reason browsers do not respect such a rule from the specification
                    AbstractRenderer infoRenderer = info.renderer;
                    AlignmentPropertyValue alignSelf = (AlignmentPropertyValue)infoRenderer.GetProperty<AlignmentPropertyValue?
                        >(Property.ALIGN_SELF, alignItems);
                    // TODO DEVSIX-5002 Stretch value shall be ignored if margin auto for cross axis is set
                    bool definiteCrossSize = isColumnDirection ? info.renderer.HasProperty(Property.WIDTH) : info.renderer.HasProperty
                        (Property.HEIGHT);
                    if ((alignSelf == AlignmentPropertyValue.STRETCH || alignSelf == AlignmentPropertyValue.NORMAL) && !definiteCrossSize
                        ) {
                        info.crossSize = info.GetInnerCrossSize(lineCrossSizes[i]);
                        float? maxCrossSize = isColumnDirection ? infoRenderer.RetrieveMaxWidth(lineCrossSizes[i]) : infoRenderer.
                            RetrieveMaxHeight();
                        if (maxCrossSize != null) {
                            info.crossSize = Math.Min((float)maxCrossSize, info.crossSize);
                        }
                        float? minCrossSize = isColumnDirection ? infoRenderer.RetrieveMinWidth(lineCrossSizes[i]) : infoRenderer.
                            RetrieveMinHeight();
                        if (minCrossSize != null) {
                            info.crossSize = Math.Max((float)minCrossSize, info.crossSize);
                        }
                    }
                    else {
                        info.crossSize = info.hypotheticalCrossSize;
                    }
                }
            }
        }
//\endcond

        private static float? RetrieveMaxHeightForMainDirection(AbstractRenderer renderer) {
            float? maxHeight = renderer.RetrieveMaxHeight();
            return renderer.HasProperty(Property.MAX_HEIGHT) ? maxHeight : null;
        }

        private static float? RetrieveMinHeightForMainDirection(AbstractRenderer renderer) {
            float? minHeight = renderer.RetrieveMinHeight();
            return renderer.HasProperty(Property.MIN_HEIGHT) && renderer.GetProperty<UnitValue>(Property.MIN_HEIGHT) !=
                 null ? minHeight : null;
        }

        private static void ApplyAlignItemsAndAlignSelf(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, FlexContainerRenderer
             renderer, IList<float> lineCrossSizes) {
            bool isColumnDirection = IsColumnDirection(renderer);
            AlignmentPropertyValue itemsAlignment = (AlignmentPropertyValue)renderer.GetProperty<AlignmentPropertyValue?
                >(Property.ALIGN_ITEMS, AlignmentPropertyValue.STRETCH);
            System.Diagnostics.Debug.Assert(lines.Count == lineCrossSizes.Count);
            // Line order becomes important for counting nextLineShift
            if (renderer.IsWrapReverse()) {
                JavaCollectionsUtil.Reverse(lines);
                JavaCollectionsUtil.Reverse(lineCrossSizes);
            }
            float lineShift;
            float nextLineShift = 0;
            for (int i = 0; i < lines.Count; ++i) {
                lineShift = nextLineShift;
                IList<FlexUtil.FlexItemCalculationInfo> line = lines[i];
                float lineCrossSize = lineCrossSizes[i];
                // Used to calculate an extra space between the right/bottom point of the current line and left/top point
                // of the next line
                nextLineShift = lineCrossSize - line[0].GetOuterCrossSize(line[0].crossSize);
                foreach (FlexUtil.FlexItemCalculationInfo itemInfo in line) {
                    if (isColumnDirection) {
                        itemInfo.xShift = lineShift;
                    }
                    else {
                        itemInfo.yShift = lineShift;
                    }
                    AlignmentPropertyValue selfAlignment = (AlignmentPropertyValue)itemInfo.renderer.GetProperty<AlignmentPropertyValue?
                        >(Property.ALIGN_SELF, itemsAlignment);
                    float freeSpace = lineCrossSize - itemInfo.GetOuterCrossSize(itemInfo.crossSize);
                    nextLineShift = Math.Min(nextLineShift, freeSpace);
                    switch (selfAlignment) {
                        case AlignmentPropertyValue.SELF_END:
                        case AlignmentPropertyValue.END: {
                            if (isColumnDirection) {
                                itemInfo.xShift += freeSpace;
                            }
                            else {
                                itemInfo.yShift += freeSpace;
                            }
                            nextLineShift = 0;
                            break;
                        }

                        case AlignmentPropertyValue.FLEX_END: {
                            if (!renderer.IsWrapReverse()) {
                                if (isColumnDirection) {
                                    itemInfo.xShift += freeSpace;
                                }
                                else {
                                    itemInfo.yShift += freeSpace;
                                }
                                nextLineShift = 0;
                            }
                            break;
                        }

                        case AlignmentPropertyValue.CENTER: {
                            if (isColumnDirection) {
                                itemInfo.xShift += freeSpace / 2;
                            }
                            else {
                                itemInfo.yShift += freeSpace / 2;
                            }
                            nextLineShift = Math.Min(nextLineShift, freeSpace / 2);
                            break;
                        }

                        case AlignmentPropertyValue.NORMAL:
                        case AlignmentPropertyValue.STRETCH: {
                            bool isFirstFlexStart = (bool)renderer.GetProperty<bool?>(Property.FLEX_FORCE_START_ON_TOP, false);
                            if (isFirstFlexStart) {
                                break;
                            }
                            goto case AlignmentPropertyValue.FLEX_START;
                        }

                        case AlignmentPropertyValue.FLEX_START: {
                            if (renderer.IsWrapReverse()) {
                                if (isColumnDirection) {
                                    itemInfo.xShift += freeSpace;
                                }
                                else {
                                    itemInfo.yShift += freeSpace;
                                }
                                nextLineShift = 0;
                            }
                            break;
                        }

                        case AlignmentPropertyValue.START:
                        case AlignmentPropertyValue.BASELINE:
                        case AlignmentPropertyValue.SELF_START:
                        default: {
                            break;
                        }
                    }
                }
            }
            // TODO DEVSIX-5167 Support baseline value for align-items and align-self
            // Note, that isWrapReverse should be taken into account for baseline.
            // We don't need to do anything in these cases
            // Reverse back
            if (renderer.IsWrapReverse()) {
                JavaCollectionsUtil.Reverse(lines);
                JavaCollectionsUtil.Reverse(lineCrossSizes);
            }
        }

        private static void ApplyJustifyContent(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, FlexContainerRenderer
             renderer, float mainSize, float containerMainSize, float gap) {
            JustifyContent justifyContent = (JustifyContent)renderer.GetProperty<JustifyContent?>(Property.JUSTIFY_CONTENT
                , JustifyContent.FLEX_START);
            bool containsFixedHeight = containerMainSize > 0;
            bool isFixedHeightAppliedOnTheCurrentPage = containsFixedHeight && containerMainSize < mainSize;
            if (renderer.IsWrapReverse()) {
                JavaCollectionsUtil.Reverse(lines);
            }
            foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                float childrenMainSize = (line.Count - 1) * gap;
                // Items order becomes important for justification
                bool isColumnReverse = FlexDirectionPropertyValue.COLUMN_REVERSE == renderer.GetProperty<FlexDirectionPropertyValue?
                    >(Property.FLEX_DIRECTION, null);
                if (isColumnReverse) {
                    JavaCollectionsUtil.Reverse(line);
                }
                IList<FlexUtil.FlexItemCalculationInfo> lineToJustify = new List<FlexUtil.FlexItemCalculationInfo>();
                for (int i = 0; i < line.Count; ++i) {
                    FlexUtil.FlexItemCalculationInfo itemInfo = line[i];
                    if (i != 0 && IsColumnDirection(renderer) && !isFixedHeightAppliedOnTheCurrentPage && lines.Count == 1 && 
                        childrenMainSize + itemInfo.GetOuterMainSize(itemInfo.mainSize) > mainSize + EPSILON) {
                        break;
                    }
                    childrenMainSize += itemInfo.GetOuterMainSize(itemInfo.mainSize);
                    lineToJustify.Add(itemInfo);
                }
                // Reverse back
                if (isColumnReverse) {
                    JavaCollectionsUtil.Reverse(line);
                    JavaCollectionsUtil.Reverse(lineToJustify);
                }
                float freeSpace = 0;
                if (!IsColumnDirection(renderer)) {
                    freeSpace = mainSize - childrenMainSize;
                }
                else {
                    if (containsFixedHeight) {
                        // In case of column direction we should align only if container contains fixed height
                        freeSpace = isFixedHeightAppliedOnTheCurrentPage ? containerMainSize - childrenMainSize : Math.Max(0, mainSize
                             - childrenMainSize);
                    }
                }
                renderer.GetFlexItemMainDirector().ApplyJustifyContent(lineToJustify, justifyContent, freeSpace);
            }
            if (renderer.IsWrapReverse()) {
                JavaCollectionsUtil.Reverse(lines);
            }
        }

        private static void ApplyGap(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, float columnGap, float 
            rowGap, FlexContainerRenderer renderer) {
            bool isDirReverse = FlexDirectionPropertyValue.COLUMN_REVERSE == renderer.GetProperty<FlexDirectionPropertyValue?
                >(Property.FLEX_DIRECTION, null) || FlexDirectionPropertyValue.ROW_REVERSE == renderer.GetProperty<FlexDirectionPropertyValue?
                >(Property.FLEX_DIRECTION, null);
            if (renderer.IsWrapReverse()) {
                JavaCollectionsUtil.Reverse(lines);
            }
            for (int j = 0; j < lines.Count; j++) {
                IList<FlexUtil.FlexItemCalculationInfo> line = lines[j];
                for (int i = 0; i < line.Count; i++) {
                    FlexUtil.FlexItemCalculationInfo info = line[i];
                    int mainDirSkipIndex = isDirReverse ? (line.Count - 1) : 0;
                    if (IsColumnDirection(renderer)) {
                        info.xShift += j == 0 ? 0 : columnGap;
                        info.yShift += i == mainDirSkipIndex ? 0 : rowGap;
                    }
                    else {
                        info.xShift += i == mainDirSkipIndex ? 0 : columnGap;
                        info.yShift += j == 0 ? 0 : rowGap;
                    }
                }
            }
            if (renderer.IsWrapReverse()) {
                JavaCollectionsUtil.Reverse(lines);
            }
        }

        private static float CalculateFreeSpace(IList<FlexUtil.FlexItemCalculationInfo> line, float initialFreeSpace
            , float gap) {
            float result = initialFreeSpace;
            foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                if (info.isFrozen) {
                    result -= info.GetOuterMainSize(info.mainSize);
                }
                else {
                    result -= info.GetOuterMainSize(info.flexBaseSize);
                }
            }
            result -= (line.Count - 1) * gap;
            return result;
        }

        private static bool HasFlexibleItems(IList<FlexUtil.FlexItemCalculationInfo> line) {
            foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                if (!info.isFrozen) {
                    return true;
                }
            }
            return false;
        }

//\cond DO_NOT_DOCUMENT
        internal static bool IsZero(float value) {
            return Math.Abs(value) < EPSILON;
        }
//\endcond

        private static IList<FlexUtil.FlexItemCalculationInfo> CreateFlexItemCalculationInfos(FlexContainerRenderer
             flexContainerRenderer, float flexContainerMainSize, float crossSize) {
            IList<IRenderer> childRenderers = flexContainerRenderer.GetChildRenderers();
            IList<FlexUtil.FlexItemCalculationInfo> flexItems = new List<FlexUtil.FlexItemCalculationInfo>();
            foreach (IRenderer renderer in childRenderers) {
                if (renderer is AbstractRenderer) {
                    AbstractRenderer abstractRenderer = (AbstractRenderer)renderer;
                    float flexBasis = 0;
                    bool flexBasisContent = false;
                    if (renderer.GetProperty<UnitValue>(Property.FLEX_BASIS) == null) {
                        // Flex base size will be calculated on FlexItemCalculationInfo creation after min/maxContent.
                        flexBasisContent = true;
                    }
                    else {
                        // For column layout layoutBox height should not be taken into account while calculating flexBasis
                        // in percents. If flex container doesn't have a definite size, flex basis percents should not be
                        // taken into account.
                        float containerMainSize = IsColumnDirection(flexContainerRenderer) ? GetMainSize(flexContainerRenderer, new 
                            Rectangle(0, 0)) : flexContainerMainSize;
                        flexBasis = (float)abstractRenderer.RetrieveUnitValue(containerMainSize, Property.FLEX_BASIS);
                        if (AbstractRenderer.IsBorderBoxSizing(abstractRenderer)) {
                            flexBasis -= AbstractRenderer.CalculatePaddingBorderWidth(abstractRenderer);
                        }
                    }
                    flexBasis = Math.Max(flexBasis, 0);
                    float flexGrow = (float)renderer.GetProperty<float?>(Property.FLEX_GROW, FLEX_GROW_INITIAL_VALUE);
                    float flexShrink = (float)renderer.GetProperty<float?>(Property.FLEX_SHRINK, FLEX_SHRINK_INITIAL_VALUE);
                    FlexUtil.FlexItemCalculationInfo flexItemInfo = new FlexUtil.FlexItemCalculationInfo((AbstractRenderer)renderer
                        , flexBasis, flexGrow, flexShrink, flexContainerMainSize, flexBasisContent, IsColumnDirection(flexContainerRenderer
                        ), crossSize);
                    flexItems.Add(flexItemInfo);
                }
            }
            return flexItems;
        }

        private static IList<float> RetrieveCurrentPageLineCrossSizes(FlexContainerRenderer flexContainerRenderer, 
            IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, IList<float> lineCrossSizes, float? crossSize, Rectangle
             layoutBox) {
            float mainSize = GetMainSize(flexContainerRenderer, new Rectangle(0, 0));
            bool isColumnDirectionWithPagination = IsColumnDirection(flexContainerRenderer) && (mainSize < EPSILON || 
                mainSize > layoutBox.GetHeight() + EPSILON);
            if (!isColumnDirectionWithPagination || crossSize == null) {
                return lineCrossSizes;
            }
            IList<float> currentPageLineCrossSizes = new List<float>();
            float flexLinesCrossSizesSum = 0;
            for (int i = 0; i < lineCrossSizes.Count; ++i) {
                float size = lineCrossSizes[i];
                if (flexLinesCrossSizesSum + size > crossSize + EPSILON || lines[i][0].mainSize > layoutBox.GetHeight() + 
                    EPSILON) {
                    if (i == 0) {
                        // We should add first line anyway
                        currentPageLineCrossSizes.Add(size);
                    }
                    break;
                }
                flexLinesCrossSizesSum += size;
                currentPageLineCrossSizes.Add(size);
            }
            return currentPageLineCrossSizes;
        }

        private static float CalculateHeight(AbstractRenderer flexItemRenderer, float width) {
            LayoutResult result = flexItemRenderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(width, AbstractRenderer
                .INF))));
            return result.GetStatus() == LayoutResult.NOTHING ? 0 : result.GetOccupiedArea().GetBBox().GetHeight();
        }

//\cond DO_NOT_DOCUMENT
        internal class FlexItemCalculationInfo {
//\cond DO_NOT_DOCUMENT
            internal AbstractRenderer renderer;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float flexBasis;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float flexShrink;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float flexGrow;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float minContent;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float maxContent;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float mainSize;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float crossSize;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float xShift;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float yShift;
//\endcond

//\cond DO_NOT_DOCUMENT
            // Calculation-related fields
            internal float scaledFlexShrinkFactor;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool isFrozen = false;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool isMinViolated = false;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool isMaxViolated = false;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float flexBaseSize;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float hypotheticalMainSize;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float hypotheticalCrossSize;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool flexBasisContent;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool isColumnDirection;
//\endcond

            public FlexItemCalculationInfo(AbstractRenderer renderer, float flexBasis, float flexGrow, float flexShrink
                , float areaMainSize, bool flexBasisContent, bool isColumnDirection, float crossSize) {
                this.isColumnDirection = isColumnDirection;
                this.flexBasisContent = flexBasisContent;
                this.renderer = renderer;
                if (flexShrink < 0) {
                    throw new ArgumentException(LayoutExceptionMessageConstant.FLEX_SHRINK_CANNOT_BE_NEGATIVE);
                }
                this.flexShrink = flexShrink;
                if (flexGrow < 0) {
                    throw new ArgumentException(LayoutExceptionMessageConstant.FLEX_GROW_CANNOT_BE_NEGATIVE);
                }
                this.flexGrow = flexGrow;
                float? definiteMinContent = isColumnDirection ? RetrieveMinHeightForMainDirection(renderer) : renderer.RetrieveMinWidth
                    (areaMainSize);
                // null means that min-width property is not set or has auto value. In both cases we should calculate it
                this.minContent = definiteMinContent == null ? CalculateMinContentAuto(areaMainSize, crossSize) : (float)definiteMinContent;
                float? maxMainSize = isColumnDirection ? RetrieveMaxHeightForMainDirection(this.renderer) : this.renderer.
                    RetrieveMaxWidth(areaMainSize);
                // As for now we assume that max main size should be calculated. For columnDirection relative min-height is
                // not taken into account, so we should handle it here.
                this.maxContent = maxMainSize == null ? AbstractRenderer.INF : Math.Max(this.minContent, (float)maxMainSize
                    );
                if (this.flexBasisContent) {
                    this.flexBasis = DetermineFlexBaseSize(areaMainSize, isColumnDirection, crossSize);
                }
                else {
                    this.flexBasis = flexBasis;
                }
            }

            public virtual Rectangle ToRectangle() {
                return isColumnDirection ? new Rectangle(xShift, yShift, GetOuterCrossSize(crossSize), GetOuterMainSize(mainSize
                    )) : new Rectangle(xShift, yShift, GetOuterMainSize(mainSize), GetOuterCrossSize(crossSize));
            }

//\cond DO_NOT_DOCUMENT
            internal virtual float GetOuterMainSize(float size) {
                return isColumnDirection ? renderer.ApplyMarginsBordersPaddings(new Rectangle(0, size), true).GetHeight() : 
                    renderer.ApplyMarginsBordersPaddings(new Rectangle(size, 0), true).GetWidth();
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual float GetInnerMainSize(float size) {
                return isColumnDirection ? renderer.ApplyMarginsBordersPaddings(new Rectangle(0, size), false).GetHeight()
                     : renderer.ApplyMarginsBordersPaddings(new Rectangle(size, 0), false).GetWidth();
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual float GetOuterCrossSize(float size) {
                return isColumnDirection ? renderer.ApplyMarginsBordersPaddings(new Rectangle(size, 0), true).GetWidth() : 
                    renderer.ApplyMarginsBordersPaddings(new Rectangle(0, size), true).GetHeight();
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual float GetInnerCrossSize(float size) {
                return isColumnDirection ? renderer.ApplyMarginsBordersPaddings(new Rectangle(size, 0), false).GetWidth() : 
                    renderer.ApplyMarginsBordersPaddings(new Rectangle(0, size), false).GetHeight();
            }
//\endcond

            private static float CalculateMaxMainSize(AbstractRenderer flexItemRenderer, float flexContainerMainSize, 
                bool isColumnDirection, float crossSize) {
                float? maxMainSize;
                if (flexItemRenderer is TableRenderer) {
                    // TODO DEVSIX-5214 we can't call TableRenderer#retrieveWidth method as far as it can throw NPE
                    if (isColumnDirection) {
                        float? itemRendererMaxHeight = flexItemRenderer.RetrieveMaxHeight();
                        maxMainSize = itemRendererMaxHeight;
                        if (maxMainSize == null) {
                            maxMainSize = CalculateHeight(flexItemRenderer, crossSize);
                        }
                    }
                    else {
                        maxMainSize = flexItemRenderer.GetMinMaxWidth().GetMaxWidth();
                    }
                    if (isColumnDirection) {
                        maxMainSize = flexItemRenderer.ApplyMarginsBordersPaddings(new Rectangle(0, (float)maxMainSize), false).GetHeight
                            ();
                    }
                    else {
                        maxMainSize = flexItemRenderer.ApplyMarginsBordersPaddings(new Rectangle((float)maxMainSize, 0), false).GetWidth
                            ();
                    }
                }
                else {
                    // We need to retrieve width and max-width manually because this methods take into account box-sizing
                    maxMainSize = isColumnDirection ? flexItemRenderer.RetrieveHeight() : flexItemRenderer.RetrieveWidth(flexContainerMainSize
                        );
                    if (maxMainSize == null) {
                        maxMainSize = isColumnDirection ? RetrieveMaxHeightForMainDirection(flexItemRenderer) : flexItemRenderer.RetrieveMaxWidth
                            (flexContainerMainSize);
                    }
                    if (maxMainSize == null) {
                        if (isColumnDirection) {
                            float? height = RetrieveMaxHeightForMainDirection(flexItemRenderer);
                            if (height == null) {
                                height = CalculateHeight(flexItemRenderer, crossSize);
                            }
                            maxMainSize = flexItemRenderer.ApplyMarginsBordersPaddings(new Rectangle(0, (float)height), false).GetHeight
                                ();
                        }
                        else {
                            maxMainSize = flexItemRenderer.ApplyMarginsBordersPaddings(new Rectangle(flexItemRenderer.GetMinMaxWidth()
                                .GetMaxWidth(), 0), false).GetWidth();
                        }
                    }
                }
                return (float)maxMainSize;
            }

            private float CalculateMinContentAuto(float flexContainerMainSize, float crossSize) {
                // Automatic Minimum Size of Flex Items https://www.w3.org/TR/css-flexbox-1/#content-based-minimum-size
                float? specifiedSizeSuggestion = CalculateSpecifiedSizeSuggestion(flexContainerMainSize);
                float contentSizeSuggestion = CalculateContentSizeSuggestion(flexContainerMainSize, crossSize);
                if (renderer.HasAspectRatio() && specifiedSizeSuggestion == null) {
                    // However, if the box has an aspect ratio and no specified size,
                    // its content-based minimum size is the smaller of its content size suggestion
                    // and its transferred size suggestion
                    float? transferredSizeSuggestion = CalculateTransferredSizeSuggestion(flexContainerMainSize);
                    if (transferredSizeSuggestion == null) {
                        return contentSizeSuggestion;
                    }
                    else {
                        return Math.Min(contentSizeSuggestion, (float)transferredSizeSuggestion);
                    }
                }
                else {
                    if (specifiedSizeSuggestion == null) {
                        // If the box has neither a specified size suggestion nor an aspect ratio,
                        // its content-based minimum size is the content size suggestion.
                        return contentSizeSuggestion;
                    }
                    else {
                        // In general, the content-based minimum size of a flex item is the smaller
                        // of its content size suggestion and its specified size suggestion
                        return Math.Min(contentSizeSuggestion, (float)specifiedSizeSuggestion);
                    }
                }
            }

            /// <summary>
            /// If the item has an intrinsic aspect ratio and its computed cross size property is definite,
            /// then the transferred size suggestion is that size (clamped by its min and max cross size properties
            /// if they are definite), converted through the aspect ratio.
            /// </summary>
            /// <remarks>
            /// If the item has an intrinsic aspect ratio and its computed cross size property is definite,
            /// then the transferred size suggestion is that size (clamped by its min and max cross size properties
            /// if they are definite), converted through the aspect ratio. It is otherwise undefined.
            /// </remarks>
            /// <returns>transferred size suggestion if it can be calculated, null otherwise</returns>
            private float? CalculateTransferredSizeSuggestion(float flexContainerMainSize) {
                float? transferredSizeSuggestion = null;
                float? crossSize = isColumnDirection ? renderer.RetrieveWidth(flexContainerMainSize) : renderer.RetrieveHeight
                    ();
                if (renderer.HasAspectRatio() && crossSize != null) {
                    transferredSizeSuggestion = crossSize * renderer.GetAspectRatio();
                    transferredSizeSuggestion = ClampValueByCrossSizesConvertedThroughAspectRatio((float)transferredSizeSuggestion
                        , flexContainerMainSize);
                }
                return transferredSizeSuggestion;
            }

            /// <summary>
            /// If the item’s computed main size property is definite,
            /// then the specified size suggestion is that size (clamped by its max main size property if it’s definite).
            /// </summary>
            /// <remarks>
            /// If the item’s computed main size property is definite,
            /// then the specified size suggestion is that size (clamped by its max main size property if it’s definite).
            /// It is otherwise undefined.
            /// </remarks>
            /// <param name="flexContainerMainSize">the width of the flex container</param>
            /// <returns>specified size suggestion if it's definite, null otherwise</returns>
            private float? CalculateSpecifiedSizeSuggestion(float flexContainerMainSize) {
                float? mainSizeSuggestion = null;
                if (isColumnDirection) {
                    if (renderer.HasProperty(Property.HEIGHT)) {
                        mainSizeSuggestion = renderer.RetrieveHeight();
                    }
                }
                else {
                    if (renderer.HasProperty(Property.WIDTH)) {
                        mainSizeSuggestion = renderer.RetrieveWidth(flexContainerMainSize);
                    }
                }
                return mainSizeSuggestion;
            }

            /// <summary>
            /// The content size suggestion is the min-content size in the main axis, clamped, if it has an aspect ratio,
            /// by any definite min and max cross size properties converted through the aspect ratio,
            /// and then further clamped by the max main size property if that is definite.
            /// </summary>
            /// <param name="flexContainerMainSize">the width of the flex container</param>
            /// <returns>content size suggestion</returns>
            private float CalculateContentSizeSuggestion(float flexContainerMainSize, float crossSize) {
                UnitValue rendererWidth = renderer.ReplaceOwnProperty<UnitValue>(Property.WIDTH, null);
                UnitValue rendererHeight = renderer.ReplaceOwnProperty<UnitValue>(Property.HEIGHT, null);
                float minContentSize;
                if (isColumnDirection) {
                    float? height = RetrieveMinHeightForMainDirection(renderer);
                    if (height == null) {
                        height = CalculateHeight(renderer, crossSize);
                    }
                    minContentSize = GetInnerMainSize((float)height);
                }
                else {
                    MinMaxWidth minMaxWidth = renderer.GetMinMaxWidth();
                    minContentSize = GetInnerMainSize(minMaxWidth.GetMinWidth());
                }
                renderer.ReturnBackOwnProperty(Property.HEIGHT, rendererHeight);
                renderer.ReturnBackOwnProperty(Property.WIDTH, rendererWidth);
                if (renderer.HasAspectRatio()) {
                    minContentSize = ClampValueByCrossSizesConvertedThroughAspectRatio(minContentSize, flexContainerMainSize);
                }
                float? maxMainSize = isColumnDirection ? RetrieveMaxHeightForMainDirection(renderer) : renderer.RetrieveMaxWidth
                    (flexContainerMainSize);
                if (maxMainSize == null) {
                    maxMainSize = AbstractRenderer.INF;
                }
                return Math.Min(minContentSize, (float)maxMainSize);
            }

            private float ClampValueByCrossSizesConvertedThroughAspectRatio(float value, float flexContainerMainSize) {
                float? maxCrossSize = isColumnDirection ? renderer.RetrieveMaxWidth(flexContainerMainSize) : renderer.RetrieveMaxHeight
                    ();
                if (maxCrossSize == null || !renderer.HasProperty(isColumnDirection ? Property.MAX_WIDTH : Property.MAX_HEIGHT
                    )) {
                    maxCrossSize = AbstractRenderer.INF;
                }
                float? minCrossSize = isColumnDirection ? renderer.RetrieveMinWidth(flexContainerMainSize) : renderer.RetrieveMinHeight
                    ();
                if (minCrossSize == null || !renderer.HasProperty(isColumnDirection ? Property.MIN_WIDTH : Property.MIN_HEIGHT
                    )) {
                    minCrossSize = 0F;
                }
                return Math.Min(Math.Max((float)(minCrossSize * renderer.GetAspectRatio()), value), (float)(maxCrossSize *
                     renderer.GetAspectRatio()));
            }

            private float DetermineFlexBaseSize(float mainSize, bool isColumnDirection, float crossSize) {
                // When determining the flex base size, the item’s min and max main sizes are ignored (no clamping occurs).
                UnitValue minMainSize = null;
                UnitValue maxMainSize = null;
                if (isColumnDirection) {
                    minMainSize = this.renderer.GetProperty<UnitValue>(Property.MIN_HEIGHT);
                    this.renderer.DeleteProperty(Property.MIN_HEIGHT);
                    maxMainSize = this.renderer.GetProperty<UnitValue>(Property.MAX_HEIGHT);
                    this.renderer.DeleteProperty(Property.MAX_HEIGHT);
                }
                else {
                    minMainSize = this.renderer.GetProperty<UnitValue>(Property.MIN_WIDTH);
                    this.renderer.DeleteProperty(Property.MIN_WIDTH);
                    maxMainSize = this.renderer.GetProperty<UnitValue>(Property.MAX_WIDTH);
                    this.renderer.DeleteProperty(Property.MAX_WIDTH);
                }
                // TODO DEVSIX-5091 improve determining of the flex base size when flex-basis: content
                float flexBaseSize = CalculateMaxMainSize(this.renderer, mainSize, isColumnDirection, crossSize);
                // Restore min and max main sizes
                if (isColumnDirection) {
                    this.renderer.ReturnBackOwnProperty(Property.MIN_HEIGHT, minMainSize);
                    this.renderer.ReturnBackOwnProperty(Property.MAX_HEIGHT, maxMainSize);
                }
                else {
                    this.renderer.ReturnBackOwnProperty(Property.MIN_WIDTH, minMainSize);
                    this.renderer.ReturnBackOwnProperty(Property.MAX_WIDTH, maxMainSize);
                }
                return flexBaseSize;
            }
        }
//\endcond
    }
//\endcond
}
