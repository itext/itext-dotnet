using System;
using System.Collections.Generic;

namespace com.itextpdf.forms
{
    internal static class FormsExtensions {
        public static void AddAll<T>(this ICollection<T> t,  IEnumerable<T> newItems)
        {
            foreach (T item in newItems)
            {
                    t.Add(item);
            }
        }

        public static String JSubstring(this String str, int beginIndex, int endIndex)
        {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static byte[] GetBytes(this String str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
    }
}