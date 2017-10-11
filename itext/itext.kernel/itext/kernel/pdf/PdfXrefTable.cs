/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using System.Text;
using iText.IO.Log;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.Kernel.Pdf {
    internal class PdfXrefTable {
        private const int INITIAL_CAPACITY = 32;

        private const int MAX_GENERATION = 65535;

        private static readonly byte[] freeXRefEntry = ByteUtils.GetIsoBytes("f \n");

        private static readonly byte[] inUseXRefEntry = ByteUtils.GetIsoBytes("n \n");

        private PdfIndirectReference[] xref;

        private int count = 0;

        /// <summary>
        /// Free references linked list is stored in a form of a map, where:
        /// key - free reference obj number;
        /// value - previous item in the linked list of free references for the object denoted by the key.
        /// </summary>
        private readonly SortedDictionary<int, PdfIndirectReference> freeReferencesLinkedList;

        public PdfXrefTable()
            : this(INITIAL_CAPACITY) {
        }

        public PdfXrefTable(int capacity) {
            if (capacity < 1) {
                capacity = INITIAL_CAPACITY;
            }
            xref = new PdfIndirectReference[capacity];
            freeReferencesLinkedList = new SortedDictionary<int, PdfIndirectReference>();
            Add(((PdfIndirectReference)new PdfIndirectReference(null, 0, MAX_GENERATION, 0).SetState(PdfObject.FREE)));
        }

        /// <summary>Adds indirect reference to list of indirect objects.</summary>
        /// <param name="reference">indirect reference to add.</param>
        public virtual PdfIndirectReference Add(PdfIndirectReference reference) {
            if (reference == null) {
                return null;
            }
            int objNr = reference.GetObjNumber();
            this.count = Math.Max(this.count, objNr);
            EnsureCount(objNr);
            xref[objNr] = reference;
            return reference;
        }

        public virtual int Size() {
            return count + 1;
        }

        public virtual PdfIndirectReference Get(int index) {
            if (index > count) {
                return null;
            }
            return xref[index];
        }

        internal virtual void InitFreeReferencesList(PdfDocument pdfDocument) {
            freeReferencesLinkedList.Clear();
            xref[0].SetState(PdfObject.FREE);
            // ensure zero object is free
            SortedSet<int> freeReferences = new SortedSet<int>();
            for (int i = 1; i < Size(); ++i) {
                PdfIndirectReference @ref = xref[i];
                if (@ref == null || @ref.IsFree()) {
                    freeReferences.Add(i);
                }
            }
            PdfIndirectReference prevFreeRef = xref[0];
            while (!freeReferences.IsEmpty<int>()) {
                int currFreeRefObjNr = -1;
                if (prevFreeRef.GetOffset() <= int.MaxValue) {
                    currFreeRefObjNr = (int)prevFreeRef.GetOffset();
                }
                if (!freeReferences.Contains(currFreeRefObjNr) || xref[currFreeRefObjNr] == null) {
                    break;
                }
                freeReferencesLinkedList.Put(currFreeRefObjNr, prevFreeRef);
                prevFreeRef = xref[currFreeRefObjNr];
                freeReferences.Remove(currFreeRefObjNr);
            }
            while (!freeReferences.IsEmpty<int>()) {
                int next = freeReferences.PollFirst();
                if (xref[next] == null) {
                    if (pdfDocument.properties.appendMode) {
                        continue;
                    }
                    xref[next] = ((PdfIndirectReference)((PdfIndirectReference)new PdfIndirectReference(pdfDocument, next, 0).
                        SetState(PdfObject.FREE)).SetState(PdfObject.MODIFIED));
                }
                else {
                    if (xref[next].GetGenNumber() == MAX_GENERATION && xref[next].GetOffset() == 0) {
                        continue;
                    }
                }
                if (prevFreeRef.GetOffset() != (long)next) {
                    ((PdfIndirectReference)prevFreeRef.SetState(PdfObject.MODIFIED)).SetOffset(next);
                }
                freeReferencesLinkedList.Put(next, prevFreeRef);
                prevFreeRef = xref[next];
            }
            if (prevFreeRef.GetOffset() != 0) {
                ((PdfIndirectReference)prevFreeRef.SetState(PdfObject.MODIFIED)).SetOffset(0);
            }
            freeReferencesLinkedList.Put(0, prevFreeRef);
        }

        //For Object streams
        internal virtual PdfIndirectReference CreateNewIndirectReference(PdfDocument document) {
            PdfIndirectReference reference = new PdfIndirectReference(document, ++count);
            Add(reference);
            return ((PdfIndirectReference)reference.SetState(PdfObject.MODIFIED));
        }

        /// <summary>Creates next available indirect reference.</summary>
        /// <returns>created indirect reference.</returns>
        protected internal virtual PdfIndirectReference CreateNextIndirectReference(PdfDocument document) {
            PdfIndirectReference reference = new PdfIndirectReference(document, ++count);
            Add(reference);
            return ((PdfIndirectReference)reference.SetState(PdfObject.MODIFIED));
        }

        protected internal virtual void FreeReference(PdfIndirectReference reference) {
            if (reference.IsFree()) {
                return;
            }
            if (reference.CheckState(PdfObject.MUST_BE_FLUSHED)) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.PdfXrefTable));
                logger.Error(iText.IO.LogMessageConstant.INDIRECT_REFERENCE_USED_IN_FLUSHED_OBJECT_MADE_FREE);
                return;
            }
            if (reference.CheckState(PdfObject.FLUSHED)) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.PdfXrefTable));
                logger.Error(iText.IO.LogMessageConstant.ALREADY_FLUSHED_INDIRECT_OBJECT_MADE_FREE);
                return;
            }
            ((PdfIndirectReference)reference.SetState(PdfObject.FREE)).SetState(PdfObject.MODIFIED);
            AppendNewRefToFreeList(reference);
            if (reference.refersTo != null) {
                reference.refersTo.SetIndirectReference(null).SetState(PdfObject.MUST_BE_INDIRECT);
                reference.refersTo = null;
            }
            if (reference.GetGenNumber() < MAX_GENERATION) {
                reference.genNr++;
            }
        }

        protected internal virtual void SetCapacity(int capacity) {
            if (capacity > xref.Length) {
                ExtendXref(capacity);
            }
        }

        /// <summary>Writes cross reference table and trailer to PDF.</summary>
        /// <exception cref="System.IO.IOException"/>
        protected internal virtual void WriteXrefTableAndTrailer(PdfDocument document, PdfObject fileId, PdfObject
             crypto) {
            PdfWriter writer = document.GetWriter();
            if (!document.properties.appendMode) {
                for (int i = count; i > 0; --i) {
                    PdfIndirectReference lastRef = xref[i];
                    if (lastRef == null || lastRef.IsFree()) {
                        RemoveFreeRefFromList(i);
                        --count;
                    }
                    else {
                        break;
                    }
                }
            }
            IList<int> sections = new List<int>();
            int first = 0;
            int len = 0;
            for (int i = 0; i < Size(); i++) {
                PdfIndirectReference reference = xref[i];
                if (document.properties.appendMode && reference != null && !reference.CheckState(PdfObject.MODIFIED)) {
                    reference = null;
                }
                if (reference == null) {
                    if (len > 0) {
                        sections.Add(first);
                        sections.Add(len);
                    }
                    len = 0;
                }
                else {
                    if (len > 0) {
                        len++;
                    }
                    else {
                        first = i;
                        len = 1;
                    }
                }
            }
            if (len > 0) {
                sections.Add(first);
                sections.Add(len);
            }
            if (document.properties.appendMode && sections.Count == 0) {
                // no modifications.
                xref = null;
                return;
            }
            long startxref = writer.GetCurrentPos();
            if (writer.IsFullCompression()) {
                PdfStream xrefStream = ((PdfStream)new PdfStream().MakeIndirect(document));
                xrefStream.MakeIndirect(document);
                xrefStream.Put(PdfName.Type, PdfName.XRef);
                xrefStream.Put(PdfName.ID, fileId);
                if (crypto != null) {
                    xrefStream.Put(PdfName.Encrypt, crypto);
                }
                xrefStream.Put(PdfName.Size, new PdfNumber(this.Size()));
                List<PdfObject> tmpArray = new List<PdfObject>(3);
                tmpArray.Add(new PdfNumber(1));
                tmpArray.Add(new PdfNumber(4));
                tmpArray.Add(new PdfNumber(2));
                xrefStream.Put(PdfName.W, new PdfArray(tmpArray));
                xrefStream.Put(PdfName.Info, document.GetDocumentInfo().GetPdfObject());
                xrefStream.Put(PdfName.Root, document.GetCatalog().GetPdfObject());
                PdfArray index = new PdfArray();
                foreach (int? section in sections) {
                    index.Add(new PdfNumber((int)section));
                }
                if (document.properties.appendMode) {
                    PdfNumber lastXref = new PdfNumber(document.reader.GetLastXref());
                    xrefStream.Put(PdfName.Prev, lastXref);
                }
                xrefStream.Put(PdfName.Index, index);
                iText.Kernel.Pdf.PdfXrefTable xrefTable = document.GetXref();
                for (int k = 0; k < sections.Count; k += 2) {
                    first = (int)sections[k];
                    len = (int)sections[k + 1];
                    for (int i = first; i < first + len; i++) {
                        PdfIndirectReference reference = xrefTable.Get(i);
                        if (reference.IsFree()) {
                            xrefStream.GetOutputStream().Write(0);
                            System.Diagnostics.Debug.Assert(reference.GetOffset() < int.MaxValue);
                            xrefStream.GetOutputStream().Write(IntToBytes((int)reference.GetOffset()));
                            xrefStream.GetOutputStream().Write(ShortToBytes(reference.GetGenNumber()));
                        }
                        else {
                            if (reference.GetObjStreamNumber() == 0) {
                                xrefStream.GetOutputStream().Write(1);
                                System.Diagnostics.Debug.Assert(reference.GetOffset() < int.MaxValue);
                                xrefStream.GetOutputStream().Write(IntToBytes((int)reference.GetOffset()));
                                xrefStream.GetOutputStream().Write(ShortToBytes(reference.GetGenNumber()));
                            }
                            else {
                                xrefStream.GetOutputStream().Write(2);
                                xrefStream.GetOutputStream().Write(IntToBytes(reference.GetObjStreamNumber()));
                                xrefStream.GetOutputStream().Write(ShortToBytes(reference.GetIndex()));
                            }
                        }
                    }
                }
                xrefStream.Flush();
            }
            else {
                writer.WriteString("xref\n");
                iText.Kernel.Pdf.PdfXrefTable xrefTable = document.GetXref();
                for (int k = 0; k < sections.Count; k += 2) {
                    first = (int)sections[k];
                    len = (int)sections[k + 1];
                    writer.WriteInteger(first).WriteSpace().WriteInteger(len).WriteByte((byte)'\n');
                    for (int i = first; i < first + len; i++) {
                        PdfIndirectReference reference = xrefTable.Get(i);
                        StringBuilder off = new StringBuilder("0000000000").Append(reference.GetOffset());
                        StringBuilder gen = new StringBuilder("00000").Append(reference.GetGenNumber());
                        writer.WriteString(off.JSubstring(off.Length - 10, off.Length)).WriteSpace().WriteString(gen.JSubstring(gen
                            .Length - 5, gen.Length)).WriteSpace();
                        if (reference.IsFree()) {
                            writer.WriteBytes(freeXRefEntry);
                        }
                        else {
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
                trailer.Put(PdfName.Size, new PdfNumber(this.Size()));
                trailer.Put(PdfName.ID, fileId);
                if (crypto != null) {
                    trailer.Put(PdfName.Encrypt, crypto);
                }
                writer.WriteString("trailer\n");
                if (document.properties.appendMode) {
                    PdfNumber lastXref = new PdfNumber(document.reader.GetLastXref());
                    trailer.Put(PdfName.Prev, lastXref);
                }
                writer.Write(document.GetTrailer());
                writer.Write('\n');
            }
            WriteKeyInfo(writer);
            writer.WriteString("startxref\n").WriteLong(startxref).WriteString("\n%%EOF\n");
            xref = null;
            freeReferencesLinkedList.Clear();
        }

        internal virtual void Clear() {
            for (int i = 1; i <= count; i++) {
                if (xref[i] != null && xref[i].IsFree()) {
                    continue;
                }
                xref[i] = null;
            }
            count = 1;
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal static void WriteKeyInfo(PdfWriter writer) {
            String platform = " for .NET";
            iText.Kernel.Version version = iText.Kernel.Version.GetInstance();
            String k = version.GetKey();
            if (k == null) {
                k = "iText";
            }
            writer.WriteString(MessageFormatUtil.Format("%{0}-{1}{2}\n", k, version.GetRelease(), platform));
        }

        private void AppendNewRefToFreeList(PdfIndirectReference reference) {
            reference.SetOffset(0);
            if (freeReferencesLinkedList.IsEmpty<int, PdfIndirectReference>()) {
                System.Diagnostics.Debug.Assert(false);
                // free references list is not initialized yet
                return;
            }
            PdfIndirectReference lastFreeRef = freeReferencesLinkedList.Get(0);
            ((PdfIndirectReference)lastFreeRef.SetState(PdfObject.MODIFIED)).SetOffset(reference.GetObjNumber());
            freeReferencesLinkedList.Put(reference.GetObjNumber(), lastFreeRef);
            freeReferencesLinkedList.Put(0, reference);
        }

        /// <summary>Removes indirect reference from free references linked list.</summary>
        /// <remarks>
        /// Removes indirect reference from free references linked list.
        /// It does not removes it from xref table and affects only the linked list formed by offset values of free references.
        /// </remarks>
        /// <param name="freeRefObjNr">
        /// object number of the reference to be removed.
        /// Removes the free reference with the least object number if this parameter is less than zero:
        /// this could be used for finding the next free reference for reusing.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="PdfIndirectReference"/>
        /// instance of the removed free reference corresponding to the object number
        /// passed as parameter.
        /// <see langword="null"/>
        /// - if given object number doesn't correspond to free reference or equals to zero.
        /// </returns>
        private PdfIndirectReference RemoveFreeRefFromList(int freeRefObjNr) {
            if (freeReferencesLinkedList.IsEmpty<int, PdfIndirectReference>()) {
                System.Diagnostics.Debug.Assert(false);
                // free references list is not initialized yet
                return null;
            }
            if (freeRefObjNr == 0) {
                return null;
            }
            if (freeRefObjNr < 0) {
                int? leastFreeRefObjNum = null;
                foreach (KeyValuePair<int, PdfIndirectReference> entry in freeReferencesLinkedList) {
                    if (entry.Key <= 0 || xref[entry.Key].GetGenNumber() >= MAX_GENERATION) {
                        continue;
                    }
                    leastFreeRefObjNum = entry.Key;
                    break;
                }
                if (leastFreeRefObjNum == null) {
                    return null;
                }
                freeRefObjNr = (int)leastFreeRefObjNum;
            }
            PdfIndirectReference freeRef = xref[freeRefObjNr];
            if (!freeRef.IsFree()) {
                return null;
            }
            PdfIndirectReference prevFreeRef = freeReferencesLinkedList.JRemove(freeRef.GetObjNumber());
            if (prevFreeRef != null) {
                freeReferencesLinkedList.Put((int)freeRef.GetOffset(), prevFreeRef);
                ((PdfIndirectReference)prevFreeRef.SetState(PdfObject.MODIFIED)).SetOffset(freeRef.GetOffset());
            }
            return freeRef;
        }

        private void EnsureCount(int count) {
            if (count >= xref.Length) {
                ExtendXref(count << 1);
            }
        }

        private void ExtendXref(int capacity) {
            PdfIndirectReference[] newXref = new PdfIndirectReference[capacity];
            System.Array.Copy(xref, 0, newXref, 0, xref.Length);
            xref = newXref;
        }

        private static byte[] ShortToBytes(int n) {
            return new byte[] { (byte)((n >> 8) & 0xFF), (byte)(n & 0xFF) };
        }

        private static byte[] IntToBytes(int n) {
            return new byte[] { (byte)((n >> 24) & 0xFF), (byte)((n >> 16) & 0xFF), (byte)((n >> 8) & 0xFF), (byte)(n 
                & 0xFF) };
        }
    }
}
