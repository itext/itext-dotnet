/*

This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = CreateFlexItemCalculationInfos(flexContainerRenderer
                , layoutBox);
            // 9.2. Line Length Determination
            // 2. Determine the available main and cross space for the flex items.
            // TODO DEVSIX-5001 min-content and max-content as width are not supported
            // if that dimension of the flex container is being sized under a min or max-content constraint,
            // the available space in that dimension is that constraint;
            float? mainSize = flexContainerRenderer.RetrieveWidth(layoutBox.GetWidth());
            if (mainSize == null) {
                mainSize = layoutBox.GetWidth();
            }
            // We need to have crossSize only if its value is definite.
            float? crossSize = flexContainerRenderer.RetrieveHeight();
            float? minCrossSize = flexContainerRenderer.RetrieveMinHeight();
            float? maxCrossSize = flexContainerRenderer.RetrieveMaxHeight();
            DetermineFlexBasisAndHypotheticalMainSizeForFlexItems(flexItemCalculationInfos, (float)mainSize);
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
            IList<float> lineCrossSizes = CalculateCrossSizeOfEachFlexLine(lines, isSingleLine, minCrossSize, crossSize
                , maxCrossSize);
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
            > flexItemCalculationInfos, float mainSize) {
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                // 3. Determine the flex base size and hypothetical main size of each item:
                // Note: We assume that flex-basis: auto was resolved (set to either width or height) on some upper level
                System.Diagnostics.Debug.Assert(null != info.flexBasis);
                // A. If the item has a definite used flex basis, that’s the flex base size.
                info.flexBaseSize = info.flexBasis.IsPercentValue() ? (info.flexBasis.GetValue() * mainSize / 100) : info.
                    flexBasis.GetValue();
                // TODO DEVSIX-5001 content as width are not supported
                // TODO DEVSIX-5004 Implement method to check whether an element has an intrinsic aspect ratio
                // B. If the flex item has ...
                // an intrinsic aspect ratio,
                // a used flex basis of content, and
                // a definite cross size,
                // then the flex base size is calculated from its inner cross size
                // and the flex item’s intrinsic aspect ratio.
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
                    LayoutResult result = info.renderer.Layout(new LayoutContext(new LayoutArea(0, new Rectangle(info.GetOuterMainSize
                        (info.mainSize), AbstractRenderer.INF))));
                    // Since main size is clamped with min-width, we do expect the result to be full
                    System.Diagnostics.Debug.Assert(result.GetStatus() == LayoutResult.FULL);
                    info.hypotheticalCrossSize = info.GetInnerCrossSize(result.GetOccupiedArea().GetBBox().GetHeight());
                }
            }
        }

        internal static IList<float> CalculateCrossSizeOfEachFlexLine(IList<IList<FlexUtil.FlexItemCalculationInfo
            >> lines, bool isSingleLine, float? minCrossSize, float? crossSize, float? maxCrossSize) {
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
                    AbstractRenderer infoRenderer = info.renderer;
                    AlignmentPropertyValue alignSelf = (AlignmentPropertyValue)infoRenderer.GetProperty<AlignmentPropertyValue?
                        >(Property.ALIGN_SELF, alignItems);
                    // TODO DEVSIX-5002 Stretch value shall be ignored if margin auto for cross axis is set
                    if (alignSelf == AlignmentPropertyValue.STRETCH || alignSelf == AlignmentPropertyValue.NORMAL) {
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
                        case AlignmentPropertyValue.FLEX_END: {
                            itemInfo.yShift = freeSpace;
                            break;
                        }

                        case AlignmentPropertyValue.CENTER: {
                            itemInfo.yShift = freeSpace / 2;
                            break;
                        }

                        case AlignmentPropertyValue.END:
                        case AlignmentPropertyValue.START:
                        case AlignmentPropertyValue.BASELINE:
                        case AlignmentPropertyValue.SELF_START:
                        case AlignmentPropertyValue.SELF_END:
                        case AlignmentPropertyValue.STRETCH:
                        case AlignmentPropertyValue.NORMAL:
                        case AlignmentPropertyValue.FLEX_START:
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
                    case JustifyContent.FLEX_END: {
                        line[0].xShift = freeSpace;
                        break;
                    }

                    case JustifyContent.CENTER: {
                        line[0].xShift = freeSpace / 2;
                        break;
                    }

                    case JustifyContent.NORMAL:
                    case JustifyContent.START:
                    case JustifyContent.END:
                    case JustifyContent.RIGHT:
                    case JustifyContent.LEFT:
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
             flexContainerRenderer, Rectangle flexContainerBBox) {
            IList<IRenderer> childRenderers = flexContainerRenderer.GetChildRenderers();
            IList<FlexUtil.FlexItemCalculationInfo> flexItems = new List<FlexUtil.FlexItemCalculationInfo>();
            foreach (IRenderer renderer in childRenderers) {
                if (renderer is AbstractRenderer) {
                    AbstractRenderer abstractRenderer = (AbstractRenderer)renderer;
                    float flexGrow = (float)renderer.GetProperty<float?>(Property.FLEX_GROW, FLEX_GROW_INITIAL_VALUE);
                    float flexShrink = (float)renderer.GetProperty<float?>(Property.FLEX_SHRINK, FLEX_SHRINK_INITIAL_VALUE);
                    // TODO DEVSIX-5091 improve determining of the flex base size when flex-basis: content
                    float maxWidth = abstractRenderer.GetMinMaxWidth().GetMaxWidth();
                    maxWidth = abstractRenderer.ApplyMarginsBordersPaddings(new Rectangle(maxWidth, 0), false).GetWidth();
                    UnitValue flexBasis = renderer.GetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(maxWidth));
                    FlexUtil.FlexItemCalculationInfo flexItemInfo = new FlexUtil.FlexItemCalculationInfo((AbstractRenderer)renderer
                        , flexBasis, flexGrow, flexShrink, flexContainerBBox.GetWidth());
                    flexItems.Add(flexItemInfo);
                }
            }
            return flexItems;
        }

        internal class FlexItemCalculationInfo {
            internal AbstractRenderer renderer;

            internal UnitValue flexBasis;

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

            public FlexItemCalculationInfo(AbstractRenderer renderer, UnitValue flexBasis, float flexGrow, float flexShrink
                , float areaWidth) {
                this.renderer = renderer;
                if (null == flexBasis) {
                    throw new ArgumentException(LayoutExceptionMessageConstant.FLEX_BASIS_CANNOT_BE_NULL);
                }
                this.flexBasis = flexBasis;
                if (flexShrink < 0) {
                    throw new ArgumentException(LayoutExceptionMessageConstant.FLEX_SHRINK_CANNOT_BE_NEGATIVE);
                }
                this.flexShrink = flexShrink;
                if (flexGrow < 0) {
                    throw new ArgumentException(LayoutExceptionMessageConstant.FLEX_GROW_CANNOT_BE_NEGATIVE);
                }
                this.flexGrow = flexGrow;
                // We always need to clamp flex item's sizes with min-width, so this calculation is necessary
                // We also need to get min-width not based on Property.WIDTH
                UnitValue rendererWidth = renderer.GetOwnProperty<UnitValue>(Property.WIDTH);
                bool hasOwnWidth = renderer.HasOwnProperty(Property.WIDTH);
                renderer.SetProperty(Property.WIDTH, null);
                MinMaxWidth minMaxWidth = renderer.GetMinMaxWidth();
                if (hasOwnWidth) {
                    renderer.SetProperty(Property.WIDTH, rendererWidth);
                }
                else {
                    renderer.DeleteOwnProperty(Property.WIDTH);
                }
                this.minContent = GetInnerMainSize(minMaxWidth.GetMinWidth());
                bool isMaxWidthApplied = null != this.renderer.RetrieveMaxWidth(areaWidth);
                // As for now we assume that max width should be calculated so
                this.maxContent = isMaxWidthApplied ? minMaxWidth.GetMaxWidth() : AbstractRenderer.INF;
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
        }
    }
}
