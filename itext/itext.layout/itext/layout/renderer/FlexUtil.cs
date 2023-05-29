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
using iText.Kernel.Geom;
using iText.Layout.Exceptions;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal sealed class FlexUtil {
        private const float EPSILON = 0.0001F;

        private const float FLEX_GROW_INITIAL_VALUE = 0F;

        private const float FLEX_SHRINK_INITIAL_VALUE = 1F;

        private static ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.FlexUtil));

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
            // Currently only width is used in this method
            float layoutBoxWidth = layoutBox.GetWidth();
            // 9.2. Line Length Determination
            // 2. Determine the available main and cross space for the flex items.
            // TODO DEVSIX-5001 min-content and max-content as width are not supported
            // if that dimension of the flex container is being sized under a min or max-content constraint,
            // the available space in that dimension is that constraint;
            float? mainSize = flexContainerRenderer.RetrieveWidth(layoutBoxWidth);
            if (mainSize == null) {
                mainSize = layoutBoxWidth;
            }
            // We need to have crossSize only if its value is definite.
            float? crossSize = flexContainerRenderer.RetrieveHeight();
            float? minCrossSize = flexContainerRenderer.RetrieveMinHeight();
            float? maxCrossSize = flexContainerRenderer.RetrieveMaxHeight();
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = CreateFlexItemCalculationInfos(flexContainerRenderer
                , (float)mainSize);
            DetermineFlexBasisAndHypotheticalMainSizeForFlexItems(flexItemCalculationInfos);
            // 9.3. Main Size Determination
            // 5. Collect flex items into flex lines:
            bool isSingleLine = !flexContainerRenderer.HasProperty(Property.FLEX_WRAP) || FlexWrapPropertyValue.NOWRAP
                 == flexContainerRenderer.GetProperty<FlexWrapPropertyValue?>(Property.FLEX_WRAP);
            IList<IList<FlexUtil.FlexItemCalculationInfo>> lines = CollectFlexItemsIntoFlexLines(flexItemCalculationInfos
                , (float)mainSize, isSingleLine);
            // 6. Resolve the flexible lengths of all the flex items to find their used main size.
            // See §9.7 Resolving Flexible Lengths.
            // 9.7. Resolving Flexible Lengths
            ResolveFlexibleLengths(lines, (float)mainSize);
            // 9.4. Cross Size Determination
            // 7. Determine the hypothetical cross size of each item by
            // performing layout with the used main size and the available space, treating auto as fit-content.
            DetermineHypotheticalCrossSizeForFlexItems(lines);
            // 8. Calculate the cross size of each flex line.
            IList<float> lineCrossSizes = CalculateCrossSizeOfEachFlexLine(lines, minCrossSize, crossSize, maxCrossSize
                );
            // TODO DEVSIX-5003 min/max height calculations are not supported
            // If the flex container is single-line, then clamp the line’s cross-size to be within
            // the container’s computed min and max cross sizes. Note that if CSS 2.1’s definition of min/max-width/height
            // applied more generally, this behavior would fall out automatically.
            float flexLinesCrossSizesSum = 0;
            foreach (float size in lineCrossSizes) {
                flexLinesCrossSizesSum += size;
            }
            // 9. Handle 'align-content: stretch'.
            HandleAlignContentStretch(flexContainerRenderer, crossSize, flexLinesCrossSizesSum, lineCrossSizes);
            // TODO DEVSIX-2090 visibility-collapse items are not supported
            // 10. Collapse visibility:collapse items.
            // 11. Determine the used cross size of each flex item.
            DetermineUsedCrossSizeOfEachFlexItem(lines, lineCrossSizes, flexContainerRenderer);
            // 9.5. Main-Axis Alignment
            // 12. Align the items along the main-axis per justify-content.
            ApplyJustifyContent(lines, flexContainerRenderer, (float)mainSize);
            // 9.6. Cross-Axis Alignment
            // TODO DEVSIX-5002 margin: auto is not supported
            // 13. Resolve cross-axis auto margins
            // 14. Align all flex items along the cross-axis
            ApplyAlignItemsAndAlignSelf(lines, flexContainerRenderer, lineCrossSizes);
            // 15. Determine the flex container’s used cross size
            // TODO DEVSIX-5164 16. Align all flex lines per align-content.
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

        internal static void DetermineFlexBasisAndHypotheticalMainSizeForFlexItems(IList<FlexUtil.FlexItemCalculationInfo
            > flexItemCalculationInfos) {
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                // 3. Determine the flex base size and hypothetical main size of each item:
                AbstractRenderer renderer = info.renderer;
                // TODO DEVSIX-5001 content as width are not supported
                // B. If the flex item has ...
                // an intrinsic aspect ratio,
                // a used flex basis of content, and
                // a definite cross size,
                // then the flex base size is calculated from its inner cross size
                // and the flex item’s intrinsic aspect ratio.
                float? rendererHeight = renderer.RetrieveHeight();
                if (renderer.HasAspectRatio() && info.flexBasisContent && rendererHeight != null) {
                    float aspectRatio = (float)renderer.GetAspectRatio();
                    info.flexBaseSize = (float)rendererHeight * aspectRatio;
                }
                else {
                    // A. If the item has a definite used flex basis, that’s the flex base size.
                    info.flexBaseSize = info.flexBasis;
                }
                // TODO DEVSIX-5001 content as width is not supported
                // C. If the used flex basis is content or depends on its available space,
                // and the flex container is being sized under a min-content or max-content constraint
                // (e.g. when performing automatic table layout [CSS21]), size the item under that constraint.
                // The flex base size is the item’s resulting main size.
                // TODO DEVSIX-5001 content as width is not supported
                // Otherwise, if the used flex basis is content or depends on its available space,
                // the available main size is infinite, and the flex item’s inline axis is parallel to the main axis,
                // lay the item out using the rules for a box in an orthogonal flow [CSS3-WRITING-MODES].
                // The flex base size is the item’s max-content main size.
                // TODO DEVSIX-5001 max-content as width is not supported
                // Otherwise, size the item into the available space using its used flex basis in place of its main size,
                // treating a value of content as max-content. If a cross size is needed to determine the main size
                // (e.g. when the flex item’s main size is in its block axis)
                // and the flex item’s cross size is auto and not definite,
                // in this calculation use fit-content as the flex item’s cross size.
                // The flex base size is the item’s resulting main size.
                // The hypothetical main size is the item’s flex base size clamped
                // according to its used min and max main sizes (and flooring the content box size at zero).
                info.hypotheticalMainSize = Math.Max(0, Math.Min(Math.Max(info.minContent, info.flexBaseSize), info.maxContent
                    ));
                // Each item in the flex line has a target main size, initially set to its flex base size
                info.mainSize = info.hypotheticalMainSize;
            }
        }

        // Note: We assume that it was resolved on some upper level
        // 4. Determine the main size of the flex container
        internal static IList<IList<FlexUtil.FlexItemCalculationInfo>> CollectFlexItemsIntoFlexLines(IList<FlexUtil.FlexItemCalculationInfo
            > flexItemCalculationInfos, float mainSize, bool isSingleLine) {
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
                            occupiedLineSpace = info.hypotheticalMainSize;
                        }
                    }
                    else {
                        currentLineInfos.Add(info);
                    }
                }
            }
            // the last line should be added
            if (!currentLineInfos.IsEmpty()) {
                lines.Add(currentLineInfos);
            }
            return lines;
        }

        internal static void ResolveFlexibleLengths(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, float mainSize
            ) {
            foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                // 1. Determine the used flex factor.
                float hypotheticalMainSizesSum = 0;
                foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                    hypotheticalMainSizesSum += info.GetOuterMainSize(info.hypotheticalMainSize);
                }
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
                float initialFreeSpace = CalculateFreeSpace(line, mainSize);
                // 4. Loop:
                // a. Check for flexible items
                while (HasFlexibleItems(line)) {
                    // b. Calculate the remaining free space as for initial free space, above.
                    float remainingFreeSpace = CalculateFreeSpace(line, mainSize);
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

        // 9.5. Main-Axis Alignment
        // 12. Distribute any remaining free space.
        // Once any of the to-do remarks below is resolved, one should add a corresponding block,
        // which will be triggered if 0 < remaining free space
        // TODO DEVSIX-5002 margin: auto is not supported
        // If the remaining free space is positive and at least one main-axis margin on this line is auto,
        // distribute the free space equally among these margins. Otherwise, set all auto margins to zero.
        internal static void DetermineHypotheticalCrossSizeForFlexItems(IList<IList<FlexUtil.FlexItemCalculationInfo
            >> lines) {
            foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                    DetermineHypotheticalCrossSizeForFlexItem(info);
                }
            }
        }

        private static void DetermineHypotheticalCrossSizeForFlexItem(FlexUtil.FlexItemCalculationInfo info) {
            if (info.renderer is FlexContainerRenderer && ((FlexContainerRenderer)info.renderer).GetHypotheticalCrossSize
                (info.mainSize) != null) {
                // Take from cache
                info.hypotheticalCrossSize = ((FlexContainerRenderer)info.renderer).GetHypotheticalCrossSize(info.mainSize
                    ).Value;
            }
            else {
                UnitValue prevWidth = info.renderer.ReplaceOwnProperty<UnitValue>(Property.WIDTH, UnitValue.CreatePointValue
                    (info.mainSize));
                UnitValue prevMinWidth = info.renderer.ReplaceOwnProperty<UnitValue>(Property.MIN_WIDTH, null);
                LayoutResult result = info.renderer.Layout(new LayoutContext(new LayoutArea(0, new Rectangle(AbstractRenderer
                    .INF, AbstractRenderer.INF))));
                info.renderer.ReturnBackOwnProperty(Property.MIN_WIDTH, prevMinWidth);
                info.renderer.ReturnBackOwnProperty(Property.WIDTH, prevWidth);
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

        internal static IList<float> CalculateCrossSizeOfEachFlexLine(IList<IList<FlexUtil.FlexItemCalculationInfo
            >> lines, float? minCrossSize, float? crossSize, float? maxCrossSize) {
            bool isSingleLine = lines.Count == 1;
            IList<float> lineCrossSizes = new List<float>();
            if (isSingleLine && crossSize != null && !lines.IsEmpty()) {
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
                        // TODO DEVSIX-5003 Condition "inline-axis is parallel to the main-axis" is not supported
                        // TODO DEVSIX-5002 margin: auto is not supported => "cross-axis margins are both non-auto" is true
                        // TODO DEVSIX-5038 Support BASELINE as align-self
                        // 2. Among all the items not collected by the previous step,
                        // find the largest outer hypothetical cross size.
                        if (largestHypotheticalCrossSize < info.GetOuterCrossSize(info.hypotheticalCrossSize)) {
                            largestHypotheticalCrossSize = info.GetOuterCrossSize(info.hypotheticalCrossSize);
                        }
                        flexLinesCrossSize = Math.Max(0, largestHypotheticalCrossSize);
                    }
                    // 3. If the flex container is single-line, then clamp the line’s cross-size to be
                    // within the container’s computed min and max cross sizes
                    if (isSingleLine && !lines.IsEmpty()) {
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

        internal static void HandleAlignContentStretch(FlexContainerRenderer flexContainerRenderer, float? crossSize
            , float flexLinesCrossSizesSum, IList<float> lineCrossSizes) {
            AlignmentPropertyValue alignContent = (AlignmentPropertyValue)flexContainerRenderer.GetProperty<AlignmentPropertyValue?
                >(Property.ALIGN_CONTENT, AlignmentPropertyValue.STRETCH);
            if (crossSize != null && alignContent == AlignmentPropertyValue.STRETCH && flexLinesCrossSizesSum < crossSize
                 - EPSILON) {
                float addition = ((float)crossSize - flexLinesCrossSizesSum) / lineCrossSizes.Count;
                for (int i = 0; i < lineCrossSizes.Count; i++) {
                    lineCrossSizes[i] = lineCrossSizes[i] + addition;
                }
            }
        }

        internal static void DetermineUsedCrossSizeOfEachFlexItem(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines
            , IList<float> lineCrossSizes, FlexContainerRenderer flexContainerRenderer) {
            AlignmentPropertyValue alignItems = (AlignmentPropertyValue)flexContainerRenderer.GetProperty<AlignmentPropertyValue?
                >(Property.ALIGN_ITEMS, AlignmentPropertyValue.STRETCH);
            System.Diagnostics.Debug.Assert(lines.Count == lineCrossSizes.Count);
            for (int i = 0; i < lines.Count; i++) {
                foreach (FlexUtil.FlexItemCalculationInfo info in lines[i]) {
                    // TODO DEVSIX-5002 margin: auto is not supported
                    // TODO DEVSIX-5003 min/max height calculations are not implemented
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
                    if ((alignSelf == AlignmentPropertyValue.STRETCH || alignSelf == AlignmentPropertyValue.NORMAL) && info.renderer
                        .GetProperty<UnitValue>(Property.HEIGHT) == null) {
                        info.crossSize = info.GetInnerCrossSize(lineCrossSizes[i]);
                        float? maxHeight = infoRenderer.RetrieveMaxHeight();
                        if (maxHeight != null) {
                            info.crossSize = Math.Min((float)maxHeight, info.crossSize);
                        }
                        float? minHeight = infoRenderer.RetrieveMinHeight();
                        if (minHeight != null) {
                            info.crossSize = Math.Max((float)minHeight, info.crossSize);
                        }
                    }
                    else {
                        info.crossSize = info.hypotheticalCrossSize;
                    }
                }
            }
        }

        private static void ApplyAlignItemsAndAlignSelf(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, FlexContainerRenderer
             renderer, IList<float> lineCrossSizes) {
            AlignmentPropertyValue itemsAlignment = (AlignmentPropertyValue)renderer.GetProperty<AlignmentPropertyValue?
                >(Property.ALIGN_ITEMS, AlignmentPropertyValue.STRETCH);
            System.Diagnostics.Debug.Assert(lines.Count == lineCrossSizes.Count);
            for (int i = 0; i < lines.Count; ++i) {
                float lineCrossSize = lineCrossSizes[i];
                foreach (FlexUtil.FlexItemCalculationInfo itemInfo in lines[i]) {
                    AlignmentPropertyValue selfAlignment = (AlignmentPropertyValue)itemInfo.renderer.GetProperty<AlignmentPropertyValue?
                        >(Property.ALIGN_SELF, itemsAlignment);
                    float freeSpace = lineCrossSize - itemInfo.GetOuterCrossSize(itemInfo.crossSize);
                    switch (selfAlignment) {
                        case AlignmentPropertyValue.SELF_END:
                        case AlignmentPropertyValue.END: {
                            itemInfo.yShift = freeSpace;
                            break;
                        }

                        case AlignmentPropertyValue.FLEX_END: {
                            if (!renderer.IsWrapReverse()) {
                                itemInfo.yShift = freeSpace;
                            }
                            break;
                        }

                        case AlignmentPropertyValue.CENTER: {
                            itemInfo.yShift = freeSpace / 2;
                            break;
                        }

                        case AlignmentPropertyValue.FLEX_START: {
                            if (renderer.IsWrapReverse()) {
                                itemInfo.yShift = freeSpace;
                            }
                            break;
                        }

                        case AlignmentPropertyValue.START:
                        case AlignmentPropertyValue.BASELINE:
                        case AlignmentPropertyValue.SELF_START:
                        case AlignmentPropertyValue.STRETCH:
                        case AlignmentPropertyValue.NORMAL:
                        default: {
                            break;
                        }
                    }
                }
            }
        }

        // We don't need to do anything in these cases
        private static void ApplyJustifyContent(IList<IList<FlexUtil.FlexItemCalculationInfo>> lines, FlexContainerRenderer
             renderer, float mainSize) {
            JustifyContent justifyContent = (JustifyContent)renderer.GetProperty<JustifyContent?>(Property.JUSTIFY_CONTENT
                , JustifyContent.FLEX_START);
            foreach (IList<FlexUtil.FlexItemCalculationInfo> line in lines) {
                float childrenWidth = 0;
                foreach (FlexUtil.FlexItemCalculationInfo itemInfo in line) {
                    childrenWidth += itemInfo.GetOuterMainSize(itemInfo.mainSize);
                }
                float freeSpace = mainSize - childrenWidth;
                switch (justifyContent) {
                    case JustifyContent.RIGHT:
                    case JustifyContent.END:
                    case JustifyContent.SELF_END:
                    case JustifyContent.FLEX_END: {
                        line[0].xShift = freeSpace;
                        break;
                    }

                    case JustifyContent.CENTER: {
                        line[0].xShift = freeSpace / 2;
                        break;
                    }

                    case JustifyContent.NORMAL:
                    case JustifyContent.STRETCH:
                    case JustifyContent.START:
                    case JustifyContent.LEFT:
                    case JustifyContent.SELF_START:
                    case JustifyContent.FLEX_START:
                    default: {
                        break;
                    }
                }
            }
        }

        // We don't need to do anything in these cases
        private static float CalculateFreeSpace(IList<FlexUtil.FlexItemCalculationInfo> line, float initialFreeSpace
            ) {
            float result = initialFreeSpace;
            foreach (FlexUtil.FlexItemCalculationInfo info in line) {
                if (info.isFrozen) {
                    result -= info.GetOuterMainSize(info.mainSize);
                }
                else {
                    result -= info.GetOuterMainSize(info.flexBaseSize);
                }
            }
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

        internal static bool IsZero(float value) {
            return Math.Abs(value) < EPSILON;
        }

        private static IList<FlexUtil.FlexItemCalculationInfo> CreateFlexItemCalculationInfos(FlexContainerRenderer
             flexContainerRenderer, float flexContainerWidth) {
            IList<IRenderer> childRenderers = flexContainerRenderer.GetChildRenderers();
            IList<FlexUtil.FlexItemCalculationInfo> flexItems = new List<FlexUtil.FlexItemCalculationInfo>();
            foreach (IRenderer renderer in childRenderers) {
                if (renderer is AbstractRenderer) {
                    AbstractRenderer abstractRenderer = (AbstractRenderer)renderer;
                    // TODO DEVSIX-5091 improve determining of the flex base size when flex-basis: content
                    float maxWidth = CalculateMaxWidth(abstractRenderer, flexContainerWidth);
                    float flexBasis;
                    bool flexBasisContent = false;
                    if (renderer.GetProperty<UnitValue>(Property.FLEX_BASIS) == null) {
                        flexBasis = maxWidth;
                        flexBasisContent = true;
                    }
                    else {
                        flexBasis = (float)abstractRenderer.RetrieveUnitValue(flexContainerWidth, Property.FLEX_BASIS);
                        if (AbstractRenderer.IsBorderBoxSizing(abstractRenderer)) {
                            flexBasis -= AbstractRenderer.CalculatePaddingBorderWidth(abstractRenderer);
                        }
                    }
                    flexBasis = Math.Max(flexBasis, 0);
                    float flexGrow = (float)renderer.GetProperty<float?>(Property.FLEX_GROW, FLEX_GROW_INITIAL_VALUE);
                    float flexShrink = (float)renderer.GetProperty<float?>(Property.FLEX_SHRINK, FLEX_SHRINK_INITIAL_VALUE);
                    FlexUtil.FlexItemCalculationInfo flexItemInfo = new FlexUtil.FlexItemCalculationInfo((AbstractRenderer)renderer
                        , flexBasis, flexGrow, flexShrink, flexContainerWidth, flexBasisContent);
                    flexItems.Add(flexItemInfo);
                }
            }
            return flexItems;
        }

        private static float CalculateMaxWidth(AbstractRenderer flexItemRenderer, float flexContainerWidth) {
            float? maxWidth;
            if (flexItemRenderer is TableRenderer) {
                // TODO DEVSIX-5214 we can't call TableRenderer#retrieveWidth method as far as it can throw NPE
                maxWidth = flexItemRenderer.GetMinMaxWidth().GetMaxWidth();
                maxWidth = flexItemRenderer.ApplyMarginsBordersPaddings(new Rectangle((float)maxWidth, 0), false).GetWidth
                    ();
            }
            else {
                // We need to retrieve width and max-width manually because this methods take into account box-sizing
                maxWidth = flexItemRenderer.RetrieveWidth(flexContainerWidth);
                if (maxWidth == null) {
                    maxWidth = flexItemRenderer.RetrieveMaxWidth(flexContainerWidth);
                }
                if (maxWidth == null) {
                    if (flexItemRenderer is ImageRenderer) {
                        // TODO DEVSIX-5269 getMinMaxWidth doesn't always return the original image width
                        maxWidth = ((ImageRenderer)flexItemRenderer).GetImageWidth();
                    }
                    else {
                        maxWidth = flexItemRenderer.ApplyMarginsBordersPaddings(new Rectangle(flexItemRenderer.GetMinMaxWidth().GetMaxWidth
                            (), 0), false).GetWidth();
                    }
                }
            }
            return (float)maxWidth;
        }

        internal class FlexItemCalculationInfo {
            internal AbstractRenderer renderer;

            internal float flexBasis;

            internal float flexShrink;

            internal float flexGrow;

            internal float minContent;

            internal float maxContent;

            internal float mainSize;

            internal float crossSize;

            internal float xShift;

            internal float yShift;

            // Calculation-related fields
            internal float scaledFlexShrinkFactor;

            internal bool isFrozen = false;

            internal bool isMinViolated = false;

            internal bool isMaxViolated = false;

            internal float flexBaseSize;

            internal float hypotheticalMainSize;

            internal float hypotheticalCrossSize;

            internal bool flexBasisContent;

            public FlexItemCalculationInfo(AbstractRenderer renderer, float flexBasis, float flexGrow, float flexShrink
                , float areaWidth, bool flexBasisContent) {
                this.flexBasisContent = flexBasisContent;
                this.renderer = renderer;
                this.flexBasis = flexBasis;
                if (flexShrink < 0) {
                    throw new ArgumentException(LayoutExceptionMessageConstant.FLEX_SHRINK_CANNOT_BE_NEGATIVE);
                }
                this.flexShrink = flexShrink;
                if (flexGrow < 0) {
                    throw new ArgumentException(LayoutExceptionMessageConstant.FLEX_GROW_CANNOT_BE_NEGATIVE);
                }
                this.flexGrow = flexGrow;
                float? definiteMinContent = renderer.RetrieveMinWidth(areaWidth);
                // null means that min-width property is not set or has auto value. In both cases we should calculate it
                this.minContent = definiteMinContent == null ? CalculateMinContentAuto(areaWidth) : (float)definiteMinContent;
                float? maxWidth = this.renderer.RetrieveMaxWidth(areaWidth);
                // As for now we assume that max width should be calculated so
                this.maxContent = maxWidth == null ? AbstractRenderer.INF : (float)maxWidth;
            }

            public virtual Rectangle ToRectangle() {
                return new Rectangle(xShift, yShift, GetOuterMainSize(mainSize), GetOuterCrossSize(crossSize));
            }

            internal virtual float GetOuterMainSize(float size) {
                return renderer.ApplyMarginsBordersPaddings(new Rectangle(size, 0), true).GetWidth();
            }

            internal virtual float GetInnerMainSize(float size) {
                return renderer.ApplyMarginsBordersPaddings(new Rectangle(size, 0), false).GetWidth();
            }

            internal virtual float GetOuterCrossSize(float size) {
                return renderer.ApplyMarginsBordersPaddings(new Rectangle(0, size), true).GetHeight();
            }

            internal virtual float GetInnerCrossSize(float size) {
                return renderer.ApplyMarginsBordersPaddings(new Rectangle(0, size), false).GetHeight();
            }

            private float CalculateMinContentAuto(float flexContainerWidth) {
                // Automatic Minimum Size of Flex Items https://www.w3.org/TR/css-flexbox-1/#content-based-minimum-size
                float? specifiedSizeSuggestion = CalculateSpecifiedSizeSuggestion(flexContainerWidth);
                float contentSizeSuggestion = CalculateContentSizeSuggestion(flexContainerWidth);
                if (renderer.HasAspectRatio() && specifiedSizeSuggestion == null) {
                    // However, if the box has an aspect ratio and no specified size,
                    // its content-based minimum size is the smaller of its content size suggestion
                    // and its transferred size suggestion
                    float? transferredSizeSuggestion = CalculateTransferredSizeSuggestion();
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
            private float? CalculateTransferredSizeSuggestion() {
                float? transferredSizeSuggestion = null;
                float? height = renderer.RetrieveHeight();
                if (renderer.HasAspectRatio() && height != null) {
                    transferredSizeSuggestion = height * renderer.GetAspectRatio();
                    transferredSizeSuggestion = ClampValueByCrossSizesConvertedThroughAspectRatio((float)transferredSizeSuggestion
                        );
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
            /// <param name="flexContainerWidth">the width of the flex container</param>
            /// <returns>specified size suggestion if it's definite, null otherwise</returns>
            private float? CalculateSpecifiedSizeSuggestion(float flexContainerWidth) {
                if (renderer.HasProperty(Property.WIDTH)) {
                    return renderer.RetrieveWidth(flexContainerWidth);
                }
                else {
                    return null;
                }
            }

            /// <summary>
            /// The content size suggestion is the min-content size in the main axis, clamped, if it has an aspect ratio,
            /// by any definite min and max cross size properties converted through the aspect ratio,
            /// and then further clamped by the max main size property if that is definite.
            /// </summary>
            /// <param name="flexContainerWidth">the width of the flex container</param>
            /// <returns>content size suggestion</returns>
            private float CalculateContentSizeSuggestion(float flexContainerWidth) {
                UnitValue rendererWidth = renderer.ReplaceOwnProperty<UnitValue>(Property.WIDTH, null);
                UnitValue rendererHeight = renderer.ReplaceOwnProperty<UnitValue>(Property.HEIGHT, null);
                MinMaxWidth minMaxWidth = renderer.GetMinMaxWidth();
                float minContentSize = GetInnerMainSize(minMaxWidth.GetMinWidth());
                renderer.ReturnBackOwnProperty(Property.HEIGHT, rendererHeight);
                renderer.ReturnBackOwnProperty(Property.WIDTH, rendererWidth);
                if (renderer.HasAspectRatio()) {
                    minContentSize = ClampValueByCrossSizesConvertedThroughAspectRatio(minContentSize);
                }
                float? maxWidth = renderer.RetrieveMaxWidth(flexContainerWidth);
                if (maxWidth == null) {
                    maxWidth = AbstractRenderer.INF;
                }
                return Math.Min(minContentSize, (float)maxWidth);
            }

            private float ClampValueByCrossSizesConvertedThroughAspectRatio(float value) {
                float? maxHeight = renderer.RetrieveMaxHeight();
                if (maxHeight == null || !renderer.HasProperty(Property.MAX_HEIGHT)) {
                    maxHeight = AbstractRenderer.INF;
                }
                float? minHeight = renderer.RetrieveMinHeight();
                if (minHeight == null || !renderer.HasProperty(Property.MIN_HEIGHT)) {
                    minHeight = 0F;
                }
                return Math.Min(Math.Max((float)(minHeight * renderer.GetAspectRatio()), value), (float)(maxHeight * renderer
                    .GetAspectRatio()));
            }
        }
    }
}
