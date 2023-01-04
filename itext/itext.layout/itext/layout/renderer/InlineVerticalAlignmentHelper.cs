/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Linq;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;
using iText.Kernel.Geom;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal sealed class InlineVerticalAlignmentHelper {
        private const float ADJUSTMENT_THRESHOLD = 0.001F;

        private const float SUPER_OFFSET = 0.3F;

        private const float SUB_OFFSET = -0.2F;

        private InlineVerticalAlignmentHelper() {
        }

        public static void AdjustChildrenYLineHtmlMode(LineRenderer lineRenderer) {
            float actualYLine = lineRenderer.occupiedArea.GetBBox().GetY() + lineRenderer.occupiedArea.GetBBox().GetHeight
                () - lineRenderer.maxDescent;
            // first round, all text based alignments
            ProcessRenderers(lineRenderer, lineRenderer.GetChildRenderers(), actualYLine, (alignment) => !IsBoxOrientedVerticalAlignment
                (alignment), (alignment) => !IsBoxOrientedVerticalAlignment(alignment));
            // next round, box oriented alignments
            IList<IRenderer> sortedRenderers = lineRenderer.GetChildRenderers().Sorted((r1, r2) => (int)MathematicUtil.Round
                ((r2.GetOccupiedArea().GetBBox().GetHeight() - r1.GetOccupiedArea().GetBBox().GetHeight()) * 1000)).ToList
                ();
            ProcessRenderers(lineRenderer, sortedRenderers, actualYLine, (alignment) => IsBoxOrientedVerticalAlignment
                (alignment), (alignment) => true);
        }

        private static bool IsBoxOrientedVerticalAlignment(InlineVerticalAlignment alignment) {
            return alignment.GetType() == InlineVerticalAlignmentType.TOP || alignment.GetType() == InlineVerticalAlignmentType
                .BOTTOM;
        }

        private static void ProcessRenderers(LineRenderer lineRenderer, IList<IRenderer> renderers, float actualYLine
            , Predicate<InlineVerticalAlignment> needProcess, Predicate<InlineVerticalAlignment> needRecalculateSizes
            ) {
            float[] fontInfo = LineHeightHelper.GetActualFontInfo(lineRenderer);
            float textTop = actualYLine + fontInfo[LineHeightHelper.ASCENDER_INDEX] - fontInfo[LineHeightHelper.LEADING_INDEX
                ] / 2;
            float textBottom = actualYLine + fontInfo[LineHeightHelper.DESCENDER_INDEX] - fontInfo[LineHeightHelper.LEADING_INDEX
                ] / 2;
            float leading = fontInfo[LineHeightHelper.LEADING_INDEX];
            float xHeight = fontInfo[LineHeightHelper.XHEIGHT_INDEX];
            float maxTop = float.Epsilon;
            float minBottom = float.MaxValue;
            float maxHeight = float.Epsilon;
            bool maxminValuesChanged = false;
            foreach (IRenderer renderer in renderers) {
                if (FloatingHelper.IsRendererFloating(renderer)) {
                    continue;
                }
                InlineVerticalAlignment alignment = renderer.GetProperty<InlineVerticalAlignment>(Property.INLINE_VERTICAL_ALIGNMENT
                    );
                if (alignment == null) {
                    alignment = new InlineVerticalAlignment();
                }
                if (needProcess(alignment)) {
                    Rectangle cBbox = GetAdjustedArea(renderer);
                    // Take into account new size but not apply it yet to the parent renderer
                    Rectangle pBbox = new Rectangle(lineRenderer.occupiedArea.GetBBox().GetX(), Math.Min(minBottom, lineRenderer
                        .occupiedArea.GetBBox().GetY()), lineRenderer.occupiedArea.GetBBox().GetWidth(), Math.Max(maxHeight, lineRenderer
                        .occupiedArea.GetBBox().GetHeight()));
                    float offset = CalculateOffset(renderer, cBbox, alignment, actualYLine, textTop, textBottom, leading, xHeight
                        , pBbox);
                    if (Math.Abs(offset) > ADJUSTMENT_THRESHOLD) {
                        renderer.Move(0, offset);
                    }
                }
                if (needRecalculateSizes(alignment)) {
                    Rectangle cBbox = GetAdjustedArea(renderer);
                    maxTop = Math.Max(maxTop, cBbox.GetTop());
                    minBottom = Math.Min(minBottom, cBbox.GetBottom());
                    maxHeight = Math.Max(maxHeight, cBbox.GetHeight());
                    maxminValuesChanged = true;
                }
            }
            // Adjust this and move children down as needed
            if (maxminValuesChanged) {
                AdjustBBox(lineRenderer, maxHeight, maxTop, minBottom);
            }
        }

        private static Rectangle GetAdjustedArea(IRenderer renderer) {
            Rectangle rect = renderer.GetOccupiedArea().GetBBox().Clone();
            if (renderer is AbstractRenderer && !(renderer is BlockRenderer) && !renderer.HasProperty(Property.INLINE_VERTICAL_ALIGNMENT
                )) {
                AbstractRenderer ar = (AbstractRenderer)renderer;
                ar.ApplyBorderBox(rect, false);
                ar.ApplyPaddings(rect, false);
            }
            return rect;
        }

        private static void AdjustBBox(LineRenderer lineRenderer, float maxHeight, float maxTop, float minBottom) {
            LineHeight lineHeight = lineRenderer.GetProperty<LineHeight>(Property.LINE_HEIGHT);
            float actualHeight = maxHeight;
            if (lineHeight != null) {
                actualHeight = Math.Max(actualHeight, LineHeightHelper.CalculateLineHeight(lineRenderer));
            }
            maxTop += (actualHeight - maxHeight) / 2;
            minBottom -= (actualHeight - maxHeight) / 2;
            maxHeight = actualHeight;
            maxHeight = Math.Max(maxHeight, maxTop - minBottom);
            float originalTop = lineRenderer.occupiedArea.GetBBox().GetTop();
            lineRenderer.occupiedArea.GetBBox().SetHeight(maxHeight);
            float delta = originalTop - lineRenderer.occupiedArea.GetBBox().GetTop();
            lineRenderer.occupiedArea.GetBBox().MoveUp(delta);
            float childDelta = originalTop - maxTop;
            foreach (IRenderer renderer in lineRenderer.GetChildRenderers()) {
                renderer.Move(0, childDelta);
            }
        }

        private static float CalculateOffset(IRenderer renderer, Rectangle cBBox, InlineVerticalAlignment alignment
            , float baseline, float textTop, float textBottom, float leading, float xHeight, Rectangle pBBox) {
            switch (alignment.GetType()) {
                case InlineVerticalAlignmentType.BASELINE: {
                    return baseline - GetChildBaseline(renderer, leading);
                }

                case InlineVerticalAlignmentType.TEXT_TOP: {
                    return textTop - cBBox.GetTop();
                }

                case InlineVerticalAlignmentType.TEXT_BOTTOM: {
                    return textBottom - cBBox.GetBottom();
                }

                case InlineVerticalAlignmentType.FIXED: {
                    float offsetFixed = 0;
                    offsetFixed = alignment.GetValue();
                    return baseline + offsetFixed - GetChildBaseline(renderer, leading);
                }

                case InlineVerticalAlignmentType.SUPER:
                case InlineVerticalAlignmentType.SUB:
                case InlineVerticalAlignmentType.FRACTION: {
                    float offsetFraction = 0;
                    if (alignment.GetType() == InlineVerticalAlignmentType.SUPER) {
                        offsetFraction = SUPER_OFFSET;
                    }
                    else {
                        if (alignment.GetType() == InlineVerticalAlignmentType.SUB) {
                            offsetFraction = SUB_OFFSET;
                        }
                        else {
                            offsetFraction = alignment.GetValue();
                        }
                    }
                    float target = baseline + (textTop - textBottom) * offsetFraction;
                    return target - GetChildBaseline(renderer, leading);
                }

                case InlineVerticalAlignmentType.MIDDLE: {
                    return (baseline + xHeight / 2) - (cBBox.GetBottom() + cBBox.GetHeight() / 2);
                }

                case InlineVerticalAlignmentType.BOTTOM: {
                    return pBBox.GetBottom() - cBBox.GetBottom();
                }

                case InlineVerticalAlignmentType.TOP: {
                    return pBBox.GetTop() - cBBox.GetTop();
                }

                default: {
                    return 0;
                }
            }
        }

        private static float GetChildBaseline(IRenderer renderer, float leading) {
            if (renderer is ILeafElementRenderer) {
                float descent = ((ILeafElementRenderer)renderer).GetDescent();
                return renderer.GetOccupiedArea().GetBBox().GetBottom() - descent;
            }
            else {
                float? yLine = LineRenderer.IsInlineBlockChild(renderer) && renderer is AbstractRenderer ? ((AbstractRenderer
                    )renderer).GetLastYLineRecursively() : null;
                return (yLine == null ? renderer.GetOccupiedArea().GetBBox().GetBottom() : (float)yLine - (leading / 2));
            }
        }
    }
}
