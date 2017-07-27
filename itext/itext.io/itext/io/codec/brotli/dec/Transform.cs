/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
namespace iText.IO.Codec.Brotli.Dec
{
	/// <summary>Transformations on dictionary words.</summary>
	internal sealed class Transform
	{
		private readonly byte[] prefix;

		private readonly int type;

		private readonly byte[] suffix;

		internal Transform(string prefix, int type, string suffix)
		{
			this.prefix = ReadUniBytes(prefix);
			this.type = type;
			this.suffix = ReadUniBytes(suffix);
		}

		internal static byte[] ReadUniBytes(string uniBytes)
		{
			byte[] result = new byte[uniBytes.Length];
			for (int i = 0; i < result.Length; ++i)
			{
				result[i] = unchecked((byte)uniBytes[i]);
			}
			return result;
		}

		internal static readonly iText.IO.Codec.Brotli.Dec.Transform[] Transforms = new iText.IO.Codec.Brotli.Dec.Transform[] { new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, 
			iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitFirst1, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " the "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity
			, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform("s ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " of "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.UppercaseFirst, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " and "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitFirst2, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast1, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(", ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity
			, ", "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " in "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.Identity, " to "), new iText.IO.Codec.Brotli.Dec.Transform("e ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "\""), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, 
			iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "."), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "\">"), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "\n"), new 
			iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast3, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "]"), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.Identity, " for "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitFirst3, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast2, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " a "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " that "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst
			, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, ". "), new iText.IO.Codec.Brotli.Dec.Transform(".", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType
			.Identity, ", "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitFirst4, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " with "), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "'"), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " from "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity
			, " by "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitFirst5, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitFirst6, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform
			(" the ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast4, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.Identity, ". The "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " on "), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " as "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " is "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast7
			, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast1, "ing "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "\n\t"), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty
			, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, ":"), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, ". "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "ed "), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitFirst9, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitFirst7, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.OmitLast6, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "("), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, ", "), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast8, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " at "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.Identity, "ly "), new iText.IO.Codec.Brotli.Dec.Transform(" the ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " of "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast5, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(
			string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.OmitLast9, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, ", "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst
			, "\""), new iText.IO.Codec.Brotli.Dec.Transform(".", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "("), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.UppercaseFirst, "\">"), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "=\""), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "."), new iText.IO.Codec.Brotli.Dec.Transform(".com/", 
			iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(" the ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " of the "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst
			, "'"), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, ". This "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, ","), new iText.IO.Codec.Brotli.Dec.Transform(".", iText.IO.Codec.Brotli.Dec.WordTransformType
			.Identity, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, "("), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, "."), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, " not "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "=\""), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "er "
			), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, " "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "al "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType
			.UppercaseAll, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "='"), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "\""), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, ". "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "("), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, 
			"ful "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, ". "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "ive "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.Identity, "less "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "'"), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "est "), new iText.IO.Codec.Brotli.Dec.Transform
			(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, "."), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "\">"), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "='"
			), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, ","), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, "ize "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType
			.UppercaseAll, "."), new iText.IO.Codec.Brotli.Dec.Transform("\u00c2\u00a0", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, string.Empty), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.Identity, ","), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty
			, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, "=\""), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "=\""), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.Identity
			, "ous "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, ", "), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, "='"), new iText.IO.Codec.Brotli.Dec.Transform(" ", 
			iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, ","), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "=\""), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, ", "), new iText.IO.Codec.Brotli.Dec.Transform
			(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, ","), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "("), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.
			UppercaseAll, ". "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "."), new iText.IO.Codec.Brotli.Dec.Transform(string.Empty, iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "='"), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType
			.UppercaseAll, ". "), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst, "=\""), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll, "='"), new iText.IO.Codec.Brotli.Dec.Transform(" ", iText.IO.Codec.Brotli.Dec.WordTransformType
			.UppercaseFirst, "='") };

		internal static int TransformDictionaryWord(byte[] dst, int dstOffset, byte[] word, int wordOffset, int len, iText.IO.Codec.Brotli.Dec.Transform transform)
		{
			int offset = dstOffset;
			// Copy prefix.
			byte[] @string = transform.prefix;
			int tmp = @string.Length;
			int i = 0;
			// In most cases tmp < 10 -> no benefits from System.arrayCopy
			while (i < tmp)
			{
				dst[offset++] = @string[i++];
			}
			// Copy trimmed word.
			int op = transform.type;
			tmp = iText.IO.Codec.Brotli.Dec.WordTransformType.GetOmitFirst(op);
			if (tmp > len)
			{
				tmp = len;
			}
			wordOffset += tmp;
			len -= tmp;
			len -= iText.IO.Codec.Brotli.Dec.WordTransformType.GetOmitLast(op);
			i = len;
			while (i > 0)
			{
				dst[offset++] = word[wordOffset++];
				i--;
			}
			if (op == iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseAll || op == iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst)
			{
				int uppercaseOffset = offset - len;
				if (op == iText.IO.Codec.Brotli.Dec.WordTransformType.UppercaseFirst)
				{
					len = 1;
				}
				while (len > 0)
				{
					tmp = dst[uppercaseOffset] & unchecked((int)(0xFF));
					if (tmp < unchecked((int)(0xc0)))
					{
						if (tmp >= 'a' && tmp <= 'z')
						{
							dst[uppercaseOffset] ^= unchecked((byte)32);
						}
						uppercaseOffset += 1;
						len -= 1;
					}
					else if (tmp < unchecked((int)(0xe0)))
					{
						dst[uppercaseOffset + 1] ^= unchecked((byte)32);
						uppercaseOffset += 2;
						len -= 2;
					}
					else
					{
						dst[uppercaseOffset + 2] ^= unchecked((byte)5);
						uppercaseOffset += 3;
						len -= 3;
					}
				}
			}
			// Copy suffix.
			@string = transform.suffix;
			tmp = @string.Length;
			i = 0;
			while (i < tmp)
			{
				dst[offset++] = @string[i++];
			}
			return offset - dstOffset;
		}
	}
}
