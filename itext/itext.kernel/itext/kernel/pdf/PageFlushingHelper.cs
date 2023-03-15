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
using iText.Commons.Utils;
using iText.Kernel.Events;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Layer;

namespace iText.Kernel.Pdf {
    /// <summary>This class allows to free the memory taken by already processed pages when handling big PDF files.
    ///     </summary>
    /// <remarks>
    /// This class allows to free the memory taken by already processed pages when handling big PDF files.
    /// It provides three alternative approaches for this, each of which has its own advantages and most suitable use cases:
    /// <see cref="UnsafeFlushDeep(int)"/>
    /// ,
    /// <see cref="ReleaseDeep(int)"/>
    /// ,
    /// <see cref="AppendModeFlush(int)"/>.
    /// <para />
    /// Each approach is designed to be most suitable for specific modes of document processing. There are four document
    /// processing modes: reading, writing, stamping and append mode.
    /// <para />
    /// Reading mode: The
    /// <see cref="PdfDocument"/>
    /// instance is initialized using only
    /// <see cref="PdfReader"/>
    /// by
    /// <see cref="PdfDocument.PdfDocument(PdfReader)"/>
    /// constructor.
    /// <para />
    /// Writing mode: The
    /// <see cref="PdfDocument"/>
    /// instance is initialized using only
    /// <see cref="PdfWriter"/>
    /// by
    /// <see cref="PdfDocument.PdfDocument(PdfWriter)"/>
    /// constructor.
    /// <para />
    /// Stamping mode: The
    /// <see cref="PdfDocument"/>
    /// instance is initialized using both
    /// <see cref="PdfReader"/>
    /// and
    /// <see cref="PdfWriter"/>
    /// by
    /// <see cref="PdfDocument.PdfDocument(PdfReader, PdfWriter)"/>
    /// constructor. If the optional third
    /// <see cref="StampingProperties"/>
    /// argument is passed, its
    /// <see cref="StampingProperties.UseAppendMode()"/>
    /// method shall NOT be called. <br />
    /// This mode allows to update the existing document by completely recreating it. The complete document will be rewritten
    /// by the end of
    /// <see cref="PdfDocument.Close()"/>
    /// call.
    /// <para />
    /// Append mode: The
    /// <see cref="PdfDocument"/>
    /// instance is initialized using both
    /// <see cref="PdfReader"/>
    /// and
    /// <see cref="PdfWriter"/>
    /// by
    /// <see cref="PdfDocument.PdfDocument(PdfReader, PdfWriter, StampingProperties)"/>
    /// constructor. The third
    /// <see cref="StampingProperties"/>
    /// argument shall have
    /// <see cref="StampingProperties.UseAppendMode()"/>
    /// method called. <br />
    /// This mode preserves the document intact with all its data, but adds additional data at the end of the file,
    /// which "overrides" and introduces amends to the original document. In this mode it's not required to rewrite the
    /// complete document which can be highly beneficial for big PDF documents handling.
    /// <para />
    /// The
    /// <see cref="PageFlushingHelper"/>
    /// class operates with two concepts of PDF objects states: flushed and released objects.
    /// <para />
    /// Flushed object is the one which is finalized and has been completely written to the output stream. This frees its
    /// memory but makes it impossible to modify it or read data from it. Whenever there is an attempt to modify or to fetch
    /// flushed object inner contents an exception will be thrown. Flushing is only possible for objects in the writing
    /// and stamping modes, also its possible to flush modified objects in append mode.
    /// <para />
    /// Released object is the one which has not been modified and has been "detached" from the
    /// <see cref="PdfDocument"/>
    /// , making it
    /// possible to remove it from memory during the GC, even if the document is not closed yet. All released object instances
    /// become read-only and any modifications will not be reflected in the resultant document. Read-only instances should be
    /// considered as copies of the original objects. Released objects can be re-read, however after re-reading new object
    /// instances are created. Releasing is only possible for not modified objects in reading, stamping and append modes.
    /// It's important to remember though, that during
    /// <see cref="PdfDocument.Close()"/>
    /// in stamping mode all released objects
    /// will be re-read.
    /// <para />
    /// The
    /// <see cref="PageFlushingHelper"/>
    /// class doesn't work with PdfADocument instances.
    /// </remarks>
    public class PageFlushingHelper {
        private static readonly PageFlushingHelper.DeepFlushingContext pageContext;

