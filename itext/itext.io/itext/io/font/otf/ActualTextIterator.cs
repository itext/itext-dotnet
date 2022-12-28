/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using iText.IO.Util;

namespace iText.IO.Font.Otf
{
	public class ActualTextIterator : IEnumerator<GlyphLine.GlyphLinePart>
	{
		private GlyphLine glyphLine;

		public ActualTextIterator(GlyphLine glyphLine)
		{
			this.glyphLine = glyphLine;
			this.pos = glyphLine.start;
		}

		public ActualTextIterator(GlyphLine glyphLine, int start, int end)
			: this(new GlyphLine(glyphLine.glyphs, glyphLine.actualText, start, end))
		{
		}

		private int pos;

		public bool MoveNext()
		{
		    if (!HasNext())
                return false;

            Current = Next();
		    return true;
		}

	    public void Reset()
	    {
	        pos = -1;
	    }

	    public GlyphLine.GlyphLinePart Next() {
            if (glyphLine.actualText == null) {
                GlyphLine.GlyphLinePart result = new GlyphLine.GlyphLinePart(pos, glyphLine.end,
                    null);
                pos = glyphLine.end;
                return result;
            } else {
                GlyphLine.GlyphLinePart currentResult = NextGlyphLinePart(pos);
                if (currentResult == null) {
                    return null;
                }
                pos = currentResult.end;

                if (!GlyphLinePartNeedsActualText(currentResult)) {
                    currentResult.actualText = null;
                    // Try to add more pieces without "actual text"
                    while (pos < glyphLine.end) {
                        GlyphLine.GlyphLinePart nextResult = NextGlyphLinePart(pos);
                        if (nextResult != null && !GlyphLinePartNeedsActualText(nextResult)) {
                            currentResult.end = nextResult.end;
                            pos = nextResult.end;
                        } else {
                            break;
                        }
                    }
                }

                return currentResult;
            }
        }

	    public bool HasNext() {
            return pos < glyphLine.end;
        }


        object IEnumerator.Current
	    {
	        get { return Current; }
	    }

	    public GlyphLine.GlyphLinePart Current { get; private set; }

		private GlyphLine.GlyphLinePart NextGlyphLinePart(int pos)
		{
			if (pos >= glyphLine.end)
			{
				return null;
			}
			int startPos = pos;
			GlyphLine.ActualText startActualText = glyphLine.actualText[pos];
			while (pos < glyphLine.end && glyphLine.actualText[pos] == startActualText)
			{
				pos++;
			}
			return new GlyphLine.GlyphLinePart(startPos, pos, startActualText != null ? startActualText
				.value : null);
		}

		private bool GlyphLinePartNeedsActualText(GlyphLine.GlyphLinePart glyphLinePart)
		{
			if (glyphLinePart.actualText == null)
			{
				return false;
			}
			bool needsActualText = false;
			StringBuilder toUnicodeMapResult = new StringBuilder();
			for (int i = glyphLinePart.start; i < glyphLinePart.end; i++)
			{
				Glyph currentGlyph = glyphLine.glyphs[i];
                if (!currentGlyph.HasValidUnicode())
				{
					needsActualText = true;
					break;
				}
				toUnicodeMapResult.Append(TextUtil.ConvertFromUtf32(currentGlyph.GetUnicode()));
			}
			return needsActualText || !toUnicodeMapResult.ToString().Equals(glyphLinePart.actualText
				);
		}

	    public void Dispose()
	    {
	    }
	}
}
