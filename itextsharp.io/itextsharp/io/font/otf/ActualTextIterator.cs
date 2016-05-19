using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using iTextSharp.IO.Util;

namespace iTextSharp.IO.Font.Otf
{
	public class ActualTextIterator : IEnumerator<GlyphLine.GlyphLinePart>
	{
		private GlyphLine glyphLine;

		public ActualTextIterator(GlyphLine glyphLine)
		{
			this.glyphLine = glyphLine;
			this.pos = glyphLine.start - 1;
		}

		public ActualTextIterator(GlyphLine glyphLine, int start, int end)
			: this(new GlyphLine(glyphLine.glyphs, glyphLine.actualText, start, end))
		{
		}

		private int pos;

		public bool MoveNext()
		{
			return pos < glyphLine.end;
		}

	    public void Reset()
	    {
	        pos = -1;
	    }

	    object IEnumerator.Current
	    {
	        get { return Current; }
	    }

	    public GlyphLine.GlyphLinePart Current
		{
			get
			{
				if (glyphLine.actualText == null)
				{
					GlyphLine.GlyphLinePart result = new GlyphLine.GlyphLinePart(pos, glyphLine.end, 
						null);
					pos = glyphLine.end;
					return result;
				}
				else
				{
					GlyphLine.GlyphLinePart currentResult = NextGlyphLinePart(pos);
					if (currentResult == null)
					{
						return null;
					}
					pos = currentResult.end;
					while (pos < glyphLine.end && !GlyphLinePartNeedsActualText(currentResult))
					{
						currentResult.actualText = null;
						GlyphLine.GlyphLinePart nextResult = NextGlyphLinePart(pos);
						if (nextResult != null && !GlyphLinePartNeedsActualText(nextResult))
						{
							currentResult.end = nextResult.end;
							pos = nextResult.end;
						}
						else
						{
							break;
						}
					}
					return currentResult;
				}
			}
		}

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
				// TODO zero glyph is a special case. Unicode might be special
				toUnicodeMapResult.Append(TextUtil.ConvertFromUtf32((int) currentGlyph.GetUnicode()));
			}
			return needsActualText || !toUnicodeMapResult.ToString().Equals(glyphLinePart.actualText
				);
		}

	    public void Dispose()
	    {
	    }
	}
}
