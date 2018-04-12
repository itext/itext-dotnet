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
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>Internal helper class which is used to copy tag structure across documents.</summary>
    internal class StructureTreeCopier {
        private static IList<PdfName> ignoreKeysForCopy = new List<PdfName>();

        private static IList<PdfName> ignoreKeysForClone = new List<PdfName>();

        static StructureTreeCopier() {
            ignoreKeysForCopy.Add(PdfName.K);
            ignoreKeysForCopy.Add(PdfName.P);
            ignoreKeysForCopy.Add(PdfName.Pg);
            ignoreKeysForCopy.Add(PdfName.Obj);
            ignoreKeysForClone.Add(PdfName.K);
            ignoreKeysForClone.Add(PdfName.P);
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>
        /// .
        /// <br/><br/>
        /// NOTE: Works only for
        /// <c>PdfStructTreeRoot</c>
        /// that is read from the document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </summary>
        /// <param name="destDocument">document to copy structure to. Shall not be current document.</param>
        /// <param name="page2page">association between original page and copied page.</param>
        public static void CopyTo(PdfDocument destDocument, IDictionary<PdfPage, PdfPage> page2page, PdfDocument callingDocument
            ) {
            if (!destDocument.IsTagged()) {
                return;
            }
            CopyTo(destDocument, page2page, callingDocument, false);
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>
        /// and insert it in a specified position in the document.
        /// <br/><br/>
        /// NOTE: Works only for
        /// <c>PdfStructTreeRoot</c>
        /// that is read from the document opened in reading mode,
        /// otherwise an exception is thrown.
        /// <br/>
        /// Also, to insert a tagged page into existing tag structure, existing tag structure shouldn't be flushed, otherwise
        /// an exception may be raised.
        /// </summary>
        /// <param name="destDocument">document to copy structure to.</param>
        /// <param name="insertBeforePage">indicates where the structure to be inserted.</param>
        /// <param name="page2page">association between original page and copied page.</param>
        public static void CopyTo(PdfDocument destDocument, int insertBeforePage, IDictionary<PdfPage, PdfPage> page2page
            , PdfDocument callingDocument) {
            if (!destDocument.IsTagged()) {
                return;
            }
            // Here we separate the structure tree in two parts: struct elems that belong to the pages which indexes are
            // less then insertBeforePage and those struct elems that belong to other pages. Some elems might belong
            // to both parts and actually these are the ones that we are looking for.
            ICollection<PdfObject> firstPartElems = new HashSet<PdfObject>();
            PdfStructTreeRoot destStructTreeRoot = destDocument.GetStructTreeRoot();
            for (int i = 1; i < insertBeforePage; ++i) {
                PdfPage pageOfFirstHalf = destDocument.GetPage(i);
                ICollection<PdfMcr> pageMcrs = destStructTreeRoot.GetPageMarkedContentReferences(pageOfFirstHalf);
                if (pageMcrs != null) {
                    foreach (PdfMcr mcr in pageMcrs) {
                        firstPartElems.Add(mcr.GetPdfObject());
                        PdfDictionary top = AddAllParentsToSet(mcr, firstPartElems);
                        if (top.IsFlushed()) {
                            throw new PdfException(PdfException.TagFromTheExistingTagStructureIsFlushedCannotAddCopiedPageTags);
                        }
                    }
                }
            }
            IList<PdfDictionary> clonedTops = new List<PdfDictionary>();
            PdfArray tops = destStructTreeRoot.GetKidsObject();
            // Now we "walk" through all the elems which belong to the first part, and look for the ones that contain both
            // kids from first and second part. We clone found elements and move kids from the second part to cloned elems.
            int lastTopBefore = 0;
            for (int i = 0; i < tops.Size(); ++i) {
                PdfDictionary top = tops.GetAsDictionary(i);
                if (firstPartElems.Contains(top)) {
                    lastTopBefore = i;
                    StructureTreeCopier.LastClonedAncestor lastCloned = new StructureTreeCopier.LastClonedAncestor();
                    lastCloned.ancestor = top;
                    PdfDictionary topClone = top.Clone(ignoreKeysForClone);
                    topClone.Put(PdfName.P, destStructTreeRoot.GetPdfObject());
                    lastCloned.clone = topClone;
                    SeparateKids(top, firstPartElems, lastCloned);
                    if (topClone.ContainsKey(PdfName.K)) {
                        topClone.MakeIndirect(destDocument);
                        clonedTops.Add(topClone);
                    }
                }
            }
            for (int i = 0; i < clonedTops.Count; ++i) {
                destStructTreeRoot.AddKidObject(lastTopBefore + 1 + i, clonedTops[i]);
            }
            CopyTo(destDocument, page2page, callingDocument, false, lastTopBefore + 1);
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>
        /// .
        /// </summary>
        /// <param name="destDocument">document to cpt structure to.</param>
        /// <param name="page2page">association between original page and copied page.</param>
        /// <param name="copyFromDestDocument">
        /// indicates if <code>page2page</code> keys and values represent pages from
        /// <paramref name="destDocument"/>
        /// .
        /// </param>
        private static void CopyTo(PdfDocument destDocument, IDictionary<PdfPage, PdfPage> page2page, PdfDocument 
            callingDocument, bool copyFromDestDocument) {
            CopyTo(destDocument, page2page, callingDocument, copyFromDestDocument, -1);
        }

        private static void CopyTo(PdfDocument destDocument, IDictionary<PdfPage, PdfPage> page2page, PdfDocument 
            callingDocument, bool copyFromDestDocument, int insertIndex) {
            PdfDocument fromDocument = copyFromDestDocument ? destDocument : callingDocument;
            ICollection<PdfDictionary> tops = new HashSet<PdfDictionary>();
            ICollection<PdfObject> objectsToCopy = new HashSet<PdfObject>();
            IDictionary<PdfDictionary, PdfDictionary> page2pageDictionaries = new Dictionary<PdfDictionary, PdfDictionary
                >();
            foreach (KeyValuePair<PdfPage, PdfPage> page in page2page) {
                page2pageDictionaries.Put(page.Key.GetPdfObject(), page.Value.GetPdfObject());
                ICollection<PdfMcr> mcrs = fromDocument.GetStructTreeRoot().GetPageMarkedContentReferences(page.Key);
                if (mcrs != null) {
                    foreach (PdfMcr mcr in mcrs) {
                        if (mcr is PdfMcrDictionary || mcr is PdfObjRef) {
                            objectsToCopy.Add(mcr.GetPdfObject());
                        }
                        PdfDictionary top = AddAllParentsToSet(mcr, objectsToCopy);
                        if (top.IsFlushed()) {
                            throw new PdfException(PdfException.CannotCopyFlushedTag);
                        }
                        tops.Add(top);
                    }
                }
            }
            IList<PdfDictionary> topsInOriginalOrder = new List<PdfDictionary>();
            foreach (IPdfStructElem kid in fromDocument.GetStructTreeRoot().GetKids()) {
                if (kid == null) {
                    continue;
                }
                PdfDictionary kidObject = ((PdfStructElem)kid).GetPdfObject();
                if (tops.Contains(kidObject)) {
                    topsInOriginalOrder.Add(kidObject);
                }
            }
            foreach (PdfDictionary top in topsInOriginalOrder) {
                PdfDictionary copied = CopyObject(top, objectsToCopy, destDocument, page2pageDictionaries, copyFromDestDocument
                    );
                destDocument.GetStructTreeRoot().AddKidObject(insertIndex, copied);
                if (insertIndex > -1) {
                    ++insertIndex;
                }
            }
        }

        private static PdfDictionary CopyObject(PdfDictionary source, ICollection<PdfObject> objectsToCopy, PdfDocument
             toDocument, IDictionary<PdfDictionary, PdfDictionary> page2page, bool copyFromDestDocument) {
            PdfDictionary copied;
            if (copyFromDestDocument) {
                copied = source.Clone(ignoreKeysForCopy);
                if (source.IsIndirect()) {
                    copied.MakeIndirect(toDocument);
                }
            }
            else {
                copied = source.CopyTo(toDocument, ignoreKeysForCopy, true);
            }
            if (source.ContainsKey(PdfName.Obj)) {
                PdfDictionary obj = source.GetAsDictionary(PdfName.Obj);
                if (!copyFromDestDocument && obj != null) {
                    // Link annotations could be not added to the toDocument, so we need to identify this case.
                    // When obj.copyTo is called, and annotation was already copied, we would get this already created copy.
                    // If it was already copied and added, /P key would be set. Otherwise /P won't be set.
                    obj = obj.CopyTo(toDocument, JavaUtil.ArraysAsList(PdfName.P), false);
                    copied.Put(PdfName.Obj, obj);
                }
            }
            PdfDictionary pg = source.GetAsDictionary(PdfName.Pg);
            if (pg != null) {
                //TODO It is possible, that pg will not be present in the page2page map. Consider the situation,
                // that we want to copy structElem because it has marked content dictionary reference, which belongs to the page from page2page,
                // but the structElem itself has /Pg which value could be arbitrary page.
                copied.Put(PdfName.Pg, page2page.Get(pg));
            }
            PdfObject k = source.Get(PdfName.K);
            if (k != null) {
                if (k.IsArray()) {
                    PdfArray kArr = (PdfArray)k;
                    PdfArray newArr = new PdfArray();
                    for (int i = 0; i < kArr.Size(); i++) {
                        PdfObject copiedKid = CopyObjectKid(kArr.Get(i), copied, objectsToCopy, toDocument, page2page, copyFromDestDocument
                            );
                        if (copiedKid != null) {
                            newArr.Add(copiedKid);
                        }
                    }
                    // TODO new array may be empty or with single element
                    copied.Put(PdfName.K, newArr);
                }
                else {
                    PdfObject copiedKid = CopyObjectKid(k, copied, objectsToCopy, toDocument, page2page, copyFromDestDocument);
                    if (copiedKid != null) {
                        copied.Put(PdfName.K, copiedKid);
                    }
                }
            }
            return copied;
        }

        private static PdfObject CopyObjectKid(PdfObject kid, PdfDictionary copiedParent, ICollection<PdfObject> objectsToCopy
            , PdfDocument toDocument, IDictionary<PdfDictionary, PdfDictionary> page2page, bool copyFromDestDocument
            ) {
            if (kid.IsNumber()) {
                toDocument.GetStructTreeRoot().GetParentTreeHandler().RegisterMcr(new PdfMcrNumber((PdfNumber)kid, new PdfStructElem
                    (copiedParent)));
                return kid;
            }
            else {
                // TODO do we always copy numbers? don't we need to check if it is supposed to be copied like objs in objectsToCopy?
                if (kid.IsDictionary()) {
                    PdfDictionary kidAsDict = (PdfDictionary)kid;
                    if (objectsToCopy.Contains(kidAsDict)) {
                        bool hasParent = kidAsDict.ContainsKey(PdfName.P);
                        PdfDictionary copiedKid = CopyObject(kidAsDict, objectsToCopy, toDocument, page2page, copyFromDestDocument
                            );
                        if (hasParent) {
                            copiedKid.Put(PdfName.P, copiedParent);
                        }
                        else {
                            PdfMcr mcr;
                            if (copiedKid.ContainsKey(PdfName.Obj)) {
                                mcr = new PdfObjRef(copiedKid, new PdfStructElem(copiedParent));
                                PdfDictionary contentItemObject = copiedKid.GetAsDictionary(PdfName.Obj);
                                if (PdfName.Link.Equals(contentItemObject.GetAsName(PdfName.Subtype)) && !contentItemObject.ContainsKey(PdfName
                                    .P)) {
                                    // Some link annotations may be not copied, because their destination page is not copied.
                                    return null;
                                }
                                contentItemObject.Put(PdfName.StructParent, new PdfNumber((int)toDocument.GetNextStructParentIndex()));
                            }
                            else {
                                mcr = new PdfMcrDictionary(copiedKid, new PdfStructElem(copiedParent));
                            }
                            toDocument.GetStructTreeRoot().GetParentTreeHandler().RegisterMcr(mcr);
                        }
                        return copiedKid;
                    }
                }
            }
            return null;
        }

        private static void SeparateKids(PdfDictionary structElem, ICollection<PdfObject> firstPartElems, StructureTreeCopier.LastClonedAncestor
             lastCloned) {
            PdfObject k = structElem.Get(PdfName.K);
            // If /K entry is not a PdfArray - it would be a kid which we won't clone at the moment, because it won't contain
            // kids from both parts at the same time. It would either be cloned as an ancestor later, or not cloned at all.
            // If it's kid is struct elem - it would definitely be structElem from the first part, so we simply call separateKids for it.
            if (!k.IsArray()) {
                if (k.IsDictionary() && PdfStructElem.IsStructElem((PdfDictionary)k)) {
                    SeparateKids((PdfDictionary)k, firstPartElems, lastCloned);
                }
            }
            else {
                PdfDocument document = structElem.GetIndirectReference().GetDocument();
                PdfArray kids = (PdfArray)k;
                for (int i = 0; i < kids.Size(); ++i) {
                    PdfObject kid = kids.Get(i);
                    PdfDictionary dictKid = null;
                    if (kid.IsDictionary()) {
                        dictKid = (PdfDictionary)kid;
                    }
                    if (dictKid != null && PdfStructElem.IsStructElem(dictKid)) {
                        if (firstPartElems.Contains(kid)) {
                            SeparateKids((PdfDictionary)kid, firstPartElems, lastCloned);
                        }
                        else {
                            if (dictKid.IsFlushed()) {
                                throw new PdfException(PdfException.TagFromTheExistingTagStructureIsFlushedCannotAddCopiedPageTags);
                            }
                            // elems with no kids will not be marked as from the first part,
                            // but nonetheless we don't want to move all of them to the second part; we just leave them as is
                            if (dictKid.ContainsKey(PdfName.K)) {
                                CloneParents(structElem, lastCloned, document);
                                kids.Remove(i--);
                                PdfStructElem.AddKidObject(lastCloned.clone, -1, kid);
                            }
                        }
                    }
                    else {
                        if (!firstPartElems.Contains(kid)) {
                            CloneParents(structElem, lastCloned, document);
                            PdfMcr mcr;
                            if (dictKid != null) {
                                if (dictKid.Get(PdfName.Type).Equals(PdfName.MCR)) {
                                    mcr = new PdfMcrDictionary(dictKid, new PdfStructElem(lastCloned.clone));
                                }
                                else {
                                    mcr = new PdfObjRef(dictKid, new PdfStructElem(lastCloned.clone));
                                }
                            }
                            else {
                                mcr = new PdfMcrNumber((PdfNumber)kid, new PdfStructElem(lastCloned.clone));
                            }
                            kids.Remove(i--);
                            PdfStructElem.AddKidObject(lastCloned.clone, -1, kid);
                            document.GetStructTreeRoot().GetParentTreeHandler().RegisterMcr(mcr);
                        }
                    }
                }
            }
            // re-register mcr
            if (lastCloned.ancestor == structElem) {
                lastCloned.ancestor = lastCloned.ancestor.GetAsDictionary(PdfName.P);
                lastCloned.clone = lastCloned.clone.GetAsDictionary(PdfName.P);
            }
        }

        private static void CloneParents(PdfDictionary structElem, StructureTreeCopier.LastClonedAncestor lastCloned
            , PdfDocument document) {
            if (lastCloned.ancestor != structElem) {
                PdfDictionary structElemClone = ((PdfDictionary)structElem.Clone(ignoreKeysForClone).MakeIndirect(document
                    ));
                PdfDictionary currClone = structElemClone;
                PdfDictionary currElem = structElem;
                while (currElem.Get(PdfName.P) != lastCloned.ancestor) {
                    PdfDictionary parent = currElem.GetAsDictionary(PdfName.P);
                    PdfDictionary parentClone = ((PdfDictionary)parent.Clone(ignoreKeysForClone).MakeIndirect(document));
                    currClone.Put(PdfName.P, parentClone);
                    parentClone.Put(PdfName.K, currClone);
                    currClone = parentClone;
                    currElem = parent;
                }
                PdfStructElem.AddKidObject(lastCloned.clone, -1, currClone);
                lastCloned.clone = structElemClone;
                lastCloned.ancestor = structElem;
            }
        }

        /// <returns>the topmost parent added to set. If encountered flushed element - stops and returns this flushed element.
        ///     </returns>
        private static PdfDictionary AddAllParentsToSet(PdfMcr mcr, ICollection<PdfObject> set) {
            PdfDictionary elem = ((PdfStructElem)mcr.GetParent()).GetPdfObject();
            set.Add(elem);
            for (; ; ) {
                if (elem.IsFlushed()) {
                    break;
                }
                PdfDictionary p = elem.GetAsDictionary(PdfName.P);
                if (p == null || PdfName.StructTreeRoot.Equals(p.GetAsName(PdfName.Type))) {
                    break;
                }
                else {
                    elem = p;
                    set.Add(elem);
                }
            }
            return elem;
        }

        internal class LastClonedAncestor {
            internal PdfDictionary ancestor;

            internal PdfDictionary clone;
        }
    }
}
