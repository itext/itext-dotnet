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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>Internal helper class which is used to copy, clone or move tag structure across documents.</summary>
    internal class StructureTreeCopier {
        private static IList<PdfName> ignoreKeysForCopy = new List<PdfName>();

        private static IList<PdfName> ignoreKeysForClone = new List<PdfName>();

        static StructureTreeCopier() {
            ignoreKeysForCopy.Add(PdfName.K);
            ignoreKeysForCopy.Add(PdfName.P);
            ignoreKeysForCopy.Add(PdfName.Pg);
            ignoreKeysForCopy.Add(PdfName.Obj);
            ignoreKeysForCopy.Add(PdfName.NS);
            ignoreKeysForClone.Add(PdfName.K);
            ignoreKeysForClone.Add(PdfName.P);
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>.
        /// </summary>
        /// <remarks>
        /// Copies structure to a
        /// <paramref name="destDocument"/>.
        /// <br/><br/>
        /// NOTE: Works only for
        /// <c>PdfStructTreeRoot</c>
        /// that is read from the document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </remarks>
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
        /// </summary>
        /// <remarks>
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
        /// </remarks>
        /// <param name="destDocument">document to copy structure to.</param>
        /// <param name="insertBeforePage">indicates where the structure to be inserted.</param>
        /// <param name="page2page">association between original page and copied page.</param>
        public static void CopyTo(PdfDocument destDocument, int insertBeforePage, IDictionary<PdfPage, PdfPage> page2page
            , PdfDocument callingDocument) {
            if (!destDocument.IsTagged()) {
                return;
            }
            CopyTo(destDocument, insertBeforePage, page2page, callingDocument, false);
        }

        /// <summary>Move tag structure of page to other place in the same document</summary>
        /// <param name="document">document in which modifications will take place (should be opened in read-write mode)
        ///     </param>
        /// <param name="from">page, which tag structure will be moved</param>
        /// <param name="insertBefore">indicates before what page number structure will be inserted to</param>
        public static void Move(PdfDocument document, PdfPage from, int insertBefore) {
            if (!document.IsTagged() || insertBefore < 1 || insertBefore > document.GetNumberOfPages() + 1) {
                return;
            }
            int fromNum = document.GetPageNumber(from);
            if (fromNum == 0 || fromNum == insertBefore || fromNum + 1 == insertBefore) {
                return;
            }
            int destStruct;
            int currStruct = 0;
            if (fromNum > insertBefore) {
                destStruct = currStruct = SeparateStructure(document, 1, insertBefore, 0);
                currStruct = SeparateStructure(document, insertBefore, fromNum, currStruct);
                currStruct = SeparateStructure(document, fromNum, fromNum + 1, currStruct);
            }
            else {
                currStruct = SeparateStructure(document, 1, fromNum, 0);
                currStruct = SeparateStructure(document, fromNum, fromNum + 1, currStruct);
                destStruct = currStruct = SeparateStructure(document, fromNum + 1, insertBefore, currStruct);
            }
            ICollection<PdfDictionary> topsToMove = new HashSet<PdfDictionary>();
            ICollection<PdfMcr> mcrs = document.GetStructTreeRoot().GetPageMarkedContentReferences(from);
            if (mcrs != null) {
                foreach (PdfMcr mcr in mcrs) {
                    PdfDictionary top = GetTopmostParent(mcr);
                    if (top != null) {
                        if (top.IsFlushed()) {
                            throw new PdfException(KernelExceptionMessageConstant.CANNOT_MOVE_FLUSHED_TAG);
                        }
                        topsToMove.Add(top);
                    }
                }
            }
            IList<PdfDictionary> orderedTopsToMove = new List<PdfDictionary>();
            PdfArray tops = document.GetStructTreeRoot().GetKidsObject();
            for (int i = 0; i < tops.Size(); ++i) {
                PdfDictionary top = tops.GetAsDictionary(i);
                if (topsToMove.Contains(top)) {
                    orderedTopsToMove.Add(top);
                    tops.Remove(i);
                    if (i < destStruct) {
                        --destStruct;
                    }
                }
            }
            foreach (PdfDictionary top in orderedTopsToMove) {
                document.GetStructTreeRoot().AddKidObject(destStruct++, top);
            }
        }

        /// <returns>structure tree index of first separated (cloned) top</returns>
        private static int SeparateStructure(PdfDocument document, int beforePage) {
            return SeparateStructure(document, 1, beforePage, 0);
        }

        private static int SeparateStructure(PdfDocument document, int startPage, int beforePage, int startPageStructTopIndex
            ) {
            if (!document.IsTagged() || 1 > startPage || startPage > beforePage || beforePage > document.GetNumberOfPages
                () + 1) {
                return -1;
            }
            else {
                if (beforePage == startPage) {
                    return startPageStructTopIndex;
                }
                else {
                    if (beforePage == document.GetNumberOfPages() + 1) {
                        return document.GetStructTreeRoot().GetKidsObject().Size();
                    }
                }
            }
            // Here we separate the structure tree in two parts: struct elems that belong to the pages which indexes are
            // less then separateBeforePage and those struct elems that belong to other pages. Some elems might belong
            // to both parts and actually these are the ones that we are looking for.
            ICollection<PdfObject> firstPartElems = new HashSet<PdfObject>();
            for (int i = startPage; i < beforePage; ++i) {
                PdfPage pageOfFirstHalf = document.GetPage(i);
                ICollection<PdfMcr> pageMcrs = document.GetStructTreeRoot().GetPageMarkedContentReferences(pageOfFirstHalf
                    );
                if (pageMcrs != null) {
                    foreach (PdfMcr mcr in pageMcrs) {
                        firstPartElems.Add(mcr.GetPdfObject());
                        PdfDictionary top = AddAllParentsToSet(mcr, firstPartElems);
                        if (top != null && top.IsFlushed()) {
                            throw new PdfException(KernelExceptionMessageConstant.TAG_FROM_THE_EXISTING_TAG_STRUCTURE_IS_FLUSHED_CANNOT_ADD_COPIED_PAGE_TAGS
                                );
                        }
                    }
                }
            }
            IList<PdfDictionary> clonedTops = new List<PdfDictionary>();
            PdfArray tops = document.GetStructTreeRoot().GetKidsObject();
            // Now we "walk" through all the elems which belong to the first part, and look for the ones that contain both
            // kids from first and second part. We clone found elements and move kids from the second part to cloned elems.
            int lastTopBefore = startPageStructTopIndex - 1;
            for (int i = 0; i < tops.Size(); ++i) {
                PdfDictionary top = tops.GetAsDictionary(i);
                if (firstPartElems.Contains(top)) {
                    lastTopBefore = i;
                    StructureTreeCopier.LastClonedAncestor lastCloned = new StructureTreeCopier.LastClonedAncestor();
                    lastCloned.ancestor = top;
                    PdfDictionary topClone = top.Clone(ignoreKeysForClone);
                    topClone.Put(PdfName.P, document.GetStructTreeRoot().GetPdfObject());
                    lastCloned.clone = topClone;
                    SeparateKids(top, firstPartElems, lastCloned, document);
                    if (topClone.ContainsKey(PdfName.K)) {
                        topClone.MakeIndirect(document);
                        clonedTops.Add(topClone);
                    }
                }
            }
            for (int i = 0; i < clonedTops.Count; ++i) {
                document.GetStructTreeRoot().AddKidObject(lastTopBefore + 1 + i, clonedTops[i]);
            }
            return lastTopBefore + 1;
        }

        private static void CopyTo(PdfDocument destDocument, int insertBeforePage, IDictionary<PdfPage, PdfPage> page2page
            , PdfDocument callingDocument, bool copyFromDestDocument) {
            if (!destDocument.IsTagged()) {
                return;
            }
            int insertIndex = SeparateStructure(destDocument, insertBeforePage);
            //Opposite should never happened.
            if (insertIndex > 0) {
                CopyTo(destDocument, page2page, callingDocument, copyFromDestDocument, insertIndex);
            }
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>.
        /// </summary>
        /// <param name="destDocument">document to cpt structure to.</param>
        /// <param name="page2page">association between original page and copied page.</param>
        /// <param name="copyFromDestDocument">
        /// indicates if <c>page2page</c> keys and values represent pages from
        /// <paramref name="destDocument"/>.
        /// </param>
        private static void CopyTo(PdfDocument destDocument, IDictionary<PdfPage, PdfPage> page2page, PdfDocument 
            callingDocument, bool copyFromDestDocument) {
            CopyTo(destDocument, page2page, callingDocument, copyFromDestDocument, -1);
        }

        private static void CopyTo(PdfDocument destDocument, IDictionary<PdfPage, PdfPage> page2page, PdfDocument 
            callingDocument, bool copyFromDestDocument, int insertIndex) {
            StructureTreeCopier.CopyStructureResult copiedStructure = CopyStructure(destDocument, page2page, callingDocument
                , copyFromDestDocument);
            PdfStructTreeRoot destStructTreeRoot = destDocument.GetStructTreeRoot();
            destStructTreeRoot.MakeIndirect(destDocument);
            foreach (PdfDictionary copied in copiedStructure.GetTopsList()) {
                destStructTreeRoot.AddKidObject(insertIndex, copied);
                if (insertIndex > -1) {
                    ++insertIndex;
                }
            }
            if (!copyFromDestDocument) {
                if (!copiedStructure.GetCopiedNamespaces().IsEmpty()) {
                    destStructTreeRoot.GetNamespacesObject().AddAll(copiedStructure.GetCopiedNamespaces());
                }
                PdfDictionary srcRoleMap = callingDocument.GetStructTreeRoot().GetRoleMap();
                PdfDictionary destRoleMap = destStructTreeRoot.GetRoleMap();
                foreach (KeyValuePair<PdfName, PdfObject> mappingEntry in srcRoleMap.EntrySet()) {
                    if (!destRoleMap.ContainsKey(mappingEntry.Key)) {
                        destRoleMap.Put(mappingEntry.Key, mappingEntry.Value);
                    }
                    else {
                        if (!mappingEntry.Value.Equals(destRoleMap.Get(mappingEntry.Key))) {
                            String srcMapping = mappingEntry.Key + " -> " + mappingEntry.Value;
                            String destMapping = mappingEntry.Key + " -> " + destRoleMap.Get(mappingEntry.Key);
                            ILogger logger = ITextLogManager.GetLogger(typeof(StructureTreeCopier));
                            logger.LogWarning(String.Format(iText.IO.Logs.IoLogMessageConstant.ROLE_MAPPING_FROM_SOURCE_IS_NOT_COPIED_ALREADY_EXIST
                                , srcMapping, destMapping));
                        }
                    }
                }
            }
        }

        private static StructureTreeCopier.CopyStructureResult CopyStructure(PdfDocument destDocument, IDictionary
            <PdfPage, PdfPage> page2page, PdfDocument callingDocument, bool copyFromDestDocument) {
            PdfDocument fromDocument = copyFromDestDocument ? destDocument : callingDocument;
            IDictionary<PdfDictionary, PdfDictionary> topsToFirstDestPage = new Dictionary<PdfDictionary, PdfDictionary
                >();
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
                        if (top != null) {
                            if (top.IsFlushed()) {
                                throw new PdfException(KernelExceptionMessageConstant.CANNOT_COPY_FLUSHED_TAG);
                            }
                            if (!topsToFirstDestPage.ContainsKey(top)) {
                                topsToFirstDestPage.Put(top, page.Value.GetPdfObject());
                            }
                        }
                    }
                }
            }
            IList<PdfDictionary> topsInOriginalOrder = new List<PdfDictionary>();
            foreach (IStructureNode kid in fromDocument.GetStructTreeRoot().GetKids()) {
                if (kid == null) {
                    continue;
                }
                PdfDictionary kidObject = ((PdfStructElem)kid).GetPdfObject();
                if (topsToFirstDestPage.ContainsKey(kidObject)) {
                    topsInOriginalOrder.Add(kidObject);
                }
            }
            StructureTreeCopier.StructElemCopyingParams structElemCopyingParams = new StructureTreeCopier.StructElemCopyingParams
                (objectsToCopy, destDocument, page2pageDictionaries, copyFromDestDocument);
            PdfStructTreeRoot destStructTreeRoot = destDocument.GetStructTreeRoot();
            destStructTreeRoot.MakeIndirect(destDocument);
            IList<PdfDictionary> copiedTops = new List<PdfDictionary>();
            foreach (PdfDictionary top in topsInOriginalOrder) {
                PdfDictionary copied = CopyObject(top, topsToFirstDestPage.Get(top), false, structElemCopyingParams);
                copiedTops.Add(copied);
            }
            return new StructureTreeCopier.CopyStructureResult(copiedTops, structElemCopyingParams.GetCopiedNamespaces
                ());
        }

        private static PdfDictionary CopyObject(PdfDictionary source, PdfDictionary destPage, bool parentChangePg, 
            StructureTreeCopier.StructElemCopyingParams copyingParams) {
            PdfDictionary copied;
            if (copyingParams.IsCopyFromDestDocument()) {
                copied = source.Clone(ignoreKeysForClone);
                if (source.IsIndirect()) {
                    copied.MakeIndirect(copyingParams.GetToDocument());
                }
                PdfDictionary pg = source.GetAsDictionary(PdfName.Pg);
                if (pg != null) {
                    if (copyingParams.IsCopyFromDestDocument()) {
                        if (pg != destPage) {
                            copied.Put(PdfName.Pg, destPage);
                            parentChangePg = true;
                        }
                        else {
                            parentChangePg = false;
                        }
                    }
                }
            }
            else {
                copied = source.CopyTo(copyingParams.GetToDocument(), ignoreKeysForCopy, true);
                PdfDictionary obj = source.GetAsDictionary(PdfName.Obj);
                if (obj != null) {
                    // Link annotations could be not added to the toDocument, so we need to identify this case.
                    // When obj.copyTo is called, and annotation was already copied, we would get this already created copy.
                    // If it was already copied and added, /P key would be set. Otherwise /P won't be set.
                    obj = obj.CopyTo(copyingParams.GetToDocument(), JavaUtil.ArraysAsList(PdfName.P), false);
                    copied.Put(PdfName.Obj, obj);
                }
                PdfDictionary nsDict = source.GetAsDictionary(PdfName.NS);
                if (nsDict != null) {
                    PdfDictionary copiedNsDict = CopyNamespaceDict(nsDict, copyingParams);
                    copied.Put(PdfName.NS, copiedNsDict);
                }
                PdfDictionary pg = source.GetAsDictionary(PdfName.Pg);
                if (pg != null) {
                    PdfDictionary pageAnalog = copyingParams.GetPage2page().Get(pg);
                    if (pageAnalog == null) {
                        pageAnalog = destPage;
                        parentChangePg = true;
                    }
                    else {
                        parentChangePg = false;
                    }
                    copied.Put(PdfName.Pg, pageAnalog);
                }
            }
            PdfObject k = source.Get(PdfName.K);
            if (k != null) {
                if (k.IsArray()) {
                    PdfArray kArr = (PdfArray)k;
                    PdfArray newArr = new PdfArray();
                    for (int i = 0; i < kArr.Size(); i++) {
                        PdfObject copiedKid = CopyObjectKid(kArr.Get(i), copied, destPage, parentChangePg, copyingParams);
                        if (copiedKid != null) {
                            newArr.Add(copiedKid);
                        }
                    }
                    if (!newArr.IsEmpty()) {
                        if (newArr.Size() == 1) {
                            copied.Put(PdfName.K, newArr.Get(0));
                        }
                        else {
                            copied.Put(PdfName.K, newArr);
                        }
                    }
                }
                else {
                    PdfObject copiedKid = CopyObjectKid(k, copied, destPage, parentChangePg, copyingParams);
                    if (copiedKid != null) {
                        copied.Put(PdfName.K, copiedKid);
                    }
                }
            }
            return copied;
        }

        private static PdfObject CopyObjectKid(PdfObject kid, PdfDictionary copiedParent, PdfDictionary destPage, 
            bool parentChangePg, StructureTreeCopier.StructElemCopyingParams copyingParams) {
            if (kid.IsNumber()) {
                if (!parentChangePg) {
                    copyingParams.GetToDocument().GetStructTreeRoot().GetParentTreeHandler().RegisterMcr(new PdfMcrNumber((PdfNumber
                        )kid, new PdfStructElem(copiedParent)));
                    return kid;
                }
            }
            else {
                if (kid.IsDictionary()) {
                    PdfDictionary kidAsDict = (PdfDictionary)kid;
                    // if element is TD and its parent is TR which was copied, then we copy it in any case
                    if (copyingParams.GetObjectsToCopy().Contains(kidAsDict) || ShouldTableElementBeCopied(kidAsDict, copiedParent
                        )) {
                        bool hasParent = kidAsDict.ContainsKey(PdfName.P);
                        PdfDictionary copiedKid = CopyObject(kidAsDict, destPage, parentChangePg, copyingParams);
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
                                contentItemObject.Put(PdfName.StructParent, new PdfNumber((int)copyingParams.GetToDocument().GetNextStructParentIndex
                                    ()));
                            }
                            else {
                                mcr = new PdfMcrDictionary(copiedKid, new PdfStructElem(copiedParent));
                            }
                            copyingParams.GetToDocument().GetStructTreeRoot().GetParentTreeHandler().RegisterMcr(mcr);
                        }
                        return copiedKid;
                    }
                }
            }
            return null;
        }

        internal static bool ShouldTableElementBeCopied(PdfDictionary obj, PdfDictionary parent) {
            return (PdfName.TD.Equals(obj.Get(PdfName.S)) || PdfName.TH.Equals(obj.Get(PdfName.S))) && PdfName.TR.Equals
                (parent.Get(PdfName.S));
        }

        private static PdfDictionary CopyNamespaceDict(PdfDictionary srcNsDict, StructureTreeCopier.StructElemCopyingParams
             copyingParams) {
            IList<PdfName> excludeKeys = JavaCollectionsUtil.SingletonList<PdfName>(PdfName.RoleMapNS);
            PdfDocument toDocument = copyingParams.GetToDocument();
            PdfDictionary copiedNsDict = srcNsDict.CopyTo(toDocument, excludeKeys, false);
            copyingParams.AddCopiedNamespace(copiedNsDict);
            PdfDictionary srcRoleMapNs = srcNsDict.GetAsDictionary(PdfName.RoleMapNS);
            // if this src namespace was already copied (or in the process of copying) it will contain role map already
            PdfDictionary copiedRoleMap = copiedNsDict.GetAsDictionary(PdfName.RoleMapNS);
            if (srcRoleMapNs != null && copiedRoleMap == null) {
                copiedRoleMap = new PdfDictionary();
                copiedNsDict.Put(PdfName.RoleMapNS, copiedRoleMap);
                foreach (KeyValuePair<PdfName, PdfObject> entry in srcRoleMapNs.EntrySet()) {
                    PdfObject copiedMapping;
                    if (entry.Value.IsArray()) {
                        PdfArray srcMappingArray = (PdfArray)entry.Value;
                        if (srcMappingArray.Size() > 1 && srcMappingArray.Get(1).IsDictionary()) {
                            PdfArray copiedMappingArray = new PdfArray();
                            copiedMappingArray.Add(srcMappingArray.Get(0).CopyTo(toDocument));
                            PdfDictionary copiedNamespace = CopyNamespaceDict(srcMappingArray.GetAsDictionary(1), copyingParams);
                            copiedMappingArray.Add(copiedNamespace);
                            copiedMapping = copiedMappingArray;
                        }
                        else {
                            ILogger logger = ITextLogManager.GetLogger(typeof(StructureTreeCopier));
                            logger.LogWarning(String.Format(iText.IO.Logs.IoLogMessageConstant.ROLE_MAPPING_FROM_SOURCE_IS_NOT_COPIED_INVALID
                                , entry.Key.ToString()));
                            continue;
                        }
                    }
                    else {
                        copiedMapping = entry.Value.CopyTo(toDocument);
                    }
                    PdfName copiedRoleFrom = (PdfName)entry.Key.CopyTo(toDocument);
                    copiedRoleMap.Put(copiedRoleFrom, copiedMapping);
                }
            }
            return copiedNsDict;
        }

        private static void SeparateKids(PdfDictionary structElem, ICollection<PdfObject> firstPartElems, StructureTreeCopier.LastClonedAncestor
             lastCloned, PdfDocument document) {
            PdfObject k = structElem.Get(PdfName.K);
            // If /K entry is not a PdfArray - it would be a kid which we won't clone at the moment, because it won't contain
            // kids from both parts at the same time. It would either be cloned as an ancestor later, or not cloned at all.
            // If it's kid is struct elem - it would definitely be structElem from the first part, so we simply call separateKids for it.
            if (!k.IsArray()) {
                if (k.IsDictionary() && PdfStructElem.IsStructElem((PdfDictionary)k)) {
                    SeparateKids((PdfDictionary)k, firstPartElems, lastCloned, document);
                }
            }
            else {
                PdfArray kids = (PdfArray)k;
                for (int i = 0; i < kids.Size(); ++i) {
                    PdfObject kid = kids.Get(i);
                    PdfDictionary dictKid = null;
                    if (kid.IsDictionary()) {
                        dictKid = (PdfDictionary)kid;
                    }
                    if (dictKid != null && PdfStructElem.IsStructElem(dictKid)) {
                        if (firstPartElems.Contains(kid)) {
                            SeparateKids((PdfDictionary)kid, firstPartElems, lastCloned, document);
                        }
                        else {
                            if (dictKid.IsFlushed()) {
                                throw new PdfException(KernelExceptionMessageConstant.TAG_FROM_THE_EXISTING_TAG_STRUCTURE_IS_FLUSHED_CANNOT_ADD_COPIED_PAGE_TAGS
                                    );
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
                            // re-register mcr
                            document.GetStructTreeRoot().GetParentTreeHandler().RegisterMcr(mcr);
                        }
                    }
                }
            }
            if (lastCloned.ancestor == structElem) {
                lastCloned.ancestor = lastCloned.ancestor.GetAsDictionary(PdfName.P);
                lastCloned.clone = lastCloned.clone.GetAsDictionary(PdfName.P);
            }
        }

        private static void CloneParents(PdfDictionary structElem, StructureTreeCopier.LastClonedAncestor lastCloned
            , PdfDocument document) {
            if (lastCloned.ancestor != structElem) {
                PdfDictionary structElemClone = (PdfDictionary)structElem.Clone(ignoreKeysForClone).MakeIndirect(document);
                PdfDictionary currClone = structElemClone;
                PdfDictionary currElem = structElem;
                while (currElem.Get(PdfName.P) != lastCloned.ancestor) {
                    PdfDictionary parent = currElem.GetAsDictionary(PdfName.P);
                    PdfDictionary parentClone = (PdfDictionary)parent.Clone(ignoreKeysForClone).MakeIndirect(document);
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
            IList<PdfDictionary> allParents = RetrieveParents(mcr, true);
            set.AddAll(allParents);
            return allParents.IsEmpty() ? null : allParents[allParents.Count - 1];
        }

        /// <summary>Gets the topmost non-root structure element parent.</summary>
        /// <remarks>Gets the topmost non-root structure element parent. May be flushed.</remarks>
        /// <param name="mcr">starting element</param>
        /// <returns>
        /// topmost non-root structure element parent, or
        /// <see langword="null"/>
        /// if it doesn't have any
        /// </returns>
        private static PdfDictionary GetTopmostParent(PdfMcr mcr) {
            return RetrieveParents(mcr, false)[0];
        }

        private static IList<PdfDictionary> RetrieveParents(PdfMcr mcr, bool all) {
            IList<PdfDictionary> parents = new List<PdfDictionary>();
            IStructureNode firstParent = mcr.GetParent();
            PdfDictionary previous = null;
            PdfDictionary current = firstParent is PdfStructElem ? ((PdfStructElem)firstParent).GetPdfObject() : null;
            while (current != null && !PdfName.StructTreeRoot.Equals(current.GetAsName(PdfName.Type))) {
                if (all) {
                    parents.Add(current);
                }
                previous = current;
                current = previous.IsFlushed() ? null : previous.GetAsDictionary(PdfName.P);
            }
            if (!all) {
                parents.Add(previous);
            }
            return parents;
        }

        internal class LastClonedAncestor {
            internal PdfDictionary ancestor;

            internal PdfDictionary clone;
        }

        private class StructElemCopyingParams {
            private readonly ICollection<PdfObject> objectsToCopy;

            private readonly PdfDocument toDocument;

            private readonly IDictionary<PdfDictionary, PdfDictionary> page2page;

            private readonly bool copyFromDestDocument;

            private readonly ICollection<PdfObject> copiedNamespaces;

            public StructElemCopyingParams(ICollection<PdfObject> objectsToCopy, PdfDocument toDocument, IDictionary<PdfDictionary
                , PdfDictionary> page2page, bool copyFromDestDocument) {
                this.objectsToCopy = objectsToCopy;
                this.toDocument = toDocument;
                this.page2page = page2page;
                this.copyFromDestDocument = copyFromDestDocument;
                this.copiedNamespaces = new LinkedHashSet<PdfObject>();
            }

            public virtual ICollection<PdfObject> GetObjectsToCopy() {
                return objectsToCopy;
            }

            public virtual PdfDocument GetToDocument() {
                return toDocument;
            }

            public virtual IDictionary<PdfDictionary, PdfDictionary> GetPage2page() {
                return page2page;
            }

            public virtual bool IsCopyFromDestDocument() {
                return copyFromDestDocument;
            }

            public virtual void AddCopiedNamespace(PdfDictionary copiedNs) {
                copiedNamespaces.Add(copiedNs);
            }

            public virtual ICollection<PdfObject> GetCopiedNamespaces() {
                return copiedNamespaces;
            }
        }

        private class CopyStructureResult {
            private readonly IList<PdfDictionary> topsList;

            private readonly ICollection<PdfObject> copiedNamespaces;

            public CopyStructureResult(IList<PdfDictionary> topsList, ICollection<PdfObject> copiedNamespaces) {
                this.topsList = topsList;
                this.copiedNamespaces = copiedNamespaces;
            }

            public virtual ICollection<PdfObject> GetCopiedNamespaces() {
                return copiedNamespaces;
            }

            public virtual IList<PdfDictionary> GetTopsList() {
                return topsList;
            }
        }
    }
}
