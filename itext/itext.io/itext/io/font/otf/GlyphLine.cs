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
using System.Text;

namespace iText.IO.Font.Otf {
    public class GlyphLine {
        private int start;

        private int end;

        private int idx;

        protected internal IList<Glyph> glyphs;

        protected internal IList<GlyphLine.ActualText> actualText;

        public GlyphLine() {
            this.glyphs = new List<Glyph>();
        }

        /// <summary>Create a new line of Glyphs.</summary>
        /// <param name="glyphs">list containing the glyphs</param>
        public GlyphLine(IList<Glyph> glyphs) {
            this.glyphs = glyphs;
            this.start = 0;
            this.end = glyphs.Count;
        }

        /// <summary>Create a new line of Glyphs from a slice of a List of Glyphs.</summary>
        /// <param name="glyphs">list of Glyphs to slice</param>
        /// <param name="start">starting index of the slice</param>
        /// <param name="end">terminating index of the slice</param>
        public GlyphLine(IList<Glyph> glyphs, int start, int end) {
            this.glyphs = glyphs;
            this.start = start;
            this.end = end;
        }

        /// <summary>Create a new line of Glyphs from a slice of a List of Glyphs, and add the actual text.</summary>
        /// <param name="glyphs">list of Glyphs to slice</param>
        /// <param name="actualText">corresponding list containing the actual text the glyphs represent</param>
        /// <param name="start">starting index of the slice</param>
        /// <param name="end">terminating index of the slice</param>
        protected internal GlyphLine(IList<Glyph> glyphs, IList<GlyphLine.ActualText> actualText, int start, int end
            )
            : this(glyphs, start, end) {
            this.actualText = actualText;
        }

        /// <summary>Copy a line of Glyphs.</summary>
        /// <param name="other">line of Glyphs to copy</param>
        public GlyphLine(iText.IO.Font.Otf.GlyphLine other) {
            this.glyphs = other.glyphs;
            this.actualText = other.actualText;
            this.start = other.start;
            this.end = other.end;
            this.idx = other.idx;
        }

        /// <summary>Copy a slice of a line of Glyphs</summary>
        /// <param name="other">line of Glyphs to copy</param>
        /// <param name="start">starting index of the slice</param>
        /// <param name="end">terminating index of the slice</param>
        public GlyphLine(iText.IO.Font.Otf.GlyphLine other, int start, int end) {
            this.glyphs = other.glyphs.SubList(start, end);
            if (other.actualText != null) {
                this.actualText = other.actualText.SubList(start, end);
            }
            this.start = 0;
            this.end = end - start;
            this.idx = other.idx - start;
        }

        /// <summary>Retrieves the start of the glyph line.</summary>
        /// <returns>start of glyph line</returns>
        public virtual int GetStart() {
            return start;
        }

        /// <summary>Sets the start of the glyph line.</summary>
        /// <param name="start">start of glyph line</param>
        public virtual void SetStart(int start) {
            this.start = start;
        }

        /// <summary>Retrieves the end of the glyph line.</summary>
        /// <returns>end of glyph line</returns>
        public virtual int GetEnd() {
            return end;
        }

        /// <summary>Sets the end of the glyph line.</summary>
        /// <param name="end">end of glyph line</param>
        public virtual void SetEnd(int end) {
            this.end = end;
        }

        /// <summary>Retrieves the idx of the glyph line.</summary>
        /// <returns>idx of glyph line</returns>
        public virtual int GetIdx() {
            return idx;
        }

        /// <summary>Sets the idx of the glyph line.</summary>
        /// <param name="idx">idx of glyph line</param>
        public virtual void SetIdx(int idx) {
            this.idx = idx;
        }

        /// <summary>Get the unicode string representation of the GlyphLine slice.</summary>
        /// <param name="start">starting index of the slice</param>
        /// <param name="end">terminating index of the slice</param>
        /// <returns>String containing the unicode representation of the slice.</returns>
        public virtual String ToUnicodeString(int start, int end) {
            ActualTextIterator iter = new ActualTextIterator(this, start, end);
            StringBuilder str = new StringBuilder();
            while (iter.HasNext()) {
                GlyphLine.GlyphLinePart part = iter.Next();
                if (part.GetActualText() != null) {
                    str.Append(part.GetActualText());
                }
                else {
                    for (int i = part.GetStart(); i < part.GetEnd(); i++) {
                        str.Append(glyphs[i].GetUnicodeChars());
                    }
                }
            }
            return str.ToString();
        }

        public override String ToString() {
            return ToUnicodeString(start, end);
        }

