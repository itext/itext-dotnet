using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto;

namespace iTextSharp.Kernel {
    internal static class KernelExtensions {
        public static String JSubstring(this String str, int beginIndex, int endIndex) {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static String JSubstring(this StringBuilder sb, int beginIndex, int endIndex) {
            return sb.ToString(beginIndex, endIndex - beginIndex);
        }

        public static bool EqualsIgnoreCase(this String str, String anotherString)
        {
            return String.Equals(str, anotherString, StringComparison.OrdinalIgnoreCase);
        }

        public static void JReset(this MemoryStream stream) {
            stream.Position = 0;
        }

        public static void Write(this Stream stream, int value) {
            stream.WriteByte((byte)value);
        }

        public static int Read(this Stream stream) {
            return stream.ReadByte();
        }

        public static int Read(this Stream stream, byte[] buffer) {
            int size = stream.Read(buffer, 0, buffer.Length);
            return size == 0 ? -1 : size;
        }

        public static int JRead(this Stream stream, byte[] buffer, int offset, int count)
        {
            int result = stream.Read(buffer, offset, count);
            return result == 0 ? -1 : result;
        }

        public static void Write(this Stream stream, byte[] buffer) {
            stream.Write(buffer, 0, buffer.Length);
        }

        public static byte[] GetBytes(this String str) {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static byte[] GetBytes(this String str, String encoding) {
            return Encoding.GetEncoding(encoding).GetBytes(str);
        }

        public static long Seek(this FileStream fs, long offset) {
            return fs.Seek(offset, SeekOrigin.Begin);
        }

        public static long Skip(this Stream s, long n) {
            s.Seek(n, SeekOrigin.Current);
            return n;
        }

        public static List<T> SubList<T>(this IList<T> list, int fromIndex, int toIndex) {
            return ((List<T>)list).GetRange(fromIndex, toIndex - fromIndex);
        }

        public static void AddAll<T>(this IList<T> list, IEnumerable<T> c) {
            ((List<T>)list).AddRange(c);
        }

        public static void AddAll<T>(this IList<T> list, int index, IList<T> c) {
            for (int i = c.Count - 1; i >= 0; i--) {
                list.Insert(index, c[i]);
            }
        }

        public static void Add<T>(this IList<T> list, int index, T elem) {
            list.Insert(index, elem);
        }

        public static void AddAll<T>(this ICollection<T> c, IEnumerable<T> collectionToAdd)
        {
            foreach (T o in collectionToAdd)
            {
                c.Add(o);
            }
        }

        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> c, IDictionary<TKey, TValue> collectionToAdd)
        {
            foreach (KeyValuePair<TKey, TValue> pair in collectionToAdd) {
                c[pair.Key] = pair.Value;
            }
        }

        public static void GetChars(this StringBuilder sb, int srcBegin, int srcEnd, char[] dst, int dstBegin) {
            sb.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
        }

        public static String[] Split(this String str, String regex) {
            return str.Split(str.ToCharArray());
        }

        public static bool Matches(this String str, String regex) {
            return Regex.IsMatch(str, regex);
        }

        public static T[] ToArray<T>(this ICollection<T> col, T[] toArray)
        {
            T[] r = col.ToArray();
            return r;
        }

        public static void ReadFully(this BinaryReader input, byte[] b, int off, int len) {
            if (len < 0)
            {
                throw new IndexOutOfRangeException();
            }
            int n = 0;
            while (n < len)
            {
                int count = input.Read(b, off + n, len - n);
                if (count < 0)
                {
                    throw new EndOfStreamException();
                }
                n += count;
            }
        }

        public static TValue JRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            dictionary.TryGetValue(key, out value);
            dictionary.Remove(key);

            return value;
        }

        public static T JRemoveAt<T>(this IList<T> list, int index)
        {
            T value = list[index];
            list.RemoveAt(index);

            return value;
        }

        public static T PollFirst<T>(this SortedSet<T> set) {
            T item = set.First();
            set.Remove(item);

            return item;
        }

		public static bool IsEmpty<T>(this ICollection<T> collection) {
			return collection.Count == 0;
		}

		public static bool IsEmpty(this ICollection collection) {
			return collection.Count == 0;
		}

		public static void SetCharAt(this StringBuilder builder, int index, char ch) {
			builder[index] = ch;
		}

        public static float NextFloat(this Random random) {
            double mantissa = random.NextDouble();
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            if (mantissa < 0 || exponent < 0) {
                int a = 5;
            }
            float val = (float) (mantissa*exponent);
            if (val < 0) {
                int b = 6;
            }
            return (float)(mantissa * exponent);
        }

        public static bool NextBoolean(this Random random) {
            return random.NextDouble() > 0.5;
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key) {
            TValue value = default(TValue);
            if (key != null)
            {
                col.TryGetValue(key, out value);
            }

            return value;
        }

        public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            return dictionary.ContainsKey(key);
        }

        public static void Update(this IDigest dgst, byte[] input)
        {
            dgst.Update(input, 0, input.Length);
        }

        public static void Update(this IDigest dgst, byte[] input, int offset, int len)
        {
            dgst.BlockUpdate(input, offset, len);
        }

        public static byte[] Digest(this IDigest dgst)
        {
            byte[] output = new byte[dgst.GetDigestSize()];
            dgst.DoFinal(output, 0);
            return output;
        }

        public static byte[] Digest(this IDigest dgst, byte[] input)
        {
            dgst.Update(input);
            return dgst.Digest();
        }

        /// <summary>
        /// IMPORTANT: USE THIS METHOD CAREFULLY.
        /// This method serves as replacement for the java method MessageDigest#digest(byte[] buf, int offset, int len).
        /// However for now, we simply omit len parameter, because it doesn't affect anything for all current usages 
        /// (there are two of them at the moment of the method addition which are in StandardHandlerUsingAes256 class).
        /// This may be not true for future possible usages, so be aware.
        /// </summary>
        public static void Digest(this IDigest dgst, byte[] buff, int offest, int len)
        {
            dgst.DoFinal(buff, offest);
        }
    }
}
