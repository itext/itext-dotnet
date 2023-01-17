/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace iText.Forms.Xfa
{
	/// <summary>A class for some basic SOM processing.</summary>
	internal class Xml2Som
	{
		/// <summary>The order the names appear in the XML, depth first.</summary>
		protected internal IList<String> order;

		/// <summary>The mapping of full names to nodes.</summary>
		protected internal IDictionary<String, XNode> name2Node;

		/// <summary>The data to do a search from the bottom hierarchy.</summary>
		protected internal IDictionary<String, InverseStore> inverseSearch;

		/// <summary>A stack to be used when parsing.</summary>
		protected internal Stack<String> stack;

		/// <summary>A temporary store for the repetition count.</summary>
		protected internal int anform;

		/// <summary>Escapes a SOM string fragment replacing "." with "\.".</summary>
		/// <param name="s">the unescaped string</param>
		/// <returns>the escaped string</returns>
		public static String EscapeSom(String s)
		{
			if (s == null)
			{
				return "";
			}
			int idx = s.IndexOf('.');
			if (idx < 0)
			{
				return s;
			}
			StringBuilder sb = new StringBuilder();
			int last = 0;
			while (idx >= 0)
			{
				sb.Append(s.JSubstring(last, idx));
				sb.Append('\\');
				last = idx;
				idx = s.IndexOf('.', idx + 1);
			}
			sb.Append(s.Substring(last));
			return sb.ToString();
		}

		/// <summary>Unescapes a SOM string fragment replacing "\." with ".".</summary>
		/// <param name="s">the escaped string</param>
		/// <returns>the unescaped string</returns>
		public static String UnescapeSom(String s)
		{
			int idx = s.IndexOf('\\');
			if (idx < 0)
			{
				return s;
			}
			StringBuilder sb = new StringBuilder();
			int last = 0;
			while (idx >= 0)
			{
				sb.Append(s.JSubstring(last, idx));
				last = idx + 1;
				idx = s.IndexOf('\\', idx + 1);
			}
			sb.Append(s.Substring(last));
			return sb.ToString();
		}

		/// <summary>
		/// Outputs the stack as the sequence of elements separated
		/// by '.'.
		/// </summary>
		/// <returns>the stack as the sequence of elements separated by '.'</returns>
		protected internal virtual String PrintStack()
		{
			if (stack.Count == 0)
			{
				return "";
			}

            IList<string> temp = new List<string>();

            while ( stack.Count > 0 )
            {
                temp.Add(stack.Pop());
            }

			StringBuilder s = new StringBuilder();

            for ( int k = temp.Count - 1; k >= 0; --k )
			{
                string name = temp.ElementAt(k);
                s.Append('.').Append(name);
                stack.Push(name);
			}

			return s.ToString().Substring(1);
		}

		/// <summary>Gets the name with the <CODE>#subform</CODE> removed.</summary>
		/// <param name="s">the long name</param>
		/// <returns>the short name</returns>
		public static String GetShortName(String s)
		{
			int idx = s.IndexOf(".#subform[");
			if (idx < 0)
			{
				return s;
			}
			int last = 0;
			StringBuilder sb = new StringBuilder();
			while (idx >= 0)
			{
				sb.Append(s.JSubstring(last, idx));
				idx = s.IndexOf("]", idx + 10);
				if (idx < 0)
				{
					return sb.ToString();
				}
				last = idx + 1;
				idx = s.IndexOf(".#subform[", last);
			}
			sb.Append(s.Substring(last));
			return sb.ToString();
		}

		/// <summary>Adds a SOM name to the search node chain.</summary>
		/// <param name="unstack">the SOM name</param>
		public virtual void InverseSearchAdd(String unstack)
		{
			InverseSearchAdd(inverseSearch, stack, unstack);
		}

		/// <summary>Adds a SOM name to the search node chain.</summary>
		/// <param name="inverseSearch">the start point</param>
		/// <param name="stack">the stack with the separated SOM parts</param>
		/// <param name="unstack">the full name</param>
		public static void InverseSearchAdd(IDictionary<String, InverseStore> inverseSearch
			, Stack<String> stack, String unstack)
		{
			String last = stack.Peek();
			InverseStore store = inverseSearch.Get(last);
			if (store == null)
			{
				store = new InverseStore();
				inverseSearch[last] = store;
			}
			for (int k = stack.Count - 2; k >= 0; --k)
			{
				last = stack.ElementAt(k);
				InverseStore store2;
				int idx = store.part.IndexOf(last);
				if (idx < 0)
				{
					store.part.Add(last);
					store2 = new InverseStore();
					store.follow.Add(store2);
				}
				else
				{
					store2 = (InverseStore)store.follow[idx];
				}
				store = store2;
			}
			store.part.Add("");
			store.follow.Add(unstack);
		}

		/// <summary>Searches the SOM hierarchy from the bottom.</summary>
		/// <param name="parts">the SOM parts</param>
		/// <returns>the full name or <CODE>null</CODE> if not found</returns>
		public virtual String InverseSearchGlobal(IList<String> parts)
		{
			if (parts.Count == 0)
			{
				return null;
			}
			InverseStore store = inverseSearch.Get(parts[parts.Count - 1]);
			if (store == null)
			{
				return null;
			}
			for (int k = parts.Count - 2; k >= 0; --k)
			{
				String part = parts[k];
				int idx = store.part.IndexOf(part);
				if (idx < 0)
				{
					if (store.IsSimilar(part))
					{
						return null;
					}
					return store.GetDefaultName();
				}
				store = (InverseStore)store.follow[idx];
			}
			return store.GetDefaultName();
		}

		/// <summary>Splits a SOM name in the individual parts.</summary>
		/// <param name="name">the full SOM name</param>
		/// <returns>the split name</returns>
		public static Stack<String> SplitParts(String name)
		{
			while (name.StartsWith("."))
			{
				name = name.Substring(1);
			}
			Stack<String> parts = new Stack<String>();
			int last = 0;
			int pos = 0;
			String part;
			while (true)
			{
				pos = last;
				while (true)
				{
					pos = name.IndexOf('.', pos);
					if (pos < 0)
					{
						break;
					}
					if (name[pos - 1] == '\\')
					{
						++pos;
					}
					else
					{
						break;
					}
				}
				if (pos < 0)
				{
					break;
				}
				part = name.JSubstring(last, pos);
				if (!part.EndsWith("]"))
				{
					part += "[0]";
				}
				parts.Push(part);
				last = pos + 1;
			}
			part = name.Substring(last);
			if (!part.EndsWith("]"))
			{
				part += "[0]";
			}
			parts.Push(part);
			return parts;
		}

		/// <summary>Gets the order the names appear in the XML, depth first.</summary>
		/// <returns>the order the names appear in the XML, depth first</returns>
		public virtual IList<String> GetOrder()
		{
			return order;
		}

		/// <summary>Sets the order the names appear in the XML, depth first</summary>
		/// <param name="order">the order the names appear in the XML, depth first</param>
		public virtual void SetOrder(IList<String> order)
		{
			this.order = order;
		}

		/// <summary>Gets the mapping of full names to nodes.</summary>
		/// <returns>the mapping of full names to nodes</returns>
		public virtual IDictionary<String, XNode> GetName2Node()
		{
			return name2Node;
		}

		/// <summary>Sets the mapping of full names to nodes.</summary>
		/// <param name="name2Node">the mapping of full names to nodes</param>
		public virtual void SetName2Node(IDictionary<String, XNode> name2Node)
		{
			this.name2Node = name2Node;
		}

		/// <summary>Gets the data to do a search from the bottom hierarchy.</summary>
		/// <returns>the data to do a search from the bottom hierarchy</returns>
		public virtual IDictionary<String, InverseStore> GetInverseSearch()
		{
			return inverseSearch;
		}

		/// <summary>Sets the data to do a search from the bottom hierarchy.</summary>
		/// <param name="inverseSearch">the data to do a search from the bottom hierarchy</param>
		public virtual void SetInverseSearch(IDictionary<String, InverseStore> inverseSearch
			)
		{
			this.inverseSearch = inverseSearch;
		}
	}
}
