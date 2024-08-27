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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Actions.Events;
using iText.Kernel.Exceptions;
using iText.Kernel.Validation.Context;

namespace iText.Kernel.Pdf {
    /// <summary>A representation of a cross-referenced table of a PDF document.</summary>
    public class PdfXrefTable {
        private const int INITIAL_CAPACITY = 32;

        private const int MAX_GENERATION = 65535;

        /// <summary>The maximum offset in a cross-reference stream.</summary>
        /// <remarks>
        /// The maximum offset in a cross-reference stream. This is a limitation of the PDF specification.
        /// SPEC1.7: 7.5.4 Cross reference trailer
        /// <para />
        /// It states that the offset should be a 10-digit byte, so the maximum value is 9999999999.
        /// This is the max value that can be represented in 10 bytes.
        /// </remarks>
        private const long MAX_OFFSET_IN_CROSS_REFERENCE_STREAM = 9_999_999_999L;

        private static readonly byte[] freeXRefEntry = ByteUtils.GetIsoBytes("f \n");

        private static readonly byte[] inUseXRefEntry = ByteUtils.GetIsoBytes("n \n");

        private PdfIndirectReference[] xref;

        private int count = 0;

        private bool readingCompleted;

        private MemoryLimitsAwareHandler memoryLimitsAwareHandler;

        /// <summary>
        /// Free references linked list is stored in a form of a map, where:
        /// key - free reference obj number;
        /// value - previous item in the linked list of free references for the object denoted by the key.
        /// </summary>
        private readonly SortedDictionary<int, PdfIndirectReference> freeReferencesLinkedList;

        /// <summary>
        /// Creates a
        /// <see cref="PdfXrefTable"/>
        /// which will be used to store xref structure of the pdf document.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfXrefTable"/>
        /// which will be used to store xref structure of the pdf document.
        /// Capacity and
        /// <see cref="MemoryLimitsAwareHandler"/>
        /// instance would be set by default values.
        /// </remarks>
        public PdfXrefTable()
            : this(INITIAL_CAPACITY) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfXrefTable"/>
        /// which will be used to store xref structure of the pdf document.
        /// </summary>
        /// <param name="capacity">initial capacity of xref table.</param>
        public PdfXrefTable(int capacity)
            : this(capacity, null) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfXrefTable"/>
        /// which will be used to store xref structure of the pdf document.
        /// </summary>
        /// <param name="memoryLimitsAwareHandler">
        /// custom
        /// <see cref="MemoryLimitsAwareHandler"/>
        /// to set.
        /// </param>
        public PdfXrefTable(MemoryLimitsAwareHandler memoryLimitsAwareHandler)
            : this(INITIAL_CAPACITY, memoryLimitsAwareHandler) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfXrefTable"/>
        /// which will be used to store xref structure of the pdf document.
        /// </summary>
        /// <param name="capacity">initial capacity of xref table.</param>
        /// <param name="memoryLimitsAwareHandler">
        /// memoryLimitsAwareHandler custom
        /// <see cref="MemoryLimitsAwareHandler"/>
        /// to set.
        /// </param>
        public PdfXrefTable(int capacity, MemoryLimitsAwareHandler memoryLimitsAwareHandler) {
            if (capacity < 1) {
                capacity = memoryLimitsAwareHandler == null ? INITIAL_CAPACITY : Math.Min(INITIAL_CAPACITY, memoryLimitsAwareHandler
                    .GetMaxNumberOfElementsInXrefStructure());
            }
            this.memoryLimitsAwareHandler = memoryLimitsAwareHandler;
            if (this.memoryLimitsAwareHandler != null) {
                this.memoryLimitsAwareHandler.CheckIfXrefStructureExceedsTheLimit(capacity);
            }
            this.xref = new PdfIndirectReference[capacity];
            this.freeReferencesLinkedList = new SortedDictionary<int, PdfIndirectReference>();
            Add((PdfIndirectReference)new PdfIndirectReference(null, 0, MAX_GENERATION, 0).SetState(PdfObject.FREE));
        }

