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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Internal helper class which is used to effectively build parent tree and also find marked content references:
    /// for specified page, by MCID or by struct parent index.
    /// </summary>
    internal class ParentTreeHandler {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagging.ParentTreeHandler
            ));

        private PdfStructTreeRoot structTreeRoot;

        /// <summary>Represents parentTree in structTreeRoot.</summary>
        /// <remarks>Represents parentTree in structTreeRoot. It contains only those entries that belong to the already flushed pages.
        ///     </remarks>
        private PdfNumTree parentTree;

        private IDictionary<PdfIndirectReference, ParentTreeHandler.PageMcrsContainer> pageToPageMcrs;

        private IDictionary<PdfIndirectReference, int?> pageToStructParentsInd;

        private IDictionary<PdfIndirectReference, int?> xObjectToStructParentsInd;

        private int maxStructParentIndex = -1;

//\cond DO_NOT_DOCUMENT
        /// <summary>Init ParentTreeHandler.</summary>
        /// <remarks>Init ParentTreeHandler. On init the parent tree is read and stored in this instance.</remarks>
        internal ParentTreeHandler(PdfStructTreeRoot structTreeRoot) {
            this.structTreeRoot = structTreeRoot;
            parentTree = new PdfNumTree(structTreeRoot.GetDocument().GetCatalog(), PdfName.ParentTree);
            xObjectToStructParentsInd = new Dictionary<PdfIndirectReference, int?>();
            RegisterAllMcrs();
            pageToStructParentsInd = new Dictionary<PdfIndirectReference, int?>();
        }
