using System;
using System.Linq;
using System.Collections.Generic;

namespace iTextSharp.Forms {
    internal static class FormsExtensions {
        public static void AddAll<T>(this ICollection<T> t, IEnumerable<T> newItems) {
            foreach (T item in newItems) {
                t.Add(item);
            }
        }

        public static String JSubstring(this String str, int beginIndex, int endIndex) {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static byte[] GetBytes(this String str) {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static T[] ToArray<T>(this ICollection<T> col, T[] toArray) {
            T[] r = col.ToArray();
            return r;
        }
    }
}