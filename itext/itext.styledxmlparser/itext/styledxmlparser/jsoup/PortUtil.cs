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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iText.IO.Util;

namespace iText.StyledXmlParser.Jsoup {
    internal static class PortUtil {
        public static readonly string EscapedSingleBracket = "'";
        public static readonly string SignedNumberFormat = ":+0;-#";

        public const int CHARACTER_MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;

        public static FileStream GetReadOnlyRandomAccesFile(FileInfo file) {
            return file.Open(FileMode.Open, FileAccess.Read);
        }

        [System.ObsoleteAttribute(@"use iText.IO.Util.Matcher.Find")]
        public static bool HasMatch(Regex pattern, String input) {

            return pattern.IsMatch(input);
        }

        public static char[] ToChars(int codePoint) {
            return char.ConvertFromUtf32(codePoint).ToCharArray();
        }

        public static int CharCount(int codePoint) {
            return codePoint >= CHARACTER_MIN_SUPPLEMENTARY_CODE_POINT ? 2 : 1;
        }

        public static Encoding NewEncoder(Encoding charset) {
            return charset;
        }

        public static bool CharsetIsSupported(string charset) {
            try {
                var enc = EncodingUtil.GetEncoding(charset);
                return true;
            } catch (ArgumentException) {
                return false;
            }
        }

        [System.ObsoleteAttribute(@"use iText.IO.Util.Matcher.Find")]
        public static bool IsSuccessful(Match m) {
            return m.Success;
        }
    }

    public class IdentityComparaor<T> : EqualityComparer<T>
    {
        public override bool Equals(T x, T y)
        {
            return GetHashCode(x) == GetHashCode(y) && object.ReferenceEquals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return Default.GetHashCode(obj);
        }
    }

    public class IdentityDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {

        public IdentityDictionary() :
            base(new IdentityComparaor<TKey>())
        {
        }

        public IdentityDictionary(Int32 capasity) :
            base(capasity, new IdentityComparaor<TKey>())
        {
        }

        public IdentityDictionary(IDictionary<TKey, TValue> dictionary) :
            base(dictionary, new IdentityComparaor<TKey>())
        {
        }
    }
}