        /// <summary>Copy a slice of this Glyphline.</summary>
        /// <param name="left">leftmost index of the slice</param>
        /// <param name="right">rightmost index of the slice</param>
        /// <returns>new GlyphLine containing the copied slice</returns>
        public virtual iText.IO.Font.Otf.GlyphLine Copy(int left, int right) {
            iText.IO.Font.Otf.GlyphLine glyphLine = new iText.IO.Font.Otf.GlyphLine();
            glyphLine.SetStart(0);
            glyphLine.SetEnd(right - left);
            glyphLine.SetGlyphs(new List<Glyph>(glyphs.SubList(left, right)));
            glyphLine.actualText = actualText == null ? null : new List<GlyphLine.ActualText>(actualText.SubList(left, 
                right));
            return glyphLine;
        }

        public virtual Glyph Get(int index) {
            return glyphs[index];
        }

        public virtual Glyph Set(int index, Glyph glyph) {
            return glyphs[index] = glyph;
        }

        public virtual void Add(Glyph glyph) {
            glyphs.Add(glyph);
            if (actualText != null) {
                actualText.Add(null);
            }
        }

        public virtual void Add(int index, Glyph glyph) {
            glyphs.Add(index, glyph);
            if (actualText != null) {
                actualText.Add(index, null);
            }
        }

        public virtual void SetGlyphs(IList<Glyph> replacementGlyphs) {
            glyphs = new List<Glyph>(replacementGlyphs);
            start = 0;
            end = replacementGlyphs.Count;
            actualText = null;
        }

        /// <summary>Add a line to the current one.</summary>
        /// <remarks>
        /// Add a line to the current one.
        /// The glyphs from the start till the end points will be copied.
        /// The same is true for the actual text.
        /// </remarks>
        /// <param name="other">the line that should be added to the current one</param>
        public virtual void Add(iText.IO.Font.Otf.GlyphLine other) {
            if (other.actualText != null) {
                if (actualText == null) {
                    actualText = new List<GlyphLine.ActualText>(glyphs.Count);
                    for (int i = 0; i < glyphs.Count; i++) {
                        actualText.Add(null);
                    }
                }
                actualText.AddAll(other.actualText.SubList(other.GetStart(), other.GetEnd()));
            }
            glyphs.AddAll(other.glyphs.SubList(other.GetStart(), other.GetEnd()));
            if (null != actualText) {
                while (actualText.Count < glyphs.Count) {
                    actualText.Add(null);
                }
            }
        }

        /// <summary>Replaces the current content with the other line's content.</summary>
        /// <param name="other">the line with the content to be set to the current one</param>
        public virtual void ReplaceContent(iText.IO.Font.Otf.GlyphLine other) {
            glyphs.Clear();
            glyphs.AddAll(other.glyphs);
            if (other.actualText != null) {
                if (actualText == null) {
                    actualText = new List<GlyphLine.ActualText>();
                }
                else {
                    actualText.Clear();
                }
                actualText.AddAll(other.actualText);
            }
            else {
                actualText = null;
            }
            start = other.GetStart();
            end = other.GetEnd();
        }

        public virtual int Size() {
            return glyphs.Count;
        }

        public virtual void SubstituteManyToOne(OpenTypeFontTableReader tableReader, int lookupFlag, int rightPartLen
            , int substitutionGlyphIndex) {
            OpenTableLookup.GlyphIndexer gidx = new OpenTableLookup.GlyphIndexer();
            gidx.SetLine(this);
            gidx.SetIdx(idx);
            StringBuilder chars = new StringBuilder();
            Glyph currentGlyph = glyphs[idx];
            if (currentGlyph.GetChars() != null) {
                chars.Append(currentGlyph.GetChars());
            }
            else {
                if (currentGlyph.HasValidUnicode()) {
                    chars.Append(iText.IO.Util.TextUtil.ConvertFromUtf32(currentGlyph.GetUnicode()));
                }
            }
            for (int j = 0; j < rightPartLen; ++j) {
                gidx.NextGlyph(tableReader, lookupFlag);
                currentGlyph = glyphs[gidx.GetIdx()];
                if (currentGlyph.GetChars() != null) {
                    chars.Append(currentGlyph.GetChars());
                }
                else {
                    if (currentGlyph.HasValidUnicode()) {
                        chars.Append(iText.IO.Util.TextUtil.ConvertFromUtf32(currentGlyph.GetUnicode()));
                    }
                }
                RemoveGlyph(gidx.GetIdx());
                gidx.SetIdx(gidx.GetIdx() - 1);
            }
            char[] newChars = new char[chars.Length];
            chars.GetChars(0, chars.Length, newChars, 0);
            Glyph newGlyph = tableReader.GetGlyph(substitutionGlyphIndex);
            newGlyph.SetChars(newChars);
            glyphs[idx] = newGlyph;
            end -= rightPartLen;
        }

