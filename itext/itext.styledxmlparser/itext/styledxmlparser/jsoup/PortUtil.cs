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
