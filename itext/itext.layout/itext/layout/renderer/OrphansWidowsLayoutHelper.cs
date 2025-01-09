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
using iText.Kernel.Geom;
using iText.Layout.Layout;
using iText.Layout.Margincollapse;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal class OrphansWidowsLayoutHelper {
        private OrphansWidowsLayoutHelper() {
        }

//\cond DO_NOT_DOCUMENT
        internal static LayoutResult OrphansWidowsAwareLayout(ParagraphRenderer renderer, LayoutContext context, ParagraphOrphansControl
             orphansControl, ParagraphWidowsControl widowsControl) {
            OrphansWidowsLayoutHelper.OrphansWidowsLayoutAttempt layoutAttempt = AttemptLayout(renderer, context, context
                .GetArea().Clone());
            if (context.IsClippedHeight() || renderer.IsPositioned() || layoutAttempt.attemptResult.GetStatus() != LayoutResult
                .PARTIAL || layoutAttempt.attemptResult.GetSplitRenderer() == null) {
                return HandleAttemptAsSuccessful(layoutAttempt, context);
            }
            ParagraphRenderer splitRenderer = (ParagraphRenderer)layoutAttempt.attemptResult.GetSplitRenderer();
            bool orphansViolation = orphansControl != null && splitRenderer != null && splitRenderer.GetLines().Count 
                < orphansControl.GetMinOrphans() && !renderer.IsFirstOnRootArea();
            bool forcedPlacement = true.Equals(renderer.GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
            if (orphansViolation && forcedPlacement) {
                orphansControl.HandleViolatedOrphans(splitRenderer, "Ignored orphans constraint due to forced placement.");
            }
            if (orphansViolation && !forcedPlacement) {
                layoutAttempt = null;
            }
            else {
                if (widowsControl != null && splitRenderer != null && layoutAttempt.attemptResult.GetOverflowRenderer() !=
                     null) {
                    ParagraphRenderer overflowRenderer = (ParagraphRenderer)layoutAttempt.attemptResult.GetOverflowRenderer();
                    // Excessively big value to check if widows constraint is violated;
                    // Make this value less in order to improve performance if you are sure
                    // that min number of widows will fit in this height. E.g. A4 page height is 842.
                    int simulationHeight = 3500;
                    LayoutArea simulationArea = new LayoutArea(context.GetArea().GetPageNumber(), context.GetArea().GetBBox().
                        Clone().SetHeight(simulationHeight));
                    // collapsingMarginsInfo might affect available space, which is redundant in case we pass arbitrary space.
                    // floatedRendererAreas list on new area is considered empty. We don't know if there will be any, however their presence in any case will result in more widows, not less.
                    // clippedHeight is undefined for the next area, because it is defined by overflow part of the paragraph parent.
                    //               Even if it will be set to true in actual overflow-part layouting, stealing lines approach will result in
                    //               giving bigger part of MAX-HEIGHT to the overflow part and resulting in bigger number of widows, which is better.
                    //               However for possible other approaches which change content "length" (like word/char spacing adjusts),
                    //               if in actual overflow-part layouting clippedHeight will be true, those widows fixing attempts will result in worse results.
                    LayoutContext simulationContext = new LayoutContext(simulationArea);
                    LayoutResult simulationResult = overflowRenderer.DirectLayout(simulationContext);
                    if (simulationResult.GetStatus() == LayoutResult.FULL) {
                        // simulationHeight is excessively big in order to allow to layout all of the content remaining in overflowRenderer:
                        // this way after all of the remaining content is layouted we can check if it has led to widows violation.
                        // To make this analysis possible, we expect to get result FULL.
                        // if result is PARTIAL: means that simulationHeight value isn't big enough to layout all of the content remaining in overflowRenderer.
                        // In this case we assume that widows aren't violated since the amount of the lines to fit the simulatedHeight is expected to be very large.
                        // if result is NOTHING: unexpected result, limitation of simulation approach. Retry again with forced placement set.
                        int extraWidows = widowsControl.GetMinWidows() - overflowRenderer.GetLines().Count;
                        if (extraWidows > 0) {
                            int extraLinesToMove = orphansControl != null ? Math.Max(orphansControl.GetMinOrphans(), 1) : 1;
                            if (extraWidows <= widowsControl.GetMaxLinesToMove() && splitRenderer.GetLines().Count - extraWidows >= extraLinesToMove
                                ) {
                                LineRenderer lastLine = splitRenderer.GetLines()[splitRenderer.GetLines().Count - 1];
                                LineRenderer lastLineToLeave = splitRenderer.GetLines()[splitRenderer.GetLines().Count - extraWidows - 1];
                                float d = lastLineToLeave.GetOccupiedArea().GetBBox().GetY() - lastLine.GetOccupiedArea().GetBBox().GetY()
                                     - AbstractRenderer.EPS;
                                Rectangle smallerBBox = new Rectangle(context.GetArea().GetBBox());
                                smallerBBox.DecreaseHeight(d);
                                smallerBBox.MoveUp(d);
                                LayoutArea smallerAvailableArea = new LayoutArea(context.GetArea().GetPageNumber(), smallerBBox);
                                layoutAttempt = AttemptLayout(renderer, context, smallerAvailableArea);
                            }
                            else {
                                if (forcedPlacement || renderer.IsFirstOnRootArea() || !widowsControl.IsOverflowOnWidowsViolation()) {
                                    if (forcedPlacement) {
                                        widowsControl.HandleViolatedWidows(overflowRenderer, "forced placement");
                                    }
                                    else {
                                        widowsControl.HandleViolatedWidows(overflowRenderer, "inability to fix it");
                                    }
                                }
                                else {
                                    layoutAttempt = null;
                                }
                            }
                        }
                    }
                }
            }
            if (layoutAttempt != null) {
                return HandleAttemptAsSuccessful(layoutAttempt, context);
            }
            else {
                return new LayoutResult(LayoutResult.NOTHING, null, null, renderer);
            }
        }
//\endcond

        private static OrphansWidowsLayoutHelper.OrphansWidowsLayoutAttempt AttemptLayout(ParagraphRenderer renderer
            , LayoutContext originalContext, LayoutArea attemptArea) {
            OrphansWidowsLayoutHelper.OrphansWidowsLayoutAttempt attemptResult = new OrphansWidowsLayoutHelper.OrphansWidowsLayoutAttempt
                ();
            MarginsCollapseInfo copiedMarginsCollapseInfo = null;
            if (originalContext.GetMarginsCollapseInfo() != null) {
                copiedMarginsCollapseInfo = MarginsCollapseInfo.CreateDeepCopy(originalContext.GetMarginsCollapseInfo());
            }
            List<Rectangle> attemptFloatRectsList = new List<Rectangle>(originalContext.GetFloatRendererAreas());
            LayoutContext attemptContext = new LayoutContext(attemptArea, copiedMarginsCollapseInfo, attemptFloatRectsList
                , originalContext.IsClippedHeight());
            attemptResult.attemptContext = attemptContext;
            attemptResult.attemptResult = renderer.DirectLayout(attemptContext);
            return attemptResult;
        }

        private static LayoutResult HandleAttemptAsSuccessful(OrphansWidowsLayoutHelper.OrphansWidowsLayoutAttempt
             attemptResult, LayoutContext originalContext) {
            originalContext.GetFloatRendererAreas().Clear();
            originalContext.GetFloatRendererAreas().AddAll(attemptResult.attemptContext.GetFloatRendererAreas());
            if (originalContext.GetMarginsCollapseInfo() != null) {
                MarginsCollapseInfo.UpdateFromCopy(originalContext.GetMarginsCollapseInfo(), attemptResult.attemptContext.
                    GetMarginsCollapseInfo());
            }
            return attemptResult.attemptResult;
        }

        private class OrphansWidowsLayoutAttempt {
//\cond DO_NOT_DOCUMENT
            internal LayoutContext attemptContext;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal LayoutResult attemptResult;
//\endcond
        }
    }
//\endcond
}