        public virtual void SubstituteOneToOne(OpenTypeFontTableReader tableReader, int substitutionGlyphIndex) {
            Glyph oldGlyph = glyphs[idx];
            Glyph newGlyph = tableReader.GetGlyph(substitutionGlyphIndex);
            if (oldGlyph.GetChars() != null) {
                newGlyph.SetChars(oldGlyph.GetChars());
            }
            else {
                if (newGlyph.HasValidUnicode()) {
                    newGlyph.SetChars(iText.IO.Util.TextUtil.ConvertFromUtf32(newGlyph.GetUnicode()));
                }
                else {
                    if (oldGlyph.HasValidUnicode()) {
                        newGlyph.SetChars(iText.IO.Util.TextUtil.ConvertFromUtf32(oldGlyph.GetUnicode()));
                    }
                }
            }
            glyphs[idx] = newGlyph;
        }

        public virtual void SubstituteOneToMany(OpenTypeFontTableReader tableReader, int[] substGlyphIds) {
            //sequence length shall be at least 1
            int substCode = substGlyphIds[0];
            Glyph oldGlyph = glyphs[idx];
            Glyph glyph = tableReader.GetGlyph(substCode);
            glyphs[idx] = glyph;
            if (substGlyphIds.Length > 1) {
                IList<Glyph> additionalGlyphs = new List<Glyph>(substGlyphIds.Length - 1);
                for (int i = 1; i < substGlyphIds.Length; ++i) {
                    substCode = substGlyphIds[i];
                    glyph = tableReader.GetGlyph(substCode);
                    additionalGlyphs.Add(glyph);
                }
                AddAllGlyphs(idx + 1, additionalGlyphs);
                if (null != actualText) {
                    if (null == actualText[idx]) {
                        actualText[idx] = new GlyphLine.ActualText(oldGlyph.GetUnicodeString());
                    }
                    for (int i = 0; i < additionalGlyphs.Count; i++) {
                        this.actualText[idx + 1 + i] = actualText[idx];
                    }
                }
                idx += substGlyphIds.Length - 1;
                end += substGlyphIds.Length - 1;
            }
        }

        public virtual iText.IO.Font.Otf.GlyphLine Filter(GlyphLine.IGlyphLineFilter filter) {
            bool anythingFiltered = false;
            IList<Glyph> filteredGlyphs = new List<Glyph>(end - start);
            IList<GlyphLine.ActualText> filteredActualText = actualText != null ? new List<GlyphLine.ActualText>(end -
                 start) : null;
            for (int i = start; i < end; i++) {
                if (filter.Accept(glyphs[i])) {
                    filteredGlyphs.Add(glyphs[i]);
                    if (filteredActualText != null) {
                        filteredActualText.Add(actualText[i]);
                    }
                }
                else {
                    anythingFiltered = true;
                }
            }
            if (anythingFiltered) {
                return new iText.IO.Font.Otf.GlyphLine(filteredGlyphs, filteredActualText, 0, filteredGlyphs.Count);
            }
            else {
                return this;
            }
        }

        public virtual void SetActualText(int left, int right, String text) {
            if (this.actualText == null) {
                this.actualText = new List<GlyphLine.ActualText>(glyphs.Count);
                for (int i = 0; i < glyphs.Count; i++) {
                    this.actualText.Add(null);
                }
            }
            GlyphLine.ActualText actualText = new GlyphLine.ActualText(text);
            for (int i = left; i < right; i++) {
                this.actualText[i] = actualText;
            }
        }

        public virtual IEnumerator<GlyphLine.GlyphLinePart> Iterator() {
            return new ActualTextIterator(this);
        }

