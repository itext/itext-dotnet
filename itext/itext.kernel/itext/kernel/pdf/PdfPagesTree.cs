/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.Kernel.Pdf {
    /// <summary>
    /// Algorithm for construction
    /// <see cref="PdfPages"/>
    /// tree
    /// </summary>
    internal class PdfPagesTree {
        internal const int DEFAULT_LEAF_SIZE = 10;

        private readonly int leafSize = DEFAULT_LEAF_SIZE;

        private IList<PdfIndirectReference> pageRefs;

        private IList<PdfPages> parents;

        private IList<PdfPage> pages;

        private PdfDocument document;

        private bool generated = false;

        private PdfPages root;

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfPagesTree));

        /// <summary>Creates a PdfPages tree.</summary>
        /// <param name="pdfCatalog">
        /// a
        /// <see cref="PdfCatalog"/>
        /// which will be used to create the tree
        /// </param>
        public PdfPagesTree(PdfCatalog pdfCatalog) {
            this.document = pdfCatalog.GetDocument();
            this.pageRefs = new List<PdfIndirectReference>();
            this.parents = new List<PdfPages>();
            this.pages = new List<PdfPage>();
            if (pdfCatalog.GetPdfObject().ContainsKey(PdfName.Pages)) {
                PdfDictionary pages = pdfCatalog.GetPdfObject().GetAsDictionary(PdfName.Pages);
                if (pages == null) {
                    throw new PdfException(KernelExceptionMessageConstant.INVALID_PAGE_STRUCTURE_PAGES_MUST_BE_PDF_DICTIONARY);
                }
                this.root = new PdfPages(0, int.MaxValue, pages, null);
                parents.Add(this.root);
                for (int i = 0; i < this.root.GetCount(); i++) {
                    this.pageRefs.Add(null);
                    this.pages.Add(null);
                }
            }
            else {
                this.root = null;
                this.parents.Add(new PdfPages(0, this.document));
            }
        }

        //in read mode we will create PdfPages from 0 to Count
        // and reserve null indexes for pageRefs and pages.
        /// <summary>
        /// Returns the
        /// <see cref="PdfPage"/>
        /// at the specified position in this list.
        /// </summary>
        /// <param name="pageNum">one-based index of the element to return</param>
        /// <returns>
        /// the
        /// <see cref="PdfPage"/>
        /// at the specified position in this list
        /// </returns>
        public virtual PdfPage GetPage(int pageNum) {
            if (pageNum < 1 || pageNum > GetNumberOfPages()) {
                throw new IndexOutOfRangeException(MessageFormatUtil.Format(KernelExceptionMessageConstant.REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                    , pageNum));
            }
            --pageNum;
            PdfPage pdfPage = pages[pageNum];
            if (pdfPage == null) {
                LoadPage(pageNum);
                if (pageRefs[pageNum] != null) {
                    int parentIndex = FindPageParent(pageNum);
                    PdfObject pageObject = pageRefs[pageNum].GetRefersTo();
                    if (pageObject is PdfDictionary) {
                        pdfPage = document.GetPageFactory().CreatePdfPage((PdfDictionary)pageObject);
                        pdfPage.parentPages = parents[parentIndex];
                    }
                    else {
                        LOGGER.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE
                            , pageNum + 1));
                    }
                }
                else {
                    LOGGER.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE
                        , pageNum + 1));
                }
                pages[pageNum] = pdfPage;
            }
            if (pdfPage == null) {
                throw new PdfException(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE
                    , pageNum + 1));
            }
            return pdfPage;
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfPage"/>
        /// by page's PdfDictionary.
        /// </summary>
        /// <param name="pageDictionary">page's PdfDictionary</param>
        /// <returns>
        /// the
        /// <c>PdfPage</c>
        /// object, that wraps
        /// <paramref name="pageDictionary"/>.
        /// </returns>
        public virtual PdfPage GetPage(PdfDictionary pageDictionary) {
            int pageNum = GetPageNumber(pageDictionary);
            if (pageNum > 0) {
                return GetPage(pageNum);
            }
            return null;
        }

        /// <summary>Gets total number of @see PdfPages.</summary>
        /// <returns>total number of pages</returns>
        public virtual int GetNumberOfPages() {
            return pageRefs.Count;
        }

        /// <summary>
        /// Returns the index of the first occurrence of the specified page
        /// in this tree, or 0 if this tree does not contain the page.
        /// </summary>
        public virtual int GetPageNumber(PdfPage page) {
            return pages.IndexOf(page) + 1;
        }

        /// <summary>
        /// Returns the index of the first occurrence of the page in this tree
        /// specified by it's PdfDictionary, or 0 if this tree does not contain the page.
        /// </summary>
        public virtual int GetPageNumber(PdfDictionary pageDictionary) {
            int pageNum = pageRefs.IndexOf(pageDictionary.GetIndirectReference());
            if (pageNum >= 0) {
                return pageNum + 1;
            }
            for (int i = 0; i < pageRefs.Count; i++) {
                if (pageRefs[i] == null) {
                    LoadPage(i);
                }
                if (pageRefs[i].Equals(pageDictionary.GetIndirectReference())) {
                    return i + 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Appends the specified
        /// <see cref="PdfPage"/>
        /// to the end of this tree.
        /// </summary>
        /// <param name="pdfPage">
        /// a
        /// <see cref="PdfPage"/>
        /// to be added
        /// </param>
        public virtual void AddPage(PdfPage pdfPage) {
            PdfPages pdfPages;
            if (root != null) {
                // in this case we save tree structure
                if (pageRefs.Count == 0) {
                    pdfPages = root;
                }
                else {
                    LoadPage(pageRefs.Count - 1);
                    pdfPages = parents[parents.Count - 1];
                }
            }
            else {
                pdfPages = parents[parents.Count - 1];
                if (pdfPages.GetCount() % leafSize == 0 && pageRefs.Count > 0) {
                    pdfPages = new PdfPages(pdfPages.GetFrom() + pdfPages.GetCount(), document);
                    parents.Add(pdfPages);
                }
            }
            pdfPage.MakeIndirect(document);
            pdfPages.AddPage(pdfPage.GetPdfObject());
            pdfPage.parentPages = pdfPages;
            pageRefs.Add(pdfPage.GetPdfObject().GetIndirectReference());
            pages.Add(pdfPage);
        }

        /// <summary>
        /// Inserts
        /// <see cref="PdfPage"/>
        /// into specific one-based position.
        /// </summary>
        /// <param name="index">one-base index of the page</param>
        /// <param name="pdfPage">
        /// 
        /// <see cref="PdfPage"/>
        /// to insert.
        /// </param>
        public virtual void AddPage(int index, PdfPage pdfPage) {
            --index;
            if (index > pageRefs.Count) {
                throw new IndexOutOfRangeException("index");
            }
            if (index == pageRefs.Count) {
                AddPage(pdfPage);
                return;
            }
            LoadPage(index);
            pdfPage.MakeIndirect(document);
            int parentIndex = FindPageParent(index);
            PdfPages parentPages = parents[parentIndex];
            parentPages.AddPage(index, pdfPage);
            pdfPage.parentPages = parentPages;
            CorrectPdfPagesFromProperty(parentIndex + 1, +1);
            pageRefs.Add(index, pdfPage.GetPdfObject().GetIndirectReference());
            pages.Add(index, pdfPage);
        }

        /// <summary>Removes the page at the specified position in this tree.</summary>
        /// <remarks>
        /// Removes the page at the specified position in this tree.
        /// Shifts any subsequent elements to the left (subtracts one from their
        /// indices).
        /// </remarks>
        /// <param name="pageNum">the one-based index of the PdfPage to be removed</param>
        /// <returns>the page that was removed from the list</returns>
        public virtual PdfPage RemovePage(int pageNum) {
            PdfPage pdfPage = GetPage(pageNum);
            if (pdfPage.IsFlushed()) {
                LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED);
            }
            if (InternalRemovePage(--pageNum)) {
                return pdfPage;
            }
            else {
                return null;
            }
        }

        internal virtual void ReleasePage(int pageNumber) {
            --pageNumber;
            if (pageRefs[pageNumber] != null && !pageRefs[pageNumber].CheckState(PdfObject.FLUSHED) && !pageRefs[pageNumber
                ].CheckState(PdfObject.MODIFIED) && (pageRefs[pageNumber].GetOffset() > 0 || pageRefs[pageNumber].GetIndex
                () >= 0)) {
                pages[pageNumber] = null;
            }
        }

        /// <summary>Generate PdfPages tree.</summary>
        /// <returns>
        /// root
        /// <see cref="PdfPages"/>
        /// </returns>
        protected internal virtual PdfObject GenerateTree() {
            if (pageRefs.Count == 0) {
                LOGGER.LogInformation(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_TO_GENERATE_PDF_PAGES_TREE_WITHOUT_ANY_PAGES
                    );
                document.AddNewPage();
            }
            if (generated) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_PAGES_TREE_COULD_BE_GENERATED_ONLY_ONCE);
            }
            if (root == null) {
                while (parents.Count != 1) {
                    IList<PdfPages> nextParents = new List<PdfPages>();
                    //dynamicLeafSize helps to avoid PdfPages leaf with only one page
                    int dynamicLeafSize = leafSize;
                    PdfPages current = null;
                    for (int i = 0; i < parents.Count; i++) {
                        PdfPages pages = parents[i];
                        int pageCount = pages.GetCount();
                        if (i % dynamicLeafSize == 0) {
                            if (pageCount <= 1) {
                                dynamicLeafSize++;
                            }
                            else {
                                current = new PdfPages(-1, document);
                                nextParents.Add(current);
                                dynamicLeafSize = leafSize;
                            }
                        }
                        current.AddPages(pages);
                    }
                    parents = nextParents;
                }
                root = parents[0];
            }
            generated = true;
            return root.GetPdfObject();
        }

        protected internal virtual void ClearPageRefs() {
            pageRefs = null;
            pages = null;
        }

        protected internal virtual IList<PdfPages> GetParents() {
            return parents;
        }

        protected internal virtual PdfPages GetRoot() {
            return root;
        }

        protected internal virtual PdfPages FindPageParent(PdfPage pdfPage) {
            int pageNum = GetPageNumber(pdfPage) - 1;
            int parentIndex = FindPageParent(pageNum);
            return parents[parentIndex];
        }

        private void LoadPage(int pageNum) {
            LoadPage(pageNum, new HashSet<PdfIndirectReference>());
        }

        /// <summary>Load page from pages tree node structure</summary>
        /// <param name="pageNum">page number to load</param>
        /// <param name="processedParents">
        /// set with already processed parents object reference numbers
        /// if this method was called recursively to avoid infinite recursion.
        /// </param>
        private void LoadPage(int pageNum, ICollection<PdfIndirectReference> processedParents) {
            PdfIndirectReference targetPage = pageRefs[pageNum];
            if (targetPage != null) {
                return;
            }
            //if we go here, we have to split PdfPages that contains pageNum
            int parentIndex = FindPageParent(pageNum);
            PdfPages parent = parents[parentIndex];
            PdfIndirectReference parentIndirectReference = parent.GetPdfObject().GetIndirectReference();
            if (parentIndirectReference != null) {
                if (processedParents.Contains(parentIndirectReference)) {
                    throw new PdfException(KernelExceptionMessageConstant.INVALID_PAGE_STRUCTURE).SetMessageParams(pageNum + 1
                        );
                }
                else {
                    processedParents.Add(parentIndirectReference);
                }
            }
            PdfArray kids = parent.GetKids();
            if (kids == null) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_PAGE_STRUCTURE).SetMessageParams(pageNum + 1
                    );
            }
            int kidsCount = parent.GetCount();
            // we should handle separated pages, it means every PdfArray kids must contain either PdfPage or PdfPages,
            // mix of PdfPage and PdfPages not allowed.
            bool findPdfPages = false;
            // NOTE optimization? when we already found needed index
            for (int i = 0; i < kids.Size(); i++) {
                PdfDictionary page = kids.GetAsDictionary(i);
                // null values not allowed in pages tree.
                if (page == null) {
                    throw new PdfException(KernelExceptionMessageConstant.INVALID_PAGE_STRUCTURE).SetMessageParams(pageNum + 1
                        );
                }
                PdfObject pageKids = page.Get(PdfName.Kids);
                if (pageKids != null) {
                    if (pageKids.IsArray()) {
                        findPdfPages = true;
                    }
                    else {
                        // kids must be of type array
                        throw new PdfException(KernelExceptionMessageConstant.INVALID_PAGE_STRUCTURE).SetMessageParams(pageNum + 1
                            );
                    }
                }
                if (document.GetReader().IsMemorySavingMode() && !findPdfPages && parent.GetFrom() + i != pageNum) {
                    page.Release();
                }
            }
            if (findPdfPages) {
                // handle mix of PdfPage and PdfPages.
                // handle count property!
                IList<PdfPages> newParents = new List<PdfPages>(kids.Size());
                PdfPages lastPdfPages = null;
                for (int i = 0; i < kids.Size() && kidsCount > 0; i++) {
                    /*
                    * We don't release pdfPagesObject in the end of each loop because we enter this for-cycle only when
                    * parent has PdfPages kids.
                    * If all of the kids are PdfPages, then there's nothing to release, because we don't release
                    * PdfPages at this point.
                    * If there are kids that are instances of PdfPage, then there's no sense in releasing them:
                    * in this case ParentTreeStructure is being rebuilt by inserting an intermediate PdfPages between
                    * the parent and a PdfPage,
                    * thus modifying the page object by resetting its parent, thus making it impossible to release the
                    * object.
                    */
                    PdfDictionary pdfPagesObject = kids.GetAsDictionary(i);
                    if (pdfPagesObject.GetAsArray(PdfName.Kids) == null) {
                        // pdfPagesObject is PdfPage
                        // possible if only first kid is PdfPage
                        if (lastPdfPages == null) {
                            lastPdfPages = new PdfPages(parent.GetFrom(), document, parent);
                            kids.Set(i, lastPdfPages.GetPdfObject());
                            newParents.Add(lastPdfPages);
                        }
                        else {
                            // Only remove from kids if we did not replace the entry with new PdfPages
                            kids.Remove(i);
                            i--;
                        }
                        // decrement count first so that page is not counted twice when moved to lastPdfPages
                        parent.DecrementCount();
                        lastPdfPages.AddPage(pdfPagesObject);
                        kidsCount--;
                    }
                    else {
                        // pdfPagesObject is PdfPages
                        int from = lastPdfPages == null ? parent.GetFrom() : lastPdfPages.GetFrom() + lastPdfPages.GetCount();
                        lastPdfPages = new PdfPages(from, kidsCount, pdfPagesObject, parent);
                        newParents.Add(lastPdfPages);
                        kidsCount -= lastPdfPages.GetCount();
                    }
                }
                parents.JRemoveAt(parentIndex);
                for (int i = newParents.Count - 1; i >= 0; i--) {
                    parents.Add(parentIndex, newParents[i]);
                }
                // recursive call, to load needed pageRef.
                // NOTE optimization? add to loadPage startParentIndex.
                LoadPage(pageNum, processedParents);
            }
            else {
                int from = parent.GetFrom();
                // Possible exception in case kids.getSize() < parent.getCount().
                // In any case parent.getCount() has higher priority.
                // NOTE optimization? when we already found needed index
                for (int i = 0; i < parent.GetCount(); i++) {
                    PdfObject kid = kids.Get(i, false);
                    if (kid is PdfIndirectReference) {
                        pageRefs[from + i] = (PdfIndirectReference)kid;
                    }
                    else {
                        pageRefs[from + i] = kid.GetIndirectReference();
                    }
                }
            }
        }

        // zero-based index
        private bool InternalRemovePage(int pageNum) {
            int parentIndex = FindPageParent(pageNum);
            PdfPages pdfPages = parents[parentIndex];
            if (pdfPages.RemovePage(pageNum)) {
                if (pdfPages.GetCount() == 0) {
                    parents.JRemoveAt(parentIndex);
                    pdfPages.RemoveFromParent();
                    --parentIndex;
                }
                if (parents.Count == 0) {
                    root = null;
                    parents.Add(new PdfPages(0, document));
                }
                else {
                    CorrectPdfPagesFromProperty(parentIndex + 1, -1);
                }
                pageRefs.JRemoveAt(pageNum);
                pages.JRemoveAt(pageNum);
                return true;
            }
            else {
                return false;
            }
        }

        // zero-based index
        private int FindPageParent(int pageNum) {
            int low = 0;
            int high = parents.Count - 1;
            while (low != high) {
                int middle = (low + high + 1) / 2;
                if (parents[middle].CompareTo(pageNum) > 0) {
                    high = middle - 1;
                }
                else {
                    low = middle;
                }
            }
            return low;
        }

        private void CorrectPdfPagesFromProperty(int index, int correction) {
            for (int i = index; i < parents.Count; i++) {
                if (parents[i] != null) {
                    parents[i].CorrectFrom(correction);
                }
            }
        }
    }
}
