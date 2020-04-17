/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Text;
using Common.Logging;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class LineRenderer : AbstractRenderer {
        // AbstractRenderer.EPS is not enough here
        private const float MIN_MAX_WIDTH_CORRECTION_EPS = 0.001f;

        private static readonly ILog logger = LogManager.GetLogger(typeof(LineRenderer));

        protected internal float maxAscent;

        protected internal float maxDescent;

        // bidi levels
        protected internal byte[] levels;

        private float maxTextAscent;

        private float maxTextDescent;

        private float maxBlockAscent;

        private float maxBlockDescent;

        public override LayoutResult Layout(LayoutContext layoutContext) {
            Rectangle layoutBox = layoutContext.GetArea().GetBBox().Clone();
            bool wasParentsHeightClipped = layoutContext.IsClippedHeight();
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            OverflowPropertyValue? oldXOverflow = null;
            bool wasXOverflowChanged = false;
            if (floatRendererAreas != null) {
                float layoutWidth = layoutBox.GetWidth();
                FloatingHelper.AdjustLineAreaAccordingToFloats(floatRendererAreas, layoutBox);
                if (layoutWidth > layoutBox.GetWidth()) {
                    oldXOverflow = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
                    wasXOverflowChanged = true;
                    SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                }
            }
            bool noSoftWrap = true.Equals(this.GetOwnProperty<bool?>(Property.NO_SOFT_WRAP_INLINE));
            LineLayoutContext lineLayoutContext = layoutContext is LineLayoutContext ? (LineLayoutContext)layoutContext
                 : new LineLayoutContext(layoutContext);
            if (lineLayoutContext.GetTextIndent() != 0) {
                layoutBox.MoveRight(lineLayoutContext.GetTextIndent()).SetWidth(layoutBox.GetWidth() - lineLayoutContext.GetTextIndent
                    ());
            }
            occupiedArea = new LayoutArea(layoutContext.GetArea().GetPageNumber(), layoutBox.Clone().MoveUp(layoutBox.
                GetHeight()).SetHeight(0).SetWidth(0));
            UpdateChildrenParent();
            float curWidth = 0;
            if (RenderingMode.HTML_MODE.Equals(this.GetProperty<RenderingMode?>(Property.RENDERING_MODE)) && HasChildRendererInHtmlMode
                ()) {
                float[] ascenderDescender = LineHeightHelper.GetActualAscenderDescender(this);
                maxAscent = ascenderDescender[0];
                maxDescent = ascenderDescender[1];
            }
            else {
                maxAscent = 0;
                maxDescent = 0;
            }
            maxTextAscent = 0;
            maxTextDescent = 0;
            maxBlockAscent = -1e20f;
            maxBlockDescent = 1e20f;
            int childPos = 0;
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            AbstractWidthHandler widthHandler;
            if (noSoftWrap) {
                widthHandler = new SumSumWidthHandler(minMaxWidth);
            }
            else {
                widthHandler = new MaxSumWidthHandler(minMaxWidth);
            }
            ResolveChildrenFonts();
            int totalNumberOfTrimmedGlyphs = TrimFirst();
            BaseDirection? baseDirection = ApplyOtf();
            UpdateBidiLevels(totalNumberOfTrimmedGlyphs, baseDirection);
            bool anythingPlaced = false;
            TabStop hangingTabStop = null;
            LineLayoutResult result = null;
            bool floatsPlaced = false;
            IDictionary<int, IRenderer> floatsToNextPageSplitRenderers = new LinkedDictionary<int, IRenderer>();
            IList<IRenderer> floatsToNextPageOverflowRenderers = new List<IRenderer>();
            IList<IRenderer> floatsOverflowedToNextLine = new List<IRenderer>();
            int lastTabIndex = 0;
            while (childPos < childRenderers.Count) {
                IRenderer childRenderer = childRenderers[childPos];
                LayoutResult childResult = null;
                Rectangle bbox = new Rectangle(layoutBox.GetX() + curWidth, layoutBox.GetY(), layoutBox.GetWidth() - curWidth
                    , layoutBox.GetHeight());
                if (childRenderer is TextRenderer) {
                    // Delete these properties in case of relayout. We might have applied them during justify().
                    childRenderer.DeleteOwnProperty(Property.CHARACTER_SPACING);
                    childRenderer.DeleteOwnProperty(Property.WORD_SPACING);
                }
                else {
                    if (childRenderer is TabRenderer) {
                        if (hangingTabStop != null) {
                            IRenderer tabRenderer = childRenderers[childPos - 1];
                            tabRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(), bbox), wasParentsHeightClipped
                                ));
                            curWidth += tabRenderer.GetOccupiedArea().GetBBox().GetWidth();
                            widthHandler.UpdateMaxChildWidth(tabRenderer.GetOccupiedArea().GetBBox().GetWidth());
                        }
                        hangingTabStop = CalculateTab(childRenderer, curWidth, layoutBox.GetWidth());
                        if (childPos == childRenderers.Count - 1) {
                            hangingTabStop = null;
                        }
                        if (hangingTabStop != null) {
                            lastTabIndex = childPos;
                            ++childPos;
                            continue;
                        }
                    }
                }
                if (hangingTabStop != null && hangingTabStop.GetTabAlignment() == TabAlignment.ANCHOR && childRenderer is 
                    TextRenderer) {
                    childRenderer.SetProperty(Property.TAB_ANCHOR, hangingTabStop.GetTabAnchor());
                }
                // Normalize child width
                Object childWidth = childRenderer.GetProperty<Object>(Property.WIDTH);
                bool childWidthWasReplaced = false;
                bool childRendererHasOwnWidthProperty = childRenderer.HasOwnProperty(Property.WIDTH);
                if (childWidth is UnitValue && ((UnitValue)childWidth).IsPercentValue()) {
                    float normalizedChildWidth = ((UnitValue)childWidth).GetValue() / 100 * layoutContext.GetArea().GetBBox().
                        GetWidth();
                    normalizedChildWidth = DecreaseRelativeWidthByChildAdditionalWidth(childRenderer, normalizedChildWidth);
                    if (normalizedChildWidth > 0) {
                        childRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(normalizedChildWidth));
                        childWidthWasReplaced = true;
                    }
                }
                FloatPropertyValue? kidFloatPropertyVal = childRenderer.GetProperty<FloatPropertyValue?>(Property.FLOAT);
                bool isChildFloating = childRenderer is AbstractRenderer && FloatingHelper.IsRendererFloating(childRenderer
                    , kidFloatPropertyVal);
                if (isChildFloating) {
                    childResult = null;
                    MinMaxWidth kidMinMaxWidth = FloatingHelper.CalculateMinMaxWidthForFloat((AbstractRenderer)childRenderer, 
                        kidFloatPropertyVal);
                    float floatingBoxFullWidth = kidMinMaxWidth.GetMaxWidth();
                    // TODO width will be recalculated on float layout;
                    // also not taking it into account (i.e. not setting it on child renderer) results in differences with html
                    // when floating span is split on other line;
                    // TODO DEVSIX-1730: may be process floating spans as inline blocks always?
                    if (!wasXOverflowChanged && childPos > 0) {
                        oldXOverflow = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
                        wasXOverflowChanged = true;
                        SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                    }
                    if (!lineLayoutContext.IsFloatOverflowedToNextPageWithNothing() && floatsOverflowedToNextLine.IsEmpty() &&
                         (!anythingPlaced || floatingBoxFullWidth <= bbox.GetWidth())) {
                        childResult = childRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(
                            ), layoutContext.GetArea().GetBBox().Clone()), null, floatRendererAreas, wasParentsHeightClipped));
                    }
                    // Get back child width so that it's not lost
                    if (childWidthWasReplaced) {
                        if (childRendererHasOwnWidthProperty) {
                            childRenderer.SetProperty(Property.WIDTH, childWidth);
                        }
                        else {
                            childRenderer.DeleteOwnProperty(Property.WIDTH);
                        }
                    }
                    float minChildWidth = 0;
                    float maxChildWidth = 0;
                    if (childResult is MinMaxWidthLayoutResult) {
                        if (!childWidthWasReplaced) {
                            minChildWidth = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth().GetMinWidth();
                        }
                        // TODO if percents width was used, max width might be huge
                        maxChildWidth = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth().GetMaxWidth();
                        widthHandler.UpdateMinChildWidth(minChildWidth + AbstractRenderer.EPS);
                        widthHandler.UpdateMaxChildWidth(maxChildWidth + AbstractRenderer.EPS);
                    }
                    else {
                        widthHandler.UpdateMinChildWidth(kidMinMaxWidth.GetMinWidth() + AbstractRenderer.EPS);
                        widthHandler.UpdateMaxChildWidth(kidMinMaxWidth.GetMaxWidth() + AbstractRenderer.EPS);
                    }
                    if (childResult == null && !lineLayoutContext.IsFloatOverflowedToNextPageWithNothing()) {
                        floatsOverflowedToNextLine.Add(childRenderer);
                    }
                    else {
                        if (lineLayoutContext.IsFloatOverflowedToNextPageWithNothing() || childResult.GetStatus() == LayoutResult.
                            NOTHING) {
                            floatsToNextPageSplitRenderers.Put(childPos, null);
                            floatsToNextPageOverflowRenderers.Add(childRenderer);
                            lineLayoutContext.SetFloatOverflowedToNextPageWithNothing(true);
                        }
                        else {
                            if (childResult.GetStatus() == LayoutResult.PARTIAL) {
                                floatsPlaced = true;
                                if (childRenderer is TextRenderer) {
                                    // This code is specifically for floating inline text elements:
                                    // inline elements cannot have fixed width, also they progress horizontally, which means
                                    // that if they don't fit in one line, they will definitely be moved onto the new line (and also
                                    // under all floats). Specifying the whole width of layout area is required to avoid possible normal
                                    // content wrapping around floating text in case floating text gets wrapped onto the next line
                                    // not evenly.
                                    LineRenderer[] split = SplitNotFittingFloat(childPos, childResult);
                                    IRenderer splitRenderer = childResult.GetSplitRenderer();
                                    if (splitRenderer is TextRenderer) {
                                        ((TextRenderer)splitRenderer).TrimFirst();
                                        ((TextRenderer)splitRenderer).TrimLast();
                                    }
                                    // ensure no other thing (like text wrapping the float) will occupy the line
                                    splitRenderer.GetOccupiedArea().GetBBox().SetWidth(layoutContext.GetArea().GetBBox().GetWidth());
                                    result = new LineLayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1], null);
                                    break;
                                }
                                else {
                                    floatsToNextPageSplitRenderers.Put(childPos, childResult.GetSplitRenderer());
                                    floatsToNextPageOverflowRenderers.Add(childResult.GetOverflowRenderer());
                                    AdjustLineOnFloatPlaced(layoutBox, childPos, kidFloatPropertyVal, childResult.GetSplitRenderer().GetOccupiedArea
                                        ().GetBBox());
                                }
                            }
                            else {
                                floatsPlaced = true;
                                if (childRenderer is TextRenderer) {
                                    ((TextRenderer)childRenderer).TrimFirst();
                                    ((TextRenderer)childRenderer).TrimLast();
                                }
                                AdjustLineOnFloatPlaced(layoutBox, childPos, kidFloatPropertyVal, childRenderer.GetOccupiedArea().GetBBox(
                                    ));
                            }
                        }
                    }
                    childPos++;
                    if (!anythingPlaced && childResult != null && childResult.GetStatus() == LayoutResult.NOTHING && floatRendererAreas
                        .IsEmpty()) {
                        if (IsFirstOnRootArea()) {
                            // Current line is empty, kid returns nothing and neither floats nor content
                            // were met on root area (e.g. page area) - return NOTHING, don't layout other line content,
                            // expect FORCED_PLACEMENT to be set.
                            break;
                        }
                    }
                    continue;
                }
                MinMaxWidth childBlockMinMaxWidth = null;
                bool isInlineBlockChild = IsInlineBlockChild(childRenderer);
                if (!childWidthWasReplaced) {
                    if (isInlineBlockChild && childRenderer is AbstractRenderer) {
                        childBlockMinMaxWidth = ((AbstractRenderer)childRenderer).GetMinMaxWidth();
                        float childMaxWidth = childBlockMinMaxWidth.GetMaxWidth();
                        float lineFullAvailableWidth = layoutContext.GetArea().GetBBox().GetWidth() - lineLayoutContext.GetTextIndent
                            ();
                        if (!noSoftWrap && childMaxWidth > bbox.GetWidth() + MIN_MAX_WIDTH_CORRECTION_EPS && bbox.GetWidth() != lineFullAvailableWidth
                            ) {
                            childResult = new LineLayoutResult(LayoutResult.NOTHING, null, null, childRenderer, childRenderer);
                        }
                        else {
                            childMaxWidth += MIN_MAX_WIDTH_CORRECTION_EPS;
                            float inlineBlockWidth = Math.Min(childMaxWidth, lineFullAvailableWidth);
                            if (!IsOverflowFit(this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X))) {
                                float childMinWidth = childBlockMinMaxWidth.GetMinWidth() + MIN_MAX_WIDTH_CORRECTION_EPS;
                                inlineBlockWidth = Math.Max(childMinWidth, inlineBlockWidth);
                            }
                            bbox.SetWidth(inlineBlockWidth);
                            if (childBlockMinMaxWidth.GetMinWidth() > bbox.GetWidth()) {
                                if (logger.IsWarnEnabled) {
                                    logger.Warn(iText.IO.LogMessageConstant.INLINE_BLOCK_ELEMENT_WILL_BE_CLIPPED);
                                }
                                childRenderer.SetProperty(Property.FORCED_PLACEMENT, true);
                            }
                        }
                        childBlockMinMaxWidth.SetChildrenMaxWidth(childBlockMinMaxWidth.GetChildrenMaxWidth() + MIN_MAX_WIDTH_CORRECTION_EPS
                            );
                        childBlockMinMaxWidth.SetChildrenMinWidth(childBlockMinMaxWidth.GetChildrenMinWidth() + MIN_MAX_WIDTH_CORRECTION_EPS
                            );
                    }
                }
                if (childResult == null) {
                    if (!wasXOverflowChanged && childPos > 0) {
                        oldXOverflow = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
                        wasXOverflowChanged = true;
                        SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                    }
                    childResult = childRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(
                        ), bbox), wasParentsHeightClipped));
                    // it means that we've already increased layout area by MIN_MAX_WIDTH_CORRECTION_EPS
                    if (childResult is MinMaxWidthLayoutResult && null != childBlockMinMaxWidth) {
                        MinMaxWidth childResultMinMaxWidth = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth();
                        childResultMinMaxWidth.SetChildrenMaxWidth(childResultMinMaxWidth.GetChildrenMaxWidth() + MIN_MAX_WIDTH_CORRECTION_EPS
                            );
                        childResultMinMaxWidth.SetChildrenMinWidth(childResultMinMaxWidth.GetChildrenMinWidth() + MIN_MAX_WIDTH_CORRECTION_EPS
                            );
                    }
                }
                // Get back child width so that it's not lost
                if (childWidthWasReplaced) {
                    if (childRendererHasOwnWidthProperty) {
                        childRenderer.SetProperty(Property.WIDTH, childWidth);
                    }
                    else {
                        childRenderer.DeleteOwnProperty(Property.WIDTH);
                    }
                }
                float minChildWidth_1 = 0;
                float maxChildWidth_1 = 0;
                if (childResult is MinMaxWidthLayoutResult) {
                    if (!childWidthWasReplaced) {
                        minChildWidth_1 = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth().GetMinWidth();
                    }
                    maxChildWidth_1 = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth().GetMaxWidth();
                }
                else {
                    if (childBlockMinMaxWidth != null) {
                        minChildWidth_1 = childBlockMinMaxWidth.GetMinWidth();
                        maxChildWidth_1 = childBlockMinMaxWidth.GetMaxWidth();
                    }
                }
                float childAscent = 0;
                float childDescent = 0;
                if (childRenderer is ILeafElementRenderer && childResult.GetStatus() != LayoutResult.NOTHING) {
                    if (RenderingMode.HTML_MODE.Equals(childRenderer.GetProperty<RenderingMode?>(Property.RENDERING_MODE)) && 
                        childRenderer is TextRenderer) {
                        float[] ascenderDescender = LineHeightHelper.GetActualAscenderDescender((TextRenderer)childRenderer);
                        childAscent = ascenderDescender[0];
                        childDescent = ascenderDescender[1];
                    }
                    else {
                        childAscent = ((ILeafElementRenderer)childRenderer).GetAscent();
                        childDescent = ((ILeafElementRenderer)childRenderer).GetDescent();
                    }
                }
                else {
                    if (isInlineBlockChild && childResult.GetStatus() != LayoutResult.NOTHING) {
                        if (childRenderer is AbstractRenderer) {
                            float? yLine = ((AbstractRenderer)childRenderer).GetLastYLineRecursively();
                            if (yLine == null) {
                                childAscent = childRenderer.GetOccupiedArea().GetBBox().GetHeight();
                            }
                            else {
                                childAscent = childRenderer.GetOccupiedArea().GetBBox().GetTop() - (float)yLine;
                                childDescent = -((float)yLine - childRenderer.GetOccupiedArea().GetBBox().GetBottom());
                            }
                        }
                        else {
                            childAscent = childRenderer.GetOccupiedArea().GetBBox().GetHeight();
                        }
                    }
                }
                bool newLineOccurred = (childResult is TextLayoutResult && ((TextLayoutResult)childResult).IsSplitForcedByNewline
                    ());
                bool shouldBreakLayouting = childResult.GetStatus() != LayoutResult.FULL || newLineOccurred;
                bool wordWasSplitAndItWillFitOntoNextLine = false;
                if (shouldBreakLayouting && childResult is TextLayoutResult && ((TextLayoutResult)childResult).IsWordHasBeenSplit
                    ()) {
                    if (wasXOverflowChanged) {
                        SetProperty(Property.OVERFLOW_X, oldXOverflow);
                    }
                    LayoutResult newLayoutResult = childRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea
                        ().GetPageNumber(), layoutBox), wasParentsHeightClipped));
                    if (wasXOverflowChanged) {
                        SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                    }
                    if (newLayoutResult is TextLayoutResult && !((TextLayoutResult)newLayoutResult).IsWordHasBeenSplit()) {
                        wordWasSplitAndItWillFitOntoNextLine = true;
                    }
                }
                if (!wordWasSplitAndItWillFitOntoNextLine) {
                    maxAscent = Math.Max(maxAscent, childAscent);
                    if (childRenderer is TextRenderer) {
                        maxTextAscent = Math.Max(maxTextAscent, childAscent);
                    }
                    else {
                        if (!isChildFloating) {
                            maxBlockAscent = Math.Max(maxBlockAscent, childAscent);
                        }
                    }
                    maxDescent = Math.Min(maxDescent, childDescent);
                    if (childRenderer is TextRenderer) {
                        maxTextDescent = Math.Min(maxTextDescent, childDescent);
                    }
                    else {
                        if (!isChildFloating) {
                            maxBlockDescent = Math.Min(maxBlockDescent, childDescent);
                        }
                    }
                }
                float maxHeight = maxAscent - maxDescent;
                float currChildTextIndent = anythingPlaced ? 0 : lineLayoutContext.GetTextIndent();
                if (hangingTabStop != null && (TabAlignment.LEFT == hangingTabStop.GetTabAlignment() || shouldBreakLayouting
                     || childRenderers.Count - 1 == childPos || childRenderers[childPos + 1] is TabRenderer)) {
                    IRenderer tabRenderer = childRenderers[lastTabIndex];
                    IList<IRenderer> affectedRenderers = new List<IRenderer>();
                    affectedRenderers.AddAll(childRenderers.SubList(lastTabIndex + 1, childPos + 1));
                    float tabWidth = CalculateTab(layoutBox, curWidth, hangingTabStop, affectedRenderers, tabRenderer);
                    tabRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(), bbox), wasParentsHeightClipped
                        ));
                    float sumOfAffectedRendererWidths = 0;
                    foreach (IRenderer renderer in affectedRenderers) {
                        renderer.Move(tabWidth + sumOfAffectedRendererWidths, 0);
                        sumOfAffectedRendererWidths += renderer.GetOccupiedArea().GetBBox().GetWidth();
                    }
                    if (childResult.GetSplitRenderer() != null) {
                        childResult.GetSplitRenderer().Move(tabWidth + sumOfAffectedRendererWidths - childResult.GetSplitRenderer(
                            ).GetOccupiedArea().GetBBox().GetWidth(), 0);
                    }
                    float tabAndNextElemWidth = tabWidth + childResult.GetOccupiedArea().GetBBox().GetWidth();
                    if (hangingTabStop.GetTabAlignment() == TabAlignment.RIGHT && curWidth + tabAndNextElemWidth < hangingTabStop
                        .GetTabPosition()) {
                        curWidth = hangingTabStop.GetTabPosition();
                    }
                    else {
                        curWidth += tabAndNextElemWidth;
                    }
                    widthHandler.UpdateMinChildWidth(minChildWidth_1 + currChildTextIndent);
                    widthHandler.UpdateMaxChildWidth(tabWidth + maxChildWidth_1 + currChildTextIndent);
                    hangingTabStop = null;
                }
                else {
                    if (null == hangingTabStop) {
                        if (childResult.GetOccupiedArea() != null && childResult.GetOccupiedArea().GetBBox() != null) {
                            curWidth += childResult.GetOccupiedArea().GetBBox().GetWidth();
                        }
                        widthHandler.UpdateMinChildWidth(minChildWidth_1 + currChildTextIndent);
                        widthHandler.UpdateMaxChildWidth(maxChildWidth_1 + currChildTextIndent);
                    }
                }
                if (!wordWasSplitAndItWillFitOntoNextLine) {
                    occupiedArea.SetBBox(new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox.GetHeight() - maxHeight, 
                        curWidth, maxHeight));
                }
                if (shouldBreakLayouting) {
                    LineRenderer[] split = Split();
                    split[0].childRenderers = new List<IRenderer>(childRenderers.SubList(0, childPos));
                    if (wordWasSplitAndItWillFitOntoNextLine) {
                        split[1].childRenderers.Add(childRenderer);
                        split[1].childRenderers.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
                    }
                    else {
                        bool forcePlacement = true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
                        bool isInlineBlockAndFirstOnRootArea = isInlineBlockChild && IsFirstOnRootArea();
                        if (childResult.GetStatus() == LayoutResult.PARTIAL && (!isInlineBlockChild || forcePlacement || isInlineBlockAndFirstOnRootArea
                            ) || childResult.GetStatus() == LayoutResult.FULL) {
                            split[0].AddChild(childResult.GetSplitRenderer());
                            anythingPlaced = true;
                        }
                        if (null != childResult.GetOverflowRenderer()) {
                            if (isInlineBlockChild && !forcePlacement && !isInlineBlockAndFirstOnRootArea) {
                                split[1].childRenderers.Add(childRenderer);
                            }
                            else {
                                if (isInlineBlockChild && childResult.GetOverflowRenderer().GetChildRenderers().Count == 0 && childResult.
                                    GetStatus() == LayoutResult.PARTIAL) {
                                    if (logger.IsWarnEnabled) {
                                        logger.Warn(iText.IO.LogMessageConstant.INLINE_BLOCK_ELEMENT_WILL_BE_CLIPPED);
                                    }
                                }
                                else {
                                    split[1].childRenderers.Add(childResult.GetOverflowRenderer());
                                }
                            }
                        }
                        split[1].childRenderers.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
                    }
                    ReplaceSplitRendererKidFloats(floatsToNextPageSplitRenderers, split[0]);
                    split[0].childRenderers.RemoveAll(floatsOverflowedToNextLine);
                    split[1].childRenderers.AddAll(0, floatsOverflowedToNextLine);
                    // no sense to process empty renderer
                    if (split[1].childRenderers.Count == 0 && floatsToNextPageOverflowRenderers.IsEmpty()) {
                        split[1] = null;
                    }
                    IRenderer causeOfNothing = childResult.GetStatus() == LayoutResult.NOTHING ? childResult.GetCauseOfNothing
                        () : childRenderer;
                    if (split[1] == null) {
                        result = new LineLayoutResult(LayoutResult.FULL, occupiedArea, split[0], split[1], causeOfNothing);
                    }
                    else {
                        if (anythingPlaced || floatsPlaced) {
                            result = new LineLayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1], causeOfNothing);
                        }
                        else {
                            result = new LineLayoutResult(LayoutResult.NOTHING, null, split[0], split[1], null);
                        }
                        result.SetFloatsOverflowedToNextPage(floatsToNextPageOverflowRenderers);
                    }
                    if (newLineOccurred) {
                        result.SetSplitForcedByNewline(true);
                    }
                    break;
                }
                else {
                    anythingPlaced = true;
                    childPos++;
                }
            }
            if (result == null) {
                bool noOverflowedFloats = floatsOverflowedToNextLine.IsEmpty() && floatsToNextPageOverflowRenderers.IsEmpty
                    ();
                if ((anythingPlaced || floatsPlaced) && noOverflowedFloats || 0 == childRenderers.Count) {
                    result = new LineLayoutResult(LayoutResult.FULL, occupiedArea, null, null);
                }
                else {
                    if (noOverflowedFloats) {
                        // all kids were some non-image and non-text kids (tab-stops?),
                        // but in this case, it should be okay to return FULL, as there is nothing to be placed
                        result = new LineLayoutResult(LayoutResult.FULL, occupiedArea, null, null);
                    }
                    else {
                        if (anythingPlaced || floatsPlaced) {
                            LineRenderer[] split = Split();
                            split[0].childRenderers.AddAll(childRenderers.SubList(0, childPos));
                            ReplaceSplitRendererKidFloats(floatsToNextPageSplitRenderers, split[0]);
                            split[0].childRenderers.RemoveAll(floatsOverflowedToNextLine);
                            // If `result` variable is null up until now but not everything was placed - there is no
                            // content overflow, only floats are overflowing.
                            // The floatsOverflowedToNextLine might be empty, while the only overflowing floats are
                            // in floatsToNextPageOverflowRenderers. This situation is handled in ParagraphRenderer separately.
                            split[1].childRenderers.AddAll(floatsOverflowedToNextLine);
                            result = new LineLayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1], null);
                            result.SetFloatsOverflowedToNextPage(floatsToNextPageOverflowRenderers);
                        }
                        else {
                            IRenderer causeOfNothing = floatsOverflowedToNextLine.IsEmpty() ? floatsToNextPageOverflowRenderers[0] : floatsOverflowedToNextLine
                                [0];
                            result = new LineLayoutResult(LayoutResult.NOTHING, null, null, this, causeOfNothing);
                        }
                    }
                }
            }
            if (baseDirection != null && baseDirection != BaseDirection.NO_BIDI) {
                IList<IRenderer> children = null;
                if (result.GetStatus() == LayoutResult.PARTIAL) {
                    children = result.GetSplitRenderer().GetChildRenderers();
                }
                else {
                    if (result.GetStatus() == LayoutResult.FULL) {
                        children = GetChildRenderers();
                    }
                }
                if (children != null) {
                    bool newLineFound = false;
                    IList<LineRenderer.RendererGlyph> lineGlyphs = new List<LineRenderer.RendererGlyph>();
                    // We shouldn't forget about images, float, inline-blocks that has to be inserted somewhere.
                    // TODO determine correct place to insert this content. Probably consider inline floats separately.
                    IDictionary<TextRenderer, IList<IRenderer>> insertAfter = new Dictionary<TextRenderer, IList<IRenderer>>();
                    IList<IRenderer> starterNonTextRenderers = new List<IRenderer>();
                    TextRenderer lastTextRenderer = null;
                    foreach (IRenderer child in children) {
                        if (newLineFound) {
                            break;
                        }
                        if (child is TextRenderer) {
                            GlyphLine childLine = ((TextRenderer)child).line;
                            for (int i = childLine.start; i < childLine.end; i++) {
                                if (iText.IO.Util.TextUtil.IsNewLine(childLine.Get(i))) {
                                    newLineFound = true;
                                    break;
                                }
                                lineGlyphs.Add(new LineRenderer.RendererGlyph(childLine.Get(i), (TextRenderer)child));
                            }
                            lastTextRenderer = (TextRenderer)child;
                        }
                        else {
                            if (lastTextRenderer != null) {
                                if (!insertAfter.ContainsKey(lastTextRenderer)) {
                                    insertAfter.Put(lastTextRenderer, new List<IRenderer>());
                                }
                                insertAfter.Get(lastTextRenderer).Add(child);
                            }
                            else {
                                starterNonTextRenderers.Add(child);
                            }
                        }
                    }
                    byte[] lineLevels = new byte[lineGlyphs.Count];
                    if (levels != null) {
                        Array.Copy(levels, 0, lineLevels, 0, lineGlyphs.Count);
                    }
                    int[] reorder = TypographyUtils.ReorderLine(lineGlyphs, lineLevels, levels);
                    if (reorder != null) {
                        children.Clear();
                        int pos = 0;
                        int initialPos = 0;
                        bool reversed = false;
                        int offset = 0;
                        // Insert non-text renderers
                        foreach (IRenderer child in starterNonTextRenderers) {
                            children.Add(child);
                        }
                        while (pos < lineGlyphs.Count) {
                            IRenderer renderer = lineGlyphs[pos].renderer;
                            TextRenderer newRenderer = new TextRenderer((TextRenderer)renderer).RemoveReversedRanges();
                            children.Add(newRenderer);
                            // Insert non-text renderers
                            if (insertAfter.ContainsKey((TextRenderer)renderer)) {
                                children.AddAll(insertAfter.Get((TextRenderer)renderer));
                                insertAfter.JRemove((TextRenderer)renderer);
                            }
                            newRenderer.line = new GlyphLine(newRenderer.line);
                            IList<Glyph> replacementGlyphs = new List<Glyph>();
                            while (pos < lineGlyphs.Count && lineGlyphs[pos].renderer == renderer) {
                                if (pos + 1 < lineGlyphs.Count) {
                                    if (reorder[pos] == reorder[pos + 1] + 1 && !iText.IO.Util.TextUtil.IsSpaceOrWhitespace(lineGlyphs[pos + 1
                                        ].glyph) && !iText.IO.Util.TextUtil.IsSpaceOrWhitespace(lineGlyphs[pos].glyph)) {
                                        reversed = true;
                                    }
                                    else {
                                        if (reversed) {
                                            IList<int[]> reversedRange = newRenderer.InitReversedRanges();
                                            reversedRange.Add(new int[] { initialPos - offset, pos - offset });
                                            reversed = false;
                                        }
                                        initialPos = pos + 1;
                                    }
                                }
                                replacementGlyphs.Add(lineGlyphs[pos].glyph);
                                pos++;
                            }
                            if (reversed) {
                                IList<int[]> reversedRange = newRenderer.InitReversedRanges();
                                reversedRange.Add(new int[] { initialPos - offset, pos - 1 - offset });
                                reversed = false;
                                initialPos = pos;
                            }
                            offset = initialPos;
                            newRenderer.line.SetGlyphs(replacementGlyphs);
                        }
                        AdjustChildPositionsAfterReordering(children, occupiedArea.GetBBox().GetLeft());
                    }
                    if (result.GetStatus() == LayoutResult.PARTIAL) {
                        LineRenderer overflow = (LineRenderer)result.GetOverflowRenderer();
                        if (levels != null) {
                            overflow.levels = new byte[levels.Length - lineLevels.Length];
                            Array.Copy(levels, lineLevels.Length, overflow.levels, 0, overflow.levels.Length);
                            if (overflow.levels.Length == 0) {
                                overflow.levels = null;
                            }
                        }
                    }
                }
            }
            LineRenderer processed = result.GetStatus() == LayoutResult.FULL ? this : (LineRenderer)result.GetSplitRenderer
                ();
            if (anythingPlaced || floatsPlaced) {
                processed.AdjustChildrenYLine().TrimLast();
                result.SetMinMaxWidth(minMaxWidth);
            }
            if (wasXOverflowChanged) {
                SetProperty(Property.OVERFLOW_X, oldXOverflow);
                if (null != result.GetSplitRenderer()) {
                    result.GetSplitRenderer().SetProperty(Property.OVERFLOW_X, oldXOverflow);
                }
                if (null != result.GetOverflowRenderer()) {
                    result.GetOverflowRenderer().SetProperty(Property.OVERFLOW_X, oldXOverflow);
                }
            }
            return result;
        }

        public virtual float GetMaxAscent() {
            return maxAscent;
        }

        public virtual float GetMaxDescent() {
            return maxDescent;
        }

        public virtual float GetYLine() {
            return occupiedArea.GetBBox().GetY() - maxDescent;
        }

        public virtual float GetLeadingValue(Leading leading) {
            switch (leading.GetLeadingType()) {
                case Leading.FIXED: {
                    return Math.Max(leading.GetValue(), maxBlockAscent - maxBlockDescent);
                }

                case Leading.MULTIPLIED: {
                    return GetTopLeadingIndent(leading) + GetBottomLeadingIndent(leading);
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
        }

        public override IRenderer GetNextRenderer() {
            return new LineRenderer();
        }

        protected internal override float? GetFirstYLineRecursively() {
            return GetYLine();
        }

        protected internal override float? GetLastYLineRecursively() {
            return GetYLine();
        }

        public virtual void Justify(float width) {
            float ratio = (float)this.GetPropertyAsFloat(Property.SPACING_RATIO);
            IRenderer lastChildRenderer = GetLastNonFloatChildRenderer();
            if (lastChildRenderer == null) {
                return;
            }
            float freeWidth = occupiedArea.GetBBox().GetX() + width - lastChildRenderer.GetOccupiedArea().GetBBox().GetX
                () - lastChildRenderer.GetOccupiedArea().GetBBox().GetWidth();
            int numberOfSpaces = GetNumberOfSpaces();
            int baseCharsCount = BaseCharactersCount();
            float baseFactor = freeWidth / (ratio * numberOfSpaces + (1 - ratio) * (baseCharsCount - 1));
            //Prevent a NaN when trying to justify a single word with spacing_ratio == 1.0
            if (float.IsInfinity(baseFactor)) {
                baseFactor = 0;
            }
            float wordSpacing = ratio * baseFactor;
            float characterSpacing = (1 - ratio) * baseFactor;
            float lastRightPos = occupiedArea.GetBBox().GetX();
            foreach (IRenderer child in childRenderers) {
                if (FloatingHelper.IsRendererFloating(child)) {
                    continue;
                }
                float childX = child.GetOccupiedArea().GetBBox().GetX();
                child.Move(lastRightPos - childX, 0);
                childX = lastRightPos;
                if (child is TextRenderer) {
                    float childHSCale = (float)((TextRenderer)child).GetPropertyAsFloat(Property.HORIZONTAL_SCALING, 1f);
                    float? oldCharacterSpacing = ((TextRenderer)child).GetPropertyAsFloat(Property.CHARACTER_SPACING);
                    float? oldWordSpacing = ((TextRenderer)child).GetPropertyAsFloat(Property.WORD_SPACING);
                    child.SetProperty(Property.CHARACTER_SPACING, (null == oldCharacterSpacing ? 0 : (float)oldCharacterSpacing
                        ) + characterSpacing / childHSCale);
                    child.SetProperty(Property.WORD_SPACING, (null == oldWordSpacing ? 0 : (float)oldWordSpacing) + wordSpacing
                         / childHSCale);
                    bool isLastTextRenderer = child == lastChildRenderer;
                    float widthAddition = (isLastTextRenderer ? (((TextRenderer)child).LineLength() - 1) : ((TextRenderer)child
                        ).LineLength()) * characterSpacing + wordSpacing * ((TextRenderer)child).GetNumberOfSpaces();
                    child.GetOccupiedArea().GetBBox().SetWidth(child.GetOccupiedArea().GetBBox().GetWidth() + widthAddition);
                }
                lastRightPos = childX + child.GetOccupiedArea().GetBBox().GetWidth();
            }
            GetOccupiedArea().GetBBox().SetWidth(width);
        }

        protected internal virtual int GetNumberOfSpaces() {
            int spaces = 0;
            foreach (IRenderer child in childRenderers) {
                if (child is TextRenderer && !FloatingHelper.IsRendererFloating(child)) {
                    spaces += ((TextRenderer)child).GetNumberOfSpaces();
                }
            }
            return spaces;
        }

        /// <summary>Gets the total lengths of characters in this line.</summary>
        /// <remarks>
        /// Gets the total lengths of characters in this line. Other elements (images, tables) are not taken
        /// into account.
        /// </remarks>
        /// <returns>the total lengths of characters in this line.</returns>
        protected internal virtual int Length() {
            int length = 0;
            foreach (IRenderer child in childRenderers) {
                if (child is TextRenderer && !FloatingHelper.IsRendererFloating(child)) {
                    length += ((TextRenderer)child).LineLength();
                }
            }
            return length;
        }

        /// <summary>Returns the number of base characters, i.e. non-mark characters</summary>
        /// <returns>the number of base non-mark characters</returns>
        protected internal virtual int BaseCharactersCount() {
            int count = 0;
            foreach (IRenderer child in childRenderers) {
                if (child is TextRenderer && !FloatingHelper.IsRendererFloating(child)) {
                    count += ((TextRenderer)child).BaseCharactersCount();
                }
            }
            return count;
        }

        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (IRenderer renderer in childRenderers) {
                sb.Append(renderer.ToString());
            }
            return sb.ToString();
        }

        protected internal virtual LineRenderer CreateSplitRenderer() {
            return (LineRenderer)GetNextRenderer();
        }

        protected internal virtual LineRenderer CreateOverflowRenderer() {
            return (LineRenderer)GetNextRenderer();
        }

        protected internal virtual LineRenderer[] Split() {
            LineRenderer splitRenderer = CreateSplitRenderer();
            splitRenderer.occupiedArea = occupiedArea.Clone();
            splitRenderer.parent = parent;
            splitRenderer.maxAscent = maxAscent;
            splitRenderer.maxDescent = maxDescent;
            splitRenderer.maxTextAscent = maxTextAscent;
            splitRenderer.maxTextDescent = maxTextDescent;
            splitRenderer.maxBlockAscent = maxBlockAscent;
            splitRenderer.maxBlockDescent = maxBlockDescent;
            splitRenderer.levels = levels;
            splitRenderer.AddAllProperties(GetOwnProperties());
            LineRenderer overflowRenderer = CreateOverflowRenderer();
            overflowRenderer.parent = parent;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            return new LineRenderer[] { splitRenderer, overflowRenderer };
        }

        protected internal virtual LineRenderer AdjustChildrenYLine() {
            float actualYLine = occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight() - maxAscent;
            foreach (IRenderer renderer in childRenderers) {
                if (FloatingHelper.IsRendererFloating(renderer)) {
                    continue;
                }
                if (renderer is ILeafElementRenderer) {
                    float descent = ((ILeafElementRenderer)renderer).GetDescent();
                    renderer.Move(0, actualYLine - renderer.GetOccupiedArea().GetBBox().GetBottom() + descent);
                }
                else {
                    float? yLine = IsInlineBlockChild(renderer) && renderer is AbstractRenderer ? ((AbstractRenderer)renderer)
                        .GetLastYLineRecursively() : null;
                    renderer.Move(0, actualYLine - (yLine == null ? renderer.GetOccupiedArea().GetBBox().GetBottom() : (float)
                        yLine));
                }
            }
            return this;
        }

        protected internal virtual void ApplyLeading(float deltaY) {
            occupiedArea.GetBBox().MoveUp(deltaY);
            occupiedArea.GetBBox().DecreaseHeight(deltaY);
            foreach (IRenderer child in childRenderers) {
                if (!FloatingHelper.IsRendererFloating(child)) {
                    child.Move(0, deltaY);
                }
            }
        }

        // TODO for floats we don't apply any leading for the moment (and therefore line-height for pdf2html is not entirely supported in terms of floats)
        protected internal virtual LineRenderer TrimLast() {
            int lastIndex = childRenderers.Count;
            IRenderer lastRenderer = null;
            while (--lastIndex >= 0) {
                lastRenderer = childRenderers[lastIndex];
                if (!FloatingHelper.IsRendererFloating(lastRenderer)) {
                    break;
                }
            }
            if (lastRenderer is TextRenderer && lastIndex >= 0) {
                float trimmedSpace = ((TextRenderer)lastRenderer).TrimLast();
                occupiedArea.GetBBox().SetWidth(occupiedArea.GetBBox().GetWidth() - trimmedSpace);
            }
            return this;
        }

        public virtual bool ContainsImage() {
            foreach (IRenderer renderer in childRenderers) {
                if (renderer is ImageRenderer) {
                    return true;
                }
            }
            return false;
        }

        public override MinMaxWidth GetMinMaxWidth() {
            LineLayoutResult result = (LineLayoutResult)Layout(new LayoutContext(new LayoutArea(1, new Rectangle(MinMaxWidthUtils
                .GetInfWidth(), AbstractRenderer.INF))));
            return result.GetMinMaxWidth();
        }

        internal virtual bool HasChildRendererInHtmlMode() {
            foreach (IRenderer childRenderer in childRenderers) {
                if (RenderingMode.HTML_MODE.Equals(childRenderer.GetProperty<RenderingMode?>(Property.RENDERING_MODE))) {
                    return true;
                }
            }
            return false;
        }

        internal virtual float GetTopLeadingIndent(Leading leading) {
            switch (leading.GetLeadingType()) {
                case Leading.FIXED: {
                    return (Math.Max(leading.GetValue(), maxBlockAscent - maxBlockDescent) - occupiedArea.GetBBox().GetHeight(
                        )) / 2;
                }

                case Leading.MULTIPLIED: {
                    UnitValue fontSize = this.GetProperty<UnitValue>(Property.FONT_SIZE, UnitValue.CreatePointValue(0f));
                    if (!fontSize.IsPointValue()) {
                        logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Property
                            .FONT_SIZE));
                    }
                    // In HTML, depending on whether <!DOCTYPE html> is present or not, and if present then depending on the version,
                    // the behavior id different. In one case, bottom leading indent is added for images, in the other it is not added.
                    // This is why !containsImage() is present below. Depending on the presence of this !containsImage() condition, the behavior changes
                    // between the two possible scenarios in HTML.
                    float textAscent = maxTextAscent == 0 && maxTextDescent == 0 && Math.Abs(maxAscent) + Math.Abs(maxDescent)
                         != 0 && !ContainsImage() ? fontSize.GetValue() * 0.8f : maxTextAscent;
                    float textDescent = maxTextAscent == 0 && maxTextDescent == 0 && Math.Abs(maxAscent) + Math.Abs(maxDescent
                        ) != 0 && !ContainsImage() ? -fontSize.GetValue() * 0.2f : maxTextDescent;
                    return Math.Max(textAscent + ((textAscent - textDescent) * (leading.GetValue() - 1)) / 2, maxBlockAscent) 
                        - maxAscent;
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
        }

        internal virtual float GetBottomLeadingIndent(Leading leading) {
            switch (leading.GetLeadingType()) {
                case Leading.FIXED: {
                    return (Math.Max(leading.GetValue(), maxBlockAscent - maxBlockDescent) - occupiedArea.GetBBox().GetHeight(
                        )) / 2;
                }

                case Leading.MULTIPLIED: {
                    UnitValue fontSize = this.GetProperty<UnitValue>(Property.FONT_SIZE, UnitValue.CreatePointValue(0f));
                    if (!fontSize.IsPointValue()) {
                        logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Property
                            .FONT_SIZE));
                    }
                    // In HTML, depending on whether <!DOCTYPE html> is present or not, and if present then depending on the version,
                    // the behavior id different. In one case, bottom leading indent is added for images, in the other it is not added.
                    // This is why !containsImage() is present below. Depending on the presence of this !containsImage() condition, the behavior changes
                    // between the two possible scenarios in HTML.
                    float textAscent = maxTextAscent == 0 && maxTextDescent == 0 && !ContainsImage() ? fontSize.GetValue() * 0.8f
                         : maxTextAscent;
                    float textDescent = maxTextAscent == 0 && maxTextDescent == 0 && !ContainsImage() ? -fontSize.GetValue() *
                         0.2f : maxTextDescent;
                    return Math.Max(-textDescent + ((textAscent - textDescent) * (leading.GetValue() - 1)) / 2, -maxBlockDescent
                        ) + maxDescent;
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
        }

        internal static void AdjustChildPositionsAfterReordering(IList<IRenderer> children, float initialXPos) {
            float currentXPos = initialXPos;
            foreach (IRenderer child in children) {
                if (!FloatingHelper.IsRendererFloating(child)) {
                    float currentWidth;
                    if (child is TextRenderer) {
                        currentWidth = ((TextRenderer)child).CalculateLineWidth();
                        UnitValue[] margins = ((TextRenderer)child).GetMargins();
                        if (!margins[1].IsPointValue() && logger.IsErrorEnabled) {
                            logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, "right margin"
                                ));
                        }
                        if (!margins[3].IsPointValue() && logger.IsErrorEnabled) {
                            logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, "left margin"
                                ));
                        }
                        UnitValue[] paddings = ((TextRenderer)child).GetPaddings();
                        if (!paddings[1].IsPointValue() && logger.IsErrorEnabled) {
                            logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, "right padding"
                                ));
                        }
                        if (!paddings[3].IsPointValue() && logger.IsErrorEnabled) {
                            logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, "left padding"
                                ));
                        }
                        currentWidth += margins[1].GetValue() + margins[3].GetValue() + paddings[1].GetValue() + paddings[3].GetValue
                            ();
                        ((TextRenderer)child).occupiedArea.GetBBox().SetX(currentXPos).SetWidth(currentWidth);
                    }
                    else {
                        currentWidth = child.GetOccupiedArea().GetBBox().GetWidth();
                        child.Move(currentXPos - child.GetOccupiedArea().GetBBox().GetX(), 0);
                    }
                    currentXPos += currentWidth;
                }
            }
        }

        private LineRenderer[] SplitNotFittingFloat(int childPos, LayoutResult childResult) {
            LineRenderer[] split = Split();
            split[0].childRenderers.AddAll(childRenderers.SubList(0, childPos));
            split[0].childRenderers.Add(childResult.GetSplitRenderer());
            split[1].childRenderers.Add(childResult.GetOverflowRenderer());
            split[1].childRenderers.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
            return split;
        }

        private void AdjustLineOnFloatPlaced(Rectangle layoutBox, int childPos, FloatPropertyValue? kidFloatPropertyVal
            , Rectangle justPlacedFloatBox) {
            if (justPlacedFloatBox.GetBottom() >= layoutBox.GetTop() || justPlacedFloatBox.GetTop() < layoutBox.GetTop
                ()) {
                return;
            }
            // TODO handle it
            bool ltr = true;
            float floatWidth = justPlacedFloatBox.GetWidth();
            if (kidFloatPropertyVal.Equals(FloatPropertyValue.LEFT)) {
                layoutBox.SetWidth(layoutBox.GetWidth() - floatWidth).MoveRight(floatWidth);
                occupiedArea.GetBBox().MoveRight(floatWidth);
                if (ltr) {
                    for (int i = 0; i < childPos; ++i) {
                        IRenderer prevChild = childRenderers[i];
                        if (!FloatingHelper.IsRendererFloating(prevChild)) {
                            prevChild.Move(floatWidth, 0);
                        }
                    }
                }
            }
            else {
                layoutBox.SetWidth(layoutBox.GetWidth() - floatWidth);
                if (!ltr) {
                }
            }
        }

        // TODO
        private void ReplaceSplitRendererKidFloats(IDictionary<int, IRenderer> floatsToNextPageSplitRenderers, LineRenderer
             splitRenderer) {
            foreach (KeyValuePair<int, IRenderer> splitFloat in floatsToNextPageSplitRenderers) {
                if (splitFloat.Value != null) {
                    splitRenderer.childRenderers[splitFloat.Key] = splitFloat.Value;
                }
                else {
                    splitRenderer.childRenderers[splitFloat.Key] = null;
                }
            }
            for (int i = splitRenderer.GetChildRenderers().Count - 1; i >= 0; --i) {
                if (splitRenderer.GetChildRenderers()[i] == null) {
                    splitRenderer.GetChildRenderers().JRemoveAt(i);
                }
            }
        }

        private IRenderer GetLastNonFloatChildRenderer() {
            for (int i = childRenderers.Count - 1; i >= 0; --i) {
                if (FloatingHelper.IsRendererFloating(childRenderers[i])) {
                    continue;
                }
                return childRenderers[i];
            }
            return null;
        }

        private TabStop GetNextTabStop(float curWidth) {
            SortedDictionary<float, TabStop> tabStops = this.GetProperty<SortedDictionary<float, TabStop>>(Property.TAB_STOPS
                );
            KeyValuePair<float, TabStop>? nextTabStopEntry = null;
            TabStop nextTabStop = null;
            if (tabStops != null) {
                nextTabStopEntry = tabStops.HigherEntry(curWidth);
            }
            if (nextTabStopEntry != null) {
                nextTabStop = ((KeyValuePair<float, TabStop>)nextTabStopEntry).Value;
            }
            return nextTabStop;
        }

        /// <summary>Calculates and sets encountered tab size.</summary>
        /// <remarks>
        /// Calculates and sets encountered tab size.
        /// Returns null, if processing is finished and layout can be performed for the tab renderer;
        /// otherwise, in case when the tab should be processed after the next element in the line, this method returns corresponding tab stop.
        /// </remarks>
        private TabStop CalculateTab(IRenderer childRenderer, float curWidth, float lineWidth) {
            TabStop nextTabStop = GetNextTabStop(curWidth);
            if (nextTabStop == null) {
                ProcessDefaultTab(childRenderer, curWidth, lineWidth);
                return null;
            }
            childRenderer.SetProperty(Property.TAB_LEADER, nextTabStop.GetTabLeader());
            childRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(nextTabStop.GetTabPosition() - curWidth
                ));
            childRenderer.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(maxAscent - maxDescent));
            if (nextTabStop.GetTabAlignment() == TabAlignment.LEFT) {
                return null;
            }
            return nextTabStop;
        }

        /// <summary>Calculates and sets tab size with the account of the element that is next in the line after the tab.
        ///     </summary>
        /// <remarks>
        /// Calculates and sets tab size with the account of the element that is next in the line after the tab.
        /// Returns resulting width of the tab.
        /// </remarks>
        private float CalculateTab(Rectangle layoutBox, float curWidth, TabStop tabStop, IList<IRenderer> affectedRenderers
            , IRenderer tabRenderer) {
            float sumOfAffectedRendererWidths = 0;
            foreach (IRenderer renderer in affectedRenderers) {
                sumOfAffectedRendererWidths += renderer.GetOccupiedArea().GetBBox().GetWidth();
            }
            float tabWidth = 0;
            switch (tabStop.GetTabAlignment()) {
                case TabAlignment.RIGHT: {
                    tabWidth = tabStop.GetTabPosition() - curWidth - sumOfAffectedRendererWidths;
                    break;
                }

                case TabAlignment.CENTER: {
                    tabWidth = tabStop.GetTabPosition() - curWidth - sumOfAffectedRendererWidths / 2;
                    break;
                }

                case TabAlignment.ANCHOR: {
                    float anchorPosition = -1;
                    float processedRenderersWidth = 0;
                    foreach (IRenderer renderer in affectedRenderers) {
                        anchorPosition = ((TextRenderer)renderer).GetTabAnchorCharacterPosition();
                        if (-1 != anchorPosition) {
                            break;
                        }
                        else {
                            processedRenderersWidth += renderer.GetOccupiedArea().GetBBox().GetWidth();
                        }
                    }
                    if (anchorPosition == -1) {
                        anchorPosition = 0;
                    }
                    tabWidth = tabStop.GetTabPosition() - curWidth - anchorPosition - processedRenderersWidth;
                    break;
                }
            }
            if (tabWidth < 0) {
                tabWidth = 0;
            }
            if (curWidth + tabWidth + sumOfAffectedRendererWidths > layoutBox.GetWidth()) {
                tabWidth -= (curWidth + sumOfAffectedRendererWidths + tabWidth) - layoutBox.GetWidth();
            }
            tabRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(tabWidth));
            tabRenderer.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(maxAscent - maxDescent));
            return tabWidth;
        }

        private void ProcessDefaultTab(IRenderer tabRenderer, float curWidth, float lineWidth) {
            float? tabDefault = this.GetPropertyAsFloat(Property.TAB_DEFAULT);
            float? tabWidth = tabDefault - curWidth % tabDefault;
            if (curWidth + tabWidth > lineWidth) {
                tabWidth = lineWidth - curWidth;
            }
            tabRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue((float)tabWidth));
            tabRenderer.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(maxAscent - maxDescent));
        }

        private void UpdateChildrenParent() {
            foreach (IRenderer renderer in childRenderers) {
                renderer.SetParent(this);
            }
        }

        /// <summary>Trim first child text renderers.</summary>
        /// <returns>total number of trimmed glyphs.</returns>
        private int TrimFirst() {
            int totalNumberOfTrimmedGlyphs = 0;
            foreach (IRenderer renderer in childRenderers) {
                if (FloatingHelper.IsRendererFloating(renderer)) {
                    continue;
                }
                if (renderer is TextRenderer) {
                    TextRenderer textRenderer = (TextRenderer)renderer;
                    GlyphLine currentText = textRenderer.GetText();
                    if (currentText != null) {
                        int prevTextStart = currentText.start;
                        textRenderer.TrimFirst();
                        int numOfTrimmedGlyphs = textRenderer.GetText().start - prevTextStart;
                        totalNumberOfTrimmedGlyphs += numOfTrimmedGlyphs;
                    }
                    if (textRenderer.Length() > 0) {
                        break;
                    }
                }
                else {
                    break;
                }
            }
            return totalNumberOfTrimmedGlyphs;
        }

        /// <summary>Apply OTF features and return the last(!) base direction of child renderer</summary>
        /// <returns>the last(!) base direction of child renderer.</returns>
        private BaseDirection? ApplyOtf() {
            BaseDirection? baseDirection = this.GetProperty<BaseDirection?>(Property.BASE_DIRECTION);
            foreach (IRenderer renderer in childRenderers) {
                if (renderer is TextRenderer) {
                    ((TextRenderer)renderer).ApplyOtf();
                    if (baseDirection == null || baseDirection == BaseDirection.NO_BIDI) {
                        baseDirection = renderer.GetOwnProperty<BaseDirection?>(Property.BASE_DIRECTION);
                    }
                }
            }
            return baseDirection;
        }

        private void UpdateBidiLevels(int totalNumberOfTrimmedGlyphs, BaseDirection? baseDirection) {
            if (totalNumberOfTrimmedGlyphs != 0 && levels != null) {
                levels = JavaUtil.ArraysCopyOfRange(levels, totalNumberOfTrimmedGlyphs, levels.Length);
            }
            IList<int> unicodeIdsReorderingList = null;
            if (levels == null && baseDirection != null && baseDirection != BaseDirection.NO_BIDI) {
                unicodeIdsReorderingList = new List<int>();
                bool newLineFound = false;
                foreach (IRenderer child in childRenderers) {
                    if (newLineFound) {
                        break;
                    }
                    if (child is TextRenderer) {
                        GlyphLine text = ((TextRenderer)child).GetText();
                        for (int i = text.start; i < text.end; i++) {
                            Glyph glyph = text.Get(i);
                            if (iText.IO.Util.TextUtil.IsNewLine(glyph)) {
                                newLineFound = true;
                                break;
                            }
                            // we assume all the chars will have the same bidi group
                            // we also assume pairing symbols won't get merged with other ones
                            int unicode = glyph.HasValidUnicode() ? glyph.GetUnicode() : glyph.GetUnicodeChars()[0];
                            unicodeIdsReorderingList.Add(unicode);
                        }
                    }
                }
                levels = unicodeIdsReorderingList.Count > 0 ? TypographyUtils.GetBidiLevels(baseDirection, ArrayUtil.ToIntArray
                    (unicodeIdsReorderingList)) : null;
            }
        }

        /// <summary>While resolving TextRenderer may split into several ones with different fonts.</summary>
        private void ResolveChildrenFonts() {
            IList<IRenderer> newChildRenderers = new List<IRenderer>(childRenderers.Count);
            bool updateChildRendrers = false;
            foreach (IRenderer child in childRenderers) {
                if (child is TextRenderer) {
                    if (((TextRenderer)child).ResolveFonts(newChildRenderers)) {
                        updateChildRendrers = true;
                    }
                }
                else {
                    newChildRenderers.Add(child);
                }
            }
            // this mean, that some TextRenderer has been replaced.
            if (updateChildRendrers) {
                childRenderers = newChildRenderers;
            }
        }

        private float DecreaseRelativeWidthByChildAdditionalWidth(IRenderer childRenderer, float normalizedChildWidth
            ) {
            // Decrease the calculated width by margins, paddings and borders so that even for 100% width the content definitely fits.
            // TODO Actually, from html/css point of view - this is wrong, however we still do it, in order to avoid NOTHING due to
            // horizontal overflow. Probably remove this when overflow-x is supported.
            if (childRenderer is AbstractRenderer) {
                Rectangle dummyRect = new Rectangle(normalizedChildWidth, 0);
                ((AbstractRenderer)childRenderer).ApplyMargins(dummyRect, false);
                if (!IsBorderBoxSizing(childRenderer)) {
                    ((AbstractRenderer)childRenderer).ApplyBorderBox(dummyRect, false);
                    ((AbstractRenderer)childRenderer).ApplyPaddings(dummyRect, false);
                }
                normalizedChildWidth = dummyRect.GetWidth();
            }
            return normalizedChildWidth;
        }

        private bool IsInlineBlockChild(IRenderer child) {
            return child is BlockRenderer || child is TableRenderer;
        }

        internal class RendererGlyph {
            public Glyph glyph;

            public TextRenderer renderer;

            public RendererGlyph(Glyph glyph, TextRenderer textRenderer) {
                this.glyph = glyph;
                this.renderer = textRenderer;
            }
        }
    }
}
