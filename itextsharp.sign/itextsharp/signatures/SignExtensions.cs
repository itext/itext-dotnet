using System;
using System.Collections.Generic;
using System.IO;

namespace iTextSharp.Signatures
{
    internal static class SignExtensions {

        public static String JSubstring(this String str, int beginIndex, int endIndex)
        {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static void AddAll<T>(this ICollection<T> t, IEnumerable<T> newItems)
        {
            foreach (T item in newItems)
            {
                t.Add(item);
            }
        }

        public static T JRemoveAt<T>(this IList<T> list, int index) {
            T value = list[index];
            list.RemoveAt(index);

            return value;
        }

        public static int Read(this Stream stream, byte[] buffer)
        {
            return stream.Read(buffer, 0, buffer.Length);
        }

        public static void JReset(this MemoryStream stream)
        {
            stream.Position = 0;
        }

        public static long Seek(this FileStream fs, long offset)
        {
            return fs.Seek(offset, SeekOrigin.Begin);
        }
    }
}