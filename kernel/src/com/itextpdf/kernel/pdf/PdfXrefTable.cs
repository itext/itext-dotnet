/*
$Id: fec7e070f5ef1a91e216205acd52f747c47d2359 $

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
using com.itextpdf.io.source;
using com.itextpdf.kernel;
using java.text;

namespace com.itextpdf.kernel.pdf
{
	internal class PdfXrefTable
	{
		private const long serialVersionUID = 4171655392492002944L;

		private const int INITIAL_CAPACITY = 32;

		private const int MAX_GENERATION = 65535;

		private static readonly DecimalFormat objectOffsetFormatter = new DecimalFormat("0000000000"
			);

		private static readonly DecimalFormat objectGenerationFormatter = new DecimalFormat
			("00000");

		private static readonly byte[] freeXRefEntry = ByteUtils.GetIsoBytes("f \n");

		private static readonly byte[] inUseXRefEntry = ByteUtils.GetIsoBytes("n \n");

		private PdfIndirectReference[] xref;

		private int count = 0;

		private readonly SortedSet<int> freeReferences;

		public PdfXrefTable()
			: this(INITIAL_CAPACITY)
		{
		}

		public PdfXrefTable(int capacity)
		{
			if (capacity < 1)
			{
				capacity = INITIAL_CAPACITY;
			}
			xref = new PdfIndirectReference[capacity];
			freeReferences = new SortedSet<int>();
			Add(((PdfIndirectReference)new PdfIndirectReference(null, 0, MAX_GENERATION, 0).SetState
				(PdfObject.FREE)));
		}

		/// <summary>Adds indirect reference to list of indirect objects.</summary>
		/// <param name="reference">indirect reference to add.</param>
		public virtual PdfIndirectReference Add(PdfIndirectReference reference)
		{
			if (reference == null)
			{
				return null;
			}
			int objNr = reference.GetObjNumber();
			this.count = Math.Max(this.count, objNr);
			EnsureCount(objNr);
			xref[objNr] = reference;
			return reference;
		}

		public virtual int Size()
		{
			return count + 1;
		}

		public virtual PdfIndirectReference Get(int index)
		{
			if (index > count)
			{
				return null;
			}
			return xref[index];
		}

		/// <summary>Creates next available indirect reference.</summary>
		/// <returns>created indirect reference.</returns>
		protected internal virtual PdfIndirectReference CreateNextIndirectReference(PdfDocument
			 document)
		{
			PdfIndirectReference reference;
			if (freeReferences.Count > 0)
			{
				int num = freeReferences.PollFirst();
				reference = xref[num];
				if (reference == null)
				{
					reference = new PdfIndirectReference(document, num);
					xref[num] = reference;
				}
				reference.SetOffset(0);
				reference.ClearState(PdfObject.FREE);
			}
			else
			{
				reference = new PdfIndirectReference(document, ++count);
				Add(reference);
			}
			return ((PdfIndirectReference)reference.SetState(PdfObject.MODIFIED));
		}

		protected internal virtual void FreeReference(PdfIndirectReference reference)
		{
			reference.SetOffset(0);
			reference.SetState(PdfObject.FREE);
			if (!reference.CheckState(PdfObject.FLUSHED))
			{
				if (reference.refersTo != null)
				{
					reference.refersTo.SetIndirectReference(null).SetState(PdfObject.MUST_BE_INDIRECT
						);
					reference.refersTo = null;
				}
				if (reference.GetGenNumber() < MAX_GENERATION)
				{
					freeReferences.Add(reference.GetObjNumber());
					xref[reference.GetObjNumber()] = null;
				}
			}
		}

		protected internal virtual void SetCapacity(int capacity)
		{
			if (capacity > xref.Length)
			{
				ExtendXref(capacity);
			}
		}

		/// <summary>Writes cross reference table and trailer to PDF.</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		protected internal virtual void WriteXrefTableAndTrailer(PdfDocument document, PdfObject
			 fileId, PdfObject crypto)
		{
			PdfWriter writer = document.GetWriter();
			if (document.IsAppendMode())
			{
				// Increment generation number for all freed references.
				foreach (int objNr in freeReferences)
				{
					xref[objNr].genNr++;
				}
			}
			else
			{
				foreach (int objNr in freeReferences)
				{
					xref[objNr] = null;
				}
			}
			freeReferences.Clear();
			IList<int> sections = new List<int>();
			int first = 0;
			int len = 1;
			if (document.IsAppendMode())
			{
				first = 1;
				len = 0;
			}
			for (int i = 1; i < Size(); i++)
			{
				PdfIndirectReference reference = xref[i];
				if (reference != null)
				{
					if ((document.properties.appendMode && !reference.CheckState(PdfObject.MODIFIED))
						 || (reference.IsFree() && reference.GetGenNumber() == 0) || (!reference.CheckState
						(PdfObject.FLUSHED)))
					{
						reference = null;
					}
				}
				if (reference == null)
				{
					if (len > 0)
					{
						sections.Add(first);
						sections.Add(len);
					}
					len = 0;
				}
				else
				{
					if (len > 0)
					{
						len++;
					}
					else
					{
						first = i;
						len = 1;
					}
				}
			}
			if (len > 0)
			{
				sections.Add(first);
				sections.Add(len);
			}
			if (document.properties.appendMode && sections.Count == 0)
			{
				// no modifications.
				xref = null;
				return;
			}
			int size = sections[sections.Count - 2] + sections[sections.Count - 1];
			long startxref = writer.GetCurrentPos();
			if (writer.IsFullCompression())
			{
				PdfStream xrefStream = ((PdfStream)new PdfStream().MakeIndirect(document));
				xrefStream.MakeIndirect(document);
				xrefStream.Put(PdfName.Type, PdfName.XRef);
				xrefStream.Put(PdfName.ID, fileId);
				if (crypto != null)
				{
					xrefStream.Put(PdfName.Encrypt, crypto);
				}
				xrefStream.Put(PdfName.Size, new PdfNumber(size));
				xrefStream.Put(PdfName.W, new PdfArray(new _List_231()));
				xrefStream.Put(PdfName.Info, document.GetDocumentInfo().GetPdfObject());
				xrefStream.Put(PdfName.Root, document.GetCatalog().GetPdfObject());
				PdfArray index = new PdfArray();
				foreach (int section in sections)
				{
					index.Add(new PdfNumber(section));
				}
				if (document.properties.appendMode)
				{
					PdfNumber lastXref = new PdfNumber(document.reader.GetLastXref());
					xrefStream.Put(PdfName.Prev, lastXref);
				}
				xrefStream.Put(PdfName.Index, index);
				com.itextpdf.kernel.pdf.PdfXrefTable xref = document.GetXref();
				for (int k = 0; k < sections.Count; k += 2)
				{
					first = sections[k];
					len = sections[k + 1];
					for (int i_1 = first; i_1 < first + len; i_1++)
					{
						PdfIndirectReference reference = xref.Get(i_1);
						if (reference == null)
						{
							continue;
						}
						if (reference.IsFree())
						{
							xrefStream.GetOutputStream().Write(0);
							//NOTE The object number of the next free object should be at this position due to spec.
							xrefStream.GetOutputStream().Write(IntToBytes(0));
							xrefStream.GetOutputStream().Write(ShortToBytes(reference.GetGenNumber()));
						}
						else
						{
							if (reference.GetObjStreamNumber() == 0)
							{
								xrefStream.GetOutputStream().Write(1);
								System.Diagnostics.Debug.Assert(reference.GetOffset() < int.MaxValue);
								xrefStream.GetOutputStream().Write(IntToBytes((int)reference.GetOffset()));
								xrefStream.GetOutputStream().Write(ShortToBytes(reference.GetGenNumber()));
							}
							else
							{
								xrefStream.GetOutputStream().Write(2);
								xrefStream.GetOutputStream().Write(IntToBytes(reference.GetObjStreamNumber()));
								xrefStream.GetOutputStream().Write(ShortToBytes(reference.GetIndex()));
							}
						}
					}
				}
				xrefStream.Flush();
			}
			else
			{
				writer.WriteString("xref\n");
				com.itextpdf.kernel.pdf.PdfXrefTable xref = document.GetXref();
				for (int k = 0; k < sections.Count; k += 2)
				{
					first = sections[k];
					len = sections[k + 1];
					writer.WriteInteger(first).WriteSpace().WriteInteger(len).WriteByte(unchecked((byte
						)'\n'));
					for (int i_1 = first; i_1 < first + len; i_1++)
					{
						PdfIndirectReference reference = xref.Get(i_1);
						writer.WriteString(objectOffsetFormatter.Format(reference.GetOffset())).WriteSpace
							().WriteString(objectGenerationFormatter.Format(reference.GetGenNumber())).WriteSpace
							();
						if (reference.IsFree())
						{
							writer.WriteBytes(freeXRefEntry);
						}
						else
						{
							writer.WriteBytes(inUseXRefEntry);
						}
					}
				}
				PdfDictionary trailer = document.GetTrailer();
				// Remove all unused keys in case stamp mode in case original file has full compression, but destination file has not.
				trailer.Remove(PdfName.W);
				trailer.Remove(PdfName.Index);
				trailer.Remove(PdfName.Type);
				trailer.Remove(PdfName.Length);
				trailer.Put(PdfName.Size, new PdfNumber(size));
				trailer.Put(PdfName.ID, fileId);
				if (crypto != null)
				{
					trailer.Put(PdfName.Encrypt, crypto);
				}
				writer.WriteString("trailer\n");
				if (document.properties.appendMode)
				{
					PdfNumber lastXref = new PdfNumber(document.reader.GetLastXref());
					trailer.Put(PdfName.Prev, lastXref);
				}
				writer.Write(document.GetTrailer());
				writer.Write('\n');
			}
			WriteKeyInfo(writer);
			writer.WriteString("startxref\n").WriteLong(startxref).WriteString("\n%%EOF\n");
			xref = null;
		}

		private sealed class _List_231 : List<PdfObject>
		{
			public _List_231()
			{
				{
					this.Add(new PdfNumber(1));
					this.Add(new PdfNumber(4));
					this.Add(new PdfNumber(2));
				}
			}
		}

		internal virtual void Clear()
		{
			for (int i = 1; i <= count; i++)
			{
				if (xref[i] != null && xref[i].IsFree())
				{
					continue;
				}
				xref[i] = null;
			}
			count = 1;
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal static void WriteKeyInfo(PdfWriter writer)
		{
			Version version = Version.GetInstance();
			String k = version.GetKey();
			if (k == null)
			{
				k = "iText";
			}
			writer.WriteString(String.Format("%%%s-%s\n", k, version.GetRelease()));
		}

		private void EnsureCount(int count)
		{
			if (count >= xref.Length)
			{
				ExtendXref(count << 1);
			}
		}

		private void ExtendXref(int capacity)
		{
			PdfIndirectReference[] newXref = new PdfIndirectReference[capacity];
			System.Array.Copy(xref, 0, newXref, 0, xref.Length);
			xref = newXref;
		}

		private static byte[] ShortToBytes(int n)
		{
			return new byte[] { unchecked((byte)((n >> 8) & 0xFF)), unchecked((byte)(n & 0xFF
				)) };
		}

		private static byte[] IntToBytes(int n)
		{
			return new byte[] { unchecked((byte)((n >> 24) & 0xFF)), unchecked((byte)((n >> 16
				) & 0xFF)), unchecked((byte)((n >> 8) & 0xFF)), unchecked((byte)(n & 0xFF)) };
		}
	}
}
