/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
