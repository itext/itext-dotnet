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
using System.Text;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Canvas.Parser.Data {
    /// <summary>
    /// Provides information and calculations needed by render listeners
    /// to display/evaluate text render operations.
    /// </summary>
    /// <remarks>
    /// Provides information and calculations needed by render listeners
    /// to display/evaluate text render operations.
    /// <br /><br />
    /// This is passed between the
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.PdfCanvasProcessor"/>
    /// and
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Listener.IEventListener"/>
    /// objects as text rendering operations are
    /// discovered
    /// </remarks>
    public class TextRenderInfo : AbstractRenderInfo {
        private readonly PdfString @string;

        private String text = null;

        private readonly Matrix textToUserSpaceTransformMatrix;

        private readonly Matrix textMatrix;

        private float unscaledWidth = float.NaN;

        /// <summary>Hierarchy of nested canvas tags for the text from the most inner (nearest to text) tag to the most outer.
        ///     </summary>
        private readonly IList<CanvasTag> canvasTagHierarchy;

        /// <summary>Creates a new TextRenderInfo object</summary>
        /// <param name="str">the PDF string that should be displayed</param>
        /// <param name="gs">the graphics state (note: at this time, this is not immutable, so don't cache it)</param>
        /// <param name="textMatrix">the text matrix at the time of the render operation</param>
        /// <param name="canvasTagHierarchy">the marked content tags sequence, if available</param>
        public TextRenderInfo(PdfString str, CanvasGraphicsState gs, Matrix textMatrix, Stack<CanvasTag> canvasTagHierarchy
            )
            : base(gs) {
            this.@string = str;
            this.textToUserSpaceTransformMatrix = textMatrix.Multiply(gs.GetCtm());
            this.textMatrix = textMatrix;
            this.canvasTagHierarchy = JavaCollectionsUtil.UnmodifiableList<CanvasTag>(new List<CanvasTag>(canvasTagHierarchy
                ));
        }

        /// <summary>Used for creating sub-TextRenderInfos for each individual character.</summary>
        /// <param name="parent">the parent TextRenderInfo</param>
        /// <param name="str">the content of a TextRenderInfo</param>
        /// <param name="horizontalOffset">the unscaled horizontal offset of the character that this TextRenderInfo represents
        ///     </param>
        private TextRenderInfo(iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo parent, PdfString str, float horizontalOffset
            )
            : base(parent.gs) {
            this.@string = str;
            Matrix offsetMatrix = new Matrix(horizontalOffset, 0);
            this.textToUserSpaceTransformMatrix = offsetMatrix.Multiply(parent.textToUserSpaceTransformMatrix);
            this.textMatrix = offsetMatrix.Multiply(parent.textMatrix);
            this.canvasTagHierarchy = parent.canvasTagHierarchy;
        }

        /// <summary>Gets the text to be rendered according to canvas operators.</summary>
        /// <returns>the text to render</returns>
        public virtual String GetText() {
            CheckGraphicsState();
            if (text == null) {
                GlyphLine gl = gs.GetFont().DecodeIntoGlyphLine(@string);
                if (!IsReversedChars()) {
                    text = gl.ToUnicodeString(gl.start, gl.end);
                }
                else {
                    StringBuilder sb = new StringBuilder(gl.end - gl.start);
                    for (int i = gl.end - 1; i >= gl.start; i--) {
                        sb.Append(gl.Get(i).GetUnicodeChars());
                    }
                    text = sb.ToString();
                }
            }
            return text;
        }

        /// <returns>original PDF string</returns>
        public virtual PdfString GetPdfString() {
            return @string;
        }

        /// <summary>Gets original Text matrix.</summary>
        /// <returns>text matrix.</returns>
        public virtual Matrix GetTextMatrix() {
            return textMatrix;
        }

        /// <summary>
        /// Checks if the text belongs to a marked content sequence
        /// with a given mcid.
        /// </summary>
        /// <param name="mcid">a marked content id</param>
        /// <returns>true if the text is marked with this id</returns>
        public virtual bool HasMcid(int mcid) {
            return HasMcid(mcid, false);
        }

        /// <summary>
        /// Checks if the text belongs to a marked content sequence
        /// with a given mcid.
        /// </summary>
        /// <param name="mcid">a marked content id</param>
        /// <param name="checkTheTopmostLevelOnly">indicates whether to check the topmost level of marked content stack only
        ///     </param>
        /// <returns>true if the text is marked with this id</returns>
        public virtual bool HasMcid(int mcid, bool checkTheTopmostLevelOnly) {
            if (checkTheTopmostLevelOnly) {
                if (canvasTagHierarchy != null) {
                    int infoMcid = GetMcid();
                    return infoMcid != -1 && infoMcid == mcid;
                }
            }
            else {
                foreach (CanvasTag tag in canvasTagHierarchy) {
                    if (tag.HasMcid()) {
                        if (tag.GetMcid() == mcid) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the marked-content identifier associated with this
        /// <see cref="TextRenderInfo"/>
        /// instance
        /// </summary>
        /// <returns>associated marked-content identifier or -1 in case content is unmarked</returns>
        public virtual int GetMcid() {
            foreach (CanvasTag tag in canvasTagHierarchy) {
                if (tag.HasMcid()) {
                    return tag.GetMcid();
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the baseline for the text (i.e. the line that the text 'sits' on)
        /// This value includes the Rise of the draw operation - see
        /// <see cref="GetRise()"/>
        /// for the amount added by Rise
        /// </summary>
        /// <returns>the baseline line segment</returns>
        public virtual LineSegment GetBaseline() {
            CheckGraphicsState();
            return GetUnscaledBaselineWithOffset(0 + gs.GetTextRise()).TransformBy(textToUserSpaceTransformMatrix);
        }

        public virtual LineSegment GetUnscaledBaseline() {
            CheckGraphicsState();
            return GetUnscaledBaselineWithOffset(0 + gs.GetTextRise());
        }

        /// <summary>Gets the ascent line for the text (i.e. the line that represents the topmost extent that a string of the current font could have).
        ///     </summary>
        /// <remarks>
        /// Gets the ascent line for the text (i.e. the line that represents the topmost extent that a string of the current font could have).
        /// This value includes the Rise of the draw operation - see
        /// <see cref="GetRise()"/>
        /// for the amount added by Rise.
        /// </remarks>
        /// <returns>a LineSegment instance</returns>
        public virtual LineSegment GetAscentLine() {
            CheckGraphicsState();
            return GetUnscaledBaselineWithOffset(GetAscentDescent()[0] + gs.GetTextRise()).TransformBy(textToUserSpaceTransformMatrix
                );
        }

        /// <summary>Gets the descent line for the text (i.e. the line that represents the bottom most extent that a string of the current font could have).
        ///     </summary>
        /// <remarks>
        /// Gets the descent line for the text (i.e. the line that represents the bottom most extent that a string of the current font could have).
        /// This value includes the Rise of the draw operation - see
        /// <see cref="GetRise()"/>
        /// for the amount added by Rise.
        /// </remarks>
        /// <returns>a LineSegment instance</returns>
        public virtual LineSegment GetDescentLine() {
            CheckGraphicsState();
            return GetUnscaledBaselineWithOffset(GetAscentDescent()[1] + gs.GetTextRise()).TransformBy(textToUserSpaceTransformMatrix
                );
        }

        /// <summary>Getter for the font</summary>
        /// <returns>the font</returns>
        public virtual PdfFont GetFont() {
            CheckGraphicsState();
            return gs.GetFont();
        }

        /// <summary>The rise represents how far above the nominal baseline the text should be rendered.</summary>
        /// <remarks>
        /// The rise represents how far above the nominal baseline the text should be rendered.  The
        /// <see cref="GetBaseline()"/>
        /// ,
        /// <see cref="GetAscentLine()"/>
        /// and
        /// <see cref="GetDescentLine()"/>
        /// methods already include Rise.
        /// This method is exposed to allow listeners to determine if an explicit rise was involved in the computation of the baseline (this might be useful, for example, for identifying superscript rendering)
        /// </remarks>
        /// <returns>The Rise for the text draw operation, in user space units (Ts value, scaled to user space)</returns>
        public virtual float GetRise() {
            CheckGraphicsState();
            // optimize the common case
            if (gs.GetTextRise() == 0) {
                return 0;
            }
            return ConvertHeightFromTextSpaceToUserSpace(gs.GetTextRise());
        }

        /// <summary>Provides detail useful if a listener needs access to the position of each individual glyph in the text render operation
        ///     </summary>
        /// <returns>
        /// A list of
        /// <see cref="TextRenderInfo"/>
        /// objects that represent each glyph used in the draw operation. The next effect is if there was a separate Tj opertion for each character in the rendered string
        /// </returns>
        public virtual IList<iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo> GetCharacterRenderInfos() {
            CheckGraphicsState();
            IList<iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo> rslt = new List<iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo
                >(@string.GetValue().Length);
            PdfString[] strings = SplitString(@string);
            float totalWidth = 0;
            foreach (PdfString str in strings) {
                float[] widthAndWordSpacing = GetWidthAndWordSpacing(str);
                iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo subInfo = new iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo
                    (this, str, totalWidth);
                rslt.Add(subInfo);
                totalWidth += (widthAndWordSpacing[0] * gs.GetFontSize() + gs.GetCharSpacing() + widthAndWordSpacing[1]) *
                     (gs.GetHorizontalScaling() / 100f);
            }
            foreach (iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo tri in rslt) {
                tri.GetUnscaledWidth();
            }
            return rslt;
        }

        /// <returns>The width, in user space units, of a single space character in the current font</returns>
        public virtual float GetSingleSpaceWidth() {
            return ConvertWidthFromTextSpaceToUserSpace(GetUnscaledFontSpaceWidth());
        }

        /// <returns>
        /// the text render mode that should be used for the text.  From the
        /// PDF specification, this means:
        /// <list type="bullet">
        /// <item><description>0 = Fill text
        /// </description></item>
        /// <item><description>1 = Stroke text
        /// </description></item>
        /// <item><description>2 = Fill, then stroke text
        /// </description></item>
        /// <item><description>3 = Invisible
        /// </description></item>
        /// <item><description>4 = Fill text and add to path for clipping
        /// </description></item>
        /// <item><description>5 = Stroke text and add to path for clipping
        /// </description></item>
        /// <item><description>6 = Fill, then stroke text and add to path for clipping
        /// </description></item>
        /// <item><description>7 = Add text to padd for clipping
        /// </description></item>
        /// </list>
        /// </returns>
        public virtual int GetTextRenderMode() {
            CheckGraphicsState();
            return gs.GetTextRenderingMode();
        }

        /// <returns>the current fill color.</returns>
        public virtual Color GetFillColor() {
            CheckGraphicsState();
            return gs.GetFillColor();
        }

        /// <returns>the current stroke color.</returns>
        public virtual Color GetStrokeColor() {
            CheckGraphicsState();
            return gs.GetStrokeColor();
        }

        public virtual float GetFontSize() {
            CheckGraphicsState();
            return gs.GetFontSize();
        }

        public virtual float GetHorizontalScaling() {
            CheckGraphicsState();
            return gs.GetHorizontalScaling();
        }

        public virtual float GetCharSpacing() {
            CheckGraphicsState();
            return gs.GetCharSpacing();
        }

        public virtual float GetWordSpacing() {
            CheckGraphicsState();
            return gs.GetWordSpacing();
        }

        public virtual float GetLeading() {
            CheckGraphicsState();
            return gs.GetLeading();
        }

        /// <summary>Gets /ActualText tag entry value if this text chunk is marked content.</summary>
        /// <returns>/ActualText value or <c>null</c> if none found</returns>
        public virtual String GetActualText() {
            String lastActualText = null;
            foreach (CanvasTag tag in canvasTagHierarchy) {
                lastActualText = tag.GetActualText();
                if (lastActualText != null) {
                    break;
                }
            }
            return lastActualText;
        }

        /// <summary>Gets /E tag (expansion text) entry value if this text chunk is marked content.</summary>
        /// <returns>/E value or <c>null</c> if none found</returns>
        public virtual String GetExpansionText() {
            String expansionText = null;
            foreach (CanvasTag tag in canvasTagHierarchy) {
                expansionText = tag.GetExpansionText();
                if (expansionText != null) {
                    break;
                }
            }
            return expansionText;
        }

        /// <summary>
        /// Determines if the text represented by this
        /// <see cref="TextRenderInfo"/>
        /// instance is written in a text showing operator
        /// wrapped by /ReversedChars marked content sequence
        /// </summary>
        /// <returns><c>true</c> if this text block lies within /ReversedChars block, <c>false</c> otherwise</returns>
        public virtual bool IsReversedChars() {
            foreach (CanvasTag tag in canvasTagHierarchy) {
                if (tag != null) {
                    if (PdfName.ReversedChars.Equals(tag.GetRole())) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Gets hierarchy of the canvas tags that wraps given text.</summary>
        /// <returns>list of the wrapping canvas tags. The first tag is the innermost (nearest to the text).</returns>
        public virtual IList<CanvasTag> GetCanvasTagHierarchy() {
            return canvasTagHierarchy;
        }

        /// <returns>the unscaled (i.e. in Text space) width of the text</returns>
        public virtual float GetUnscaledWidth() {
            if (float.IsNaN(unscaledWidth)) {
                unscaledWidth = GetPdfStringWidth(@string, false);
            }
            return unscaledWidth;
        }

        private LineSegment GetUnscaledBaselineWithOffset(float yOffset) {
            CheckGraphicsState();
            // we need to correct the width so we don't have an extra character and word spaces at the end.  The extra character and word spaces
            // are important for tracking relative text coordinate systems, but should not be part of the baseline
            String unicodeStr = @string.ToUnicodeString();
            float correctedUnscaledWidth = GetUnscaledWidth() - (gs.GetCharSpacing() + (unicodeStr.Length > 0 && unicodeStr
                [unicodeStr.Length - 1] == ' ' ? gs.GetWordSpacing() : 0)) * (gs.GetHorizontalScaling() / 100f);
            return new LineSegment(new Vector(0, yOffset, 1), new Vector(correctedUnscaledWidth, yOffset, 1));
        }

        /// <param name="width">the width, in text space</param>
        /// <returns>the width in user space</returns>
        private float ConvertWidthFromTextSpaceToUserSpace(float width) {
            LineSegment textSpace = new LineSegment(new Vector(0, 0, 1), new Vector(width, 0, 1));
            LineSegment userSpace = textSpace.TransformBy(textToUserSpaceTransformMatrix);
            return userSpace.GetLength();
        }

        /// <param name="height">the height, in text space</param>
        /// <returns>the height in user space</returns>
        private float ConvertHeightFromTextSpaceToUserSpace(float height) {
            LineSegment textSpace = new LineSegment(new Vector(0, 0, 1), new Vector(0, height, 1));
            LineSegment userSpace = textSpace.TransformBy(textToUserSpaceTransformMatrix);
            return userSpace.GetLength();
        }

        /// <summary>Calculates the width of a space character in text space units.</summary>
        /// <returns>the width of a single space character in text space units</returns>
        private float GetUnscaledFontSpaceWidth() {
            CheckGraphicsState();
            char spaceChar = ' ';
            int charWidth = gs.GetFont().GetWidth(spaceChar);
            if (charWidth == 0) {
                charWidth = gs.GetFont().GetFontProgram().GetAvgWidth();
            }
            float charWidthInGlyphSpace = FontProgram.ConvertTextSpaceToGlyphSpace(charWidth);
            return (charWidthInGlyphSpace * gs.GetFontSize() + gs.GetCharSpacing() + gs.GetWordSpacing()) * gs.GetHorizontalScaling
                () / 100F;
        }

        /// <summary>Gets the width of a PDF string in text space units</summary>
        /// <param name="string">the string that needs measuring</param>
        /// <returns>the width of a String in text space units</returns>
        private float GetPdfStringWidth(PdfString @string, bool singleCharString) {
            CheckGraphicsState();
            if (singleCharString) {
                float[] widthAndWordSpacing = GetWidthAndWordSpacing(@string);
                return (float)(((double)widthAndWordSpacing[0] * (double)gs.GetFontSize() + (double)gs.GetCharSpacing() + 
                    (double)widthAndWordSpacing[1]) * (double)gs.GetHorizontalScaling() / 100f);
            }
            else {
                float totalWidth = 0;
                foreach (PdfString str in SplitString(@string)) {
                    totalWidth += GetPdfStringWidth(str, true);
                }
                return totalWidth;
            }
        }

        /// <summary>Calculates width and word spacing of a single character PDF string.</summary>
        /// <remarks>
        /// Calculates width and word spacing of a single character PDF string.
        /// IMPORTANT: Shall ONLY be used for a single character pdf strings.
        /// </remarks>
        /// <param name="string">a character to calculate width.</param>
        /// <returns>array of 2 items: first item is a character width, second item is a calculated word spacing.</returns>
        private float[] GetWidthAndWordSpacing(PdfString @string) {
            CheckGraphicsState();
            return new float[] { FontProgram.ConvertTextSpaceToGlyphSpace(gs.GetFont().GetContentWidth(@string)), " ".
                Equals(@string.GetValue()) ? gs.GetWordSpacing() : 0 };
        }

        /// <summary>Converts a single character string to char code.</summary>
        /// <param name="string">single character string to convert to.</param>
        /// <returns>char code.</returns>
        private int GetCharCode(String @string) {
            try {
                byte[] b = @string.GetBytes("UTF-16BE");
                int value = 0;
                for (int i = 0; i < b.Length - 1; i++) {
                    value += b[i] & 0xff;
                    value <<= 8;
                }
                if (b.Length > 0) {
                    value += b[b.Length - 1] & 0xff;
                }
                return value;
            }
            catch (ArgumentException) {
            }
            return 0;
        }

        /// <summary>Split PDF string into array of single character PDF strings.</summary>
        /// <param name="string">PDF string to be split.</param>
        /// <returns>split PDF string.</returns>
        private PdfString[] SplitString(PdfString @string) {
            CheckGraphicsState();
            PdfFont font = gs.GetFont();
            if (font is PdfType0Font) {
                // Number of bytes forming one glyph can be arbitrary from [1; 4] range
                IList<PdfString> strings = new List<PdfString>();
                GlyphLine glyphLine = gs.GetFont().DecodeIntoGlyphLine(@string);
                for (int i = glyphLine.start; i < glyphLine.end; i++) {
                    strings.Add(new PdfString(gs.GetFont().ConvertToBytes(glyphLine.Get(i))));
                }
                return strings.ToArray(new PdfString[strings.Count]);
            }
            else {
                // One byte corresponds to one character
                PdfString[] strings = new PdfString[@string.GetValue().Length];
                for (int i = 0; i < @string.GetValue().Length; i++) {
                    strings[i] = new PdfString(@string.GetValue().JSubstring(i, i + 1), @string.GetEncoding());
                }
                return strings;
            }
        }

        private float[] GetAscentDescent() {
            CheckGraphicsState();
            float ascent = gs.GetFont().GetFontProgram().GetFontMetrics().GetTypoAscender();
            float descent = gs.GetFont().GetFontProgram().GetFontMetrics().GetTypoDescender();
            // If descent is positive, we consider it a bug and fix it
            if (descent > 0) {
                descent = -descent;
            }
            float scale = (ascent - descent < 700) ? (ascent - descent) : FontProgram.UNITS_NORMALIZATION;
            descent = descent / scale * gs.GetFontSize();
            ascent = ascent / scale * gs.GetFontSize();
            return new float[] { ascent, descent };
        }
    }
}
