/*
$Id$

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
using System.Text;

namespace iTextSharp.IO.Util
{
	public sealed class TextUtil
	{
		private TextUtil()
		{
		}

		/// <summary>
		/// Check if the value of a character belongs to a certain interval
		/// that indicates it's the higher part of a surrogate pair.
		/// </summary>
		/// <param name="c">the character</param>
		/// <returns>true if the character belongs to the interval</returns>
		public static bool IsSurrogateHigh(char c)
		{
			return c >= '\ud800' && c <= '\udbff';
		}

		/// <summary>
		/// Check if the value of a character belongs to a certain interval
		/// that indicates it's the lower part of a surrogate pair.
		/// </summary>
		/// <param name="c">the character</param>
		/// <returns>true if the character belongs to the interval</returns>
		public static bool IsSurrogateLow(char c)
		{
			return c >= '\udc00' && c <= '\udfff';
		}

		/// <summary>
		/// Checks if two subsequent characters in a String are
		/// are the higher and the lower character in a surrogate
		/// pair (and therefore eligible for conversion to a UTF 32 character).
		/// </summary>
		/// <param name="text">the String with the high and low surrogate characters</param>
		/// <param name="idx">the index of the 'high' character in the pair</param>
		/// <returns>true if the characters are surrogate pairs</returns>
		public static bool IsSurrogatePair(String text, int idx)
		{
			return !(idx < 0 || idx > text.Length - 2) && IsSurrogateHigh(text[idx]) && IsSurrogateLow
				(text[idx + 1]);
		}

		/// <summary>
		/// Checks if two subsequent characters in a character array are
		/// are the higher and the lower character in a surrogate
		/// pair (and therefore eligible for conversion to a UTF 32 character).
		/// </summary>
		/// <param name="text">the character array with the high and low surrogate characters
		/// 	</param>
		/// <param name="idx">the index of the 'high' character in the pair</param>
		/// <returns>true if the characters are surrogate pairs</returns>
		public static bool IsSurrogatePair(char[] text, int idx)
		{
			return !(idx < 0 || idx > text.Length - 2) && IsSurrogateHigh(text[idx]) && IsSurrogateLow
				(text[idx + 1]);
		}

		/// <summary>
		/// Returns the code point of a UTF32 character corresponding with
		/// a high and a low surrogate value.
		/// </summary>
		/// <param name="highSurrogate">the high surrogate value</param>
		/// <param name="lowSurrogate">the low surrogate value</param>
		/// <returns>a code point value</returns>
		public static int ConvertToUtf32(char highSurrogate, char lowSurrogate)
		{
			return (highSurrogate - 0xd800) * 0x400 + lowSurrogate - 0xdc00 + 0x10000;
		}

		/// <summary>Converts a unicode character in a character array to a UTF 32 code point value.
		/// 	</summary>
		/// <param name="text">a character array that has the unicode character(s)</param>
		/// <param name="idx">the index of the 'high' character</param>
		/// <returns>the code point value</returns>
		public static int ConvertToUtf32(char[] text, int idx)
		{
			return (text[idx] - 0xd800) * 0x400 + text[idx + 1] - 0xdc00 + 0x10000;
		}

		/// <summary>Converts a unicode character in a String to a UTF32 code point value</summary>
		/// <param name="text">a String that has the unicode character(s)</param>
		/// <param name="idx">the index of the 'high' character</param>
		/// <returns>the codepoint value</returns>
		public static int ConvertToUtf32(String text, int idx)
		{
			return (text[idx] - 0xd800) * 0x400 + text[idx + 1] - 0xdc00 + 0x10000;
		}

		public static int[] ConvertToUtf32(String text)
		{
			if (text == null)
			{
				return null;
			}
			IList<int> charCodes = new List<int>(text.Length);
			int pos = 0;
			while (pos < text.Length)
			{
				if (IsSurrogatePair(text, pos))
				{
					charCodes.Add(ConvertToUtf32(text, pos));
					pos += 2;
				}
				else
				{
					charCodes.Add((int)text[pos]);
					pos++;
				}
			}
			return ArrayUtil.ToArray(charCodes);
		}

		/// <summary>Converts a UTF32 code point value to a String with the corresponding character(s).
		/// 	</summary>
		/// <param name="codePoint">a Unicode value</param>
		/// <returns>the corresponding characters in a String</returns>
		public static char[] ConvertFromUtf32(int codePoint)
		{
			if (codePoint < 0x10000)
			{
				return new char[] { (char)codePoint };
			}
			codePoint -= 0x10000;
			return new char[] { (char)(codePoint / 0x400 + 0xd800), (char)(codePoint % 0x400 
				+ 0xdc00) };
		}

		/// <summary>
		/// /
		/// Converts a UTF32 code point sequence to a String with the corresponding character(s).
		/// </summary>
		/// <param name="text">a Unicode text sequence</param>
		/// <param name="startPos">start position of text to convert, inclusive</param>
		/// <param name="endPos">end position of txt to convert, exclusive</param>
		/// <returns>the corresponding characters in a String</returns>
		public static String ConvertFromUtf32(int[] text, int startPos, int endPos)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = startPos; i < endPos; i++)
			{
				sb.Append(ConvertFromUtf32ToCharArray(text[i]));
			}
			return sb.ToString();
		}

		/// <summary>Converts a UTF32 code point value to a char array with the corresponding character(s).
		/// 	</summary>
		/// <param name="codePoint">a Unicode value</param>
		/// <returns>the corresponding characters in a char arrat</returns>
		public static char[] ConvertFromUtf32ToCharArray(int codePoint)
		{
			if (codePoint < 0x10000)
			{
				return new char[] { (char)codePoint };
			}
			codePoint -= 0x10000;
			return new char[] { (char)(codePoint / 0x400 + 0xd800), (char)(codePoint % 0x400 
				+ 0xdc00) };
		}
	}
}