        /// <summary>
        /// Sets custom
        /// <see cref="MemoryLimitsAwareHandler"/>.
        /// </summary>
        /// <param name="memoryLimitsAwareHandler">instance to set.</param>
        public virtual void SetMemoryLimitsAwareHandler(MemoryLimitsAwareHandler memoryLimitsAwareHandler) {
            this.memoryLimitsAwareHandler = memoryLimitsAwareHandler;
        }

        /// <summary>Adds indirect reference to list of indirect objects.</summary>
        /// <param name="reference">indirect reference to add.</param>
        /// <returns>reference from param</returns>
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

        /// <summary>Get size of cross-reference table.</summary>
        /// <returns>amount of lines including zero-object</returns>
        public virtual int Size() {
            return count + 1;
        }

        /// <summary>Calculates a number of stored references to indirect objects.</summary>
        /// <returns>number of indirect objects</returns>
        public virtual int GetCountOfIndirectObjects() {
            int countOfIndirectObjects = 0;
            foreach (PdfIndirectReference @ref in xref) {
                if (@ref != null && !@ref.IsFree()) {
                    countOfIndirectObjects++;
                }
            }
            return countOfIndirectObjects;
        }

        /// <summary>Get appropriate reference to indirect object.</summary>
        /// <param name="index">is the index of required object</param>
        /// <returns>reference to object with the provided index</returns>
        public virtual PdfIndirectReference Get(int index) {
            if (index > count) {
                return null;
            }
            return xref[index];
        }

        /// <summary>Creates next available indirect reference.</summary>
        /// <param name="document">
        /// is the current
        /// <see cref="PdfDocument">document</see>
        /// </param>
        /// <returns>created indirect reference.</returns>
        protected internal virtual PdfIndirectReference CreateNextIndirectReference(PdfDocument document) {
            PdfIndirectReference reference = new PdfIndirectReference(document, ++count);
            Add(reference);
            return (PdfIndirectReference)reference.SetState(PdfObject.MODIFIED);
        }

