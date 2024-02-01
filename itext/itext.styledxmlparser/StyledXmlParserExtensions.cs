/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup.Helper;
using System.Reflection;
using iText.Commons.Utils;

internal static class StyledXmlParserExtensions {
    public static String Name(this Encoding e) {
        return e.WebName.ToUpperInvariant();
    }

    public static String DisplayName(this Encoding e) {
        return e.WebName.ToUpperInvariant();
    }

    public static bool RegionMatches(this string s, bool ignoreCase, int toffset, String other, int ooffset, int len) {
        return 0 == String.Compare(s, toffset, other, ooffset, len, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }

    public static bool StartsWith(this string s, string prefix, int pos) {
        int to = pos;
        int po = 0;
        int pc = prefix.Length;
        if ((pos < 0) || (pos > s.Length - pc)) {
            return false;
        }
        while (--pc >= 0) {
            if (s[to++] != prefix[po++]) {
                return false;
            }
        }
        return true;
    }

    public static void ReadFully(this FileStream stream, byte[] bytes) {
        stream.Read(bytes, 0, bytes.Length);
    }

    public static String Decode(this Encoding encoding, ByteBuffer byteBuffer) {
        byte[] bom;
        int offset = 0;
        Encoding temp = null;
        if (encoding.CodePage == Encoding.Unicode.CodePage && byteBuffer.Remaining() >= 2) {
            bom = new byte[2];
            byteBuffer.Peek(bom);
            if (bom[0] == (byte) 0xFE && bom[1] == (byte) 0xFF) {
                temp = Encoding.BigEndianUnicode;
                offset = 2;
            }
            if (bom[0] == (byte) 0xFF && bom[1] == (byte) 0xFE) {
                offset = 2;
            }
        }
        if (encoding.CodePage == Encoding.UTF32.CodePage && byteBuffer.Remaining() >= 4) {
            bom = new byte[4];
            byteBuffer.Peek(bom);
            if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == (byte) 0xFE && bom[3] == (byte) 0xFF) {
                temp = EncodingUtil.GetEncoding("utf-32be");
                offset = 4;
            }
            if (bom[0] == (byte) 0xFF && bom[1] == (byte) 0xFE && bom[2] == 0x00 && bom[3] == 0x00) {
                offset = 4;
            }
        }
        if (temp == null) {
            temp = EncodingUtil.GetEncoding(encoding.CodePage, EncoderFallback.ReplacementFallback,
                DecoderFallback.ReplacementFallback);
        }
        var result = temp.GetString(byteBuffer.buffer, byteBuffer.position + offset, byteBuffer.Remaining() - offset);
        byteBuffer.Position(byteBuffer.buffer.Length - 1);
        return result;
    }

    public static String ToExternalForm(this Uri u) {
        /*
        // pre-compute length of StringBuffer
        int len = u.Scheme.Length + 1;
        if (!String.IsNullOrEmpty(u.Authority))
            len += 2 + u.Authority.Length;
        if (!String.IsNullOrEmpty(u.AbsolutePath))
            len += u.AbsolutePath.Length - (u.AbsolutePath.EndsWith("/") ? 1 : 0);
        if (!String.IsNullOrEmpty(u.Query))
            len += u.Query.Length;
        if (!String.IsNullOrEmpty(u.Fragment))
            len += 1 + u.Fragment.Length;

        StringBuilder result = new StringBuilder(len);
        result.Append(u.Scheme);
        result.Append(":");
        if (!String.IsNullOrEmpty(u.Authority)) {
            result.Append("//");
            result.Append(u.Authority);
        }
        if (!String.IsNullOrEmpty(u.AbsolutePath)) {
            var path = u.AbsolutePath;
            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);
            result.Append(path);
        }
        if (!String.IsNullOrEmpty(u.Query))
            result.Append(u.Query);
        if (!String.IsNullOrEmpty(u.Fragment))
            result.Append(u.Fragment);
        return result.ToString();
        */
        return u.AbsoluteUri;
    }

    public static bool CanEncode(this Encoding encoding, char c) {
        return encoding.CanEncode(c.ToString());
    }

    public static bool CanEncode(this Encoding encoding, String chars) {
        byte[] src = Encoding.Unicode.GetBytes(chars);
        return encoding.CanEncode(src);
    }

    public static bool CanEncode(this Encoding encoding, byte[] src) {
        try {
            byte[] dest = Encoding.Convert(Encoding.Unicode,
                EncodingUtil.GetEncoding(encoding.CodePage, new EncoderExceptionFallback(),
                    new DecoderExceptionFallback()), src);
        } catch (EncoderFallbackException) {
            return false;
        }
        return true;
    }

