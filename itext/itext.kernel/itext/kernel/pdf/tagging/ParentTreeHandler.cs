/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Linq;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>
    /// Internal helper class which is used to effectively build parent tree and also find marked content references:
    /// for specified page, by MCID or by struct parent index.
    /// </summary>
    internal class ParentTreeHandler {
        private PdfStructTreeRoot structTreeRoot;

        /// <summary>Represents parentTree in structTreeRoot.</summary>
        /// <remarks>Represents parentTree in structTreeRoot. It contains only those entries that belong to the already flushed pages.
        ///     </remarks>
        private PdfNumTree parentTree;

        private IDictionary<PdfIndirectReference, ParentTreeHandler.PageMcrsContainer> pageToPageMcrs;

        private IDictionary<PdfIndirectReference, int?> pageToStructParentsInd;

        private IDictionary<PdfIndirectReference, int?> xObjectToStructParentsInd;

        /// <summary>Init ParentTreeHandler.</summary>
        /// <remarks>Init ParentTreeHandler. On init the parent tree is read and stored in this instance.</remarks>
        internal ParentTreeHandler(PdfStructTreeRoot structTreeRoot) {
            this.structTreeRoot = structTreeRoot;
            parentTree = new PdfNumTree(structTreeRoot.GetDocument().GetCatalog(), PdfName.ParentTree);
            xObjectToStructParentsInd = new Dictionary<PdfIndirectReference, int?>();
            RegisterAllMcrs();
            pageToStructParentsInd = new Dictionary<PdfIndirectReference, int?>();
        }

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
            // TODO checking for XObject-related mcrs is here to keep up the same behaviour that should be fixed in the scope of DEVSIX-3351
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
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagging.ParentTreeHandler));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ENCOUNTERED_INVALID_MCR);
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
                }
                else {
                    // TODO DEVSIX-3351 an error is thrown here because right now no /StructParents will be created.
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagging.ParentTreeHandler));
                    logger.LogError(iText.IO.Logs.IoLogMessageConstant.XOBJECT_HAS_NO_STRUCT_PARENTS);
                }
                pageMcrs.PutXObjectMcr(stmIndRef, mcr);
                if (registeringOnInit) {
                    xObjectStream.Release();
                }
            }
            else {
                if (mcr is PdfObjRef) {
                    PdfDictionary obj = ((PdfDictionary)mcr.GetPdfObject()).GetAsDictionary(PdfName.Obj);
                    if (obj == null || obj.IsFlushed()) {
                        throw new PdfException(KernelExceptionMessageConstant.WHEN_ADDING_OBJECT_REFERENCE_TO_THE_TAG_TREE_IT_MUST_BE_CONNECTED_TO_NOT_FLUSHED_OBJECT
                            );
                    }
                    PdfNumber n = obj.GetAsNumber(PdfName.StructParent);
                    if (n != null) {
                        pageMcrs.PutObjectReferenceMcr(n.IntValue(), mcr);
                    }
                    else {
                        throw new PdfException(KernelExceptionMessageConstant.STRUCT_PARENT_INDEX_NOT_FOUND_IN_TAGGED_OBJECT);
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
                        PdfDictionary obj = ((PdfDictionary)mcrToUnregister.GetPdfObject()).GetAsDictionary(PdfName.Obj);
                        if (obj != null && !obj.IsFlushed()) {
                            PdfNumber n = obj.GetAsNumber(PdfName.StructParent);
                            if (n != null) {
                                pageMcrs.GetObjRefs().JRemove(n.IntValue());
                                structTreeRoot.SetModified();
                                return;
                            }
                        }
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

        private void RegisterAllMcrs() {
            pageToPageMcrs = new Dictionary<PdfIndirectReference, ParentTreeHandler.PageMcrsContainer>();
            // we create new number tree and not using parentTree, because we want parentTree to be empty
            IDictionary<int?, PdfObject> parentTreeEntries = new PdfNumTree(structTreeRoot.GetDocument().GetCatalog(), 
                PdfName.ParentTree).GetNumbers();
            ICollection<PdfDictionary> mcrParents = new LinkedHashSet<PdfDictionary>();
            int maxStructParentIndex = -1;
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

        internal class PageMcrsContainer {
            internal IDictionary<int, PdfMcr> objRefs;

            internal SortedDictionary<int, PdfMcr> pageContentStreams;

            /*
            * Keys of this map are indirect references to XObjects contained in page's resources,
            * values are the mcrs contained in the corresponding XObject streams, stored as mappings "MCID-number to PdfMcr".
            */
            internal IDictionary<PdfIndirectReference, SortedDictionary<int, PdfMcr>> pageResourceXObjects;

            internal PageMcrsContainer() {
                objRefs = new LinkedDictionary<int, PdfMcr>();
                pageContentStreams = new SortedDictionary<int, PdfMcr>();
                pageResourceXObjects = new LinkedDictionary<PdfIndirectReference, SortedDictionary<int, PdfMcr>>();
            }

            internal virtual void PutObjectReferenceMcr(int structParentIndex, PdfMcr mcr) {
                objRefs.Put(structParentIndex, mcr);
            }

            internal virtual void PutPageContentStreamMcr(int mcid, PdfMcr mcr) {
                pageContentStreams.Put(mcid, mcr);
            }

            internal virtual void PutXObjectMcr(PdfIndirectReference xObjectIndRef, PdfMcr mcr) {
                SortedDictionary<int, PdfMcr> xObjectMcrs = pageResourceXObjects.Get(xObjectIndRef);
                if (xObjectMcrs == null) {
                    xObjectMcrs = new SortedDictionary<int, PdfMcr>();
                    pageResourceXObjects.Put(xObjectIndRef, xObjectMcrs);
                }
                pageResourceXObjects.Get(xObjectIndRef).Put(mcr.GetMcid(), mcr);
            }

            internal virtual SortedDictionary<int, PdfMcr> GetPageContentStreamsMcrs() {
                return pageContentStreams;
            }

            internal virtual IDictionary<int, PdfMcr> GetObjRefs() {
                return objRefs;
            }

            internal virtual IDictionary<PdfIndirectReference, SortedDictionary<int, PdfMcr>> GetPageResourceXObjects(
                ) {
                return pageResourceXObjects;
            }

            internal virtual ICollection<PdfMcr> GetAllMcrsAsCollection() {
                ICollection<PdfMcr> collection = new List<PdfMcr>();
                collection.AddAll(objRefs.Values);
                collection.AddAll(pageContentStreams.Values);
                foreach (KeyValuePair<PdfIndirectReference, SortedDictionary<int, PdfMcr>> entry in pageResourceXObjects) {
                    collection.AddAll(entry.Value.Values);
                }
                return collection;
            }
        }
    }
}
