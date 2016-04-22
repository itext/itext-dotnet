/*
$Id: 70b935765eed666009d0762efd8c49c3cc0c13e1 $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using com.itextpdf.kernel;

namespace com.itextpdf.kernel.pdf
{
	public class PdfNameTree
	{
		private const int NODE_SIZE = 40;

		private PdfCatalog catalog;

		private IDictionary<String, PdfObject> items = new Dictionary<String, PdfObject>(
			);

		private PdfName treeType;

		private bool modified;

		/// <summary>Creates the NameTree of current Document</summary>
		/// <param name="catalog">Document catalog</param>
		/// <param name="treeType">the type of tree. Dests Tree, AP Tree etc.</param>
		public PdfNameTree(PdfCatalog catalog, PdfName treeType)
		{
			this.treeType = treeType;
			this.catalog = catalog;
			items = GetNames();
		}

		public virtual IDictionary<String, PdfObject> GetNames()
		{
			if (items.Count > 0)
			{
				return items;
			}
			PdfDictionary dictionary = catalog.GetPdfObject().GetAsDictionary(PdfName.Names);
			if (dictionary != null)
			{
				dictionary = dictionary.GetAsDictionary(treeType);
				if (dictionary != null)
				{
					items = ReadTree(dictionary);
					for (IEnumerator<KeyValuePair<String, PdfObject>> it = items.GetEnumerator(); it.
						MoveNext(); )
					{
						KeyValuePair<String, PdfObject> entry = it.Current;
						PdfArray arr = GetNameArray(entry.Value);
						if (arr != null)
						{
							entry.SetValue(arr);
						}
						else
						{
							it.Remove();
						}
					}
				}
			}
			if (treeType.Equals(PdfName.Dests))
			{
				PdfDictionary destinations = catalog.GetPdfObject().GetAsDictionary(PdfName.Dests
					);
				if (destinations != null)
				{
					ICollection<PdfName> keys = destinations.KeySet();
					foreach (PdfName key in keys)
					{
						PdfArray array = GetNameArray(destinations.Get(key));
						if (array == null)
						{
							continue;
						}
						items[key.GetValue()] = array;
					}
				}
			}
			return items;
		}

		public virtual void AddEntry(String key, PdfObject value)
		{
			if (items.Keys.Contains(key))
			{
				throw new PdfException(PdfException.NameAlreadyExistsInTheNameTree);
			}
			modified = true;
			items[key] = value;
		}

		public virtual bool IsModified()
		{
			return modified;
		}

		public virtual PdfDictionary BuildTree()
		{
			String[] names = new String[items.Count];
			names = items.Keys.ToArray(names);
			System.Array.Sort(names);
			if (names.Length <= NODE_SIZE)
			{
				PdfDictionary dic = new PdfDictionary();
				PdfArray ar = new PdfArray();
				for (int k = 0; k < names.Length; ++k)
				{
					ar.Add(new PdfString(names[k], null));
					ar.Add(items[names[k]]);
				}
				dic.Put(PdfName.Names, ar);
				return dic;
			}
			int skip = NODE_SIZE;
			PdfDictionary[] kids = new PdfDictionary[(names.Length + NODE_SIZE - 1) / NODE_SIZE
				];
			for (int k_1 = 0; k_1 < kids.Length; ++k_1)
			{
				int offset = k_1 * NODE_SIZE;
				int end = Math.Min(offset + NODE_SIZE, names.Length);
				PdfDictionary dic = new PdfDictionary();
				PdfArray arr = new PdfArray();
				arr.Add(new PdfString(names[offset], null));
				arr.Add(new PdfString(names[end - 1], null));
				dic.Put(PdfName.Limits, arr);
				arr = new PdfArray();
				for (; offset < end; ++offset)
				{
					arr.Add(new PdfString(names[offset], null));
					arr.Add(items[names[offset]]);
				}
				dic.Put(PdfName.Names, arr);
				dic.MakeIndirect(catalog.GetDocument());
				kids[k_1] = dic;
			}
			int top = kids.Length;
			while (true)
			{
				if (top <= NODE_SIZE)
				{
					PdfArray arr = new PdfArray();
					for (int k = 0; k_1 < top; ++k_1)
					{
						arr.Add(kids[k_1]);
					}
					PdfDictionary dic = new PdfDictionary();
					dic.Put(PdfName.Kids, arr);
					return dic;
				}
				skip *= NODE_SIZE;
				int tt = (names.Length + skip - 1) / skip;
				for (int k_2 = 0; k_2 < tt; ++k_2)
				{
					int offset = k_2 * NODE_SIZE;
					int end = Math.Min(offset + NODE_SIZE, top);
					PdfDictionary dic = ((PdfDictionary)new PdfDictionary().MakeIndirect(catalog.GetDocument
						()));
					PdfArray arr = new PdfArray();
					arr.Add(new PdfString(names[k_2 * skip], null));
					arr.Add(new PdfString(names[Math.Min((k_2 + 1) * skip, names.Length) - 1], null));
					dic.Put(PdfName.Limits, arr);
					arr = new PdfArray();
					for (; offset < end; ++offset)
					{
						arr.Add(kids[offset]);
					}
					dic.Put(PdfName.Kids, arr);
					kids[k_2] = dic;
				}
				top = tt;
			}
		}

		private IDictionary<String, PdfObject> ReadTree(PdfDictionary dictionary)
		{
			IDictionary<String, PdfObject> items = new Dictionary<String, PdfObject>();
			if (dictionary != null)
			{
				IterateItems(dictionary, items, null);
			}
			return items;
		}

		private PdfString IterateItems(PdfDictionary dictionary, IDictionary<String, PdfObject
			> items, PdfString leftOver)
		{
			PdfArray names = dictionary.GetAsArray(PdfName.Names);
			if (names != null)
			{
				for (int k = 0; k < names.Size(); k++)
				{
					PdfString name;
					if (leftOver == null)
					{
						name = names.GetAsString(k++);
					}
					else
					{
						name = leftOver;
						leftOver = null;
					}
					if (k < names.Size())
					{
						items[name.ToUnicodeString()] = names.Get(k);
					}
					else
					{
						return name;
					}
				}
			}
			else
			{
				if ((names = dictionary.GetAsArray(PdfName.Kids)) != null)
				{
					for (int k = 0; k < names.Size(); k++)
					{
						PdfDictionary kid = names.GetAsDictionary(k);
						leftOver = IterateItems(kid, items, leftOver);
					}
				}
			}
			return null;
		}

		private PdfArray GetNameArray(PdfObject obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj.IsArray())
			{
				return (PdfArray)obj;
			}
			else
			{
				if (obj.IsDictionary())
				{
					PdfArray arr = ((PdfDictionary)obj).GetAsArray(PdfName.D);
					return arr;
				}
			}
			return null;
		}
	}
}
