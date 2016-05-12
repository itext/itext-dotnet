/*
$Id: 06c30d6f7768ae8fa8e55f9f572b7bb366d86655 $

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
using com.itextpdf.kernel.geom;

namespace com.itextpdf.kernel.pdf
{
	/// <summary>A representation of a Dictionary as described by the PDF Specification.</summary>
	/// <remarks>
	/// A representation of a Dictionary as described by the PDF Specification. A Dictionary is a mapping between keys
	/// and values. Keys are
	/// <see cref="PdfName">PdfNames</see>
	/// and the values are
	/// <see cref="PdfObject">PdfObjects</see>
	/// . Each key can only be associated with one value and
	/// adding a new value to an existing key will override the previous value. A value of null should be ignored when
	/// the PdfDocument is closed.
	/// </remarks>
	public class PdfDictionary : PdfObject
	{
		private const long serialVersionUID = -1122075818690871644L;

		private IDictionary<PdfName, PdfObject> map = new SortedDictionary<PdfName, PdfObject
			>();

		/// <summary>Creates a new PdfDictionary instance.</summary>
		public PdfDictionary()
			: base()
		{
		}

		/// <summary>Creates a new PdfDictionary instance.</summary>
		/// <remarks>
		/// Creates a new PdfDictionary instance. This constructor inserts the content of the specified Map into this
		/// PdfDictionary instance.
		/// </remarks>
		/// <param name="map">Map containing values to be inserted into PdfDictionary</param>
		public PdfDictionary(IDictionary<PdfName, PdfObject> map)
		{
			this.map.AddAll(map);
		}

		/// <summary>Creates a new PdfDictionary instance.</summary>
		/// <remarks>
		/// Creates a new PdfDictionary instance. This constructor inserts the content of the specified Set into this
		/// PdfDictionary instance.
		/// </remarks>
		/// <param name="entrySet">Set containing Map#Entries to be inserted into PdfDictionary
		/// 	</param>
		public PdfDictionary(ICollection<KeyValuePair<PdfName, PdfObject>> entrySet)
		{
			foreach (KeyValuePair<PdfName, PdfObject> entry in entrySet)
			{
				this.map[entry.Key] = entry.Value;
			}
		}

		/// <summary>Creates a new PdfDictionary instance.</summary>
		/// <remarks>
		/// Creates a new PdfDictionary instance. This constructor inserts the content of the specified PdfDictionary
		/// into this PdfDictionary instance.
		/// </remarks>
		/// <param name="dictionary">PdfDictionary containing values to be inserted into PdfDictionary
		/// 	</param>
		public PdfDictionary(com.itextpdf.kernel.pdf.PdfDictionary dictionary)
		{
			map.AddAll(dictionary.map);
		}

		/// <summary>Returns the number of key-value pairs in this PdfDictionary.</summary>
		/// <returns>number of key-value pairs</returns>
		public virtual int Size()
		{
			return map.Count;
		}

		/// <summary>Returns true if there are no key-value pairs in this PdfDictionary.</summary>
		/// <returns>true if there are no key-value pairs in this PdfDictionary</returns>
		public virtual bool IsEmpty()
		{
			return map.IsEmpty();
		}

		/// <summary>Returns true if this PdfDictionary contains the specified key.</summary>
		/// <param name="key">the key to check</param>
		/// <returns>true if key is present in the PdfDictionary</returns>
		public virtual bool ContainsKey(PdfName key)
		{
			return map.ContainsKey(key);
		}

		/// <summary>Returns true if this PdfDictionary contains the specified value.</summary>
		/// <param name="value">the value to check</param>
		/// <returns>true if value is present in the PdfDictionary</returns>
		public virtual bool ContainsValue(PdfObject value)
		{
			return map.ContainsValue(value);
		}

		/// <summary>Returns the value associated to this key.</summary>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>the value associated with this key</returns>
		public virtual PdfObject Get(PdfName key)
		{
			return Get(key, true);
		}

		/// <summary>Returns the value associated to this key as a PdfArray.</summary>
		/// <remarks>Returns the value associated to this key as a PdfArray. If the value isn't a PdfArray, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>PdfArray associated with this key</returns>
		public virtual PdfArray GetAsArray(PdfName key)
		{
			PdfObject direct = Get(key, true);
			if (direct != null && direct.GetType() == PdfObject.ARRAY)
			{
				return (PdfArray)direct;
			}
			return null;
		}

		/// <summary>Returns the value associated to this key as a PdfDictionary.</summary>
		/// <remarks>Returns the value associated to this key as a PdfDictionary. If the value isn't a PdfDictionary, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>PdfDictionary associated with this key</returns>
		public virtual com.itextpdf.kernel.pdf.PdfDictionary GetAsDictionary(PdfName key)
		{
			PdfObject direct = Get(key, true);
			if (direct != null && direct.GetType() == PdfObject.DICTIONARY)
			{
				return (com.itextpdf.kernel.pdf.PdfDictionary)direct;
			}
			return null;
		}

		/// <summary>Returns the value associated to this key as a PdfStream.</summary>
		/// <remarks>Returns the value associated to this key as a PdfStream. If the value isn't a PdfStream, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>PdfStream associated with this key</returns>
		public virtual PdfStream GetAsStream(PdfName key)
		{
			PdfObject direct = Get(key, true);
			if (direct != null && direct.GetType() == PdfObject.STREAM)
			{
				return (PdfStream)direct;
			}
			return null;
		}

		/// <summary>Returns the value associated to this key as a PdfNumber.</summary>
		/// <remarks>Returns the value associated to this key as a PdfNumber. If the value isn't a PdfNumber, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>PdfNumber associated with this key</returns>
		public virtual PdfNumber GetAsNumber(PdfName key)
		{
			PdfObject direct = Get(key, true);
			if (direct != null && direct.GetType() == PdfObject.NUMBER)
			{
				return (PdfNumber)direct;
			}
			return null;
		}

		/// <summary>Returns the value associated to this key as a PdfName.</summary>
		/// <remarks>Returns the value associated to this key as a PdfName. If the value isn't a PdfName, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>PdfName associated with this key</returns>
		public virtual PdfName GetAsName(PdfName key)
		{
			PdfObject direct = Get(key, true);
			if (direct != null && direct.GetType() == PdfObject.NAME)
			{
				return (PdfName)direct;
			}
			return null;
		}

		/// <summary>Returns the value associated to this key as a PdfString.</summary>
		/// <remarks>Returns the value associated to this key as a PdfString. If the value isn't a PdfString, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>PdfString associated with this key</returns>
		public virtual PdfString GetAsString(PdfName key)
		{
			PdfObject direct = Get(key, true);
			if (direct != null && direct.GetType() == PdfObject.STRING)
			{
				return (PdfString)direct;
			}
			return null;
		}

		/// <summary>Returns the value associated to this key as a PdfBoolean.</summary>
		/// <remarks>Returns the value associated to this key as a PdfBoolean. If the value isn't a PdfBoolean, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>PdfBoolean associated with this key</returns>
		public virtual PdfBoolean GetAsBoolean(PdfName key)
		{
			PdfObject direct = Get(key, true);
			if (direct != null && direct.GetType() == PdfObject.BOOLEAN)
			{
				return (PdfBoolean)direct;
			}
			return null;
		}

		/// <summary>Returns the value associated to this key as a Rectangle.</summary>
		/// <remarks>
		/// Returns the value associated to this key as a Rectangle. If the value isn't a PdfArray of which the
		/// firt four elements are PdfNumbers, null is returned.
		/// </remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>PdfArray associated with this key</returns>
		/// <seealso cref="PdfArray.ToRectangle()"/>
		public virtual Rectangle GetAsRectangle(PdfName key)
		{
			PdfArray a = GetAsArray(key);
			return a == null ? null : a.ToRectangle();
		}

		/// <summary>Returns the value associated to this key as a Float.</summary>
		/// <remarks>Returns the value associated to this key as a Float. If the value isn't a Pdfnumber, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>Float associated with this key</returns>
		public virtual float GetAsFloat(PdfName key)
		{
			PdfNumber number = GetAsNumber(key);
			return number == null ? null : number.FloatValue();
		}

		/// <summary>Returns the value associated to this key as an Integer.</summary>
		/// <remarks>Returns the value associated to this key as an Integer. If the value isn't a Pdfnumber, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>Integer associated with this key</returns>
		public virtual int GetAsInt(PdfName key)
		{
			PdfNumber number = GetAsNumber(key);
			return number == null ? null : number.IntValue();
		}

		/// <summary>Returns the value associated to this key as a Boolean.</summary>
		/// <remarks>Returns the value associated to this key as a Boolean. If the value isn't a PdfBoolean, null is returned.
		/// 	</remarks>
		/// <param name="key">the key of which the associated value needs to be returned</param>
		/// <returns>Boolean associated with this key</returns>
		public virtual bool GetAsBool(PdfName key)
		{
			PdfBoolean b = GetAsBoolean(key);
			return b == null ? null : b.GetValue();
		}

		/// <summary>Inserts the value into this PdfDictionary and associates it with the specified key.
		/// 	</summary>
		/// <remarks>
		/// Inserts the value into this PdfDictionary and associates it with the specified key. If the key is already
		/// present in this PdfDictionary, this method will override the old value with the specified one.
		/// </remarks>
		/// <param name="key">key to insert or to override</param>
		/// <param name="value">the value to associate with the specified key</param>
		/// <returns>the previous PdfObject associated with this key</returns>
		public virtual PdfObject Put(PdfName key, PdfObject value)
		{
			System.Diagnostics.Debug.Assert(value != null);
			return map[key] = value;
		}

		/// <summary>Removes the specified key from this PdfDictionary.</summary>
		/// <param name="key">key to be removed</param>
		/// <returns>the removed value associated with the specified key</returns>
		public virtual PdfObject Remove(PdfName key)
		{
			return map.Remove(key);
		}

		/// <summary>Inserts all the key-value pairs into this PdfDictionary.</summary>
		/// <param name="d">PdfDictionary holding the key-value pairs to be copied</param>
		public virtual void PutAll(com.itextpdf.kernel.pdf.PdfDictionary d)
		{
			map.AddAll(d.map);
		}

		/// <summary>Removes all key-value pairs from this PdfDictionary.</summary>
		public virtual void Clear()
		{
			map.Clear();
		}

		/// <summary>Returns all the keys of this PdfDictionary as a Set.</summary>
		/// <returns>Set of keys</returns>
		public virtual ICollection<PdfName> KeySet()
		{
			return map.Keys;
		}

		/// <summary>Returns all the values of this map in a Collection.</summary>
		/// <returns>a Collection holding all the values</returns>
		public virtual ICollection<PdfObject> Values()
		{
			return map.Values;
		}

		/// <summary>Returns a Set holding the key-value pairs as Map#Entry objects.</summary>
		/// <returns>a Set of Map.Entry objects</returns>
		public virtual ICollection<KeyValuePair<PdfName, PdfObject>> EntrySet()
		{
			return map;
		}

		public override byte GetType()
		{
			return DICTIONARY;
		}

		public override String ToString()
		{
			if (!IsFlushed())
			{
				String @string = "<<";
				foreach (KeyValuePair<PdfName, PdfObject> entry in EntrySet())
				{
					PdfIndirectReference indirectReference = entry.Value.GetIndirectReference();
					@string = @string + entry.Key.ToString() + " " + (indirectReference == null ? entry
						.Value.ToString() : indirectReference.ToString()) + " ";
				}
				@string += ">>";
				return @string;
			}
			else
			{
				return indirectReference.ToString();
			}
		}

		/// <summary>Creates clones of the dictionary in the current document.</summary>
		/// <remarks>
		/// Creates clones of the dictionary in the current document.
		/// It's possible to pass a list of keys to exclude when cloning.
		/// </remarks>
		/// <param name="excludeKeys">list of objects to exclude when cloning dictionary.</param>
		/// <returns>cloned dictionary.</returns>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		public virtual com.itextpdf.kernel.pdf.PdfDictionary Clone(IList<PdfName> excludeKeys
			)
		{
			IDictionary<PdfName, PdfObject> excluded = new SortedDictionary<PdfName, PdfObject
				>();
			foreach (PdfName key in excludeKeys)
			{
				PdfObject obj = map[key];
				if (obj != null)
				{
					excluded[key] = map.Remove(key);
				}
			}
			com.itextpdf.kernel.pdf.PdfDictionary dictionary = (com.itextpdf.kernel.pdf.PdfDictionary
				)Clone();
			map.AddAll(excluded);
			return dictionary;
		}

		/// <summary>Marks object to be saved as indirect.</summary>
		/// <param name="document">a document the indirect reference will belong to.</param>
		/// <returns>object itself.</returns>
		public override PdfObject MakeIndirect(PdfDocument document)
		{
			return (com.itextpdf.kernel.pdf.PdfDictionary)base.MakeIndirect(document);
		}

		/// <summary>Marks object to be saved as indirect.</summary>
		/// <param name="document">a document the indirect reference will belong to.</param>
		/// <returns>object itself.</returns>
		public override PdfObject MakeIndirect(PdfDocument document, PdfIndirectReference
			 reference)
		{
			return (com.itextpdf.kernel.pdf.PdfDictionary)base.MakeIndirect(document, reference
				);
		}

		/// <summary>Copies object to a specified document.</summary>
		/// <remarks>
		/// Copies object to a specified document.
		/// Works only for objects that are read from existing document, otherwise an exception is thrown.
		/// </remarks>
		/// <param name="document">document to copy object to.</param>
		/// <returns>copied object.</returns>
		public override PdfObject CopyTo(PdfDocument document)
		{
			return (com.itextpdf.kernel.pdf.PdfDictionary)base.CopyTo(document, true);
		}

		/// <summary>Copies object to a specified document.</summary>
		/// <remarks>
		/// Copies object to a specified document.
		/// Works only for objects that are read from existing document, otherwise an exception is thrown.
		/// </remarks>
		/// <param name="document">document to copy object to.</param>
		/// <param name="allowDuplicating">
		/// indicates if to allow copy objects which already have been copied.
		/// If object is associated with any indirect reference and allowDuplicating is false then already existing reference will be returned instead of copying object.
		/// If allowDuplicating is true then object will be copied and new indirect reference will be assigned.
		/// </param>
		/// <returns>copied object.</returns>
		public override PdfObject CopyTo(PdfDocument document, bool allowDuplicating)
		{
			return (com.itextpdf.kernel.pdf.PdfDictionary)base.CopyTo(document, allowDuplicating
				);
		}

		/// <summary>Copies dictionary to specified document.</summary>
		/// <remarks>
		/// Copies dictionary to specified document.
		/// It's possible to pass a list of keys to exclude when copying.
		/// </remarks>
		/// <param name="document">document to copy dictionary to.</param>
		/// <param name="excludeKeys">list of objects to exclude when copying dictionary.</param>
		/// <param name="allowDuplicating">
		/// 
		/// <see cref="PdfObject.CopyTo(PdfDocument, bool)"/>
		/// </param>
		/// <returns>copied dictionary.</returns>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		public virtual com.itextpdf.kernel.pdf.PdfDictionary CopyTo(PdfDocument document, 
			IList<PdfName> excludeKeys, bool allowDuplicating)
		{
			IDictionary<PdfName, PdfObject> excluded = new SortedDictionary<PdfName, PdfObject
				>();
			foreach (PdfName key in excludeKeys)
			{
				PdfObject obj = map[key];
				if (obj != null)
				{
					excluded[key] = map.Remove(key);
				}
			}
			com.itextpdf.kernel.pdf.PdfDictionary dictionary = ((com.itextpdf.kernel.pdf.PdfDictionary
				)CopyTo(document, allowDuplicating));
			map.AddAll(excluded);
			return dictionary;
		}

		/// <param name="asDirect">true is to extract direct object always.</param>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		public virtual PdfObject Get(PdfName key, bool asDirect)
		{
			if (!asDirect)
			{
				return map[key];
			}
			else
			{
				PdfObject obj = map[key];
				if (obj != null && obj.GetType() == INDIRECT_REFERENCE)
				{
					return ((PdfIndirectReference)obj).GetRefersTo(true);
				}
				else
				{
					return obj;
				}
			}
		}

		/// <summary>This method merges different fields from two dictionaries into the current one
		/// 	</summary>
		/// <param name="other">a dictionary whose fields should be merged into the current dictionary.
		/// 	</param>
		public virtual void MergeDifferent(com.itextpdf.kernel.pdf.PdfDictionary other)
		{
			foreach (PdfName key in other.KeySet())
			{
				if (!ContainsKey(key))
				{
					Put(key, other.Get(key));
				}
			}
		}

		protected internal override PdfObject NewInstance()
		{
			return new com.itextpdf.kernel.pdf.PdfDictionary();
		}

		protected internal override void CopyContent(PdfObject from, PdfDocument document
			)
		{
			base.CopyContent(from, document);
			com.itextpdf.kernel.pdf.PdfDictionary dictionary = (com.itextpdf.kernel.pdf.PdfDictionary
				)from;
			foreach (KeyValuePair<PdfName, PdfObject> entry in dictionary.EntrySet())
			{
				map[entry.Key] = entry.Value.ProcessCopying(document, false);
			}
		}

		/// <summary>Release content of PdfDictionary.</summary>
		protected internal virtual void ReleaseContent()
		{
			map = null;
		}
	}
}
