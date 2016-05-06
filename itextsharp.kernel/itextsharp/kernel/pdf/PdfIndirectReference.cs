/*
$Id: c7506ee0286cbddf4c8bb77aae95184362c5b7d9 $

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
using System.Text;

namespace com.itextpdf.kernel.pdf
{
	public class PdfIndirectReference : PdfObject, IComparable<com.itextpdf.kernel.pdf.PdfIndirectReference
		>
	{
		private const long serialVersionUID = -8293603068792908601L;

		private const int LENGTH_OF_INDIRECTS_CHAIN = 31;

		/// <summary>Object number.</summary>
		protected internal readonly int objNr;

		/// <summary>Object generation.</summary>
		protected internal int genNr;

		/// <summary>PdfObject that current PdfIndirectReference instance refers to.</summary>
		protected internal PdfObject refersTo = null;

		/// <summary>Indirect reference number of object stream containing refersTo object.</summary>
		/// <remarks>
		/// Indirect reference number of object stream containing refersTo object.
		/// If refersTo is not placed into object stream - objectStreamNumber = 0.
		/// </remarks>
		protected internal int objectStreamNumber = 0;

		/// <summary>
		/// Offset in a document of the
		/// <c>refersTo</c>
		/// object.
		/// If the object placed into object stream then it is an object index inside object stream.
		/// </summary>
		protected internal long offsetOrIndex = 0;

		/// <summary>PdfDocument object belongs to.</summary>
		/// <remarks>PdfDocument object belongs to. For direct objects it is null.</remarks>
		protected internal PdfDocument pdfDocument = null;

		protected internal PdfIndirectReference(PdfDocument doc, int objNr)
			: this(doc, objNr, 0)
		{
		}

		protected internal PdfIndirectReference(PdfDocument doc, int objNr, int genNr)
			: base()
		{
			this.pdfDocument = doc;
			this.objNr = objNr;
			this.genNr = genNr;
		}

		protected internal PdfIndirectReference(PdfDocument doc, int objNr, int genNr, long
			 offset)
			: base()
		{
			this.pdfDocument = doc;
			this.objNr = objNr;
			this.genNr = genNr;
			this.offsetOrIndex = offset;
			System.Diagnostics.Debug.Assert(offset >= 0);
		}

		public virtual int GetObjNumber()
		{
			return objNr;
		}

		public virtual int GetGenNumber()
		{
			return genNr;
		}

		public virtual PdfObject GetRefersTo()
		{
			return GetRefersTo(true);
		}

		/// <summary>Gets direct object and try to resolve indirects chain.</summary>
		/// <remarks>
		/// Gets direct object and try to resolve indirects chain.
		/// <p>
		/// Note: If chain of references has length of more than 32,
		/// this method return 31st reference in chain.
		/// </p>
		/// </remarks>
		public virtual PdfObject GetRefersTo(bool recursively)
		{
			if (!recursively)
			{
				if (refersTo == null && !CheckState(FLUSHED) && !CheckState(MODIFIED) && GetReader
					() != null)
				{
					refersTo = GetReader().ReadObject(this);
				}
				return refersTo;
			}
			else
			{
				PdfObject currentRefersTo = GetRefersTo(false);
				for (int i = 0; i < LENGTH_OF_INDIRECTS_CHAIN; i++)
				{
					if (currentRefersTo is com.itextpdf.kernel.pdf.PdfIndirectReference)
					{
						currentRefersTo = ((com.itextpdf.kernel.pdf.PdfIndirectReference)currentRefersTo)
							.GetRefersTo(false);
					}
					else
					{
						break;
					}
				}
				return currentRefersTo;
			}
		}

		protected internal virtual void SetRefersTo(PdfObject refersTo)
		{
			this.refersTo = refersTo;
		}

		public virtual int GetObjStreamNumber()
		{
			return objectStreamNumber;
		}

		/// <summary>Gets refersTo object offset in a document.</summary>
		/// <returns>object offset in a document. If refersTo object is in object stream then -1.
		/// 	</returns>
		public virtual long GetOffset()
		{
			return objectStreamNumber == 0 ? offsetOrIndex : -1;
		}

		/// <summary>Gets refersTo object index in the object stream.</summary>
		/// <returns>object index in a document. If refersTo object is not in object stream then -1.
		/// 	</returns>
		public virtual int GetIndex()
		{
			return objectStreamNumber == 0 ? -1 : (int)offsetOrIndex;
		}

		public override bool Equals(Object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || GetClass() != o.GetClass())
			{
				return false;
			}
			com.itextpdf.kernel.pdf.PdfIndirectReference that = (com.itextpdf.kernel.pdf.PdfIndirectReference
				)o;
			return objNr == that.objNr && genNr == that.genNr;
		}

		public override int GetHashCode()
		{
			int result = objNr;
			result = 31 * result + genNr;
			return result;
		}

		public virtual int CompareTo(com.itextpdf.kernel.pdf.PdfIndirectReference o)
		{
			if (objNr == o.objNr)
			{
				if (genNr == o.genNr)
				{
					return 0;
				}
				return (genNr > o.genNr) ? 1 : -1;
			}
			return (objNr > o.objNr) ? 1 : -1;
		}

		public override byte GetType()
		{
			return INDIRECT_REFERENCE;
		}

		public virtual PdfDocument GetDocument()
		{
			return pdfDocument;
		}

		/// <summary>Releases indirect reference from the document.</summary>
		/// <remarks>
		/// Releases indirect reference from the document. Remove link to the referenced indirect object.
		/// <p>
		/// Note: Be careful when using this method. Do not use this method for wrapper objects,
		/// it can be cause of errors.
		/// Free indirect reference could be reused for a new indirect object.
		/// </p>
		/// </remarks>
		public virtual void SetFree()
		{
			GetDocument().GetXref().FreeReference(this);
		}

		public override String ToString()
		{
			StringBuilder states = new StringBuilder(" ");
			if (CheckState(FREE))
			{
				states.Append("Free; ");
			}
			if (CheckState(MODIFIED))
			{
				states.Append("Modified; ");
			}
			if (CheckState(MUST_BE_FLUSHED))
			{
				states.Append("MustBeFlushed; ");
			}
			if (CheckState(READING))
			{
				states.Append("Reading; ");
			}
			if (CheckState(FLUSHED))
			{
				states.Append("Flushed; ");
			}
			if (CheckState(ORIGINAL_OBJECT_STREAM))
			{
				states.Append("OriginalObjectStream; ");
			}
			if (CheckState(FORBID_RELEASE))
			{
				states.Append("ForbidRelease; ");
			}
			if (CheckState(READ_ONLY))
			{
				states.Append("ReadOnly; ");
			}
			return String.Format("{0} {1} R{2}", GetObjNumber(), GetGenNumber(), states.Substring
				(0, states.Length - 1));
		}

		/// <summary>Gets a PdfWriter associated with the document object belongs to.</summary>
		/// <returns>PdfWriter.</returns>
		protected internal virtual PdfWriter GetWriter()
		{
			if (GetDocument() != null)
			{
				return GetDocument().GetWriter();
			}
			return null;
		}

		/// <summary>Gets a PdfReader associated with the document object belongs to.</summary>
		/// <returns>PdfReader.</returns>
		protected internal virtual PdfReader GetReader()
		{
			if (GetDocument() != null)
			{
				return GetDocument().GetReader();
			}
			return null;
		}

		// NOTE In append mode object could be OriginalObjectStream, but not Modified,
		// so information about this reference would not be added to the new Cross-Reference table.
		// In stamp mode without append the reference will be free.
		protected internal virtual bool IsFree()
		{
			return CheckState(FREE) || CheckState(ORIGINAL_OBJECT_STREAM);
		}

		protected internal override PdfObject NewInstance()
		{
			return PdfNull.PDF_NULL;
		}

		protected internal override void CopyContent(PdfObject from, PdfDocument document
			)
		{
		}

		/// <summary>Sets special states of current object.</summary>
		/// <param name="state">special flag of current object</param>
		protected internal override PdfObject SetState(short state)
		{
			return (com.itextpdf.kernel.pdf.PdfIndirectReference)base.SetState(state);
		}

		internal virtual void SetObjStreamNumber(int objectStreamNumber)
		{
			this.objectStreamNumber = objectStreamNumber;
		}

		internal virtual void SetIndex(long index)
		{
			this.offsetOrIndex = index;
		}

		internal virtual void SetOffset(long offset)
		{
			this.offsetOrIndex = offset;
			this.objectStreamNumber = 0;
		}

		internal virtual void FixOffset(long offset)
		{
			if (!IsFree())
			{
				this.offsetOrIndex = offset;
			}
		}
	}
}
