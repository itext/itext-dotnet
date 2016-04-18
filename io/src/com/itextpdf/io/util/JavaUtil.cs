using System;
using System.Collections.Generic;
using System.Text;

namespace com.itextpdf.io.util
{
    public static class JavaUtil
    {
        public static String GetStringForBytes(byte[] bytes, int offset, int length)
        {
            return Encoding.UTF8.GetString(bytes, offset, length);
        }

        public static String GetStringForBytes(byte[] bytes, int offset, int length, String encoding)
        {
            return Encoding.GetEncoding(encoding).GetString(bytes, offset, length);
        }

        public static String GetStringForBytes(byte[] bytes, String encoding)
        {
            return GetStringForBytes(bytes, 0, bytes.Length, encoding);
        }

        public static String GetStringForBytes(byte[] bytes)
        {
            return GetStringForBytes(bytes, 0, bytes.Length);
        }

        public static int FloatToIntBits(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static long DoubleToLongBits(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static float IntBitsToFloat(int bits)
        {
            byte[] bytes = BitConverter.GetBytes(bits);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double LongBitsToDouble(long bits)
        {
            byte[] bytes = BitConverter.GetBytes(bits);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static String IntegerToHexString(int i)
        {
            return Convert.ToString(i, 16);
        }

        public static String IntegerToOctalString(int i)
        {
            return Convert.ToString(i, 8);
        }

        public static bool ArraysEquals<T>(T[] a, T[] a2) where T : IComparable
        {
            if (a == a2)
                return true;
            if (a == null || a2 == null)
                return false;

            if (a.Length != a2.Length) return false;

            for (int i = 0; i < a.Length; i++)
                if (a[i].CompareTo(a2) != 0)
                    return false;

            return true;
        }

        public static int ArraysHashCode<T>(T[] a)
        {
            if (a == null)
                return 0;
            int result = 1;
            foreach (T element in a)
            {
                result = 31*result + element.GetHashCode();
            }
            return result;
        }

        public static String ArraysToString(char[] a)
        {
            if (a == null)
                return "null";
            if (a.Length == 0)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++)
            {
                b.Append(a[i]);
                if (i == a.Length - 1)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        public static bool IsValidCodePoint(int codePoint)
        {
            // see http://www.unicode.org/glossary/#code_point
            return codePoint >= 0 && codePoint <= 0x10FFFF;
        }

        public static readonly int MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;
        public static readonly char MIN_HIGH_SURROGATE = '\uD800';
        public static readonly char MIN_LOW_SURROGATE  = '\uDC00';

        public static int ToCodePoint(char high, char low)
        {
            // Optimized form of:
            // return ((high - MIN_HIGH_SURROGATE) << 10)
            //         + (low - MIN_LOW_SURROGATE)
            //         + MIN_SUPPLEMENTARY_CODE_POINT;
            return ((high << 10) + low) + (MIN_SUPPLEMENTARY_CODE_POINT
                                           - (MIN_HIGH_SURROGATE << 10)
                                           - MIN_LOW_SURROGATE);
        }
        
        public static IList<T> ArraysAsList<T>(params T[] a)
        {
            return new List<T>(a);
        }

        public static int ArraysBinarySearch<T>(T[] a, T key)
        {
            return Array.BinarySearch(a, key);
        }

        public static String IntegerToString(int i)
        {
            return i.ToString();
        }

        public static void CollectionsAddAll<T>(IList<T> c, params T[] elements)
        {
            foreach (T element in elements)
            {
                c.Add(element);
            }
        }

        public static double Random()
        {
            return new Random().NextDouble();
        }
    }
}