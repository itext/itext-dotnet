using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Commons.Exceptions;

namespace iText.IO.Font.Cmap
{
    public sealed class CMapCharsetEncoder
    {
        private const char MIN_BMP_VALUE = '\u0000';
        private const char MAX_BMP_VALUE = '\uFFFF';

        private readonly Encoding targetCharset;

        private readonly bool bmpOnly;

        public CMapCharsetEncoder(Encoding targetCharset) : this(targetCharset, false)
        {
            // Empty constructor
        }

        public CMapCharsetEncoder(Encoding targetCharset, bool bmpOnly)
        {
            this.targetCharset = targetCharset;
            this.bmpOnly = bmpOnly;
        }

        public byte[] EncodeUnicodeCodePoint(int cp)
        {
            if (!isBmpCodePoint(cp) && bmpOnly)
            {
                throw new ITextException("This encoder only accepts BMP codepoints");
            }

            String s = Char.ConvertFromUtf32(cp);

            return targetCharset.GetBytes(s);
        }

        private static bool isBmpCodePoint(int codePoint)
        {
            return codePoint >= MIN_BMP_VALUE && codePoint <= MAX_BMP_VALUE;
        }
    }
}
