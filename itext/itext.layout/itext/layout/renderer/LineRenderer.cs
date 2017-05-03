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
using System.Text;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class LineRenderer : AbstractRenderer {
        protected internal float maxAscent;

        protected internal float maxDescent;

        protected internal byte[] levels;

        protected internal IList<Rectangle> currentLineFloatRenderers = new List<Rectangle>();

        protected internal LineRenderer()
            : base() {
        }

        protected internal bool affectedByFloat = false;

        protected internal LineRenderer(iText.Layout.Renderer.LineRenderer other) {
            // bidi levels
            this.childRenderers = other.childRenderers;
            this.positionedRenderers = other.positionedRenderers;
            this.modelElement = other.modelElement;
            this.flushed = other.flushed;
            this.occupiedArea = other.occupiedArea != null ? other.occupiedArea.Clone() : null;
            this.parent = other.parent;
            this.properties.AddAll(other.properties);
            this.isLastRendererForModelElement = other.isLastRendererForModelElement;
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            Rectangle layoutBox = layoutContext.GetArea().GetBBox().Clone();
            IList<Rectangle> floatRenderers = layoutContext.GetFloatedRenderers();
            if (floatRenderers != null) {
                AdjustLineRendererAccordingToFloatRenderers(floatRenderers, layoutBox);
            }
            occupiedArea = new LayoutArea(layoutContext.GetArea().GetPageNumber(), layoutBox.Clone().MoveDown(-layoutBox
                .GetHeight()).SetHeight(0));
            float curWidth = 0;
            maxAscent = 0;
            maxDescent = 0;
            int childPos = 0;
            MinMaxWidth minMaxWidth = new MinMaxWidth(0, layoutBox.GetWidth());
            AbstractWidthHandler widthHandler = new MaxSumWidthHandler(minMaxWidth);
            UpdateChildrenParent();
            ResolveChildrenFonts();
            int totalNumberOfTrimmedGlyphs = TrimFirst();
            BaseDirection? baseDirection = ApplyOtf();
            UpdateBidiLevels(totalNumberOfTrimmedGlyphs, baseDirection);
            bool anythingPlaced = false;
            TabStop hangingTabStop = null;
            LineLayoutResult result = null;
            while (childPos < childRenderers.Count) {
                IRenderer childRenderer = childRenderers[childPos];
                LayoutResult childResult;
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
                            tabRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(), bbox)));
                            curWidth += tabRenderer.GetOccupiedArea().GetBBox().GetWidth();
                            widthHandler.UpdateMaxChildWidth(tabRenderer.GetOccupiedArea().GetBBox().GetWidth());
                        }
                        hangingTabStop = CalculateTab(childRenderer, curWidth, layoutBox.GetWidth());
                        if (childPos == childRenderers.Count - 1) {
                            hangingTabStop = null;
                        }
                        if (hangingTabStop != null) {
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
                    // Decrease the calculated width by margins, paddings and borders so that even for 100% width the content definitely fits
                    // TODO DEVSIX-1174 fix depending of box-sizing
                    if (childRenderer is AbstractRenderer) {
                        Rectangle dummyRect = new Rectangle(normalizedChildWidth, 0);
                        ((AbstractRenderer)childRenderer).ApplyMargins(dummyRect, false);
                        ((AbstractRenderer)childRenderer).ApplyBorderBox(dummyRect, false);
                        ((AbstractRenderer)childRenderer).ApplyPaddings(dummyRect, false);
                        normalizedChildWidth = dummyRect.GetWidth();
                    }
                    if (normalizedChildWidth > 0) {
                        childRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(normalizedChildWidth));
                        childWidthWasReplaced = true;
                    }
                }
                childResult = childRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(
                    ), bbox)));
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
                        minChildWidth = ((MinMaxWidthLayoutResult)childResult).GetNotNullMinMaxWidth(bbox.GetWidth()).GetMinWidth(
                            );
                    }
                    maxChildWidth = ((MinMaxWidthLayoutResult)childResult).GetNotNullMinMaxWidth(bbox.GetWidth()).GetMaxWidth(
                        );
                }
                if (childRenderer.HasProperty(Property.FLOAT)) {
                    floatRenderers.Add(childRenderer.GetOccupiedArea().GetBBox());
                }
                float childAscent = 0;
                float childDescent = 0;
                if (childRenderer is ILeafElementRenderer) {
                    childAscent = ((ILeafElementRenderer)childRenderer).GetAscent();
                    childDescent = ((ILeafElementRenderer)childRenderer).GetDescent();
                }
                maxAscent = Math.Max(maxAscent, childAscent);
                maxDescent = Math.Min(maxDescent, childDescent);
                float maxHeight = maxAscent - maxDescent;
                if (hangingTabStop != null) {
                    IRenderer tabRenderer = childRenderers[childPos - 1];
                    float tabWidth = CalculateTab(layoutBox, curWidth, hangingTabStop, childRenderer, childResult, tabRenderer
                        );
                    tabRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(), bbox)));
                    childResult.GetOccupiedArea().GetBBox().MoveRight(tabWidth);
                    if (childResult.GetSplitRenderer() != null) {
                        childResult.GetSplitRenderer().GetOccupiedArea().GetBBox().MoveRight(tabWidth);
                    }
                    float tabAndNextElemWidth = tabWidth + childResult.GetOccupiedArea().GetBBox().GetWidth();
                    if (hangingTabStop.GetTabAlignment() == TabAlignment.RIGHT && curWidth + tabAndNextElemWidth < hangingTabStop
                        .GetTabPosition()) {
                        curWidth = hangingTabStop.GetTabPosition();
                    }
                    else {
                        curWidth += tabAndNextElemWidth;
                    }
                    widthHandler.UpdateMinChildWidth(minChildWidth);
                    widthHandler.UpdateMaxChildWidth(tabWidth + maxChildWidth);
                    hangingTabStop = null;
                }
                else {
                    curWidth += childResult.GetOccupiedArea().GetBBox().GetWidth();
                    widthHandler.UpdateMinChildWidth(minChildWidth);
                    widthHandler.UpdateMaxChildWidth(maxChildWidth);
                }
                occupiedArea.SetBBox(new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox.GetHeight() - maxHeight, 
                    curWidth, maxHeight));
                bool newLineOccurred = (childResult is TextLayoutResult && ((TextLayoutResult)childResult).IsSplitForcedByNewline
                    ());
                bool shouldBreakLayouting = childResult.GetStatus() != LayoutResult.FULL || newLineOccurred;
                if (shouldBreakLayouting) {
                    iText.Layout.Renderer.LineRenderer[] split = Split();
                    split[0].childRenderers = new List<IRenderer>(childRenderers.SubList(0, childPos));
                    bool wordWasSplitAndItWillFitOntoNextLine = false;
                    if (childResult is TextLayoutResult && ((TextLayoutResult)childResult).IsWordHasBeenSplit()) {
                        LayoutResult newLayoutResult = childRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea
                            ().GetPageNumber(), layoutBox)));
                        if (newLayoutResult is TextLayoutResult && !((TextLayoutResult)newLayoutResult).IsWordHasBeenSplit()) {
                            wordWasSplitAndItWillFitOntoNextLine = true;
                        }
                    }
                    if (wordWasSplitAndItWillFitOntoNextLine) {
                        split[1].childRenderers.Add(childRenderer);
                        split[1].childRenderers.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
                    }
                    else {
                        if (childResult.GetStatus() == LayoutResult.PARTIAL || childResult.GetStatus() == LayoutResult.FULL) {
                            split[0].AddChild(childResult.GetSplitRenderer());
                            anythingPlaced = true;
                        }
                        if (childResult.GetStatus() == LayoutResult.PARTIAL && childResult.GetOverflowRenderer() is ImageRenderer) {
                            ((ImageRenderer)childResult.GetOverflowRenderer()).AutoScale(layoutContext.GetArea());
                        }
                        if (null != childResult.GetOverflowRenderer()) {
                            split[1].childRenderers.Add(childResult.GetOverflowRenderer());
                        }
                        split[1].childRenderers.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
                        // no sense to process empty renderer
                        if (split[1].childRenderers.Count == 0) {
                            split[1] = null;
                        }
                    }
                    IRenderer causeOfNothing = childResult.GetStatus() == LayoutResult.NOTHING ? childResult.GetCauseOfNothing
                        () : childRenderer;
                    if (split[1] == null) {
                        result = new LineLayoutResult(LayoutResult.FULL, occupiedArea, split[0], split[1], causeOfNothing, floatRenderers
                            );
                    }
                    else {
                        if (anythingPlaced) {
                            result = new LineLayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1], causeOfNothing, floatRenderers
                                );
                        }
                        else {
                            result = new LineLayoutResult(LayoutResult.NOTHING, null, split[0], split[1], causeOfNothing, floatRenderers
                                );
                        }
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
                if (anythingPlaced || 0 == childRenderers.Count) {
                    result = new LineLayoutResult(LayoutResult.FULL, occupiedArea, null, null);
                }
                else {
                    result = new LineLayoutResult(LayoutResult.NOTHING, null, null, this, this);
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
                    foreach (IRenderer child in children) {
                        if (newLineFound) {
                            break;
                        }
                        if (child is TextRenderer) {
                            GlyphLine childLine = ((TextRenderer)child).line;
                            for (int i = childLine.start; i < childLine.end; i++) {
                                if (TextUtil.IsNewLine(childLine.Get(i))) {
                                    newLineFound = true;
                                    break;
                                }
                                lineGlyphs.Add(new LineRenderer.RendererGlyph(childLine.Get(i), (TextRenderer)child));
                            }
                        }
                    }
                    byte[] lineLevels = new byte[lineGlyphs.Count];
                    if (levels != null) {
                        System.Array.Copy(levels, 0, lineLevels, 0, lineGlyphs.Count);
                    }
                    int[] reorder = TypographyUtils.ReorderLine(lineGlyphs, lineLevels, levels);
                    if (reorder != null) {
                        children.Clear();
                        int pos = 0;
                        int initialPos = 0;
                        bool reversed = false;
                        int offset = 0;
                        while (pos < lineGlyphs.Count) {
                            IRenderer renderer = lineGlyphs[pos].renderer;
                            TextRenderer newRenderer = new TextRenderer((TextRenderer)renderer).RemoveReversedRanges();
                            children.Add(newRenderer);
                            newRenderer.line = new GlyphLine(newRenderer.line);
                            IList<Glyph> replacementGlyphs = new List<Glyph>();
                            while (pos < lineGlyphs.Count && lineGlyphs[pos].renderer == renderer) {
                                if (pos + 1 < lineGlyphs.Count) {
                                    if (reorder[pos] == reorder[pos + 1] + 1 && !TextUtil.IsSpaceOrWhitespace(lineGlyphs[pos + 1].glyph) && !TextUtil
                                        .IsSpaceOrWhitespace(lineGlyphs[pos].glyph)) {
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
                        float currentXPos = layoutContext.GetArea().GetBBox().GetLeft();
                        foreach (IRenderer child in children) {
                            float currentWidth = ((TextRenderer)child).CalculateLineWidth();
                            float[] margins = ((TextRenderer)child).GetMargins();
                            currentWidth += margins[1] + margins[3];
                            ((TextRenderer)child).occupiedArea.GetBBox().SetX(currentXPos).SetWidth(currentWidth);
                            currentXPos += currentWidth;
                        }
                    }
                    if (result.GetStatus() == LayoutResult.PARTIAL) {
                        iText.Layout.Renderer.LineRenderer overflow = (iText.Layout.Renderer.LineRenderer)result.GetOverflowRenderer
                            ();
                        if (levels != null) {
                            overflow.levels = new byte[levels.Length - lineLevels.Length];
                            System.Array.Copy(levels, lineLevels.Length, overflow.levels, 0, overflow.levels.Length);
                            if (overflow.levels.Length == 0) {
                                overflow.levels = null;
                            }
                        }
                    }
                }
            }
            iText.Layout.Renderer.LineRenderer processed = result.GetStatus() == LayoutResult.FULL ? this : (iText.Layout.Renderer.LineRenderer
                )result.GetSplitRenderer();
            if (anythingPlaced) {
                processed.AdjustChildrenYLine().TrimLast();
                result.SetMinMaxWidth(minMaxWidth);
            }
            else {
                if (floatRenderers.Count > 0) {
                    affectedByFloat = true;
                    float maxFloatHeight = 0;
                    foreach (Rectangle floatRenderer in floatRenderers) {
                        if (floatRenderer.GetX() < processed.GetOccupiedArea().GetBBox().GetX() && maxFloatHeight < floatRenderer.
                            GetHeight()) {
                            maxFloatHeight = floatRenderer.GetHeight();
                        }
                    }
                    processed.GetOccupiedArea().GetBBox().SetHeight(maxFloatHeight);
                    processed.GetOccupiedArea().GetBBox().MoveDown(maxFloatHeight);
                    foreach (Rectangle rend in floatRenderers) {
                        rend.SetHeight(rend.GetHeight() - processed.GetOccupiedArea().GetBBox().GetHeight());
                    }
                }
            }
            ReduceFloatRenderersOccupiedArea(processed, floatRenderers);
            IList<IRenderer> currentLineChildRenderers = result.GetStatus() == LayoutResult.FULL ? this.childRenderers
                 : result.GetSplitRenderer().GetChildRenderers();
            LayoutArea editedArea = ApplyFloatPropertyOnCurrentArea(currentLineChildRenderers, floatRenderers, layoutContext
                .GetArea().GetBBox().GetWidth());
            if (editedArea != null) {
                processed.GetOccupiedArea().SetBBox(editedArea.GetBBox());
            }
            if (floatRenderers != null) {
                result.GetFloatRenderers().AddAll(floatRenderers);
            }
            return result;
        }

        protected internal virtual LayoutArea ApplyFloatPropertyOnCurrentArea(IList<IRenderer> childRenderers, IList
            <Rectangle> floatRenderers, float availableWidth) {
            LayoutArea editedArea = null;
            float lineHeight = 0;
            bool lineHasFloatProperty = false;
            foreach (IRenderer renderer in childRenderers) {
                if (renderer.HasProperty(Property.FLOAT)) {
                    lineHasFloatProperty = true;
                }
                else {
                    if (renderer.GetOccupiedArea() != null && renderer.GetOccupiedArea().GetBBox().GetHeight() > lineHeight) {
                        lineHeight = renderer.GetOccupiedArea().GetBBox().GetHeight();
                    }
                }
            }
            if (lineHasFloatProperty && lineHeight > 0) {
                editedArea = occupiedArea.Clone();
                editedArea.GetBBox().MoveUp(editedArea.GetBBox().GetHeight() - lineHeight + maxDescent);
            }
            return editedArea;
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
                    return leading.GetValue();
                }

                case Leading.MULTIPLIED: {
                    return occupiedArea.GetBBox().GetHeight() * leading.GetValue();
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.LineRenderer();
        }

        protected internal override float? GetFirstYLineRecursively() {
            return GetYLine();
        }

        public virtual void Justify(float width) {
            float ratio = (float)this.GetPropertyAsFloat(Property.SPACING_RATIO);
            float freeWidth = occupiedArea.GetBBox().GetX() + width - GetLastChildRenderer().GetOccupiedArea().GetBBox
                ().GetX() - GetLastChildRenderer().GetOccupiedArea().GetBBox().GetWidth();
            int numberOfSpaces = GetNumberOfSpaces();
            int baseCharsCount = BaseCharactersCount();
            float baseFactor = freeWidth / (ratio * numberOfSpaces + (1 - ratio) * (baseCharsCount - 1));
            float wordSpacing = ratio * baseFactor;
            float characterSpacing = (1 - ratio) * baseFactor;
            float lastRightPos = occupiedArea.GetBBox().GetX();
            for (int i = 0; i < childRenderers.Count; ++i) {
                IRenderer child = childRenderers[i];
                float childX = child.GetOccupiedArea().GetBBox().GetX();
                child.Move(lastRightPos - childX, 0);
                childX = lastRightPos;
                if (child is TextRenderer) {
                    float childHSCale = (float)((TextRenderer)child).GetPropertyAsFloat(Property.HORIZONTAL_SCALING, 1f);
                    child.SetProperty(Property.CHARACTER_SPACING, characterSpacing / childHSCale);
                    child.SetProperty(Property.WORD_SPACING, wordSpacing / childHSCale);
                    bool isLastTextRenderer = i + 1 == childRenderers.Count;
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
                if (child is TextRenderer) {
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
        protected internal virtual int Length() {
            int length = 0;
            foreach (IRenderer child in childRenderers) {
                if (child is TextRenderer) {
                    length += ((TextRenderer)child).LineLength();
                }
            }
            return length;
        }

        /// <summary>Returns the number of base characters, i.e.</summary>
        /// <remarks>Returns the number of base characters, i.e. non-mark characters</remarks>
        protected internal virtual int BaseCharactersCount() {
            int count = 0;
            foreach (IRenderer child in childRenderers) {
                if (child is TextRenderer) {
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

        protected internal virtual iText.Layout.Renderer.LineRenderer CreateSplitRenderer() {
            return (iText.Layout.Renderer.LineRenderer)GetNextRenderer();
        }

        protected internal virtual iText.Layout.Renderer.LineRenderer CreateOverflowRenderer() {
            return (iText.Layout.Renderer.LineRenderer)GetNextRenderer();
        }

        protected internal virtual iText.Layout.Renderer.LineRenderer[] Split() {
            iText.Layout.Renderer.LineRenderer splitRenderer = CreateSplitRenderer();
            splitRenderer.occupiedArea = occupiedArea.Clone();
            splitRenderer.parent = parent;
            splitRenderer.maxAscent = maxAscent;
            splitRenderer.maxDescent = maxDescent;
            splitRenderer.levels = levels;
            splitRenderer.AddAllProperties(GetOwnProperties());
            iText.Layout.Renderer.LineRenderer overflowRenderer = CreateOverflowRenderer();
            overflowRenderer.parent = parent;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            //        overflowRenderer.currentLineFloatRenderers = currentLineFloatRenderers;
            return new iText.Layout.Renderer.LineRenderer[] { splitRenderer, overflowRenderer };
        }

        protected internal virtual iText.Layout.Renderer.LineRenderer AdjustChildrenYLine() {
            float actualYLine = occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight() - maxAscent;
            bool lineHasFloatElements = false;
            foreach (IRenderer renderer in childRenderers) {
                if (renderer is ILeafElementRenderer) {
                    float descent = ((ILeafElementRenderer)renderer).GetDescent();
                    renderer.Move(0, actualYLine - renderer.GetOccupiedArea().GetBBox().GetBottom() + descent);
                }
                else {
                    renderer.Move(0, occupiedArea.GetBBox().GetY() - renderer.GetOccupiedArea().GetBBox().GetBottom());
                }
                if (!lineHasFloatElements && renderer.HasProperty(Property.FLOAT)) {
                    lineHasFloatElements = true;
                }
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.LineRenderer TrimLast() {
            IRenderer lastRenderer = childRenderers.Count > 0 ? childRenderers[childRenderers.Count - 1] : null;
            if (lastRenderer is TextRenderer) {
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

        internal override MinMaxWidth GetMinMaxWidth(float availableWidth) {
            LineLayoutResult result = (LineLayoutResult)((LineLayoutResult)Layout(new LayoutContext(new LayoutArea(1, 
                new Rectangle(availableWidth, AbstractRenderer.INF)))));
            return result.GetNotNullMinMaxWidth(availableWidth);
        }

        protected internal virtual void ReduceFloatRenderersOccupiedArea(IRenderer processed, IList<Rectangle> floatRenderers
            ) {
            float maxNonFloatRenderersHeight = 0;
            if (!parent.HasProperty(Property.FLOAT)) {
                foreach (IRenderer renderer in processed.GetChildRenderers()) {
                    float rendererHeight = renderer.GetOccupiedArea().GetBBox().GetHeight();
                    if (!renderer.HasProperty(Property.FLOAT)) {
                        if (maxNonFloatRenderersHeight < rendererHeight) {
                            maxNonFloatRenderersHeight = rendererHeight;
                        }
                    }
                }
            }
            IList<Rectangle> renderersToRemove = new List<Rectangle>();
            foreach (Rectangle floatRenderer in floatRenderers) {
                floatRenderer.SetHeight(floatRenderer.GetHeight() - maxNonFloatRenderersHeight);
                if (floatRenderer.GetHeight() <= 0) {
                    renderersToRemove.Add(floatRenderer);
                }
            }
            foreach (Rectangle rendererToRemove in renderersToRemove) {
                floatRenderers.Remove(rendererToRemove);
            }
        }

        private IRenderer GetLastChildRenderer() {
            return childRenderers[childRenderers.Count - 1];
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
            childRenderer.SetProperty(Property.MIN_HEIGHT, maxAscent - maxDescent);
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
        private float CalculateTab(Rectangle layoutBox, float curWidth, TabStop tabStop, IRenderer nextElementRenderer
            , LayoutResult nextElementResult, IRenderer tabRenderer) {
            float childWidth = 0;
            if (nextElementRenderer != null) {
                childWidth = nextElementRenderer.GetOccupiedArea().GetBBox().GetWidth();
            }
            float tabWidth = 0;
            switch (tabStop.GetTabAlignment()) {
                case TabAlignment.RIGHT: {
                    tabWidth = tabStop.GetTabPosition() - curWidth - childWidth;
                    break;
                }

                case TabAlignment.CENTER: {
                    tabWidth = tabStop.GetTabPosition() - curWidth - childWidth / 2;
                    break;
                }

                case TabAlignment.ANCHOR: {
                    float anchorPosition = -1;
                    if (nextElementRenderer is TextRenderer) {
                        anchorPosition = ((TextRenderer)nextElementRenderer).GetTabAnchorCharacterPosition();
                    }
                    if (anchorPosition == -1) {
                        anchorPosition = childWidth;
                    }
                    tabWidth = tabStop.GetTabPosition() - curWidth - anchorPosition;
                    break;
                }
            }
            if (tabWidth < 0) {
                tabWidth = 0;
            }
            if (curWidth + tabWidth + childWidth > layoutBox.GetWidth()) {
                tabWidth -= (curWidth + childWidth + tabWidth) - layoutBox.GetWidth();
            }
            tabRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(tabWidth));
            tabRenderer.SetProperty(Property.MIN_HEIGHT, maxAscent - maxDescent);
            return tabWidth;
        }

        private void ProcessDefaultTab(IRenderer tabRenderer, float curWidth, float lineWidth) {
            float? tabDefault = this.GetPropertyAsFloat(Property.TAB_DEFAULT);
            float? tabWidth = tabDefault - curWidth % tabDefault;
            if (curWidth + tabWidth > lineWidth) {
                tabWidth = lineWidth - curWidth;
            }
            tabRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue((float)tabWidth));
            tabRenderer.SetProperty(Property.MIN_HEIGHT, maxAscent - maxDescent);
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
                levels = iText.IO.Util.JavaUtil.ArraysCopyOfRange(levels, totalNumberOfTrimmedGlyphs, levels.Length);
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
                            if (TextUtil.IsNewLine(glyph)) {
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
                levels = unicodeIdsReorderingList.Count > 0 ? TypographyUtils.GetBidiLevels(baseDirection, ArrayUtil.ToArray
                    (unicodeIdsReorderingList)) : null;
            }
        }

        private void AdjustLineRendererAccordingToFloatRenderers(IList<Rectangle> floatRenderers, Rectangle layoutBox
            ) {
            float maxWidth = 0;
            float maxHeight = 0;
            foreach (Rectangle floatRenderer in floatRenderers) {
                if (floatRenderer != null) {
                    FloatPropertyValue? floatPropertyValue = parent.GetProperty(Property.FLOAT);
                    if (floatRenderer.GetX() <= layoutBox.GetX() && (floatPropertyValue == null || !floatPropertyValue.Equals(
                        FloatPropertyValue.RIGHT))) {
                        if (maxWidth < floatRenderer.GetWidth()) {
                            maxWidth = floatRenderer.GetWidth();
                        }
                        if (maxHeight < floatRenderer.GetHeight()) {
                            maxHeight = floatRenderer.GetHeight();
                        }
                    }
                }
            }
            layoutBox.MoveRight(maxWidth);
            if (!parent.HasProperty(Property.FLOAT)) {
                layoutBox.SetWidth(layoutBox.GetWidth() - maxWidth);
            }
        }

        private void AdjustLineRendererToCurrentLineFloatRendererers(IList<Rectangle> floatRenderers, Rectangle layoutBox
            ) {
            float maxWidth = 0;
            float maxHeight = 0;
            foreach (Rectangle floatRenderer in floatRenderers) {
                if (floatRenderer != null) {
                    if (floatRenderer.GetX() <= layoutBox.GetX() && !parent.HasProperty(Property.FLOAT)) {
                        if (maxWidth < floatRenderer.GetWidth()) {
                            maxWidth = floatRenderer.GetWidth();
                        }
                        if (maxHeight < floatRenderer.GetHeight()) {
                            maxHeight = floatRenderer.GetHeight();
                        }
                    }
                }
            }
            layoutBox.MoveRight(maxWidth);
            layoutBox.SetWidth(layoutBox.GetWidth() - maxWidth);
            layoutBox.SetHeight(layoutBox.GetHeight() + maxHeight);
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

        internal class RendererGlyph {
            public RendererGlyph(Glyph glyph, TextRenderer textRenderer) {
                this.glyph = glyph;
                this.renderer = textRenderer;
            }

            public Glyph glyph;

            public TextRenderer renderer;
        }

        private float[] CalculateAscenderDescender() {
            PdfFont listItemFont = ResolveFirstPdfFont();
            float? fontSize = this.GetPropertyAsFloat(Property.FONT_SIZE);
            if (listItemFont != null && fontSize != null) {
                float[] ascenderDescender = TextRenderer.CalculateAscenderDescender(listItemFont);
                return new float[] { (float)fontSize * ascenderDescender[0] / TextRenderer.TEXT_SPACE_COEFF, (float)fontSize
                     * ascenderDescender[1] / TextRenderer.TEXT_SPACE_COEFF };
            }
            return new float[] { 0, 0 };
        }
    }
}