    public static Stream GetResourceAsStream(this Type type, string filename) {
        Stream s =
            ResourceUtil.GetResourceStream(type.Namespace.ToLowerInvariant() + "." + filename,
                type);
        if (s == null) {
            throw new IOException();
        }
        return s;
    }

    public static int CodePointAt(this String str, int index) {
        return char.ConvertToUtf32(str, index);
    }

    public static StringBuilder AppendCodePoint(this StringBuilder sb, int codePoint) {
        return sb.Append(char.ConvertFromUtf32(codePoint));
    }

    public static String JSubstring(this String str, int beginIndex, int endIndex) {
        return str.Substring(beginIndex, endIndex - beginIndex);
    }

    public static void JReset(this MemoryStream stream) {
        stream.Position = 0;
    }

    public static StringBuilder Delete(this StringBuilder sb, int beginIndex, int endIndex) {
        return sb.Remove(beginIndex, endIndex - beginIndex);
    }

    public static void Write(this Stream stream, int value) {
        stream.WriteByte((byte) value);
    }

    public static int Read(this Stream stream) {
        return stream.ReadByte();
    }

    public static int Read(this Stream stream, byte[] buffer) {
        int size = stream.Read(buffer, 0, buffer.Length);
        return size == 0 ? -1 : size;
    }

    public static int JRead(this Stream stream, byte[] buffer, int offset, int count) {
        int result = stream.Read(buffer, offset, count);
        return result == 0 ? -1 : result;
    }

    public static void Write(this Stream stream, byte[] buffer) {
        stream.Write(buffer, 0, buffer.Length);
    }

    public static byte[] GetBytes(this String str) {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    public static byte[] GetBytes(this String str, Encoding encoding) {
        return encoding.GetBytes(str);
    }

    public static byte[] GetBytes(this String str, String encodingName) {
        return EncodingUtil.GetEncoding(encodingName).GetBytes(str);
    }

    public static ByteBuffer Encode(this Encoding encoding, String str) {
        return ByteBuffer.Wrap(encoding.GetBytes(str));
    }

    public static long Seek(this FileStream fs, long offset) {
        return fs.Seek(offset, SeekOrigin.Begin);
    }

    public static long Skip(this Stream s, long n) {
        s.Seek(n, SeekOrigin.Current);
        return n;
    }

    public static List<T> SubList<T>(this IList<T> list, int fromIndex, int toIndex) {
        return ((List<T>) list).GetRange(fromIndex, toIndex - fromIndex);
    }

    public static void GetChars(this StringBuilder sb, int srcBegin, int srcEnd, char[] dst, int dstBegin) {
        sb.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
    }

    public static String[] Split(this String str, String regex) {
        return str.Split(regex.ToCharArray());
    }

    public static void AddAll<T>(this ICollection<T> c, IEnumerable<T> collectionToAdd) {
        foreach (T o in collectionToAdd) {
            c.Add(o);
        }
    }
    
    public static void AddAll<T>(this Stack<T> c, IEnumerable<T> collectionToAdd) {
        foreach (T o in collectionToAdd) {
            c.Push(o);
        }
    }

    public static T[] ToArray<T>(this ICollection<T> col) {
        T[] result = new T[col.Count];
        return col.ToArray<T>(result);
    }

    public static T[] ToArray<T>(this ICollection<T> col, T[] toArray) {
        T[] r;
        int colSize = col.Count;
        if (colSize <= toArray.Length) {
            col.CopyTo(toArray, 0);
            if (colSize != toArray.Length) {
                toArray[colSize] = default(T);
            }
            r = toArray;
        } else {
            r = new T[colSize];
            col.CopyTo(r, 0);
        }

        return r;
    }

    public static bool RemoveAll<T>(this ICollection<T> set, ICollection<T> c) {
        bool modified = false;
        foreach (var item in c) {
            if (set.Remove(item)) modified = true;
        }
        return modified;
    }

    public static TValue JRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
        TValue value;
        dictionary.TryGetValue(key, out value);
        dictionary.Remove(key);

        return value;
    }

    public static T JRemoveFirst<T>(this LinkedList<T> list)
    {
        T value = list.First.Value;
        list.RemoveFirst();

        return value;
    }

    public static bool Matches(this String str, String regex) {
        return Regex.IsMatch(str, "^"+regex+"$");
    }

    public static String ReplaceFirst(this String input, String pattern, String replacement) {
        var regex = new Regex(pattern);
        return regex.Replace(input, replacement, 1);
    }

    public static bool EqualsIgnoreCase(this String str, String anotherString) {
        return String.Equals(str, anotherString, StringComparison.OrdinalIgnoreCase);
    }

    public static Uri ToUri(this String s) {
        return new Uri(s);
    }

    public static FileInfo ToFile(this String s) {
        return new FileInfo(s);
    }