        static PageFlushingHelper() {
            pageContext = InitPageFlushingContext();
        }

        private PdfDocument pdfDoc;

        private bool release;

        // only PdfDictionary/PdfStream or PdfArray can be in this set.
        // Explicitly using HashSet for as field type for the sake of autoporting.
        private HashSet<PdfObject> currNestedObjParents = new HashSet<PdfObject>();

        private ICollection<PdfIndirectReference> layersRefs = new HashSet<PdfIndirectReference>();

        public PageFlushingHelper(PdfDocument pdfDoc) {
            this.pdfDoc = pdfDoc;
        }

        /// <summary>Flushes to the output stream all objects belonging to the given page.</summary>
        /// <remarks>
        /// Flushes to the output stream all objects belonging to the given page. This frees the memory taken by those
        /// objects, but makes it impossible to modify them or read data from them.
        /// <para />
        /// This method is mainly designed for writing and stamping modes. It will throw an exception for documents
        /// opened in reading mode (see
        /// <see cref="PageFlushingHelper"/>
        /// for more details on modes). This method can also be used for append
        /// mode if new pages are added or existing pages are heavily modified and
        /// <see cref="AppendModeFlush(int)"/>
        /// is not enough.
        /// <para />
        /// This method is highly effective in freeing the memory and works properly for the vast majority of documents
        /// and use cases, however it can potentially cause failures. If document handling fails with exception after
        /// using this method, one should re-process the document with a "safe flushing" alternative
        /// (see
        /// <see cref="PdfPage.Flush()"/>
        /// or consider using append mode and
        /// <see cref="AppendModeFlush(int)"/>
        /// method).
        /// <para />
        /// The unsafety comes from the possibility of objects being shared between pages and the fact that object data
        /// cannot be read after the flushing. Whenever flushed object is attempted to be modified or its data is fetched
        /// the exception will be thrown (flushed object can be added to the other objects, though).
        /// <para />
        /// In stamping/append mode the issue occurs if some object is shared between two or more pages, and the first page
        /// is flushed, and later for processing of the second page this object is required to be read/modified. Normally only
        /// page resources (like images and fonts) are shared, which are often not required for page processing: for example
        /// for page stamping (e.g. adding watermarks, headers, etc) only new resources are added. Among examples of when the
        /// page resources are indeed required (and therefore the risk of this method causing failures being high) would be
        /// page contents parsing: text extraction, any general
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.PdfCanvasProcessor"/>
        /// class usage, usage of pdfSweep addon.
        /// <para />
        /// In writing mode this method normally will work without issues: by default iText creates page objects in such way
        /// that they are independent from each other. Again, the resources can be shared, but as mentioned above
        /// it's safe to add already flushed resources to the other pages because this doesn't require reading data from them.
        /// <para />
        /// For append mode only modified objects are flushed, all others are released and can be re-read later on.
        /// <para />
        /// This method shall be used only when it's known that the page and its inner structures processing is finished.
        /// This includes reading data from pages, page modification and page handling via addons/utilities.
        /// </remarks>
        /// <param name="pageNum">the page number which low level objects structure is to be flushed to the output stream.
        ///     </param>
        public virtual void UnsafeFlushDeep(int pageNum) {
            if (pdfDoc.GetWriter() == null) {
                throw new ArgumentException(KernelExceptionMessageConstant.FLUSHING_HELPER_FLUSHING_MODE_IS_NOT_FOR_DOC_READING_MODE
                    );
            }
            release = false;
            FlushPage(pageNum);
        }

