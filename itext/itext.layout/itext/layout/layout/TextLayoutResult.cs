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
using iText.Layout.Renderer;

namespace iText.Layout.Layout {
    /// <summary>
    /// Represents the result of a text
    /// <see cref="iText.Layout.Renderer.TextRenderer.Layout(LayoutContext)">layout</see>.
    /// </summary>
    public class TextLayoutResult : MinMaxWidthLayoutResult {
        /// <summary>
        /// Indicates whether some word was split during
        /// <see cref="iText.Layout.Renderer.TextRenderer.Layout(LayoutContext)">layout</see>.
        /// </summary>
        protected internal bool wordHasBeenSplit;

        /// <summary>Indicates whether split was forced by new line symbol in text or not.</summary>
        protected internal bool splitForcedByNewline;

        protected internal bool containsPossibleBreak = false;

        protected internal bool startsWithSplitCharacterWhiteSpace = false;

        protected internal bool endsWithSplitCharacter = false;

        protected internal float leftMinWidth;

        protected internal float rightMinWidth;

        /// <summary>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.TextRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// </summary>
        /// <remarks>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.TextRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// The
        /// <see cref="LayoutResult.causeOfNothing"/>
        /// will be set as null.
        /// </remarks>
        /// <param name="status">
        /// the status of
        /// <see cref="iText.Layout.Renderer.TextRenderer.Layout(LayoutContext)"/>
        /// </param>
        /// <param name="occupiedArea">the area occupied by the content</param>
        /// <param name="splitRenderer">the renderer to draw the split part of the content</param>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        public TextLayoutResult(int status, LayoutArea occupiedArea, IRenderer splitRenderer, IRenderer overflowRenderer
            )
            : base(status, occupiedArea, splitRenderer, overflowRenderer) {
        }

        /// <summary>
        /// Creates the
        /// <see cref="LayoutResult"/>
        /// result of
        /// <see cref="iText.Layout.Renderer.TextRenderer.Layout(LayoutContext)">layouting</see>
        /// }.
        /// </summary>
        /// <param name="status">
        /// the status of
        /// <see cref="iText.Layout.Renderer.TextRenderer.Layout(LayoutContext)"/>
        /// </param>
        /// <param name="occupiedArea">the area occupied by the content</param>
        /// <param name="splitRenderer">the renderer to draw the split part of the content</param>
        /// <param name="overflowRenderer">the renderer to draw the overflowed part of the content</param>
        /// <param name="cause">
        /// the first renderer to produce
        /// <see cref="LayoutResult.NOTHING"/>
        /// </param>
        public TextLayoutResult(int status, LayoutArea occupiedArea, IRenderer splitRenderer, IRenderer overflowRenderer
            , IRenderer cause)
            : base(status, occupiedArea, splitRenderer, overflowRenderer, cause) {
        }

        /// <summary>
        /// Indicates whether some word in a rendered text was split during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layout</see>.
        /// </summary>
        /// <remarks>
        /// Indicates whether some word in a rendered text was split during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layout</see>.
        /// The value will be set as true if, for example, the rendered words width is bigger than the width of layout area.
        /// </remarks>
        /// <returns>whether some word was split or not.</returns>
        public virtual bool IsWordHasBeenSplit() {
            return wordHasBeenSplit;
        }

        /// <summary>
        /// Sets
        /// <see cref="wordHasBeenSplit"/>
        /// </summary>
        /// <param name="wordHasBeenSplit">
        /// indicates that some word was split during
        /// <see cref="iText.Layout.Renderer.IRenderer.Layout(LayoutContext)">layout</see>.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="TextLayoutResult">this layout result</see>
        /// the setting was applied on
        /// </returns>
        /// <seealso cref="wordHasBeenSplit"/>
        public virtual iText.Layout.Layout.TextLayoutResult SetWordHasBeenSplit(bool wordHasBeenSplit) {
            this.wordHasBeenSplit = wordHasBeenSplit;
            return this;
        }

        /// <summary>Indicates whether split was forced by new line symbol in rendered text.</summary>
        /// <remarks>
        /// Indicates whether split was forced by new line symbol in rendered text.
        /// The value will be set as true if, for example, the rendered text contains '\n' symbol.
        /// This value can also be true even if the text was fully placed, but had line break at the end.
        /// </remarks>
        /// <returns>whether split was forced by new line or not.</returns>
        public virtual bool IsSplitForcedByNewline() {
            return splitForcedByNewline;
        }

        /// <summary>
        /// Sets
        /// <see cref="IsSplitForcedByNewline()"/>
        /// </summary>
        /// <param name="isSplitForcedByNewline">indicates that split was forced by new line symbol in rendered text.</param>
        /// <returns>
        /// 
        /// <see cref="TextLayoutResult">this layout result</see>
        /// the setting was applied on.
        /// </returns>
        /// <seealso cref="SetSplitForcedByNewline(bool)"/>
        public virtual iText.Layout.Layout.TextLayoutResult SetSplitForcedByNewline(bool isSplitForcedByNewline) {
            this.splitForcedByNewline = isSplitForcedByNewline;
            return this;
        }

        /// <summary>Indicates whether split renderer contains possible break.</summary>
        /// <remarks>
        /// Indicates whether split renderer contains possible break.
        /// Possible breaks are either whitespaces or split characters.
        /// </remarks>
        /// <returns>true if there's a possible break within the split renderer.</returns>
        /// <seealso cref="iText.Layout.Splitting.ISplitCharacters"/>
        public virtual bool IsContainsPossibleBreak() {
            return containsPossibleBreak;
        }

