using System;
using System.Collections.Generic;
using System.IO;

namespace iTextSharp.Pdfa {
    internal static class PdfAExtensions {

        public static void AddAll<T>(this IList<T> list, IEnumerable<T> c) {
            ((List<T>)list).AddRange(c);
        }

        public static byte[] GetBytes(this String str) {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key) {
            TValue value = default(TValue);
            if (key != null) {
                col.TryGetValue(key, out value);
            }

            return value;
        }

        public static int JRead(this Stream stream, byte[] buffer, int offset, int count) {
            int result = stream.Read(buffer, offset, count);
            return result == 0 ? -1 : result;
        }
    }
}
