/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.IO;
using System.Linq;
using System.Text;

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class JavaUtil {
        public static String GetStringForChars(char[] chars) {
            return new String(chars);
        }

        public static String GetStringForChars(char[] chars, int offset, int length) {
            return new String(chars, offset, length);
        }

        public static String GetStringForBytes(byte[] bytes, int offset, int length) {
            return Encoding.UTF8.GetString(bytes, offset, length);
        }

        public static String GetStringForBytes(byte[] bytes, int offset, int length, String encoding) {
            return EncodingUtil.GetEncoding(encoding).GetString(bytes, offset, length);
        }
        
        public static String GetStringForBytes(byte[] bytes, int offset, int length, Encoding encoding) {
            return encoding.GetString(bytes, offset, length);
        }

        public static String GetStringForBytes(byte[] bytes, String encoding) {
            return GetStringForBytes(bytes, 0, bytes.Length, encoding);
        }

        public static String GetStringForBytes(byte[] bytes, Encoding encoding) {
            return encoding.GetString(bytes);
        }

        public static String GetStringForBytes(byte[] bytes) {
            return GetStringForBytes(bytes, 0, bytes.Length);
        }

        public static int FloatToIntBits(float value) {
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static long DoubleToLongBits(double value) {
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static float IntBitsToFloat(int bits) {
            byte[] bytes = BitConverter.GetBytes(bits);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double LongBitsToDouble(long bits) {
            byte[] bytes = BitConverter.GetBytes(bits);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static String IntegerToHexString(int i) {
            return Convert.ToString(i, 16);
        }

        public static String IntegerToOctalString(int i) {
            return Convert.ToString(i, 8);
        }

        public static bool DictionariesEquals<TKey, TValue>(IDictionary<TKey, TValue> that, IDictionary<TKey, TValue> other) {
            if (other == that)
                return true;
            if (that == null || other == null)
                return false;

            return !that.Except(other).Any();
        }

        public static int DictionaryHashCode<TKey, TValue>(IDictionary<TKey, TValue> dict) {
            int result = 0;
            if (dict != null) {
                foreach (KeyValuePair<TKey, TValue> entry in dict) {
                    result += entry.GetHashCode();
                }
            }
            return result;
        }

        public static bool SetEquals<T>(ISet<T> that, ISet<T> other) {
            if (other == that)
                return true;
            if (that == null || other == null)
                return false;

            return that.SetEquals(other);
        }

        public static int SetHashCode<T>(ISet<T> set) {
            int result = 0;
            if (set != null) {
                foreach (T value in set) {
                    result += value.GetHashCode();
                }
            }
            return result;
        }

        public static bool ArraysEquals<T>(T[] a, T[] a2) {
            if (a == a2)
                return true;
            if (a == null || a2 == null)
                return false;

            if (a.Length != a2.Length) 
                return false;

            for (int i = 0; i < a.Length; i++)
                if (!(a[i] == null ? a2[i] == null : a[i].Equals(a2[i])))
                    return false;

            return true;
        }

        public static int ArraysHashCode<T>(params T[] a) {
            if (a == null)
                return 0;
            int result = 1;
            foreach (T element in a) {
                result = 31*result + (element == null ? 0 : element.GetHashCode());
            }
            return result;
        }

        public static String ArraysToString<T>(T[] a) {
            if (a == null)
                return "null";
            if (a.Length == 0)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0;; i++) {
                b.Append(a[i]);
                if (i == a.Length - 1)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        public static IEnumerable<T> ArraysToEnumerable<T>(T[] a) {
            return a;
        }

        public static bool IsValidCodePoint(int codePoint) {
            // see http://www.unicode.org/glossary/#code_point
            return codePoint >= 0 && codePoint <= 0x10FFFF;
        }

        public static readonly int MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;
        public static readonly char MIN_HIGH_SURROGATE = '\uD800';
        public static readonly char MIN_LOW_SURROGATE = '\uDC00';

        public static int ToCodePoint(char high, char low) {
            // Optimized form of:
            // return ((high - MIN_HIGH_SURROGATE) << 10)
            //         + (low - MIN_LOW_SURROGATE)
            //         + MIN_SUPPLEMENTARY_CODE_POINT;
            return ((high << 10) + low) + (MIN_SUPPLEMENTARY_CODE_POINT
                                           - (MIN_HIGH_SURROGATE << 10)
                                           - MIN_LOW_SURROGATE);
        }

        public static IList<T> ArraysAsList<T>(params T[] a) {
            return new List<T>(a);
        }

        public static int ArraysBinarySearch<T>(T[] a, T key) {
            return Array.BinarySearch(a, key);
        }

        public static String IntegerToString(int i) {
            return i.ToString();
        }

        public static double Random() {
            return new Random().NextDouble();
        }

        public static void Fill(byte[] a, byte val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Fill(char[] a, char val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Fill(bool[] a, bool val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Fill(short[] a, short val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Fill(int[] a, int val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Fill(long[] a, long val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Fill(float[] a, float val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Fill(double[] a, double val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Fill(object[] a, object val) {
            for (int i = 0, len = a.Length; i < len; i++) {
                a[i] = val;
            }
        }

        public static void Sort<T>(T[] array) {
            Sort(array, null);
        }

        public static void Sort<T>(T[] array, IComparer<T> comparer) {
            Sort(array, 0, array.Length, comparer);
        }

        public static void Sort<T>(T[] array, int from, int to) {
            Sort(array, from, to, null);
        }

        public static void Sort<T>(T[] array, int from, int to, IComparer<T> comparer) {
            SortUtil.MergeSort(array, from, to, comparer);
        }

        public static void Sort(String[] array) {
            Sort(array, null);
        }

        public static void Sort(String[] array, IComparer<String> comparer) {
            SortUtil.MergeSort(array, comparer);
        }

        public static int IntegerCompare(int a, int b) {
            return a.CompareTo(b);
        }

        public static int FloatCompare(float a, float b) {
            return a.CompareTo(b);
        }

        public static int DoubleCompare(double a, double b) {
            return a.CompareTo(b);
        }

        public static T[] ArraysCopyOf<T>(T[] original, int newLength) {
            T[] copy = new T[newLength];
            System.Array.Copy(original, 0, copy, 0, Math.Min(original.Length, newLength));
            return copy;
        }

        public static T[] ArraysCopyOfRange<T>(T[] original, int from, int to) {
            int newLength = to - from;
            if (newLength < 0)
                throw new ArgumentException(from + " > " + to);
            T[] copy = new T[newLength];
            System.Array.Copy(original, from, copy, 0, Math.Min(original.Length - from, newLength));
            return copy;
        }

        public static Stream CorrectWavFile(Stream stream) {
            String header = "";
            for (int i = 0; i < 4; i++) {
                header = header + (char) stream.Read();
            }
            stream.Position = 0;
            if (header.Equals("RIFF")) {
                stream.Read();
            }

            return stream;
        }

        public static int CharacterDigit(char ch, int radix) {
            return Convert.ToInt32(new String(new[] {ch}), radix);
        }

        public static String CharToString(char ch) {
            return ch.ToString();
        }
    }
}