        /// <summary>Releases memory taken by all not modified objects belonging to the given page, including the page dictionary itself.
        ///     </summary>
        /// <remarks>
        /// Releases memory taken by all not modified objects belonging to the given page, including the page dictionary itself.
        /// This affects only the objects that are read from the existing input PDF.
        /// <para />
        /// This method is mainly designed for reading mode and also can be used in append mode (see
        /// <see cref="PageFlushingHelper"/>
        /// for more details on modes). In append mode modified objects will be kept in memory.
        /// The page and all its inner structure objects can be re-read again.
        /// <para />
        /// This method will not have any effect in the writing mode. It is also not advised to be used in stamping mode:
        /// even though it will indeed release the objects, they will be definitely re-read again on document closing, which
        /// would affect performance.
        /// <para />
        /// When using this method in append mode (or in stamping mode), be careful not to try to modify the object instances
        /// obtained before the releasing! See
        /// <see cref="PageFlushingHelper"/>
        /// for details on released objects state.
        /// <para />
        /// This method shall be used only when it's known that the page and its inner structures processing is finished.
        /// This includes reading data from pages, page modification and page handling via addons/utilities.
        /// </remarks>
        /// <param name="pageNum">the page number which low level objects structure is to be released from memory.</param>
        public virtual void ReleaseDeep(int pageNum) {
            release = true;
            FlushPage(pageNum);
        }