//\endcond

        /// <summary>Gets a list of all marked content references on the page.</summary>
        public virtual ParentTreeHandler.PageMcrsContainer GetPageMarkedContentReferences(PdfPage page) {
            return pageToPageMcrs.Get(page.GetPdfObject().GetIndirectReference());
        }

        // Mind that this method searches among items contained in page's content stream  only
        public virtual PdfMcr FindMcrByMcid(PdfDictionary pageDict, int mcid) {
            ParentTreeHandler.PageMcrsContainer pageMcrs = pageToPageMcrs.Get(pageDict.GetIndirectReference());
            return pageMcrs != null ? pageMcrs.GetPageContentStreamsMcrs().Get(mcid) : null;
        }

        public virtual PdfObjRef FindObjRefByStructParentIndex(PdfDictionary pageDict, int structParentIndex) {
            ParentTreeHandler.PageMcrsContainer pageMcrs = pageToPageMcrs.Get(pageDict.GetIndirectReference());
            return pageMcrs != null ? (PdfObjRef)pageMcrs.GetObjRefs().Get(structParentIndex) : null;
        }

        public virtual int GetNextMcidForPage(PdfPage page) {
            ParentTreeHandler.PageMcrsContainer pageMcrs = GetPageMarkedContentReferences(page);
            if (pageMcrs == null || pageMcrs.GetPageContentStreamsMcrs().Count == 0) {
                return 0;
            }
            else {
                return (int)Enumerable.Last(pageMcrs.GetPageContentStreamsMcrs()).Key + 1;
            }
        }

        /// <summary>Creates and flushes parent tree entry for the page.</summary>
        /// <remarks>
        /// Creates and flushes parent tree entry for the page.
        /// Effectively this means that new content mustn't be added to the page.
        /// </remarks>
        /// <param name="page">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// for which to create parent tree entry. Typically this page is flushed after this
        /// call.
        /// </param>
        public virtual void CreateParentTreeEntryForPage(PdfPage page) {
            ParentTreeHandler.PageMcrsContainer mcrs = GetPageMarkedContentReferences(page);
            if (mcrs == null) {
                return;
            }
            pageToPageMcrs.JRemove(page.GetPdfObject().GetIndirectReference());
            if (UpdateStructParentTreeEntries(page, mcrs)) {
                structTreeRoot.SetModified();
            }
        }

        public virtual void SavePageStructParentIndexIfNeeded(PdfPage page) {
            PdfIndirectReference indRef = page.GetPdfObject().GetIndirectReference();
            if (page.IsFlushed() || pageToPageMcrs.Get(indRef) == null) {
                return;
            }
            bool hasNonObjRefMcr = pageToPageMcrs.Get(indRef).GetPageContentStreamsMcrs().Count > 0 || pageToPageMcrs.
                Get(indRef).GetPageResourceXObjects().Count > 0;
            if (hasNonObjRefMcr) {
                pageToStructParentsInd.Put(indRef, (int?)GetOrCreatePageStructParentIndex(page));
            }
        }

        public virtual PdfDictionary BuildParentTree() {
            return (PdfDictionary)parentTree.BuildTree().MakeIndirect(structTreeRoot.GetDocument());
        }

        public virtual void RegisterMcr(PdfMcr mcr) {
            RegisterMcr(mcr, false);
        }

        private void RegisterMcr(PdfMcr mcr, bool registeringOnInit) {
            PdfIndirectReference mcrPageIndRef = mcr.GetPageIndirectReference();
            if (mcrPageIndRef == null || (!(mcr is PdfObjRef) && mcr.GetMcid() < 0)) {
                LOGGER.LogError(iText.IO.Logs.IoLogMessageConstant.ENCOUNTERED_INVALID_MCR);
                return;
            }
            ParentTreeHandler.PageMcrsContainer pageMcrs = pageToPageMcrs.Get(mcrPageIndRef);
            if (pageMcrs == null) {
                pageMcrs = new ParentTreeHandler.PageMcrsContainer();
                pageToPageMcrs.Put(mcrPageIndRef, pageMcrs);
            }
            PdfObject stm;
            if ((stm = GetStm(mcr)) != null) {
                PdfIndirectReference stmIndRef;
                PdfStream xObjectStream;
                if (stm is PdfIndirectReference) {
                    stmIndRef = (PdfIndirectReference)stm;
                    xObjectStream = (PdfStream)stmIndRef.GetRefersTo();
                }
                else {
                    if (stm.GetIndirectReference() == null) {
                        stm.MakeIndirect(structTreeRoot.GetDocument());
                    }
                    stmIndRef = stm.GetIndirectReference();
                    xObjectStream = (PdfStream)stm;
                }
                int? structParent = xObjectStream.GetAsInt(PdfName.StructParents);
                if (structParent != null) {
                    xObjectToStructParentsInd.Put(stmIndRef, structParent);
                    if (registeringOnInit) {
                        xObjectStream.Release();
                    }
                }
                else {
                    if (IsModificationAllowed()) {
                        maxStructParentIndex++;
                        xObjectToStructParentsInd.Put(stmIndRef, maxStructParentIndex);
                        xObjectStream.Put(PdfName.StructParents, new PdfNumber(maxStructParentIndex));
                        structTreeRoot.GetPdfObject().Put(PdfName.ParentTreeNextKey, new PdfNumber(maxStructParentIndex + 1));
                        LOGGER.LogWarning(KernelLogMessageConstant.XOBJECT_STRUCT_PARENT_INDEX_MISSED_AND_RECREATED);
                    }
                    else {
                        throw new PdfException(KernelExceptionMessageConstant.XOBJECT_STRUCT_PARENT_INDEX_MISSED);
                    }
                }
                pageMcrs.PutXObjectMcr(stmIndRef, mcr);
            }
            else {
                if (mcr is PdfObjRef) {
                    PdfObject mcrObj = ((PdfDictionary)mcr.GetPdfObject()).Get(PdfName.Obj);
                    if (!(mcrObj is PdfDictionary)) {
                        throw new PdfException(KernelExceptionMessageConstant.INVALID_OBJECT_REFERENCE_TYPE);
                    }
                    PdfDictionary obj = (PdfDictionary)mcrObj;
                    if (obj.IsFlushed()) {
                        throw new PdfException(KernelExceptionMessageConstant.WHEN_ADDING_OBJECT_REFERENCE_TO_THE_TAG_TREE_IT_MUST_BE_CONNECTED_TO_NOT_FLUSHED_OBJECT
                            );
                    }
                    PdfNumber n = obj.GetAsNumber(PdfName.StructParent);
                    if (n != null) {
                        pageMcrs.PutObjectReferenceMcr(n.IntValue(), mcr);
                    }
                    else {
                        if (IsModificationAllowed()) {
                            maxStructParentIndex++;
                            pageMcrs.PutObjectReferenceMcr(maxStructParentIndex, mcr);
                            obj.Put(PdfName.StructParent, new PdfNumber(maxStructParentIndex));
                            structTreeRoot.GetPdfObject().Put(PdfName.ParentTreeNextKey, new PdfNumber(maxStructParentIndex + 1));
                            LOGGER.LogWarning(KernelLogMessageConstant.STRUCT_PARENT_INDEX_MISSED_AND_RECREATED);
                        }
                        else {
                            throw new PdfException(KernelExceptionMessageConstant.STRUCT_PARENT_INDEX_NOT_FOUND_IN_TAGGED_OBJECT);
                        }
                    }
                }
                else {
                    pageMcrs.PutPageContentStreamMcr(mcr.GetMcid(), mcr);
                }
            }
            if (!registeringOnInit) {
                structTreeRoot.SetModified();
            }
        }

        public virtual void UnregisterMcr(PdfMcr mcrToUnregister) {
            PdfDictionary pageDict = mcrToUnregister.GetPageObject();
            if (pageDict == null) {
                // invalid mcr, ignore
                return;
            }
            if (pageDict.IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_REMOVE_MARKED_CONTENT_REFERENCE_BECAUSE_ITS_PAGE_WAS_ALREADY_FLUSHED
                    );
            }
            ParentTreeHandler.PageMcrsContainer pageMcrs = pageToPageMcrs.Get(pageDict.GetIndirectReference());
            if (pageMcrs != null) {
                PdfObject stm;
                if ((stm = GetStm(mcrToUnregister)) != null) {
                    PdfIndirectReference xObjectReference = stm is PdfIndirectReference ? (PdfIndirectReference)stm : stm.GetIndirectReference
                        ();
                    pageMcrs.GetPageResourceXObjects().Get(xObjectReference).JRemove(mcrToUnregister.GetMcid());
                    if (pageMcrs.GetPageResourceXObjects().Get(xObjectReference).IsEmpty()) {
                        pageMcrs.GetPageResourceXObjects().JRemove(xObjectReference);
                        xObjectToStructParentsInd.JRemove(xObjectReference);
                    }
                    structTreeRoot.SetModified();
                }
                else {
                    if (mcrToUnregister is PdfObjRef) {
                        foreach (KeyValuePair<int, PdfMcr> entry in pageMcrs.GetObjRefs()) {
                            if (entry.Value.GetPdfObject() == mcrToUnregister.GetPdfObject()) {
                                pageMcrs.GetObjRefs().JRemove(entry.Key);
                                structTreeRoot.SetModified();
                                break;
                            }
                        }
                    }
                    else {
                        pageMcrs.GetPageContentStreamsMcrs().JRemove(mcrToUnregister.GetMcid());
                        structTreeRoot.SetModified();
                    }
                }
            }
        }

        private bool IsModificationAllowed() {
            PdfReader reader = this.structTreeRoot.GetDocument().GetReader();
            if (reader != null) {
                return PdfReader.StrictnessLevel.CONSERVATIVE.IsStricter(reader.GetStrictnessLevel());
            }
            else {
                return true;
            }
        }

        private void RegisterAllMcrs() {
            pageToPageMcrs = new Dictionary<PdfIndirectReference, ParentTreeHandler.PageMcrsContainer>();
            // we create new number tree and not using parentTree, because we want parentTree to be empty
            IDictionary<int?, PdfObject> parentTreeEntries = new PdfNumTree(structTreeRoot.GetDocument().GetCatalog(), 
                PdfName.ParentTree).GetNumbers();
            ICollection<PdfDictionary> mcrParents = new LinkedHashSet<PdfDictionary>();
            foreach (KeyValuePair<int?, PdfObject> entry in parentTreeEntries) {
                if (entry.Key > maxStructParentIndex) {
                    maxStructParentIndex = (int)entry.Key;
                }
                PdfObject entryValue = entry.Value;
                if (entryValue.IsDictionary()) {
                    mcrParents.Add((PdfDictionary)entryValue);
                }
                else {
                    if (entryValue.IsArray()) {
                        PdfArray parentsArray = (PdfArray)entryValue;
                        for (int i = 0; i < parentsArray.Size(); ++i) {
                            PdfDictionary parent = parentsArray.GetAsDictionary(i);
                            if (parent != null) {
                                mcrParents.Add(parent);
                            }
                        }
                    }
                }
            }
            structTreeRoot.GetPdfObject().Put(PdfName.ParentTreeNextKey, new PdfNumber(maxStructParentIndex + 1));
            foreach (PdfObject mcrParent in mcrParents) {
                PdfStructElem mcrParentStructElem = new PdfStructElem((PdfDictionary)mcrParent);
                foreach (IStructureNode kid in mcrParentStructElem.GetKids()) {
                    if (kid is PdfMcr) {
                        RegisterMcr((PdfMcr)kid, true);
                    }
                }
            }
        }

        private bool UpdateStructParentTreeEntries(PdfPage page, ParentTreeHandler.PageMcrsContainer mcrs) {
            bool res = false;
            foreach (KeyValuePair<int, PdfMcr> entry in mcrs.GetObjRefs()) {
                PdfMcr mcr = entry.Value;
                PdfDictionary parentObj = ((PdfStructElem)mcr.GetParent()).GetPdfObject();
                if (!parentObj.IsIndirect()) {
                    continue;
                }
                int structParent = entry.Key;
                parentTree.AddEntry(structParent, parentObj);
                res = true;
            }
            int pageStructParentIndex;
            foreach (KeyValuePair<PdfIndirectReference, SortedDictionary<int, PdfMcr>> entry in mcrs.GetPageResourceXObjects
                ()) {
                PdfIndirectReference xObjectRef = entry.Key;
                if (xObjectToStructParentsInd.ContainsKey(xObjectRef)) {
                    pageStructParentIndex = (int)xObjectToStructParentsInd.JRemove(xObjectRef);
                    if (UpdateStructParentTreeForContentStreamEntries(entry.Value, pageStructParentIndex)) {
                        res = true;
                    }
                }
            }
            if (page.IsFlushed()) {
                PdfIndirectReference pageRef = page.GetPdfObject().GetIndirectReference();
                if (!pageToStructParentsInd.ContainsKey(pageRef)) {
                    return res;
                }
                pageStructParentIndex = (int)pageToStructParentsInd.JRemove(pageRef);
            }
            else {
                pageStructParentIndex = GetOrCreatePageStructParentIndex(page);
            }
            if (UpdateStructParentTreeForContentStreamEntries(mcrs.GetPageContentStreamsMcrs(), pageStructParentIndex)
                ) {
                res = true;
            }
            return res;
        }

        private bool UpdateStructParentTreeForContentStreamEntries(IDictionary<int, PdfMcr> mcrsOfContentStream, int
             pageStructParentIndex) {
            // element indices in parentsOfMcrs shall be the same as mcid of one of their kids.
            // See "Finding Structure Elements from Content Items" in pdf spec.
            PdfArray parentsOfMcrs = new PdfArray();
            int currentMcid = 0;
            foreach (KeyValuePair<int, PdfMcr> entry in mcrsOfContentStream) {
                PdfMcr mcr = entry.Value;
                PdfDictionary parentObj = ((PdfStructElem)mcr.GetParent()).GetPdfObject();
                if (!parentObj.IsIndirect()) {
                    continue;
                }
                // if for some reason some mcrs were not registered or don't exist, we ensure that the rest
                // of the parent objects were placed at correct index
                while (currentMcid++ < mcr.GetMcid()) {
                    parentsOfMcrs.Add(PdfNull.PDF_NULL);
                }
                parentsOfMcrs.Add(parentObj);
            }
            if (!parentsOfMcrs.IsEmpty()) {
                parentsOfMcrs.MakeIndirect(structTreeRoot.GetDocument());
                parentTree.AddEntry(pageStructParentIndex, parentsOfMcrs);
                structTreeRoot.GetDocument().CheckIsoConformance(parentsOfMcrs, IsoKey.TAG_STRUCTURE_ELEMENT);
                parentsOfMcrs.Flush();
                return true;
            }
            return false;
        }

        private int GetOrCreatePageStructParentIndex(PdfPage page) {
            int structParentIndex = page.GetStructParentIndex();
            if (structParentIndex < 0) {
                structParentIndex = page.GetDocument().GetNextStructParentIndex();
                page.GetPdfObject().Put(PdfName.StructParents, new PdfNumber(structParentIndex));
            }
            return structParentIndex;
        }

        private static PdfObject GetStm(PdfMcr mcr) {
            /*
            * Presence of Stm guarantees that the mcr belongs to XObject, absence of Stm guarantees that the mcr belongs to page content stream.
            * See 14.7.4.2 Marked-Content Sequences as Content Items, Table 324 – Entries in a marked-content reference dictionary.
            */
            if (mcr is PdfMcrDictionary) {
                return ((PdfDictionary)mcr.GetPdfObject()).Get(PdfName.Stm, false);
            }
            return null;
        }

