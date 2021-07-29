/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