    public static Uri ToUrl(this Uri u) {
        return u;
    }

    public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> c,
        IDictionary<TKey, TValue> collectionToAdd) {
        foreach (KeyValuePair<TKey, TValue> pair in collectionToAdd) {
            c[pair.Key] = pair.Value;
        }
    }

    public static void AddAll<T>(this IList<T> list, int index, IList<T> c) {
        for (int i = c.Count - 1; i >= 0; i--) {
            list.Insert(index, c[i]);
        }
    }

    public static bool IsEmpty<T>(this Stack<T> c) {
        return c.Count == 0;
    }

    public static bool IsEmpty<T>(this ICollection<T> c) {
        return c.Count == 0;
    }

    public static bool IsEmpty<T>(this IList<T> l) {
        return l.Count == 0;
    }

    public static void Add<T>(this IList<T> list, int index, T elem) {
        list.Insert(index, elem);
    }

    public static bool Add<T>(this LinkedList<T> list, T elem) {
        list.AddLast(elem);
        return true;
    }

    public static T JRemove<T>(this LinkedList<T> list) {
        T head = list.First.Value;
        list.RemoveFirst();
        return head;
    }

    public static T JRemoveAt<T>(this IList<T> list, int index) {
        T value = list[index];
        list.RemoveAt(index);

        return value;
    }

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key) {
        TValue value = default(TValue);
        if (key != null) {
            col.TryGetValue(key, out value);
        }

        return value;
    }

    public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key, TValue value) {
        TValue oldVal = col.Get(key);
        col[key] = value;
        return oldVal;
    }

    public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
        return dictionary.ContainsKey(key);
    }

    public static T JGetFirst<T>(this LinkedList<T> list) {
        return list.First.Value;
    }

    public static void EnsureCapacity<T>(this List<T> list, int capacity) {
        if (capacity > list.Count) {
            list.Capacity = capacity;
        }
    }

    public static StringBuilder JAppend(this StringBuilder sb, String str, int begin, int end) {
        return sb.Append(str, begin, end - begin);
    }

    public static StringBuilder Reverse(this StringBuilder sb) {
        char MIN_SURROGATE = '\uD800';
        char MAX_SURROGATE = '\uDFFF';

        bool hasSurrogate = false;
        int n = sb.Length - 1;
        for (int j = (n - 1) >> 1; j >= 0; --j) {
            char temp = sb[j];
            char temp2 = sb[n - j];
            if (!hasSurrogate) {
                hasSurrogate = (temp >= MIN_SURROGATE && temp <= MAX_SURROGATE)
                    || (temp2 >= MIN_SURROGATE && temp2 <= MAX_SURROGATE);
            }
            sb[j] = temp2;
            sb[n - j] = temp;
        }
        if (hasSurrogate) {
            // Reverse back all valid surrogate pairs
            for (int i = 0; i < sb.Length - 1; i++) {
                char c2 = sb[i];
                if (char.IsLowSurrogate(c2)) {
                    char c1 = sb[i + 1];
                    if (char.IsHighSurrogate(c1)) {
                        sb[i++] = c1;
                        sb[i] = c2;
                    }
                }
            }
        }
        return sb;
    }

#if !NETSTANDARD2_0
    public static Attribute GetCustomAttribute(this Assembly assembly, Type attributeType)
    {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(attributeType, false);
        if (customAttributes.Length > 0 && customAttributes[0] is Attribute)
        {
            return customAttributes[0] as Attribute;
        }
        else
        {
            return null;
        }
    }
#endif

    public static Assembly GetAssembly(this Type type)
    {
#if !NETSTANDARD2_0
        return type.Assembly;
#else
        return type.GetTypeInfo().Assembly;
#endif
    }

#if NETSTANDARD2_0
    public static MethodInfo GetMethod(this Type type, String methodName, Type[] parameterTypes) {
        return type.GetTypeInfo().GetMethod(methodName, parameterTypes);
    }

    public static MethodInfo GetMethod(this Type type, String methodName) {
        return type.GetTypeInfo().GetMethod(methodName);
    }

    public static ConstructorInfo GetConstructor(this Type type, Type[] parameterTypes) {
        return type.GetTypeInfo().GetConstructor(parameterTypes);
    }

    public static bool IsInstanceOfType(this Type type, object objToCheck) {
        return type.GetTypeInfo().IsInstanceOfType(objToCheck);
    }

    public static FieldInfo[] GetFields(this Type type, BindingFlags flags) {
        return type.GetTypeInfo().GetFields(flags);
    }

    public static byte[] GetBuffer(this MemoryStream memoryStream) {
        ArraySegment<byte> buf;
        memoryStream.TryGetBuffer(out buf);
        return buf.Array;
    }
#endif
}