        /// <summary>
        /// Sets
        /// <see cref="IsContainsPossibleBreak()"/>.
        /// </summary>
        /// <param name="containsPossibleBreak">indicates that split renderer contains possible break.</param>
        /// <returns>
        /// 
        /// <see cref="TextLayoutResult">this layout result</see>
        /// the setting was applied on.
        /// </returns>
        /// <seealso cref="IsContainsPossibleBreak()"/>
        public virtual iText.Layout.Layout.TextLayoutResult SetContainsPossibleBreak(bool containsPossibleBreak) {
            this.containsPossibleBreak = containsPossibleBreak;
            return this;
        }

        /// <summary>
        /// Sets
        /// <see cref="IsStartsWithSplitCharacterWhiteSpace()"/>.
        /// </summary>
        /// <param name="startsWithSplitCharacterWhiteSpace">
        /// indicates if TextRenderer#line starts with a split character that is
        /// also a whitespace.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="TextLayoutResult">this layout result</see>
        /// the setting was applied on.
        /// </returns>
        /// <seealso cref="IsStartsWithSplitCharacterWhiteSpace()"/>
        public virtual iText.Layout.Layout.TextLayoutResult SetStartsWithSplitCharacterWhiteSpace(bool startsWithSplitCharacterWhiteSpace
            ) {
            this.startsWithSplitCharacterWhiteSpace = startsWithSplitCharacterWhiteSpace;
            return this;
        }

        /// <summary>Indicates whether TextRenderer#line starts with a whitespace.</summary>
        /// <returns>true if TextRenderer#line starts with a whitespace.</returns>
        public virtual bool IsStartsWithSplitCharacterWhiteSpace() {
            return startsWithSplitCharacterWhiteSpace;
        }

        /// <summary>
        /// Sets
        /// <see cref="IsEndsWithSplitCharacter()"/>.
        /// </summary>
        /// <param name="endsWithSplitCharacter">indicates if TextRenderer#line ends with a splitCharacter.</param>
        /// <returns>
        /// 
        /// <see cref="TextLayoutResult">this layout result</see>
        /// the setting was applied on.
        /// </returns>
        /// <seealso cref="IsEndsWithSplitCharacter()"/>
        public virtual iText.Layout.Layout.TextLayoutResult SetEndsWithSplitCharacter(bool endsWithSplitCharacter) {
            this.endsWithSplitCharacter = endsWithSplitCharacter;
            return this;
        }

        /// <summary>Indicates whether TextRenderer#line ends with a splitCharacter.</summary>
        /// <returns>true if TextRenderer#line ends with a splitCharacter.</returns>
        /// <seealso cref="iText.Layout.Splitting.ISplitCharacters"/>
        public virtual bool IsEndsWithSplitCharacter() {
            return endsWithSplitCharacter;
        }

        /// <summary>Sets min width of the leftmost unbreakable part of the TextRenderer#line after layout.</summary>
        /// <remarks>
        /// Sets min width of the leftmost unbreakable part of the TextRenderer#line after layout.
        /// This value includes left-side additional width, i.e. left margin, border and padding widths.
        /// In case when entire TextRenderer#line is unbreakable, leftMinWidth also includes right-side additional width.
        /// </remarks>
        /// <param name="leftMinWidth">min width of the leftmost unbreakable part of the TextRenderer#line after layout.
        ///     </param>
        /// <returns>
        /// 
        /// <see cref="TextLayoutResult">this layout result</see>
        /// the setting was applied on.
        /// </returns>
        public virtual iText.Layout.Layout.TextLayoutResult SetLeftMinWidth(float leftMinWidth) {
            this.leftMinWidth = leftMinWidth;
            return this;
        }

        /// <summary>Gets min width of the leftmost unbreakable part of the TextRenderer#line after layout.</summary>
        /// <remarks>
        /// Gets min width of the leftmost unbreakable part of the TextRenderer#line after layout.
        /// This value leftMinWidth includes left-side additional width, i.e. left margin, border and padding widths.
        /// In case when entire TextRenderer#line is unbreakable, leftMinWidth also includes right-side additional width.
        /// </remarks>
        /// <returns>min width of the leftmost unbreakable part of the TextRenderer#line after layout.</returns>
        public virtual float GetLeftMinWidth() {
            return leftMinWidth;
        }

        /// <summary>Sets min width of the rightmost unbreakable part of the TextRenderer#line after layout.</summary>
        /// <remarks>
        /// Sets min width of the rightmost unbreakable part of the TextRenderer#line after layout.
        /// This value includes right-side additional width, i.e. right margin, border and padding widths.
        /// In case when entire TextRenderer#line is unbreakable, this value must be -1
        /// and right-side additional width must be included in leftMinWidth.
        /// </remarks>
        /// <param name="rightMinWidth">min width of the rightmost unbreakable part of the TextRenderer#line after layout.
        ///     </param>
        /// <returns>
        /// 
        /// <see cref="TextLayoutResult">this layout result</see>
        /// the setting was applied on.
        /// </returns>
        public virtual iText.Layout.Layout.TextLayoutResult SetRightMinWidth(float rightMinWidth) {
            this.rightMinWidth = rightMinWidth;
            return this;
        }

        /// <summary>Gets min width of the rightmost unbreakable part of the TextRenderer#line after layout.</summary>
        /// <remarks>
        /// Gets min width of the rightmost unbreakable part of the TextRenderer#line after layout.
        /// This value includes right-side additional width, i.e. right margin, border and padding widths.
        /// In case when entire TextRenderer#line is unbreakable, this value must be -1
        /// and right-side additional width must be included in leftMinWidth.
        /// </remarks>
        /// <returns>min width of the leftmost unbreakable part of the TextRenderer#line after layout.</returns>
        public virtual float GetRightMinWidth() {
            return rightMinWidth;
        }
    }
}
