//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;

namespace com.itextpdf.kernel.xmp.impl
{
	/// <summary>
	/// A utility class to perform base64 encoding and decoding as specified
	/// in RFC-1521.
	/// </summary>
	/// <remarks>
	/// A utility class to perform base64 encoding and decoding as specified
	/// in RFC-1521. See also RFC 1421.
	/// </remarks>
	/// <version>$Revision: 1.4 $</version>
	public class Base64
	{
		/// <summary>marker for invalid bytes</summary>
		private const byte INVALID = -1;

		/// <summary>marker for accepted whitespace bytes</summary>
		private const byte WHITESPACE = -2;

		/// <summary>marker for an equal symbol</summary>
		private const byte EQUAL = -3;

		private static byte[] base64 = new byte[] { unchecked((byte)(byte)('A')), unchecked(
			(byte)(byte)('B')), unchecked((byte)(byte)('C')), unchecked((byte)(byte)('D')), 
			unchecked((byte)(byte)('E')), unchecked((byte)(byte)('F')), unchecked((byte)(byte
			)('G')), unchecked((byte)(byte)('H')), unchecked((byte)(byte)('I')), unchecked((
			byte)(byte)('J')), unchecked((byte)(byte)('K')), unchecked((byte)(byte)('L')), unchecked(
			(byte)(byte)('M')), unchecked((byte)(byte)('N')), unchecked((byte)(byte)('O')), 
			unchecked((byte)(byte)('P')), unchecked((byte)(byte)('Q')), unchecked((byte)(byte
			)('R')), unchecked((byte)(byte)('S')), unchecked((byte)(byte)('T')), unchecked((
			byte)(byte)('U')), unchecked((byte)(byte)('V')), unchecked((byte)(byte)('W')), unchecked(
			(byte)(byte)('X')), unchecked((byte)(byte)('Y')), unchecked((byte)(byte)('Z')), 
			unchecked((byte)(byte)('a')), unchecked((byte)(byte)('b')), unchecked((byte)(byte
			)('c')), unchecked((byte)(byte)('d')), unchecked((byte)(byte)('e')), unchecked((
			byte)(byte)('f')), unchecked((byte)(byte)('g')), unchecked((byte)(byte)('h')), unchecked(
			(byte)(byte)('i')), unchecked((byte)(byte)('j')), unchecked((byte)(byte)('k')), 
			unchecked((byte)(byte)('l')), unchecked((byte)(byte)('m')), unchecked((byte)(byte
			)('n')), unchecked((byte)(byte)('o')), unchecked((byte)(byte)('p')), unchecked((
			byte)(byte)('q')), unchecked((byte)(byte)('r')), unchecked((byte)(byte)('s')), unchecked(
			(byte)(byte)('t')), unchecked((byte)(byte)('u')), unchecked((byte)(byte)('v')), 
			unchecked((byte)(byte)('w')), unchecked((byte)(byte)('x')), unchecked((byte)(byte
			)('y')), unchecked((byte)(byte)('z')), unchecked((byte)(byte)('0')), unchecked((
			byte)(byte)('1')), unchecked((byte)(byte)('2')), unchecked((byte)(byte)('3')), unchecked(
			(byte)(byte)('4')), unchecked((byte)(byte)('5')), unchecked((byte)(byte)('6')), 
			unchecked((byte)(byte)('7')), unchecked((byte)(byte)('8')), unchecked((byte)(byte
			)('9')), unchecked((byte)(byte)('+')), unchecked((byte)(byte)('/')) };

		private static byte[] ascii = new byte[255];

		static Base64()
		{
			//  0 to  3
			//  4 to  7
			//  8 to 11
			// 11 to 15
			// 16 to 19
			// 20 to 23
			// 24 to 27
			// 28 to 31
			// 32 to 35
			// 36 to 39
			// 40 to 43
			// 44 to 47
			// 48 to 51
			// 52 to 55
			// 56 to 59
			// 60 to 63
			// not valid bytes
			for (int idx = 0; idx < 255; idx++)
			{
				ascii[idx] = INVALID;
			}
			// valid bytes
			for (int idx_1 = 0; idx_1 < base64.Length; idx_1++)
			{
				ascii[base64[idx_1]] = unchecked((byte)idx_1);
			}
			// whitespaces
			ascii[0x09] = WHITESPACE;
			ascii[0x0A] = WHITESPACE;
			ascii[0x0D] = WHITESPACE;
			ascii[0x20] = WHITESPACE;
			// trailing equals
			ascii[0x3d] = EQUAL;
		}

		/// <summary>Encode the given byte[].</summary>
		/// <param name="src">the source string.</param>
		/// <returns>the base64-encoded data.</returns>
		public static byte[] Encode(byte[] src)
		{
			return Encode(src, 0);
		}