        /// <summary>Set the reference to free state.</summary>
        /// <param name="reference">is a reference to be updated.</param>
        protected internal virtual void FreeReference(PdfIndirectReference reference) {
            if (reference.IsFree()) {
                return;
            }
            if (reference.CheckState(PdfObject.MUST_BE_FLUSHED)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfXrefTable));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.INDIRECT_REFERENCE_USED_IN_FLUSHED_OBJECT_MADE_FREE);
                return;
            }
            if (reference.CheckState(PdfObject.FLUSHED)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfXrefTable));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ALREADY_FLUSHED_INDIRECT_OBJECT_MADE_FREE);
                return;
            }
            reference.SetState(PdfObject.FREE).SetState(PdfObject.MODIFIED);
            AppendNewRefToFreeList(reference);
            if (reference.GetGenNumber() < MAX_GENERATION) {
                reference.genNr++;
            }
        }

        /// <summary>Gets the capacity of xref stream.</summary>
        /// <returns>the capacity of xref stream.</returns>
        protected internal virtual int GetCapacity() {
            return xref.Length;
        }

        /// <summary>Increase capacity of the array of indirect references.</summary>
        /// <param name="capacity">is a new capacity to set</param>
        protected internal virtual void SetCapacity(int capacity) {
            if (capacity > xref.Length) {
                ExtendXref(capacity);
            }
        }

        /// <summary>Writes cross reference table and trailer to PDF.</summary>
        /// <param name="document">
        /// is the current
        /// <see cref="PdfDocument">document</see>
        /// </param>
        /// <param name="fileId">field id</param>
        /// <param name="crypto">pdf encryption</param>
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
            PdfStream xrefStream = null;
            if (writer.IsFullCompression()) {
                xrefStream = new PdfStream();
                xrefStream.MakeIndirect(document);
            }
            IList<int> sections = CreateSections(document, false);
            bool noModifiedObjects = (sections.Count == 0) || (xrefStream != null && sections.Count == 2 && sections[0
                ] == count && sections[1] == 1);
            if (document.properties.appendMode && noModifiedObjects) {
                // No modifications in document
                xref = null;
                return;
            }
            document.CheckIsoConformance(new XrefTableValidationContext(this));
            long startxref = writer.GetCurrentPos();
            long xRefStmPos = -1;
            if (xrefStream != null) {
                xrefStream.Put(PdfName.Type, PdfName.XRef);
                xrefStream.Put(PdfName.ID, fileId);
                if (crypto != null) {
                    xrefStream.Put(PdfName.Encrypt, crypto);
                }
                xrefStream.Put(PdfName.Size, new PdfNumber(this.Size()));
                int offsetSize = GetOffsetSize(Math.Max(startxref, Size()));
                xrefStream.Put(PdfName.W, new PdfArray(JavaUtil.ArraysAsList((PdfObject)new PdfNumber(1), new PdfNumber(offsetSize
                    ), new PdfNumber(2))));
                xrefStream.Put(PdfName.Info, document.GetDocumentInfo().GetPdfObject());
                xrefStream.Put(PdfName.Root, document.GetCatalog().GetPdfObject());
                PdfArray index = new PdfArray();
                foreach (int? section in sections) {
                    index.Add(new PdfNumber((int)section));
                }
                if (document.properties.appendMode && !document.reader.hybridXref) {
                    // "not meaningful in hybrid-reference files"
                    PdfNumber lastXref = new PdfNumber(document.reader.GetLastXref());
                    xrefStream.Put(PdfName.Prev, lastXref);
                }
                xrefStream.Put(PdfName.Index, index);
                xrefStream.GetIndirectReference().SetOffset(startxref);
                iText.Kernel.Pdf.PdfXrefTable xrefTable = document.GetXref();
                for (int k = 0; k < sections.Count; k += 2) {
                    int first = (int)sections[k];
                    int len = (int)sections[k + 1];
                    for (int i = first; i < first + len; i++) {
                        PdfIndirectReference reference = xrefTable.Get(i);
                        if (reference.IsFree()) {
                            xrefStream.GetOutputStream().Write(0);
                            xrefStream.GetOutputStream().Write(reference.GetOffset(), offsetSize);
                            xrefStream.GetOutputStream().Write(reference.GetGenNumber(), 2);
                        }
                        else {
                            if (reference.GetObjStreamNumber() == 0) {
                                xrefStream.GetOutputStream().Write(1);
                                xrefStream.GetOutputStream().Write(reference.GetOffset(), offsetSize);
                                xrefStream.GetOutputStream().Write(reference.GetGenNumber(), 2);
                            }
                            else {
                                xrefStream.GetOutputStream().Write(2);
                                xrefStream.GetOutputStream().Write(reference.GetObjStreamNumber(), offsetSize);
                                xrefStream.GetOutputStream().Write(reference.GetIndex(), 2);
                            }
                        }
                    }
                }
                xrefStream.Flush();
                xRefStmPos = startxref;
            }
            // For documents with hybrid cross-reference table, i.e. containing xref streams as well as regular xref sections,
            // we write additional regular xref section at the end of the document because the /Prev reference from
            // xref stream to a regular xref section doesn't seem to be valid
            bool needsRegularXref = !writer.IsFullCompression() || (document.properties.appendMode && document.reader.
                hybridXref);
            if (needsRegularXref) {
                startxref = writer.GetCurrentPos();
                writer.WriteString("xref\n");
                iText.Kernel.Pdf.PdfXrefTable xrefTable = document.GetXref();
                if (xRefStmPos != -1) {
                    // Get rid of all objects from object stream. This is done for hybrid documents
                    sections = CreateSections(document, true);
                }
                for (int k = 0; k < sections.Count; k += 2) {
                    int first = (int)sections[k];
                    int len = (int)sections[k + 1];
                    writer.WriteInteger(first).WriteSpace().WriteInteger(len).WriteByte((byte)'\n');
                    for (int i = first; i < first + len; i++) {
                        PdfIndirectReference reference = xrefTable.Get(i);
                        if (reference.GetOffset() > MAX_OFFSET_IN_CROSS_REFERENCE_STREAM) {
                            throw new PdfException(KernelExceptionMessageConstant.XREF_HAS_AN_ENTRY_WITH_TOO_BIG_OFFSET);
                        }
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
                if (xRefStmPos != -1) {
                    trailer.Put(PdfName.XRefStm, new PdfNumber(xRefStmPos));
                }
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
            EventManager.GetInstance().OnEvent(new AddFingerPrintEvent(document));
            writer.WriteString("startxref\n").WriteLong(startxref).WriteString("\n%%EOF\n");
            xref = null;
            freeReferencesLinkedList.Clear();
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Change the state of the cross-reference table to mark that reading of the document
        /// was completed.
        /// </summary>
        internal virtual void MarkReadingCompleted() {
            readingCompleted = true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Change the state of the cross-reference table to unmark that reading of the document
        /// was completed.
        /// </summary>
        internal virtual void UnmarkReadingCompleted() {
            readingCompleted = false;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Check if reading of the document was completed.</summary>
        /// <returns>true if reading was completed and false otherwise</returns>
        internal virtual bool IsReadingCompleted() {
            return readingCompleted;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set up appropriate state for the free references list.</summary>
        /// <param name="pdfDocument">
        /// is the current
        /// <see cref="PdfDocument">document</see>
        /// </param>
        internal virtual void InitFreeReferencesList(PdfDocument pdfDocument) {
            freeReferencesLinkedList.Clear();
            // ensure zero object is free
            xref[0].SetState(PdfObject.FREE);
            SortedSet<int> freeReferences = new SortedSet<int>();
            for (int i = 1; i < Size() && i < xref.Length; ++i) {
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
                    xref[next] = (PdfIndirectReference)new PdfIndirectReference(pdfDocument, next, 0).SetState(PdfObject.FREE)
                        .SetState(PdfObject.MODIFIED);
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Method is used for object streams to avoid reuse existed references.</summary>
        /// <param name="document">
        /// is the current
        /// <see cref="PdfDocument">document</see>
        /// </param>
        /// <returns>created indirect reference to the object stream</returns>
        internal virtual PdfIndirectReference CreateNewIndirectReference(PdfDocument document) {
            PdfIndirectReference reference = new PdfIndirectReference(document, ++count);
            Add(reference);
            return (PdfIndirectReference)reference.SetState(PdfObject.MODIFIED);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Clear the state of the cross-reference table without free references removal.</summary>
        internal virtual void Clear() {
            for (int i = 1; i <= count; i++) {
                if (xref[i] != null && xref[i].IsFree()) {
                    continue;
                }
                xref[i] = null;
            }
            count = 1;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Clear the state of the cross-reference table including free references.</summary>
        internal virtual void ClearAllReferences() {
            for (int i = 1; i <= count; i++) {
                xref[i] = null;
            }
            count = 1;
        }
//\endcond

        private IList<int> CreateSections(PdfDocument document, bool dropObjectsFromObjectStream) {
            IList<int> sections = new List<int>();
            int first = 0;
            int len = 0;
            for (int i = 0; i < Size(); i++) {
                PdfIndirectReference reference = xref[i];
                if (document.properties.appendMode && reference != null && (!reference.CheckState(PdfObject.MODIFIED) || (
                    dropObjectsFromObjectStream && reference.GetObjStreamNumber() != 0))) {
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
            return sections;
        }

        /// <summary>Gets size of the offset.</summary>
        /// <remarks>Gets size of the offset. Max size is 2^40, i.e. 1 Tb.</remarks>
        private int GetOffsetSize(long startxref) {
            System.Diagnostics.Debug.Assert(startxref >= 0 && startxref < (1L << 40));
            //initial size = 5 bytes. It is 1 Tb. Shall be enough.
            int size = 5;
            long mask = unchecked((long)(0xff00000000L));
            for (; size > 1; size--) {
                if ((mask & startxref) != 0) {
                    break;
                }
                // there is no need to use >>> because mask is positive
                mask >>= 8;
            }
            return size;
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
            if (this.memoryLimitsAwareHandler != null) {
                this.memoryLimitsAwareHandler.CheckIfXrefStructureExceedsTheLimit(capacity);
            }
            PdfIndirectReference[] newXref = new PdfIndirectReference[capacity];
            Array.Copy(this.xref, 0, newXref, 0, this.xref.Length);
            this.xref = newXref;
        }
    }
}
