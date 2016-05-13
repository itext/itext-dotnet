/*
$Id: 4bfd8df152d3b1d95156c9260f50970f1ac39f93 $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iTextSharp.IO.Util;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;

namespace iTextSharp.Kernel.Pdf.Canvas.Parser.Data
{
	/// <summary>
	/// Provides information and calculations needed by render listeners
	/// to display/evaluate text render operations.
	/// </summary>
	/// <remarks>
	/// Provides information and calculations needed by render listeners
	/// to display/evaluate text render operations.
	/// <br /><br />
	/// This is passed between the
	/// <see cref="iTextSharp.Kernel.Pdf.Canvas.Parser.PdfCanvasProcessor"/>
	/// and
	/// <see cref="iTextSharp.Kernel.Pdf.Canvas.Parser.Listener.IEventListener"/>
	/// objects as text rendering operations are
	/// discovered
	/// </remarks>
	public class TextRenderInfo : IEventData
	{
		private readonly PdfString @string;

		private String text = null;

		private readonly Matrix textToUserSpaceTransformMatrix;

		private readonly CanvasGraphicsState gs;

		private float unscaledWidth = float.NaN;

		private double[] fontMatrix = null;

		/// <summary>Hierarchy of nested canvas tags for the text from the most inner (nearest to text) tag to the most outer.
		/// 	</summary>
		private readonly IList<CanvasTag> canvasTagHierarchy;

		/// <summary>Creates a new TextRenderInfo object</summary>
		/// <param name="string">the PDF string that should be displayed</param>
		/// <param name="gs">the graphics state (note: at this time, this is not immutable, so don't cache it)
		/// 	</param>
		/// <param name="textMatrix">the text matrix at the time of the render operation</param>
		/// <param name="canvasTagHierarchy">the marked content tags sequence, if available</param>
		public TextRenderInfo(PdfString @string, CanvasGraphicsState gs, Matrix textMatrix
			, Stack<CanvasTag> canvasTagHierarchy)
		{
			this.@string = @string;
			this.textToUserSpaceTransformMatrix = textMatrix.Multiply(gs.GetCtm());
			this.gs = gs;
			this.canvasTagHierarchy = JavaCollectionsUtil.UnmodifiableList(new List<CanvasTag
				>(canvasTagHierarchy));
			this.fontMatrix = gs.GetFont().GetFontMatrix();
		}

		/// <summary>Used for creating sub-TextRenderInfos for each individual character</summary>
		/// <param name="parent">the parent TextRenderInfo</param>
		/// <param name="string">the content of a TextRenderInfo</param>
		/// <param name="horizontalOffset">the unscaled horizontal offset of the character that this TextRenderInfo represents
		/// 	</param>
		private TextRenderInfo(iTextSharp.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo parent
			, PdfString @string, float horizontalOffset)
		{
			this.@string = @string;
			this.textToUserSpaceTransformMatrix = new Matrix(horizontalOffset, 0).Multiply(parent
				.textToUserSpaceTransformMatrix);
			this.gs = parent.gs;
			this.canvasTagHierarchy = parent.canvasTagHierarchy;
			this.fontMatrix = gs.GetFont().GetFontMatrix();
		}

		/// <returns>the text to render</returns>
		public virtual String GetText()
		{
			if (text == null)
			{
				text = gs.GetFont().Decode(@string);
			}
			return text;
		}

		/// <returns>original PDF string</returns>
		public virtual PdfString GetPdfString()
		{
			return @string;
		}

		/// <summary>
		/// Checks if the text belongs to a marked content sequence
		/// with a given mcid.
		/// </summary>
		/// <param name="mcid">a marked content id</param>
		/// <returns>true if the text is marked with this id</returns>
		public virtual bool HasMcid(int mcid)
		{
			return HasMcid(mcid, false);
		}

		/// <summary>
		/// Checks if the text belongs to a marked content sequence
		/// with a given mcid.
		/// </summary>
		/// <param name="mcid">a marked content id</param>
		/// <param name="checkTheTopmostLevelOnly">indicates whether to check the topmost level of marked content stack only
		/// 	</param>
		/// <returns>true if the text is marked with this id</returns>
		public virtual bool HasMcid(int mcid, bool checkTheTopmostLevelOnly)
		{
			if (checkTheTopmostLevelOnly)
			{
				if (canvasTagHierarchy != null)
				{
					int infoMcid = GetMcid();
					return infoMcid != -1 && infoMcid == mcid;
				}
			}
			else
			{
				foreach (CanvasTag tag in canvasTagHierarchy)
				{
					if (tag.HasMcid())
					{
						if (tag.GetMcid() == mcid)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <returns>the marked content associated with the TextRenderInfo instance.</returns>
		public virtual int GetMcid()
		{
			foreach (CanvasTag tag in canvasTagHierarchy)
			{
				if (tag.HasMcid())
				{
					return tag.GetMcid();
				}
			}
			return -1;
		}

		/// <summary>Gets the baseline for the text (i.e.</summary>
		/// <remarks>
		/// Gets the baseline for the text (i.e. the line that the text 'sits' on)
		/// This value includes the Rise of the draw operation - see
		/// <see cref="GetRise()"/>
		/// for the amount added by Rise
		/// </remarks>
		/// <returns>the baseline line segment</returns>
		public virtual LineSegment GetBaseline()
		{
			return GetUnscaledBaselineWithOffset(0 + gs.GetTextRise()).TransformBy(textToUserSpaceTransformMatrix
				);
		}

		public virtual LineSegment GetUnscaledBaseline()
		{
			return GetUnscaledBaselineWithOffset(0 + gs.GetTextRise());
		}

		/// <summary>Gets the ascentline for the text (i.e.</summary>
		/// <remarks>
		/// Gets the ascentline for the text (i.e. the line that represents the topmost extent that a string of the current font could have)
		/// This value includes the Rise of the draw operation - see
		/// <see cref="GetRise()"/>
		/// for the amount added by Rise
		/// </remarks>
		/// <returns>the ascentline line segment</returns>
		public virtual LineSegment GetAscentLine()
		{
			float ascent = gs.GetFont().GetFontProgram().GetFontMetrics().GetTypoAscender() *
				 gs.GetFontSize() / 1000f;
			return GetUnscaledBaselineWithOffset(ascent + gs.GetTextRise()).TransformBy(textToUserSpaceTransformMatrix
				);
		}

		/// <summary>Gets the descentline for the text (i.e.</summary>
		/// <remarks>
		/// Gets the descentline for the text (i.e. the line that represents the bottom most extent that a string of the current font could have).
		/// This value includes the Rise of the draw operation - see
		/// <see cref="GetRise()"/>
		/// for the amount added by Rise
		/// </remarks>
		/// <returns>the descentline line segment</returns>
		public virtual LineSegment GetDescentLine()
		{
			// per getFontDescription() API, descent is returned as a negative number, so we apply that as a normal vertical offset
			float descent = gs.GetFont().GetFontProgram().GetFontMetrics().GetTypoDescender()
				 * gs.GetFontSize() / 1000f;
			return GetUnscaledBaselineWithOffset(descent + gs.GetTextRise()).TransformBy(textToUserSpaceTransformMatrix
				);
		}

		/// <summary>Getter for the font</summary>
		/// <returns>the font</returns>
		public virtual PdfFont GetFont()
		{
			return gs.GetFont();
		}

		/// <summary>The rise represents how far above the nominal baseline the text should be rendered.
		/// 	</summary>
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
		/// <returns>The Rise for the text draw operation, in user space units (Ts value, scaled to user space)
		/// 	</returns>
		public virtual float GetRise()
		{
			if (gs.GetTextRise() == 0)
			{
				return 0;
			}
			// optimize the common case
			return ConvertHeightFromTextSpaceToUserSpace(gs.GetTextRise());
		}

		/// <summary>Provides detail useful if a listener needs access to the position of each individual glyph in the text render operation
		/// 	</summary>
		/// <returns>
		/// A list of
		/// <see cref="TextRenderInfo"/>
		/// objects that represent each glyph used in the draw operation. The next effect is if there was a separate Tj opertion for each character in the rendered string
		/// </returns>
		public virtual IList<iTextSharp.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo> GetCharacterRenderInfos
			()
		{
			IList<iTextSharp.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo> rslt = new List<iTextSharp.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo
				>(@string.GetValue().Length);
			PdfString[] strings = SplitString(@string);
			float totalWidth = 0;
			foreach (PdfString str in strings)
			{
				float[] widthAndWordSpacing = GetWidthAndWordSpacing(str);
				iTextSharp.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo subInfo = new iTextSharp.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo
					(this, str, totalWidth);
				rslt.Add(subInfo);
				totalWidth += (widthAndWordSpacing[0] * gs.GetFontSize() + gs.GetCharSpacing() + 
					widthAndWordSpacing[1]) * (gs.GetHorizontalScaling() / 100f);
			}
			foreach (iTextSharp.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo tri in rslt)
			{
				tri.GetUnscaledWidth();
			}
			return rslt;
		}

		/// <returns>The width, in user space units, of a single space character in the current font
		/// 	</returns>
		public virtual float GetSingleSpaceWidth()
		{
			return ConvertWidthFromTextSpaceToUserSpace(GetUnscaledFontSpaceWidth());
		}

		/// <returns>
		/// the text render mode that should be used for the text.  From the
		/// PDF specification, this means:
		/// <ul>
		/// <li>0 = Fill text</li>
		/// <li>1 = Stroke text</li>
		/// <li>2 = Fill, then stroke text</li>
		/// <li>3 = Invisible</li>
		/// <li>4 = Fill text and add to path for clipping</li>
		/// <li>5 = Stroke text and add to path for clipping</li>
		/// <li>6 = Fill, then stroke text and add to path for clipping</li>
		/// <li>7 = Add text to padd for clipping</li>
		/// </ul>
		/// </returns>
		public virtual int GetTextRenderMode()
		{
			return gs.GetTextRenderingMode();
		}

		/// <returns>the current fill color.</returns>
		public virtual iTextSharp.Kernel.Color.Color GetFillColor()
		{
			return gs.GetFillColor();
		}

		/// <returns>the current stroke color.</returns>
		public virtual iTextSharp.Kernel.Color.Color GetStrokeColor()
		{
			return gs.GetStrokeColor();
		}

		public virtual float GetFontSize()
		{
			return gs.GetFontSize();
		}

		public virtual float GetHorizontalScaling()
		{
			return gs.GetHorizontalScaling();
		}

		public virtual float GetCharSpacing()
		{
			return gs.GetCharSpacing();
		}

		public virtual float GetWordSpacing()
		{
			return gs.GetWordSpacing();
		}

		public virtual float GetLeading()
		{
			return gs.GetLeading();
		}

		/// <summary>Gets /ActualText tag entry value if this text chunk is marked content.</summary>
		/// <returns>/ActualText value</returns>
		public virtual String GetActualText()
		{
			String lastActualText = null;
			foreach (CanvasTag tag in canvasTagHierarchy)
			{
				lastActualText = tag.GetActualText();
				if (lastActualText != null)
				{
					break;
				}
			}
			return lastActualText;
		}

		/// <summary>Gets hierarchy of the canvas tags that wraps given text.</summary>
		/// <returns>list of the wrapping canvas tags. The first tag is the innermost (nearest to the text).
		/// 	</returns>
		public virtual IList<CanvasTag> GetCanvasTagHierarchy()
		{
			return canvasTagHierarchy;
		}

		/// <returns>the unscaled (i.e. in Text space) width of the text</returns>
		public virtual float GetUnscaledWidth()
		{
			if (float.IsNaN(unscaledWidth))
			{
				unscaledWidth = GetPdfStringWidth(@string, false);
			}
			return unscaledWidth;
		}

		private LineSegment GetUnscaledBaselineWithOffset(float yOffset)
		{
			// we need to correct the width so we don't have an extra character and word spaces at the end.  The extra character and word spaces
			// are important for tracking relative text coordinate systems, but should not be part of the baseline
			String unicodeStr = @string.ToUnicodeString();
			float correctedUnscaledWidth = GetUnscaledWidth() - (gs.GetCharSpacing() + (unicodeStr
				.Length > 0 && unicodeStr[unicodeStr.Length - 1] == ' ' ? gs.GetWordSpacing() : 
				0)) * (gs.GetHorizontalScaling() / 100f);
			return new LineSegment(new Vector(0, yOffset, 1), new Vector(correctedUnscaledWidth
				, yOffset, 1));
		}

		/// <param name="width">the width, in text space</param>
		/// <returns>the width in user space</returns>
		private float ConvertWidthFromTextSpaceToUserSpace(float width)
		{
			LineSegment textSpace = new LineSegment(new Vector(0, 0, 1), new Vector(width, 0, 
				1));
			LineSegment userSpace = textSpace.TransformBy(textToUserSpaceTransformMatrix);
			return userSpace.GetLength();
		}

		/// <param name="height">the height, in text space</param>
		/// <returns>the height in user space</returns>
		private float ConvertHeightFromTextSpaceToUserSpace(float height)
		{
			LineSegment textSpace = new LineSegment(new Vector(0, 0, 1), new Vector(0, height
				, 1));
			LineSegment userSpace = textSpace.TransformBy(textToUserSpaceTransformMatrix);
			return userSpace.GetLength();
		}

		/// <summary>Calculates the width of a space character.</summary>
		/// <remarks>
		/// Calculates the width of a space character.  If the font does not define
		/// a width for a standard space character \u0020, we also attempt to use
		/// the width of \u00A0 (a non-breaking space in many fonts)
		/// </remarks>
		/// <returns>the width of a single space character in text space units</returns>
		private float GetUnscaledFontSpaceWidth()
		{
			char charToUse = ' ';
			if (gs.GetFont().GetWidth(charToUse) == 0)
			{
				return gs.GetFont().GetFontProgram().GetAvgWidth() / 1000f;
			}
			else
			{
				return GetStringWidth(charToUse.ToString());
			}
		}

		/// <summary>Gets the width of a String in text space units</summary>
		/// <param name="string">the string that needs measuring</param>
		/// <returns>the width of a String in text space units</returns>
		private float GetStringWidth(String @string)
		{
			float totalWidth = 0;
			for (int i = 0; i < @string.Length; i++)
			{
				char c = @string[i];
				float w = (float)(gs.GetFont().GetWidth(c) * fontMatrix[0]);
				float wordSpacing = c == 32 ? gs.GetWordSpacing() : 0f;
				totalWidth += (w * gs.GetFontSize() + gs.GetCharSpacing() + wordSpacing) * gs.GetHorizontalScaling
					() / 100f;
			}
			return totalWidth;
		}

		/// <summary>Gets the width of a PDF string in text space units</summary>
		/// <param name="string">the string that needs measuring</param>
		/// <returns>the width of a String in text space units</returns>
		private float GetPdfStringWidth(PdfString @string, bool singleCharString)
		{
			if (singleCharString)
			{
				float[] widthAndWordSpacing = GetWidthAndWordSpacing(@string);
				return (widthAndWordSpacing[0] * gs.GetFontSize() + gs.GetCharSpacing() + widthAndWordSpacing
					[1]) * gs.GetHorizontalScaling() / 100f;
			}
			else
			{
				float totalWidth = 0;
				foreach (PdfString str in SplitString(@string))
				{
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
		/// <returns>array of 2 items: first item is a character width, second item is a calculated word spacing.
		/// 	</returns>
		private float[] GetWidthAndWordSpacing(PdfString @string)
		{
			float[] result = new float[2];
			result[0] = (float)((gs.GetFont().GetContentWidth(@string) * fontMatrix[0]));
			result[1] = " ".Equals(@string.GetValue()) ? gs.GetWordSpacing() : 0;
			return result;
		}

		/// <summary>Converts a single character string to char code.</summary>
		/// <param name="string">single character string to convert to.</param>
		/// <returns>char code.</returns>
		private int GetCharCode(String @string)
		{
			try
			{
				byte[] b = @string.GetBytes("UTF-16BE");
				int value = 0;
				for (int i = 0; i < b.Length - 1; i++)
				{
					value += b[i] & 0xff;
					value <<= 8;
				}
				if (b.Length > 0)
				{
					value += b[b.Length - 1] & 0xff;
				}
				return value;
			}
			catch (ArgumentException)
			{
			}
			return 0;
		}

		/// <summary>Split PDF string into array of single character PDF strings.</summary>
		/// <param name="string">PDF string to be splitted.</param>
		/// <returns>splitted PDF string.</returns>
		private PdfString[] SplitString(PdfString @string)
		{
			IList<PdfString> strings = new List<PdfString>();
			String stringValue = @string.GetValue();
			for (int i = 0; i < stringValue.Length; i++)
			{
				PdfString newString = new PdfString(stringValue.JSubstring(i, i + 1), @string.GetEncoding
					());
				String text = gs.GetFont().Decode(newString);
				if (text.Length == 0 && i < stringValue.Length - 1)
				{
					newString = new PdfString(stringValue.JSubstring(i, i + 2), @string.GetEncoding()
						);
					i++;
				}
				strings.Add(newString);
			}
			return strings.ToArray(new PdfString[strings.Count]);
		}
	}
}