        /// <summary>
        /// Flushes to the output stream modified objects that can belong only to the given page, which makes this method
        /// "safe" compared to the
        /// <see cref="UnsafeFlushDeep(int)"/>.
        /// </summary>
        /// <remarks>
        /// Flushes to the output stream modified objects that can belong only to the given page, which makes this method
        /// "safe" compared to the
        /// <see cref="UnsafeFlushDeep(int)"/>
        /// . Flushed object frees the memory, but it's impossible to
        /// modify such objects or read data from them. This method releases all other page structure objects that are not
        /// modified.
        /// <para />
        /// This method is mainly designed for the append mode. It is similar to the
        /// <see cref="PdfPage.Flush()"/>
        /// , but it
        /// additionally releases all page objects that were not flushed. This method is ideal for small amendments of pages,
        /// but it makes more sense to use
        /// <see cref="PdfPage.Flush()"/>
        /// for newly created or heavily modified pages. <br />
        /// This method will throw an exception for documents opened in reading mode (see
        /// <see cref="PageFlushingHelper"/>
        /// for more details on modes). It is also not advised to be used in stamping mode: even though it will indeed
        /// release the objects and free the memory, the released objects will definitely be re-read again on document
        /// closing, which would affect performance.
        /// <para />
        /// When using this method in append mode (or in stamping mode), be careful not to try to modify the object instances
        /// obtained before this method call! See
        /// <see cref="PageFlushingHelper"/>
        /// for details on released and flushed objects state.
        /// <para />
        /// This method shall be used only when it's known that the page and its inner structures processing is finished.
        /// This includes reading data from pages, page modification and page handling via addons/utilities.
        /// </remarks>
        /// <param name="pageNum">the page number which low level objects structure is to be flushed or released from memory.
        ///     </param>
        public virtual void AppendModeFlush(int pageNum) {
            if (pdfDoc.GetWriter() == null) {
                throw new ArgumentException(KernelExceptionMessageConstant.FLUSHING_HELPER_FLUSHING_MODE_IS_NOT_FOR_DOC_READING_MODE
                    );
            }
            PdfPage page = pdfDoc.GetPage(pageNum);
            if (page.IsFlushed()) {
                return;
            }
            page.GetDocument().DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.END_PAGE, page));
            bool pageWasModified = page.GetPdfObject().IsModified();
            page.SetModified();
            release = true;
            pageWasModified = FlushPage(pageNum) || pageWasModified;
            PdfArray annots = page.GetPdfObject().GetAsArray(PdfName.Annots);
            if (annots != null && !annots.IsFlushed()) {
                ArrayFlushIfModified(annots);
            }
            PdfObject thumb = page.GetPdfObject().Get(PdfName.Thumb, false);
            FlushIfModified(thumb);
            PdfObject contents = page.GetPdfObject().Get(PdfName.Contents, false);
            if (contents is PdfIndirectReference) {
                if (contents.CheckState(PdfObject.MODIFIED) && !contents.CheckState(PdfObject.FLUSHED)) {
                    PdfObject contentsDirectObj = ((PdfIndirectReference)contents).GetRefersTo();
                    if (contentsDirectObj.IsArray()) {
                        ArrayFlushIfModified((PdfArray)contentsDirectObj);
                    }
                    else {
                        // already checked that modified
                        contentsDirectObj.Flush();
                    }
                }
            }
            else {
                if (contents is PdfArray) {
                    ArrayFlushIfModified((PdfArray)contents);
                }
                else {
                    if (contents is PdfStream) {
                        FlushIfModified(contents);
                    }
                }
            }
            // Page tags flushing is supported only in PdfPage#flush and #unsafeFlushDeep: it makes sense to flush tags
            // completely for heavily modified or new pages. For the slightly modified pages it should be enough to release
            // the tag structure objects via tag structure releasing utility.
            if (!pageWasModified) {
                page.GetPdfObject().GetIndirectReference().ClearState(PdfObject.MODIFIED);
                pdfDoc.GetCatalog().GetPageTree().ReleasePage(pageNum);
                page.UnsetForbidRelease();
                page.GetPdfObject().Release();
            }
            else {
                // inherited and modified resources are handled in #flushPage call in the beginning of method
                page.ReleaseInstanceFields();
                page.GetPdfObject().Flush();
            }
        }

        private bool FlushPage(int pageNum) {
            PdfPage page = pdfDoc.GetPage(pageNum);
            if (page.IsFlushed()) {
                return false;
            }
            bool pageChanged = false;
            if (!release) {
                pdfDoc.DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.END_PAGE, page));
                InitCurrentLayers(pdfDoc);
            }
            PdfDictionary pageDict = page.GetPdfObject();
            // Using PdfPage package internal methods in order to avoid PdfResources initialization: initializing PdfResources
            // limits processing possibilities only to cases in which resources and specific resource type dictionaries are not flushed.
            // inits /Resources dict entry if not inherited and not created yet
            PdfDictionary resourcesDict = page.InitResources(false);
            PdfResources resources = page.GetResources(false);
            if (resources != null && resources.IsModified() && !resources.IsReadOnly()) {
                resourcesDict = resources.GetPdfObject();
                pageDict.Put(PdfName.Resources, resources.GetPdfObject());
                pageDict.SetModified();
                pageChanged = true;
            }
            if (!resourcesDict.IsFlushed()) {
                FlushDictRecursively(resourcesDict, null);
                FlushOrRelease(resourcesDict);
            }
            FlushDictRecursively(pageDict, pageContext);
            if (release) {
                if (!page.GetPdfObject().IsModified()) {
                    pdfDoc.GetCatalog().GetPageTree().ReleasePage(pageNum);
                    page.UnsetForbidRelease();
                    page.GetPdfObject().Release();
                }
            }
            else {
                if (pdfDoc.IsTagged() && !pdfDoc.GetStructTreeRoot().IsFlushed()) {
                    page.TryFlushPageTags();
                }
                if (!pdfDoc.IsAppendMode() || page.GetPdfObject().IsModified()) {
                    page.ReleaseInstanceFields();
                    page.GetPdfObject().Flush();
                }
                else {
                    // it's append mode
                    pdfDoc.GetCatalog().GetPageTree().ReleasePage(pageNum);
                    page.UnsetForbidRelease();
                    page.GetPdfObject().Release();
                }
            }
            layersRefs.Clear();
            return pageChanged;
        }

        private void InitCurrentLayers(PdfDocument pdfDoc) {
            if (pdfDoc.GetCatalog().IsOCPropertiesMayHaveChanged()) {
                IList<PdfLayer> layers = pdfDoc.GetCatalog().GetOCProperties(false).GetLayers();
                foreach (PdfLayer layer in layers) {
                    layersRefs.Add(layer.GetPdfObject().GetIndirectReference());
                }
            }
        }

        private void FlushObjectRecursively(PdfObject obj, PageFlushingHelper.DeepFlushingContext context) {
            if (obj == null) {
                return;
            }
            bool avoidReleaseForIndirectObjInstance = false;
            if (obj.IsIndirectReference()) {
                PdfIndirectReference indRef = (PdfIndirectReference)obj;
                if (indRef.refersTo == null || indRef.CheckState(PdfObject.FLUSHED)) {
                    return;
                }
                obj = indRef.GetRefersTo();
            }
            else {
                if (obj.IsFlushed()) {
                    return;
                }
                else {
                    if (release && obj.IsIndirect()) {
                        // We should avoid the case when object is going to be released but is stored in containing object
                        // not as indirect reference. This can happen when containing object is somehow modified.
                        // Generally containing objects should not contain released read-only object instance.
                        System.Diagnostics.Debug.Assert(obj.IsReleaseForbidden() || obj.GetIndirectReference() == null);
                        avoidReleaseForIndirectObjInstance = true;
                    }
                }
            }
            if (pdfDoc.IsDocumentFont(obj.GetIndirectReference()) || layersRefs.Contains(obj.GetIndirectReference())) {
                return;
            }
            if (obj.IsDictionary() || obj.IsStream()) {
                if (!currNestedObjParents.Add(obj)) {
                    return;
                }
                FlushDictRecursively((PdfDictionary)obj, context);
                currNestedObjParents.Remove(obj);
            }
            else {
                if (obj.IsArray()) {
                    if (!currNestedObjParents.Add(obj)) {
                        return;
                    }
                    PdfArray array = (PdfArray)obj;
                    for (int i = 0; i < array.Size(); ++i) {
                        FlushObjectRecursively(array.Get(i, false), context);
                    }
                    currNestedObjParents.Remove(obj);
                }
            }
            if (!avoidReleaseForIndirectObjInstance) {
                FlushOrRelease(obj);
            }
        }

        private void FlushDictRecursively(PdfDictionary dict, PageFlushingHelper.DeepFlushingContext context) {
            foreach (PdfName key in dict.KeySet()) {
                PageFlushingHelper.DeepFlushingContext innerContext = null;
                if (context != null) {
                    if (context.IsKeyInBlackList(key)) {
                        continue;
                    }
                    innerContext = context.GetInnerContextFor(key);
                }
                PdfObject value = dict.Get(key, false);
                FlushObjectRecursively(value, innerContext);
            }
        }

        private void FlushOrRelease(PdfObject obj) {
            if (release) {
                if (!obj.IsReleaseForbidden()) {
                    obj.Release();
                }
            }
            else {
                MakeIndirectIfNeeded(obj);
                if (!pdfDoc.IsAppendMode() || obj.IsModified()) {
                    obj.Flush();
                }
                else {
                    if (!obj.IsReleaseForbidden()) {
                        obj.Release();
                    }
                }
            }
        }

        private void FlushIfModified(PdfObject o) {
            if (o != null && !(o is PdfIndirectReference)) {
                MakeIndirectIfNeeded(o);
                o = o.GetIndirectReference();
            }
            if (o != null && o.CheckState(PdfObject.MODIFIED) && !o.CheckState(PdfObject.FLUSHED)) {
                ((PdfIndirectReference)o).GetRefersTo().Flush();
            }
        }

        private void ArrayFlushIfModified(PdfArray contentsArr) {
            for (int i = 0; i < contentsArr.Size(); ++i) {
                PdfObject c = contentsArr.Get(i, false);
                FlushIfModified(c);
            }
        }

        private void MakeIndirectIfNeeded(PdfObject o) {
            if (o.CheckState(PdfObject.MUST_BE_INDIRECT)) {
                o.MakeIndirect(pdfDoc);
            }
        }

        private static PageFlushingHelper.DeepFlushingContext InitPageFlushingContext() {
            ICollection<PdfName> ALL_KEYS_IN_BLACK_LIST = null;
            IDictionary<PdfName, PageFlushingHelper.DeepFlushingContext> NO_INNER_CONTEXTS = JavaCollectionsUtil.EmptyMap
                <PdfName, PageFlushingHelper.DeepFlushingContext>();
            // --- action dictionary context ---
            PageFlushingHelper.DeepFlushingContext actionContext = new PageFlushingHelper.DeepFlushingContext(
                        // actions keys flushing blacklist
                        new LinkedHashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.D, PdfName.SD, PdfName.Dp, PdfName.B, PdfName.Annotation
                , PdfName.T, PdfName.AN, PdfName.TA)), NO_INNER_CONTEXTS);
            PageFlushingHelper.DeepFlushingContext aaContext = new PageFlushingHelper.DeepFlushingContext(
                        // all inner entries leading to this context
                        actionContext);
            // ---
            // --- annotation dictionary context ---
            LinkedDictionary<PdfName, PageFlushingHelper.DeepFlushingContext> annotInnerContexts = new LinkedDictionary
                <PdfName, PageFlushingHelper.DeepFlushingContext>();
            PageFlushingHelper.DeepFlushingContext annotsContext = new PageFlushingHelper.DeepFlushingContext(
                        // annotations flushing blacklist
                        new LinkedHashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.P, PdfName.Popup, PdfName.Dest, PdfName.Parent, PdfName
                .V)), 
                        // keys that belong to form fields which can be merged with widget annotations
                        annotInnerContexts);
            annotInnerContexts.Put(PdfName.A, actionContext);
            annotInnerContexts.Put(PdfName.PA, actionContext);
            annotInnerContexts.Put(PdfName.AA, aaContext);
            // ---
            // --- separation info dictionary context ---
            PageFlushingHelper.DeepFlushingContext sepInfoContext = new PageFlushingHelper.DeepFlushingContext(
                        // separation info dict flushing blacklist
                        new LinkedHashSet<PdfName>(JavaCollectionsUtil.SingletonList(PdfName.Pages)), NO_INNER_CONTEXTS);
            // ---
            // --- bead dictionary context ---
            PageFlushingHelper.DeepFlushingContext bContext = new PageFlushingHelper.DeepFlushingContext(
                        // bead dict flushing blacklist
                        ALL_KEYS_IN_BLACK_LIST, NO_INNER_CONTEXTS);
            // ---
            // --- pres steps dictionary context ---
            LinkedDictionary<PdfName, PageFlushingHelper.DeepFlushingContext> presStepsInnerContexts = new LinkedDictionary
                <PdfName, PageFlushingHelper.DeepFlushingContext>();
            PageFlushingHelper.DeepFlushingContext presStepsContext = new PageFlushingHelper.DeepFlushingContext(
                        // pres step dict flushing blacklist
                        new LinkedHashSet<PdfName>(JavaCollectionsUtil.SingletonList(PdfName.Prev)), presStepsInnerContexts);
            presStepsInnerContexts.Put(PdfName.NA, actionContext);
            presStepsInnerContexts.Put(PdfName.PA, actionContext);
            // ---
            // --- page dictionary context ---
            LinkedDictionary<PdfName, PageFlushingHelper.DeepFlushingContext> pageInnerContexts = new LinkedDictionary
                <PdfName, PageFlushingHelper.DeepFlushingContext>();
            PageFlushingHelper.DeepFlushingContext pageContext = new PageFlushingHelper.DeepFlushingContext(new LinkedHashSet
                <PdfName>(JavaUtil.ArraysAsList(PdfName.Parent, PdfName.DPart)), pageInnerContexts);
            pageInnerContexts.Put(PdfName.Annots, annotsContext);
            pageInnerContexts.Put(PdfName.B, bContext);
            pageInnerContexts.Put(PdfName.AA, aaContext);
            pageInnerContexts.Put(PdfName.SeparationInfo, sepInfoContext);
            pageInnerContexts.Put(PdfName.PresSteps, presStepsContext);
            // ---
            return pageContext;
        }

        private class DeepFlushingContext {
            // null stands for every key to be in black list
            internal ICollection<PdfName> blackList;

            // null stands for every key to be taking unconditional context
            internal IDictionary<PdfName, PageFlushingHelper.DeepFlushingContext> innerContexts;

            internal PageFlushingHelper.DeepFlushingContext unconditionalInnerContext;

            public DeepFlushingContext(ICollection<PdfName> blackList, IDictionary<PdfName, PageFlushingHelper.DeepFlushingContext
                > innerContexts) {
                this.blackList = blackList;
                this.innerContexts = innerContexts;
            }

            public DeepFlushingContext(PageFlushingHelper.DeepFlushingContext unconditionalInnerContext) {
                this.blackList = JavaCollectionsUtil.EmptySet<PdfName>();
                this.innerContexts = null;
                this.unconditionalInnerContext = unconditionalInnerContext;
            }

            public virtual bool IsKeyInBlackList(PdfName key) {
                return blackList == null || blackList.Contains(key);
            }

            public virtual PageFlushingHelper.DeepFlushingContext GetInnerContextFor(PdfName key) {
                return innerContexts == null ? unconditionalInnerContext : innerContexts.Get(key);
            }
        }
    }
}
