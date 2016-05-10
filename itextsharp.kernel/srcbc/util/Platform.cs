using System;
using System.Globalization;
using System.IO;
using System.Text;

#if SILVERLIGHT
using System.Collections.Generic;
#else
using System.Collections;
#endif

namespace Org.BouncyCastle.Utilities
{
	internal sealed class Platform
	{
		private Platform()
		{
		}

#if NETCF_1_0 || NETCF_2_0
		private static string GetNewLine()
		{
			MemoryStream buf = new MemoryStream();
			StreamWriter w = new StreamWriter(buf, Encoding.UTF8);
			w.WriteLine();
			w.Close();
			byte[] bs = buf.ToArray();
            return Encoding.UTF8.GetString(bs, 0, bs.Length);
		}
#else
        private static string GetNewLine()
        {
            return Environment.NewLine;
        }
#endif

        internal static int CompareIgnoreCase(string a, string b)
        {
#if SILVERLIGHT
            return String.Compare(a, b, StringComparison.InvariantCultureIgnoreCase);
#else
            return String.Compare(a, b, true);
#endif
        }

#if NETCF_1_0 || NETCF_2_0 || SILVERLIGHT
		internal static string GetEnvironmentVariable(
			string variable)
		{
			return null;
		}
#else
		internal static string GetEnvironmentVariable(
			string variable)
		{
			try
			{
				return Environment.GetEnvironmentVariable(variable);
			}
			catch (System.Security.SecurityException)
			{
				// We don't have the required permission to read this environment variable,
				// which is fine, just act as if it's not set
				return null;
			}
		}
#endif

#if NETCF_1_0
		internal static Exception CreateNotImplementedException(
			string message)
		{
			return new Exception("Not implemented: " + message);
		}

		internal static bool Equals(
			object	a,
			object	b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}
#else
		internal static Exception CreateNotImplementedException(
			string message)
		{
			return new NotImplementedException(message);
		}
#endif

#if SILVERLIGHT
        internal static System.Collections.IList CreateArrayList()
        {
            return new List<object>();
        }
        internal static System.Collections.IList CreateArrayList(int capacity)
        {
            return new List<object>(capacity);
        }
        internal static System.Collections.IList CreateArrayList(System.Collections.ICollection collection)
        {
            System.Collections.IList result = new List<object>(collection.Count);
            foreach (object o in collection)
            {
                result.Add(o);
            }
            return result;
        }
        internal static System.Collections.IList CreateArrayList(System.Collections.IEnumerable collection)
        {
            System.Collections.IList result = new List<object>();
            foreach (object o in collection)
            {
                result.Add(o);
            }
            return result;
        }
        internal static System.Collections.IDictionary CreateHashtable()
        {
            return new Dictionary<object, object>();
        }
        internal static System.Collections.IDictionary CreateHashtable(int capacity)
        {
            return new Dictionary<object, object>(capacity);
        }
        internal static System.Collections.IDictionary CreateHashtable(System.Collections.IDictionary dictionary)
        {
            System.Collections.IDictionary result = new Dictionary<object, object>(dictionary.Count);
            foreach (System.Collections.DictionaryEntry entry in dictionary)
            {
                result.Add(entry.Key, entry.Value);
            }
            return result;
        }
#else
        internal static System.Collections.IList CreateArrayList()
        {
            return new ArrayList();
        }
        internal static System.Collections.IList CreateArrayList(int capacity)
        {
            return new ArrayList(capacity);
        }
        internal static System.Collections.IList CreateArrayList(System.Collections.ICollection collection)
        {
            return new ArrayList(collection);
        }
        internal static System.Collections.IList CreateArrayList(System.Collections.IEnumerable collection)
        {
            ArrayList result = new ArrayList();
            foreach (object o in collection)
            {
                result.Add(o);
            }
            return result;
        }
        internal static System.Collections.IDictionary CreateHashtable()
        {
            return new Hashtable();
        }
        internal static System.Collections.IDictionary CreateHashtable(int capacity)
        {
            return new Hashtable(capacity);
        }
        internal static System.Collections.IDictionary CreateHashtable(System.Collections.IDictionary dictionary)
        {
            return new Hashtable(dictionary);
        }
#endif

        internal static string ToLowerInvariant(string s)
        {
            return s.ToLower(CultureInfo.InvariantCulture);
        }

        internal static string ToUpperInvariant(string s)
        {
            return s.ToUpper(CultureInfo.InvariantCulture);
        }

        internal static readonly string NewLine = GetNewLine();
	}
}