//\cond DO_NOT_DOCUMENT
        internal class PageMcrsContainer {
//\cond DO_NOT_DOCUMENT
            internal IDictionary<int, PdfMcr> objRefs;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal SortedDictionary<int, PdfMcr> pageContentStreams;
//\endcond

//\cond DO_NOT_DOCUMENT
            /*
            * Keys of this map are indirect references to XObjects contained in page's resources,
            * values are the mcrs contained in the corresponding XObject streams, stored as mappings "MCID-number to PdfMcr".
            */
            internal IDictionary<PdfIndirectReference, SortedDictionary<int, PdfMcr>> pageResourceXObjects;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal PageMcrsContainer() {
                objRefs = new LinkedDictionary<int, PdfMcr>();
                pageContentStreams = new SortedDictionary<int, PdfMcr>();
                pageResourceXObjects = new LinkedDictionary<PdfIndirectReference, SortedDictionary<int, PdfMcr>>();
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual void PutObjectReferenceMcr(int structParentIndex, PdfMcr mcr) {
                objRefs.Put(structParentIndex, mcr);
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual void PutPageContentStreamMcr(int mcid, PdfMcr mcr) {
                pageContentStreams.Put(mcid, mcr);
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual void PutXObjectMcr(PdfIndirectReference xObjectIndRef, PdfMcr mcr) {
                SortedDictionary<int, PdfMcr> xObjectMcrs = pageResourceXObjects.Get(xObjectIndRef);
                if (xObjectMcrs == null) {
                    xObjectMcrs = new SortedDictionary<int, PdfMcr>();
                    pageResourceXObjects.Put(xObjectIndRef, xObjectMcrs);
                }
                pageResourceXObjects.Get(xObjectIndRef).Put(mcr.GetMcid(), mcr);
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual SortedDictionary<int, PdfMcr> GetPageContentStreamsMcrs() {
                return pageContentStreams;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual IDictionary<int, PdfMcr> GetObjRefs() {
                return objRefs;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual IDictionary<PdfIndirectReference, SortedDictionary<int, PdfMcr>> GetPageResourceXObjects(
                ) {
                return pageResourceXObjects;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual ICollection<PdfMcr> GetAllMcrsAsCollection() {
                ICollection<PdfMcr> collection = new List<PdfMcr>();
                collection.AddAll(objRefs.Values);
                collection.AddAll(pageContentStreams.Values);
                foreach (KeyValuePair<PdfIndirectReference, SortedDictionary<int, PdfMcr>> entry in pageResourceXObjects) {
                    collection.AddAll(entry.Value.Values);
                }
                return collection;
            }
//\endcond
        }
//\endcond
    }
//\endcond
}