        public override bool Equals(Object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            iText.IO.Font.Otf.GlyphLine other = (iText.IO.Font.Otf.GlyphLine)obj;
            if (end - start != other.end - other.start) {
                return false;
            }
            if (actualText == null && other.actualText != null || actualText != null && other.actualText == null) {
                return false;
            }
            for (int i = start; i < end; i++) {
                int otherPos = other.start + i - start;
                Glyph myGlyph = Get(i);
                Glyph otherGlyph = other.Get(otherPos);
                if (myGlyph == null && otherGlyph != null || myGlyph != null && !myGlyph.Equals(otherGlyph)) {
                    return false;
                }
                GlyphLine.ActualText myAT = actualText == null ? null : actualText[i];
                GlyphLine.ActualText otherAT = other.actualText == null ? null : other.actualText[otherPos];
                if (myAT == null && otherAT != null || myAT != null && !myAT.Equals(otherAT)) {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode() {
            int result = 0;
            result = 31 * result + start;
            result = 31 * result + end;
            for (int i = start; i < end; i++) {
                result = 31 * result + glyphs[i].GetHashCode();
            }
            if (null != actualText) {
                for (int i = start; i < end; i++) {
                    result = 31 * result;
                    if (null != actualText[i]) {
                        result += actualText[i].GetHashCode();
                    }
                }
            }
            return result;
        }

        private void RemoveGlyph(int index) {
            glyphs.JRemoveAt(index);
            if (actualText != null) {
                actualText.JRemoveAt(index);
            }
        }

        private void AddAllGlyphs(int index, IList<Glyph> additionalGlyphs) {
            glyphs.AddAll(index, additionalGlyphs);
            if (actualText != null) {
                for (int i = 0; i < additionalGlyphs.Count; i++) {
                    this.actualText.Add(index, null);
                }
            }
        }

        public interface IGlyphLineFilter {
            bool Accept(Glyph glyph);
        }

        public class GlyphLinePart {
            private int start;

            private int end;

            // Might be null if it's not necessary
            private String actualText;

            private bool reversed;

            /// <summary>Creates a glyph line part object with given start and end values.</summary>
            /// <remarks>
            /// Creates a glyph line part object with given start and end values.
            /// Actual text is set to null.
            /// </remarks>
            /// <param name="start">start value of the glyph line part</param>
            /// <param name="end">end value of the glyph line part</param>
            public GlyphLinePart(int start, int end)
                : this(start, end, null) {
            }

            /// <summary>Creates a glyph line part object with given start, end and actual text values.</summary>
            /// <param name="start">start value of the glyph line part</param>
            /// <param name="end">end value of the glyph line part</param>
            /// <param name="actualText">actual text</param>
            public GlyphLinePart(int start, int end, String actualText) {
                this.start = start;
                this.end = end;
                this.actualText = actualText;
            }

            /// <summary>Retrieves the start of the glyph line part.</summary>
            /// <returns>start value of glyph line part</returns>
            public virtual int GetStart() {
                return start;
            }

            /// <summary>Sets the start of the glyph line part.</summary>
            /// <param name="start">start of the glyph line part</param>
            /// <returns>Altered glyph line part object</returns>
            public virtual GlyphLine.GlyphLinePart SetStart(int start) {
                this.start = start;
                return this;
            }

            /// <summary>Retrieves the end of the glyph line part.</summary>
            /// <returns>end value of glyph line part</returns>
            public virtual int GetEnd() {
                return end;
            }

            /// <summary>Sets the end of the glyph line part.</summary>
            /// <param name="end">end value of glyph line part</param>
            /// <returns>Altered glyph line part object</returns>
            public virtual GlyphLine.GlyphLinePart SetEnd(int end) {
                this.end = end;
                return this;
            }

            /// <summary>Retrieves the actual text of the glyph line part.</summary>
            /// <returns>Actual text of glyph line part</returns>
            public virtual String GetActualText() {
                return actualText;
            }

            /// <summary>Sets the actual text of the glyph line part.</summary>
            /// <param name="actualText">Actual text of glyph line part</param>
            /// <returns>Altered Glyph line part object</returns>
            public virtual GlyphLine.GlyphLinePart SetActualText(String actualText) {
                this.actualText = actualText;
                return this;
            }

            /// <summary>Retrieves whether the glyph line part is reversed.</summary>
            /// <returns>True if it is reversed, false otherwise.</returns>
            public virtual bool IsReversed() {
                return reversed;
            }

            /// <summary>Sets whether the glyph line part is reversed.</summary>
            /// <param name="reversed">true if it should be reversed, false otherwise</param>
            /// <returns>Altered glyph line part object</returns>
            public virtual GlyphLine.GlyphLinePart SetReversed(bool reversed) {
                this.reversed = reversed;
                return this;
            }
        }

        protected internal class ActualText {
            private readonly String value;

            public ActualText(String value) {
                this.value = value;
            }

            /// <summary>Retrieves the value of the actual text.</summary>
            /// <returns>actual text value</returns>
            public virtual String GetValue() {
                return value;
            }

            public override bool Equals(Object obj) {
                if (this == obj) {
                    return true;
                }
                if (obj == null || GetType() != obj.GetType()) {
                    return false;
                }
                GlyphLine.ActualText other = (GlyphLine.ActualText)obj;
                return value == null && other.GetValue() == null || value.Equals(other.GetValue());
            }

            public override int GetHashCode() {
                return 31 * value.GetHashCode();
            }
        }
    }
}
