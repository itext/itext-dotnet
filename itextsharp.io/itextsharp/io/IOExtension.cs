using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.itextpdf.io
{
    internal static class IOExtensions
    {
        public static String JSubstring(this String str, int beginIndex, int endIndex)
        {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static void JReset(this MemoryStream stream)
        {
            stream.Position = 0;
        }

        public static void Write(this Stream stream, int value)
        {
            stream.WriteByte((byte)value);
        }

        public static int Read(this Stream stream)
        {
            return stream.ReadByte();
        }

        public static int Read(this Stream stream, byte[] buffer)
        {
            int size = stream.Read(buffer, 0, buffer.Length);
            return size == 0 ? -1 : size;
        }

        public static void Write(this Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }

        public static byte[] GetBytes(this String str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static long Seek(this FileStream fs, long offset)
        {
            return fs.Seek(offset, SeekOrigin.Begin);
        }

        public static long Skip(this Stream s, long n)
        {
            s.Seek(n, SeekOrigin.Current);
            return n;
        }

        public static List<T> SubList<T>(this IList<T> list, int fromIndex, int toIndex)
        {
            return ((List<T>)list).GetRange(fromIndex, toIndex - fromIndex);
        }

        public static void GetChars(this StringBuilder sb, int srcBegin, int srcEnd, char[] dst, int dstBegin)
        {
            sb.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
        }

        public static String[] Split(this String str, String regex)
        {
            return str.Split(str.ToCharArray());
        }

        public static void AddAll<T>(this ICollection<T> c, ICollection<T> collectionToAdd)
        {
            foreach (T o in collectionToAdd)
            {
                c.Add(o);
            }
        }

        public static void AddAll<T>(this IList<T> list, int index, IList<T> c)
        {
            for (int i = c.Count - 1; i >= 0; i--)
            {
                list.Insert(index, c[i]);
            }
        }
    }
}