		/// <summary>Encode the given byte[].</summary>
		/// <param name="src">the source string.</param>
		/// <param name="lineFeed">
		/// a linefeed is added after <code>linefeed</code> characters;
		/// must be dividable by four; 0 means no linefeeds
		/// </param>
		/// <returns>the base64-encoded data.</returns>
		public static byte[] Encode(byte[] src, int lineFeed)
		{
			// linefeed must be dividable by 4
			lineFeed = lineFeed / 4 * 4;
			if (lineFeed < 0)
			{
				lineFeed = 0;
			}
			// determine code length
			int codeLength = ((src.Length + 2) / 3) * 4;
			if (lineFeed > 0)
			{
				codeLength += (codeLength - 1) / lineFeed;
			}
			byte[] dst = new byte[codeLength];
			int bits24;
			int bits6;
			//
			// Do 3-byte to 4-byte conversion + 0-63 to ascii printable conversion
			//
			int didx = 0;
			int sidx = 0;
			int lf = 0;
			while (sidx + 3 <= src.Length)
			{
				bits24 = (src[sidx++] & 0xFF) << 16;
				bits24 |= (src[sidx++] & 0xFF) << 8;
				bits24 |= (src[sidx++] & 0xFF) << 0;
				bits6 = (bits24 & 0x00FC0000) >> 18;
				dst[didx++] = base64[bits6];
				bits6 = (bits24 & 0x0003F000) >> 12;
				dst[didx++] = base64[bits6];
				bits6 = (bits24 & 0x00000FC0) >> 6;
				dst[didx++] = base64[bits6];
				bits6 = (bits24 & 0x0000003F);
				dst[didx++] = base64[bits6];
				lf += 4;
				if (didx < codeLength && lineFeed > 0 && lf % lineFeed == 0)
				{
					dst[didx++] = 0x0A;
				}
			}
			if (src.Length - sidx == 2)
			{
				bits24 = (src[sidx] & 0xFF) << 16;
				bits24 |= (src[sidx + 1] & 0xFF) << 8;
				bits6 = (bits24 & 0x00FC0000) >> 18;
				dst[didx++] = base64[bits6];
				bits6 = (bits24 & 0x0003F000) >> 12;
				dst[didx++] = base64[bits6];
				bits6 = (bits24 & 0x00000FC0) >> 6;
				dst[didx++] = base64[bits6];
				dst[didx++] = unchecked((byte)(byte)('='));
			}
			else
			{
				if (src.Length - sidx == 1)
				{
					bits24 = (src[sidx] & 0xFF) << 16;
					bits6 = (bits24 & 0x00FC0000) >> 18;
					dst[didx++] = base64[bits6];
					bits6 = (bits24 & 0x0003F000) >> 12;
					dst[didx++] = base64[bits6];
					dst[didx++] = unchecked((byte)(byte)('='));
					dst[didx++] = unchecked((byte)(byte)('='));
				}
			}
			return dst;
		}

		/// <summary>Encode the given string.</summary>
		/// <param name="src">the source string.</param>
		/// <returns>the base64-encoded string.</returns>
		public static String Encode(String src)
		{
			return com.itextpdf.io.util.JavaUtil.GetStringForBytes(Encode(src.GetBytes()));
		}

		/// <summary>Decode the given byte[].</summary>
		/// <param name="src">the base64-encoded data.</param>
		/// <returns>the decoded data.</returns>
		/// <exception cref="System.ArgumentException">
		/// Thrown if the base 64 strings contains non-valid characters,
		/// beside the bas64 chars, LF, CR, tab and space are accepted.
		/// </exception>
		public static byte[] Decode(byte[] src)
		{
			//
			// Do ascii printable to 0-63 conversion.
			//
			int sidx;
			int srcLen = 0;
			for (sidx = 0; sidx < src.Length; sidx++)
			{
				byte val = ascii[src[sidx]];
				if (val >= 0)
				{
					src[srcLen++] = val;
				}
				else
				{
					if (val == INVALID)
					{
						throw new ArgumentException("Invalid base 64 string");
					}
				}
			}
			//
			// Trim any padding.
			//
			while (srcLen > 0 && src[srcLen - 1] == EQUAL)
			{
				srcLen--;
			}
			byte[] dst = new byte[srcLen * 3 / 4];
			//
			// Do 4-byte to 3-byte conversion.
			//
			int didx;
			for (sidx = 0, didx = 0; didx < dst.Length - 2; sidx += 4, didx += 3)
			{
				dst[didx] = unchecked((byte)(((src[sidx] << 2) & 0xFF) | ((src[sidx + 1] >> 4) & 
					0x03)));
				dst[didx + 1] = unchecked((byte)(((src[sidx + 1] << 4) & 0xFF) | ((src[sidx + 2] 
					>> 2) & 0x0F)));
				dst[didx + 2] = unchecked((byte)(((src[sidx + 2] << 6) & 0xFF) | ((src[sidx + 3])
					 & 0x3F)));
			}
			if (didx < dst.Length)
			{
				dst[didx] = unchecked((byte)(((src[sidx] << 2) & 0xFF) | ((src[sidx + 1] >> 4) & 
					0x03)));
			}
			if (++didx < dst.Length)
			{
				dst[didx] = unchecked((byte)(((src[sidx + 1] << 4) & 0xFF) | ((src[sidx + 2] >> 2
					) & 0x0F)));
			}
			return dst;
		}

		/// <summary>Decode the given string.</summary>
		/// <param name="src">the base64-encoded string.</param>
		/// <returns>the decoded string.</returns>
		public static String Decode(String src)
		{
			return com.itextpdf.io.util.JavaUtil.GetStringForBytes(Decode(src.GetBytes()));
		}
	}
}
