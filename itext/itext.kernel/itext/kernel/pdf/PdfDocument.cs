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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Actions.Data;
using iText.Kernel.Actions.Events;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Numbering;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Collection;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Statistics;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.Pdf {
    /// <summary>Main enter point to work with PDF document.</summary>
    public class PdfDocument : IEventDispatcher, IDisposable {
        //
        private static readonly PdfName[] PDF_NAMES_TO_REMOVE_FROM_ORIGINAL_TRAILER = new PdfName[] { PdfName.Encrypt
            , PdfName.Size, PdfName.Prev, PdfName.Root, PdfName.Info, PdfName.ID, PdfName.XRefStm };

        private static readonly IPdfPageFactory pdfPageFactory = new PdfPageFactory();

        protected internal readonly StampingProperties properties;

        /// <summary>List of indirect objects used in the document.</summary>
        internal readonly PdfXrefTable xref = new PdfXrefTable();

        private readonly IDictionary<PdfIndirectReference, PdfFont> documentFonts = new Dictionary<PdfIndirectReference
            , PdfFont>();

        private readonly SequenceId documentId;

        /// <summary>To be adjusted destinations.</summary>
        /// <remarks>
        /// To be adjusted destinations.
        /// Key - originating page on the source document
        /// Value - a hashmap of Parent pdf objects and destinations to be updated
        /// </remarks>
        private readonly IList<PdfDocument.DestinationMutationInfo> pendingDestinationMutations = new List<PdfDocument.DestinationMutationInfo
            >();

        protected internal EventDispatcher eventDispatcher = new EventDispatcher();

        /// <summary>PdfWriter associated with the document.</summary>
        /// <remarks>
        /// PdfWriter associated with the document.
        /// Not null if document opened either in writing or stamping mode.
        /// </remarks>
        protected internal PdfWriter writer = null;

        /// <summary>PdfReader associated with the document.</summary>
        /// <remarks>
        /// PdfReader associated with the document.
        /// Not null if document is opened either in reading or stamping mode.
        /// </remarks>
        protected internal PdfReader reader = null;

        /// <summary>XMP Metadata for the document.</summary>
        protected internal byte[] xmpMetadata = null;

        /// <summary>Document catalog.</summary>
        protected internal PdfCatalog catalog = null;

        /// <summary>Document trailed.</summary>
        protected internal PdfDictionary trailer = null;

        /// <summary>Document info.</summary>
        protected internal PdfDocumentInfo info = null;

        /// <summary>Document version.</summary>
        protected internal PdfVersion pdfVersion = PdfVersion.PDF_1_7;

        protected internal FingerPrint fingerPrint;

        protected internal SerializeOptions serializeOptions = new SerializeOptions();

        protected internal PdfStructTreeRoot structTreeRoot;

        protected internal int structParentIndex = -1;

        protected internal bool closeReader = true;

        protected internal bool closeWriter = true;

        protected internal bool isClosing = false;

        protected internal bool closed = false;

        /// <summary>flag determines whether to write unused objects to result document</summary>
        protected internal bool flushUnusedObjects = false;

        protected internal TagStructureContext tagStructureContext;

        /// <summary>Cache of already serialized objects from this document for smart mode.</summary>
        internal IDictionary<PdfIndirectReference, byte[]> serializedObjectsCache = new Dictionary<PdfIndirectReference
            , byte[]>();

        /// <summary>Handler which will be used for decompression of pdf streams.</summary>
        internal MemoryLimitsAwareHandler memoryLimitsAwareHandler = null;

        /// <summary>Default page size.</summary>
        /// <remarks>
        /// Default page size.
        /// New page by default will be created with this size.
        /// </remarks>
        private PageSize defaultPageSize = PageSize.DEFAULT;

        /// <summary>The original (first) id when the document is read initially.</summary>
        private PdfString originalDocumentId;

        /// <summary>The original modified (second) id when the document is read initially.</summary>
        private PdfString modifiedDocumentId;

        private PdfFont defaultFont = null;

        private EncryptedEmbeddedStreamsHandler encryptedEmbeddedStreamsHandler;

        /// <summary>Open PDF document in reading mode.</summary>
        /// <param name="reader">PDF reader.</param>
        public PdfDocument(PdfReader reader)
            : this(reader, new DocumentProperties()) {
        }

        /// <summary>Open PDF document in reading mode.</summary>
        /// <param name="reader">PDF reader.</param>
        /// <param name="properties">document properties</param>
        public PdfDocument(PdfReader reader, DocumentProperties properties) {
            if (reader == null) {
                throw new ArgumentException("The reader in PdfDocument constructor can not be null.");
            }
            documentId = new SequenceId();
            this.reader = reader;
            // default values of the StampingProperties doesn't affect anything
            this.properties = new StampingProperties();
            this.properties.SetEventCountingMetaInfo(properties.metaInfo);
            Open(null);
        }

        /// <summary>Open PDF document in writing mode.</summary>
        /// <remarks>
        /// Open PDF document in writing mode.
        /// Document has no pages when initialized.
        /// </remarks>
        /// <param name="writer">PDF writer</param>
        public PdfDocument(PdfWriter writer)
            : this(writer, new DocumentProperties()) {
        }

        /// <summary>Open PDF document in writing mode.</summary>
        /// <remarks>
        /// Open PDF document in writing mode.
        /// Document has no pages when initialized.
        /// </remarks>
        /// <param name="writer">PDF writer</param>
        /// <param name="properties">document properties</param>
        public PdfDocument(PdfWriter writer, DocumentProperties properties) {
            if (writer == null) {
                throw new ArgumentException("The writer in PdfDocument constructor can not be null.");
            }
            documentId = new SequenceId();
            this.writer = writer;
            // default values of the StampingProperties doesn't affect anything
            this.properties = new StampingProperties();
            this.properties.SetEventCountingMetaInfo(properties.metaInfo);
            Open(writer.properties.pdfVersion);
        }

        /// <summary>Opens PDF document in the stamping mode.</summary>
        /// <remarks>
        /// Opens PDF document in the stamping mode.
        /// <br />
        /// </remarks>
        /// <param name="reader">PDF reader.</param>
        /// <param name="writer">PDF writer.</param>
        public PdfDocument(PdfReader reader, PdfWriter writer)
            : this(reader, writer, new StampingProperties()) {
        }

        /// <summary>Open PDF document in stamping mode.</summary>
        /// <param name="reader">PDF reader.</param>
        /// <param name="writer">PDF writer.</param>
        /// <param name="properties">properties of the stamping process</param>
        public PdfDocument(PdfReader reader, PdfWriter writer, StampingProperties properties) {
            if (reader == null) {
                throw new ArgumentException("The reader in PdfDocument constructor can not be null.");
            }
            if (writer == null) {
                throw new ArgumentException("The writer in PdfDocument constructor can not be null.");
            }
            documentId = new SequenceId();
            this.reader = reader;
            this.writer = writer;
            this.properties = properties;
            bool writerHasEncryption = WriterHasEncryption();
            if (properties.appendMode && writerHasEncryption) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.WRITER_ENCRYPTION_IS_IGNORED_APPEND);
            }
            if (properties.preserveEncryption && writerHasEncryption) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.WRITER_ENCRYPTION_IS_IGNORED_PRESERVE);
            }
            Open(writer.properties.pdfVersion);
        }

        /// <summary>Sets the XMP Metadata.</summary>
        /// <param name="xmpMeta">the xmpMetadata to set</param>
        /// <param name="serializeOptions">serialization options</param>
        public virtual void SetXmpMetadata(XMPMeta xmpMeta, SerializeOptions serializeOptions) {
            this.serializeOptions = serializeOptions;
            SetXmpMetadata(XMPMetaFactory.SerializeToBuffer(xmpMeta, serializeOptions));
        }

        /// <summary>Use this method to set the XMP Metadata.</summary>
        /// <param name="xmpMetadata">The xmpMetadata to set.</param>
        protected internal virtual void SetXmpMetadata(byte[] xmpMetadata) {
            this.xmpMetadata = xmpMetadata;
        }

        /// <summary>Sets the XMP Metadata.</summary>
        /// <param name="xmpMeta">the xmpMetadata to set</param>
        public virtual void SetXmpMetadata(XMPMeta xmpMeta) {
            serializeOptions.SetPadding(2000);
            SetXmpMetadata(xmpMeta, serializeOptions);
        }

        /// <summary>Gets XMPMetadata.</summary>
        /// <returns>the XMPMetadata</returns>
        public virtual byte[] GetXmpMetadata() {
            return GetXmpMetadata(false);
        }

        /// <summary>Gets XMPMetadata or create a new one.</summary>
        /// <param name="createNew">if true, create a new empty XMPMetadata if it did not present.</param>
        /// <returns>existed or newly created XMPMetadata byte array.</returns>
        public virtual byte[] GetXmpMetadata(bool createNew) {
            if (xmpMetadata == null && createNew) {
                XMPMeta xmpMeta = XMPMetaFactory.Create();
                xmpMeta.SetObjectName(XMPConst.TAG_XMPMETA);
                xmpMeta.SetObjectName("");
                AddCustomMetadataExtensions(xmpMeta);
                try {
                    xmpMeta.SetProperty(XMPConst.NS_DC, PdfConst.Format, "application/pdf");
                    SetXmpMetadata(xmpMeta);
                }
                catch (XMPException) {
                }
            }
            return xmpMetadata;
        }

        /// <summary>Gets PdfObject by object number.</summary>
        /// <param name="objNum">object number.</param>
        /// <returns>
        /// 
        /// <see cref="PdfObject"/>
        /// or
        /// <see langword="null"/>
        /// , if object not found.
        /// </returns>
        public virtual PdfObject GetPdfObject(int objNum) {
            CheckClosingStatus();
            PdfIndirectReference reference = xref.Get(objNum);
            if (reference == null) {
                return null;
            }
            else {
                return reference.GetRefersTo();
            }
        }

        /// <summary>Get number of indirect objects in the document.</summary>
        /// <returns>number of indirect objects.</returns>
        public virtual int GetNumberOfPdfObjects() {
            return xref.Size();
        }

        /// <summary>Gets the page by page number.</summary>
        /// <param name="pageNum">page number.</param>
        /// <returns>page by page number.</returns>
        public virtual PdfPage GetPage(int pageNum) {
            CheckClosingStatus();
            return catalog.GetPageTree().GetPage(pageNum);
        }

        /// <summary>
        /// Gets the
        /// <see cref="PdfPage"/>
        /// instance by
        /// <see cref="PdfDictionary"/>.
        /// </summary>
        /// <param name="pageDictionary">
        /// 
        /// <see cref="PdfDictionary"/>
        /// that present page.
        /// </param>
        /// <returns>
        /// page by
        /// <see cref="PdfDictionary"/>.
        /// </returns>
        public virtual PdfPage GetPage(PdfDictionary pageDictionary) {
            CheckClosingStatus();
            return catalog.GetPageTree().GetPage(pageDictionary);
        }

        /// <summary>Get the first page of the document.</summary>
        /// <returns>first page of the document.</returns>
        public virtual PdfPage GetFirstPage() {
            CheckClosingStatus();
            return GetPage(1);
        }

        /// <summary>Gets the last page of the document.</summary>
        /// <returns>last page.</returns>
        public virtual PdfPage GetLastPage() {
            return GetPage(GetNumberOfPages());
        }

        /// <summary>
        /// Marks
        /// <see cref="PdfStream"/>
        /// object as embedded file stream.
        /// </summary>
        /// <remarks>
        /// Marks
        /// <see cref="PdfStream"/>
        /// object as embedded file stream. Note that this method is for internal usage.
        /// To add an embedded file to the PDF document please use specialized API for file attachments.
        /// (e.g.
        /// <see cref="AddFileAttachment(System.String, iText.Kernel.Pdf.Filespec.PdfFileSpec)"/>
        /// ,
        /// <see cref="PdfPage.AddAnnotation(iText.Kernel.Pdf.Annot.PdfAnnotation)"/>
        /// )
        /// </remarks>
        /// <param name="stream">to be marked as embedded file stream</param>
        public virtual void MarkStreamAsEmbeddedFile(PdfStream stream) {
            encryptedEmbeddedStreamsHandler.StoreEmbeddedStream(stream);
        }

        /// <summary>Creates and adds new page to the end of document.</summary>
        /// <returns>added page</returns>
        public virtual PdfPage AddNewPage() {
            return AddNewPage(GetDefaultPageSize());
        }

        /// <summary>Creates and adds new page with the specified page size.</summary>
        /// <param name="pageSize">page size of the new page</param>
        /// <returns>added page</returns>
        public virtual PdfPage AddNewPage(PageSize pageSize) {
            CheckClosingStatus();
            PdfPage page = GetPageFactory().CreatePdfPage(this, pageSize);
            CheckAndAddPage(page);
            DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.START_PAGE, page));
            DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.INSERT_PAGE, page));
            return page;
        }

        /// <summary>Creates and inserts new page to the document.</summary>
        /// <param name="index">position to addPage page to</param>
        /// <returns>inserted page</returns>
        public virtual PdfPage AddNewPage(int index) {
            return AddNewPage(index, GetDefaultPageSize());
        }

        /// <summary>Creates and inserts new page to the document.</summary>
        /// <param name="index">position to addPage page to</param>
        /// <param name="pageSize">page size of the new page</param>
        /// <returns>inserted page</returns>
        public virtual PdfPage AddNewPage(int index, PageSize pageSize) {
            CheckClosingStatus();
            PdfPage page = GetPageFactory().CreatePdfPage(this, pageSize);
            CheckAndAddPage(index, page);
            DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.START_PAGE, page));
            DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.INSERT_PAGE, page));
            return page;
        }

        /// <summary>Adds page to the end of document.</summary>
        /// <param name="page">page to add.</param>
        /// <returns>added page.</returns>
        public virtual PdfPage AddPage(PdfPage page) {
            CheckClosingStatus();
            CheckAndAddPage(page);
            DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.INSERT_PAGE, page));
            return page;
        }

        /// <summary>Inserts page to the document.</summary>
        /// <param name="index">position to addPage page to</param>
        /// <param name="page">page to addPage</param>
        /// <returns>inserted page</returns>
        public virtual PdfPage AddPage(int index, PdfPage page) {
            CheckClosingStatus();
            CheckAndAddPage(index, page);
            DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.INSERT_PAGE, page));
            return page;
        }

        /// <summary>Gets number of pages of the document.</summary>
        /// <returns>number of pages.</returns>
        public virtual int GetNumberOfPages() {
            CheckClosingStatus();
            return catalog.GetPageTree().GetNumberOfPages();
        }

        /// <summary>Gets page number by page.</summary>
        /// <param name="page">the page.</param>
        /// <returns>page number.</returns>
        public virtual int GetPageNumber(PdfPage page) {
            CheckClosingStatus();
            return catalog.GetPageTree().GetPageNumber(page);
        }

        /// <summary>
        /// Gets page number by
        /// <see cref="PdfDictionary"/>.
        /// </summary>
        /// <param name="pageDictionary">
        /// 
        /// <see cref="PdfDictionary"/>
        /// that present page.
        /// </param>
        /// <returns>
        /// page number by
        /// <see cref="PdfDictionary"/>.
        /// </returns>
        public virtual int GetPageNumber(PdfDictionary pageDictionary) {
            return catalog.GetPageTree().GetPageNumber(pageDictionary);
        }

        /// <summary>Moves page to new place in same document with all it tag structure</summary>
        /// <param name="page">page to be moved in document if present</param>
        /// <param name="insertBefore">indicates before which page new one will be inserted to</param>
        /// <returns><tt>true</tt> if this document contained the specified page</returns>
        public virtual bool MovePage(PdfPage page, int insertBefore) {
            CheckClosingStatus();
            int pageNum = GetPageNumber(page);
            if (pageNum > 0) {
                MovePage(pageNum, insertBefore);
                return true;
            }
            return false;
        }

        /// <summary>Moves page to new place in same document with all it tag structure</summary>
        /// <param name="pageNumber">number of Page that will be moved</param>
        /// <param name="insertBefore">indicates before which page new one will be inserted to</param>
        public virtual void MovePage(int pageNumber, int insertBefore) {
            CheckClosingStatus();
            if (insertBefore < 1 || insertBefore > GetNumberOfPages() + 1) {
                throw new IndexOutOfRangeException(MessageFormatUtil.Format(KernelExceptionMessageConstant.REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                    , insertBefore));
            }
            PdfPage page = GetPage(pageNumber);
            if (IsTagged()) {
                GetStructTreeRoot().Move(page, insertBefore);
                GetTagStructureContext().NormalizeDocumentRootTag();
            }
            PdfPage removedPage = catalog.GetPageTree().RemovePage(pageNumber);
            if (insertBefore > pageNumber) {
                --insertBefore;
            }
            catalog.GetPageTree().AddPage(insertBefore, removedPage);
        }

        /// <summary>
        /// Removes the first occurrence of the specified page from this document,
        /// if it is present.
        /// </summary>
        /// <remarks>
        /// Removes the first occurrence of the specified page from this document,
        /// if it is present. Returns <tt>true</tt> if this document
        /// contained the specified element (or equivalently, if this document
        /// changed as a result of the call).
        /// </remarks>
        /// <param name="page">page to be removed from this document, if present</param>
        /// <returns><tt>true</tt> if this document contained the specified page</returns>
        public virtual bool RemovePage(PdfPage page) {
            CheckClosingStatus();
            int pageNum = GetPageNumber(page);
            if (pageNum >= 1) {
                RemovePage(pageNum);
                return true;
            }
            return false;
        }

        /// <summary>Removes page from the document by page number.</summary>
        /// <param name="pageNum">the one-based index of the PdfPage to be removed</param>
        public virtual void RemovePage(int pageNum) {
            CheckClosingStatus();
            PdfPage removedPage = GetPage(pageNum);
            if (removedPage != null && removedPage.IsFlushed() && (IsTagged() || HasAcroForm())) {
                throw new PdfException(KernelExceptionMessageConstant.FLUSHED_PAGE_CANNOT_BE_REMOVED);
            }
            if (removedPage != null) {
                catalog.RemoveOutlines(removedPage);
                RemoveUnusedWidgetsFromFields(removedPage);
                if (IsTagged()) {
                    GetTagStructureContext().RemovePageTags(removedPage);
                }
                if (!removedPage.IsFlushed()) {
                    removedPage.GetPdfObject().Remove(PdfName.Parent);
                    removedPage.GetPdfObject().GetIndirectReference().SetFree();
                }
                DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.REMOVE_PAGE, removedPage));
            }
            catalog.GetPageTree().RemovePage(pageNum);
        }

        /// <summary>Gets document information dictionary.</summary>
        /// <remarks>
        /// Gets document information dictionary.
        /// <see cref="info"/>
        /// is lazy initialized. It will be initialized during the first call of this method.
        /// </remarks>
        /// <returns>document information dictionary.</returns>
        public virtual PdfDocumentInfo GetDocumentInfo() {
            CheckClosingStatus();
            if (info == null) {
                PdfObject infoDict = trailer.Get(PdfName.Info);
                info = new PdfDocumentInfo(infoDict is PdfDictionary ? (PdfDictionary)infoDict : new PdfDictionary(), this
                    );
                XmpMetaInfoConverter.AppendMetadataToInfo(xmpMetadata, info);
            }
            return info;
        }

        /// <summary>Gets original document id</summary>
        /// <remarks>
        /// Gets original document id
        /// <para />
        /// In order to set originalDocumentId
        /// <see cref="WriterProperties.SetInitialDocumentId(PdfString)"/>
        /// should be used
        /// </remarks>
        /// <returns>original dccument id</returns>
        public virtual PdfString GetOriginalDocumentId() {
            return originalDocumentId;
        }

        /// <summary>Gets modified document id</summary>
        /// <remarks>
        /// Gets modified document id
        /// <para />
        /// In order to set modifiedDocumentId
        /// <see cref="WriterProperties.SetModifiedDocumentId(PdfString)"/>
        /// should be used
        /// </remarks>
        /// <returns>modified document id</returns>
        public virtual PdfString GetModifiedDocumentId() {
            return modifiedDocumentId;
        }

        /// <summary>Gets default page size.</summary>
        /// <remarks>
        /// Gets default page size.
        /// New pages by default are created with this size.
        /// </remarks>
        /// <returns>default page size</returns>
        public virtual PageSize GetDefaultPageSize() {
            return defaultPageSize;
        }

        /// <summary>Sets default page size.</summary>
        /// <remarks>
        /// Sets default page size.
        /// New pages by default will be created with this size.
        /// </remarks>
        /// <param name="pageSize">page size to be set as default</param>
        public virtual void SetDefaultPageSize(PageSize pageSize) {
            defaultPageSize = pageSize;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddEventHandler(String type, iText.Kernel.Events.IEventHandler handler) {
            eventDispatcher.AddEventHandler(type, handler);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void DispatchEvent(Event @event) {
            eventDispatcher.DispatchEvent(@event);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void DispatchEvent(Event @event, bool delayed) {
            eventDispatcher.DispatchEvent(@event, delayed);
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasEventHandler(String type) {
            return eventDispatcher.HasEventHandler(type);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void RemoveEventHandler(String type, iText.Kernel.Events.IEventHandler handler) {
            eventDispatcher.RemoveEventHandler(type, handler);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void RemoveAllHandlers() {
            eventDispatcher.RemoveAllHandlers();
        }

        /// <summary>
        /// Gets
        /// <c>PdfWriter</c>
        /// associated with the document.
        /// </summary>
        /// <returns>PdfWriter associated with the document.</returns>
        public virtual PdfWriter GetWriter() {
            CheckClosingStatus();
            return writer;
        }

        /// <summary>
        /// Gets
        /// <c>PdfReader</c>
        /// associated with the document.
        /// </summary>
        /// <returns>PdfReader associated with the document.</returns>
        public virtual PdfReader GetReader() {
            CheckClosingStatus();
            return reader;
        }

        /// <summary>
        /// Returns
        /// <see langword="true"/>
        /// if the document is opened in append mode, and
        /// <see langword="false"/>
        /// otherwise.
        /// </summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the document is opened in append mode, and
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool IsAppendMode() {
            CheckClosingStatus();
            return properties.appendMode;
        }

        /// <summary>Creates next available indirect reference.</summary>
        /// <returns>created indirect reference.</returns>
        public virtual PdfIndirectReference CreateNextIndirectReference() {
            CheckClosingStatus();
            return xref.CreateNextIndirectReference(this);
        }

        /// <summary>Gets PDF version.</summary>
        /// <returns>PDF version.</returns>
        public virtual PdfVersion GetPdfVersion() {
            return pdfVersion;
        }

        /// <summary>Gets PDF catalog.</summary>
        /// <returns>PDF catalog.</returns>
        public virtual PdfCatalog GetCatalog() {
            CheckClosingStatus();
            return catalog;
        }

        /// <summary>Close PDF document.</summary>
        public virtual void Close() {
            if (closed) {
                return;
            }
            isClosing = true;
            try {
                if (writer != null) {
                    if (catalog.IsFlushed()) {
                        throw new PdfException(KernelExceptionMessageConstant.CANNOT_CLOSE_DOCUMENT_WITH_ALREADY_FLUSHED_PDF_CATALOG
                            );
                    }
                    EventManager manager = EventManager.GetInstance();
                    manager.OnEvent(new NumberOfPagesStatisticsEvent(catalog.GetPageTree().GetNumberOfPages(), ITextCoreProductData
                        .GetInstance()));
                    // The event will prepare document for flushing, i.e. will set an appropriate producer line
                    manager.OnEvent(new FlushPdfDocumentEvent(this));
                    UpdateXmpMetadata();
                    // In PDF 2.0, all the values except CreationDate and ModDate are deprecated. Remove them now
                    if (pdfVersion.CompareTo(PdfVersion.PDF_2_0) >= 0) {
                        foreach (PdfName deprecatedKey in PdfDocumentInfo.PDF20_DEPRECATED_KEYS) {
                            GetDocumentInfo().GetPdfObject().Remove(deprecatedKey);
                        }
                    }
                    if (GetXmpMetadata() != null) {
                        PdfStream xmp = catalog.GetPdfObject().GetAsStream(PdfName.Metadata);
                        if (IsAppendMode() && xmp != null && !xmp.IsFlushed() && xmp.GetIndirectReference() != null) {
                            // Use existing object for append mode
                            xmp.SetData(xmpMetadata);
                            xmp.SetModified();
                        }
                        else {
                            // Create new object
                            xmp = (PdfStream)new PdfStream().MakeIndirect(this);
                            xmp.GetOutputStream().Write(xmpMetadata);
                            catalog.GetPdfObject().Put(PdfName.Metadata, xmp);
                            catalog.SetModified();
                        }
                        xmp.Put(PdfName.Type, PdfName.Metadata);
                        xmp.Put(PdfName.Subtype, PdfName.XML);
                        if (writer.crypto != null && !writer.crypto.IsMetadataEncrypted()) {
                            PdfArray ar = new PdfArray();
                            ar.Add(PdfName.Crypt);
                            xmp.Put(PdfName.Filter, ar);
                        }
                    }
                    CheckIsoConformance();
                    if (GetNumberOfPages() == 0) {
                        // Add new page here, not in PdfPagesTree#generateTree method, so that any page
                        // operations are available when handling the START_PAGE and INSERT_PAGE events
                        AddNewPage();
                    }
                    PdfObject crypto = null;
                    ICollection<PdfIndirectReference> forbiddenToFlush = new HashSet<PdfIndirectReference>();
                    if (properties.appendMode) {
                        if (structTreeRoot != null) {
                            TryFlushTagStructure(true);
                        }
                        if (catalog.IsOCPropertiesMayHaveChanged() && catalog.GetOCProperties(false).GetPdfObject().IsModified()) {
                            catalog.GetOCProperties(false).Flush();
                        }
                        if (catalog.pageLabels != null) {
                            catalog.Put(PdfName.PageLabels, catalog.pageLabels.BuildTree());
                        }
                        foreach (KeyValuePair<PdfName, PdfNameTree> entry in catalog.nameTrees) {
                            PdfNameTree tree = entry.Value;
                            if (tree.IsModified()) {
                                EnsureTreeRootAddedToNames(tree.BuildTree().MakeIndirect(this), entry.Key);
                            }
                        }
                        PdfObject pageRoot = catalog.GetPageTree().GenerateTree();
                        if (catalog.GetPdfObject().IsModified() || pageRoot.IsModified()) {
                            catalog.Put(PdfName.Pages, pageRoot);
                            catalog.GetPdfObject().Flush(false);
                        }
                        if (GetDocumentInfo().GetPdfObject().IsModified()) {
                            GetDocumentInfo().GetPdfObject().Flush(false);
                        }
                        FlushFonts();
                        if (writer.crypto != null) {
                            System.Diagnostics.Debug.Assert(reader.decrypt.GetPdfObject() == writer.crypto.GetPdfObject(), "Conflict with source encryption"
                                );
                            crypto = reader.decrypt.GetPdfObject();
                            if (crypto.GetIndirectReference() != null) {
                                // Checking just for extra safety, encryption dictionary shall never be direct.
                                forbiddenToFlush.Add(crypto.GetIndirectReference());
                            }
                        }
                        writer.FlushModifiedWaitingObjects(forbiddenToFlush);
                        for (int i = 0; i < xref.Size(); i++) {
                            PdfIndirectReference indirectReference = xref.Get(i);
                            if (indirectReference != null && !indirectReference.IsFree() && indirectReference.CheckState(PdfObject.MODIFIED
                                ) && !indirectReference.CheckState(PdfObject.FLUSHED) && !forbiddenToFlush.Contains(indirectReference)
                                ) {
                                indirectReference.SetFree();
                            }
                        }
                    }
                    else {
                        if (catalog.IsOCPropertiesMayHaveChanged()) {
                            catalog.GetPdfObject().Put(PdfName.OCProperties, catalog.GetOCProperties(false).GetPdfObject());
                            catalog.GetOCProperties(false).Flush();
                        }
                        if (catalog.pageLabels != null) {
                            catalog.Put(PdfName.PageLabels, catalog.pageLabels.BuildTree());
                        }
                        catalog.GetPdfObject().Put(PdfName.Pages, catalog.GetPageTree().GenerateTree());
                        foreach (KeyValuePair<PdfName, PdfNameTree> entry in catalog.nameTrees) {
                            PdfNameTree tree = entry.Value;
                            if (tree.IsModified()) {
                                EnsureTreeRootAddedToNames(tree.BuildTree().MakeIndirect(this), entry.Key);
                            }
                        }
                        for (int pageNum = 1; pageNum <= GetNumberOfPages(); pageNum++) {
                            PdfPage page = GetPage(pageNum);
                            if (page != null) {
                                page.Flush();
                            }
                        }
                        if (structTreeRoot != null) {
                            TryFlushTagStructure(false);
                        }
                        catalog.GetPdfObject().Flush(false);
                        GetDocumentInfo().GetPdfObject().Flush(false);
                        FlushFonts();
                        if (writer.crypto != null) {
                            crypto = writer.crypto.GetPdfObject();
                            crypto.MakeIndirect(this);
                            forbiddenToFlush.Add(crypto.GetIndirectReference());
                        }
                        writer.FlushWaitingObjects(forbiddenToFlush);
                        for (int i = 0; i < xref.Size(); i++) {
                            PdfIndirectReference indirectReference = xref.Get(i);
                            if (indirectReference != null && !indirectReference.IsFree() && !indirectReference.CheckState(PdfObject.FLUSHED
                                ) && !forbiddenToFlush.Contains(indirectReference)) {
                                PdfObject @object;
                                if (IsFlushUnusedObjects() && !indirectReference.CheckState(PdfObject.ORIGINAL_OBJECT_STREAM) && (@object 
                                    = indirectReference.GetRefersTo(false)) != null) {
                                    @object.Flush();
                                }
                                else {
                                    indirectReference.SetFree();
                                }
                            }
                        }
                    }
                    // To avoid encryption of XrefStream and Encryption dictionary remove crypto.
                    // NOTE. No need in reverting, because it is the last operation with the document.
                    writer.crypto = null;
                    if (!properties.appendMode && crypto != null) {
                        // no need to flush crypto in append mode, it shall not have changed in this case
                        crypto.Flush(false);
                    }
                    // The following two operators prevents the possible inconsistency between root and info
                    // entries existing in the trailer object and corresponding fields. This inconsistency
                    // may appear when user gets trailer and explicitly sets new root or info dictionaries.
                    trailer.Put(PdfName.Root, catalog.GetPdfObject());
                    trailer.Put(PdfName.Info, GetDocumentInfo().GetPdfObject());
                    //By this time original and modified document ids should always be not null due to initializing in
                    // either writer properties, or in the writer init section on document open or from pdfreader. So we
                    // shouldn't worry about it being null next
                    PdfObject fileId = PdfEncryption.CreateInfoId(ByteUtils.GetIsoBytes(originalDocumentId.GetValue()), ByteUtils
                        .GetIsoBytes(modifiedDocumentId.GetValue()));
                    xref.WriteXrefTableAndTrailer(this, fileId, crypto);
                    writer.Flush();
                    if (writer.GetOutputStream() is CountOutputStream) {
                        long amountOfBytes = ((CountOutputStream)writer.GetOutputStream()).GetAmountOfWrittenBytes();
                        manager.OnEvent(new SizeOfPdfStatisticsEvent(amountOfBytes, ITextCoreProductData.GetInstance()));
                    }
                }
                catalog.GetPageTree().ClearPageRefs();
                RemoveAllHandlers();
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CLOSE_DOCUMENT, e, this);
            }
            finally {
                if (writer != null && IsCloseWriter()) {
                    try {
                        writer.Dispose();
                    }
                    catch (Exception e) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                        logger.LogError(e, iText.IO.Logs.IoLogMessageConstant.PDF_WRITER_CLOSING_FAILED);
                    }
                }
                if (reader != null && IsCloseReader()) {
                    try {
                        reader.Close();
                    }
                    catch (Exception e) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                        logger.LogError(e, iText.IO.Logs.IoLogMessageConstant.PDF_READER_CLOSING_FAILED);
                    }
                }
            }
            closed = true;
        }

        /// <summary>Gets close status of the document.</summary>
        /// <returns>true, if the document has already been closed, otherwise false.</returns>
        public virtual bool IsClosed() {
            return closed;
        }

        /// <summary>Gets tagged status of the document.</summary>
        /// <returns>true, if the document has tag structure, otherwise false.</returns>
        public virtual bool IsTagged() {
            return structTreeRoot != null;
        }

        /// <summary>Specifies that document shall contain tag structure.</summary>
        /// <remarks>
        /// Specifies that document shall contain tag structure.
        /// See ISO 32000-1, section 14.8 "Tagged PDF"
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="PdfDocument"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfDocument SetTagged() {
            CheckClosingStatus();
            if (structTreeRoot == null) {
                structTreeRoot = new PdfStructTreeRoot(this);
                catalog.GetPdfObject().Put(PdfName.StructTreeRoot, structTreeRoot.GetPdfObject());
                UpdateValueInMarkInfoDict(PdfName.Marked, PdfBoolean.TRUE);
                structParentIndex = 0;
            }
            return this;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructTreeRoot"/>
        /// of tagged document.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructTreeRoot"/>
        /// in case document is tagged, otherwise it returns null.
        /// </returns>
        /// <seealso cref="IsTagged()"/>
        /// <seealso cref="GetNextStructParentIndex()"/>
        public virtual PdfStructTreeRoot GetStructTreeRoot() {
            return structTreeRoot;
        }

        /// <summary>Gets next parent index of tagged document.</summary>
        /// <returns>-1 if document is not tagged, or &gt;= 0 if tagged.</returns>
        /// <seealso cref="IsTagged()"/>
        /// <seealso cref="GetNextStructParentIndex()"/>
        public virtual int GetNextStructParentIndex() {
            return structParentIndex < 0 ? -1 : structParentIndex++;
        }

        /// <summary>
        /// Gets document
        /// <c>TagStructureContext</c>.
        /// </summary>
        /// <remarks>
        /// Gets document
        /// <c>TagStructureContext</c>.
        /// The document must be tagged, otherwise an exception will be thrown.
        /// </remarks>
        /// <returns>
        /// document
        /// <c>TagStructureContext</c>.
        /// </returns>
        public virtual TagStructureContext GetTagStructureContext() {
            CheckClosingStatus();
            if (tagStructureContext == null) {
                if (!IsTagged()) {
                    throw new PdfException(KernelExceptionMessageConstant.MUST_BE_A_TAGGED_DOCUMENT);
                }
                InitTagStructureContext();
            }
            return tagStructureContext;
        }

        /// <summary>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>.
        /// </summary>
        /// <remarks>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>.
        /// Use this method if you want to copy pages across tagged documents.
        /// This will keep resultant PDF structure consistent.
        /// <para />
        /// If outlines destination names are the same in different documents, all
        /// such outlines will lead to a single location in the resultant document.
        /// In this case iText will log a warning. This can be avoided by renaming
        /// destinations names in the source document.
        /// </remarks>
        /// <param name="pageFrom">start of the range of pages to be copied.</param>
        /// <param name="pageTo">end of the range of pages to be copied.</param>
        /// <param name="toDocument">a document to copy pages to.</param>
        /// <param name="insertBeforePage">a position where to insert copied pages.</param>
        /// <returns>list of copied pages</returns>
        public virtual IList<PdfPage> CopyPagesTo(int pageFrom, int pageTo, iText.Kernel.Pdf.PdfDocument toDocument
            , int insertBeforePage) {
            return CopyPagesTo(pageFrom, pageTo, toDocument, insertBeforePage, null);
        }

        /// <summary>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>.
        /// </summary>
        /// <remarks>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// . This range is inclusive, both
        /// <c>page</c>
        /// and
        /// <paramref name="pageTo"/>
        /// are included in list of copied pages.
        /// Use this method if you want to copy pages across tagged documents.
        /// This will keep resultant PDF structure consistent.
        /// <para />
        /// If outlines destination names are the same in different documents, all
        /// such outlines will lead to a single location in the resultant document.
        /// In this case iText will log a warning. This can be avoided by renaming
        /// destinations names in the source document.
        /// </remarks>
        /// <param name="pageFrom">1-based start of the range of pages to be copied.</param>
        /// <param name="pageTo">
        /// 1-based end (inclusive) of the range of pages to be copied. This page is included in list
        /// of copied pages.
        /// </param>
        /// <param name="toDocument">a document to copy pages to.</param>
        /// <param name="insertBeforePage">a position where to insert copied pages.</param>
        /// <param name="copier">
        /// a copier which bears a special copy logic. May be null.
        /// It is recommended to use the same instance of
        /// <see cref="IPdfPageExtraCopier"/>
        /// for the same output document.
        /// </param>
        /// <returns>list of new copied pages</returns>
        public virtual IList<PdfPage> CopyPagesTo(int pageFrom, int pageTo, iText.Kernel.Pdf.PdfDocument toDocument
            , int insertBeforePage, IPdfPageExtraCopier copier) {
            IList<int> pages = new List<int>();
            for (int i = pageFrom; i <= pageTo; i++) {
                pages.Add(i);
            }
            return CopyPagesTo(pages, toDocument, insertBeforePage, copier);
        }

        /// <summary>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// appending copied pages to the end.
        /// </summary>
        /// <remarks>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// appending copied pages to the end. This range
        /// is inclusive, both
        /// <c>page</c>
        /// and
        /// <paramref name="pageTo"/>
        /// are included in list of copied pages.
        /// Use this method if you want to copy pages across tagged documents.
        /// This will keep resultant PDF structure consistent.
        /// <para />
        /// If outlines destination names are the same in different documents, all
        /// such outlines will lead to a single location in the resultant document.
        /// In this case iText will log a warning. This can be avoided by renaming
        /// destinations names in the source document.
        /// </remarks>
        /// <param name="pageFrom">1-based start of the range of pages to be copied.</param>
        /// <param name="pageTo">
        /// 1-based end (inclusive) of the range of pages to be copied. This page is included in list of
        /// copied pages.
        /// </param>
        /// <param name="toDocument">a document to copy pages to.</param>
        /// <returns>list of new copied pages</returns>
        public virtual IList<PdfPage> CopyPagesTo(int pageFrom, int pageTo, iText.Kernel.Pdf.PdfDocument toDocument
            ) {
            return CopyPagesTo(pageFrom, pageTo, toDocument, null);
        }

        /// <summary>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// appending copied pages to the end.
        /// </summary>
        /// <remarks>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// appending copied pages to the end. This range
        /// is inclusive, both
        /// <c>page</c>
        /// and
        /// <paramref name="pageTo"/>
        /// are included in list of copied pages.
        /// Use this method if you want to copy pages across tagged documents.
        /// This will keep resultant PDF structure consistent.
        /// <para />
        /// If outlines destination names are the same in different documents, all
        /// such outlines will lead to a single location in the resultant document.
        /// In this case iText will log a warning. This can be avoided by renaming
        /// destinations names in the source document.
        /// </remarks>
        /// <param name="pageFrom">1-based start of the range of pages to be copied.</param>
        /// <param name="pageTo">
        /// 1-based end (inclusive) of the range of pages to be copied. This page is included in list of
        /// copied pages.
        /// </param>
        /// <param name="toDocument">a document to copy pages to.</param>
        /// <param name="copier">
        /// a copier which bears a special copy logic. May be null.
        /// It is recommended to use the same instance of
        /// <see cref="IPdfPageExtraCopier"/>
        /// for the same output document.
        /// </param>
        /// <returns>list of new copied pages.</returns>
        public virtual IList<PdfPage> CopyPagesTo(int pageFrom, int pageTo, iText.Kernel.Pdf.PdfDocument toDocument
            , IPdfPageExtraCopier copier) {
            return CopyPagesTo(pageFrom, pageTo, toDocument, toDocument.GetNumberOfPages() + 1, copier);
        }

        /// <summary>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>.
        /// </summary>
        /// <remarks>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>.
        /// Use this method if you want to copy pages across tagged documents.
        /// This will keep resultant PDF structure consistent.
        /// <para />
        /// If outlines destination names are the same in different documents, all
        /// such outlines will lead to a single location in the resultant document.
        /// In this case iText will log a warning. This can be avoided by renaming
        /// destinations names in the source document.
        /// </remarks>
        /// <param name="pagesToCopy">list of pages to be copied.</param>
        /// <param name="toDocument">a document to copy pages to.</param>
        /// <param name="insertBeforePage">a position where to insert copied pages.</param>
        /// <returns>list of new copied pages</returns>
        public virtual IList<PdfPage> CopyPagesTo(IList<int> pagesToCopy, iText.Kernel.Pdf.PdfDocument toDocument, 
            int insertBeforePage) {
            return CopyPagesTo(pagesToCopy, toDocument, insertBeforePage, null);
        }

        /// <summary>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>.
        /// </summary>
        /// <remarks>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>.
        /// Use this method if you want to copy pages across tagged documents.
        /// This will keep resultant PDF structure consistent.
        /// <para />
        /// If outlines destination names are the same in different documents, all
        /// such outlines will lead to a single location in the resultant document.
        /// In this case iText will log a warning. This can be avoided by renaming
        /// destinations names in the source document.
        /// </remarks>
        /// <param name="pagesToCopy">list of pages to be copied.</param>
        /// <param name="toDocument">a document to copy pages to.</param>
        /// <param name="insertBeforePage">a position where to insert copied pages.</param>
        /// <param name="copier">
        /// a copier which bears a special copy logic. May be null.
        /// It is recommended to use the same instance of
        /// <see cref="IPdfPageExtraCopier"/>
        /// for the same output document.
        /// </param>
        /// <returns>list of new copied pages</returns>
        public virtual IList<PdfPage> CopyPagesTo(IList<int> pagesToCopy, iText.Kernel.Pdf.PdfDocument toDocument, 
            int insertBeforePage, IPdfPageExtraCopier copier) {
            if (pagesToCopy.IsEmpty()) {
                return JavaCollectionsUtil.EmptyList<PdfPage>();
            }
            pendingDestinationMutations.Clear();
            CheckClosingStatus();
            IList<PdfPage> copiedPages = new List<PdfPage>();
            IDictionary<PdfPage, PdfPage> page2page = new LinkedDictionary<PdfPage, PdfPage>();
            ICollection<PdfOutline> outlinesToCopy = new HashSet<PdfOutline>();
            IList<IDictionary<PdfPage, PdfPage>> rangesOfPagesWithIncreasingNumbers = new List<IDictionary<PdfPage, PdfPage
                >>();
            int lastCopiedPageNum = (int)pagesToCopy[0];
            int pageInsertIndex = insertBeforePage;
            bool insertInBetween = insertBeforePage < toDocument.GetNumberOfPages() + 1;
            foreach (int? pageNum in pagesToCopy) {
                PdfPage page = GetPage((int)pageNum);
                PdfPage newPage = page.CopyTo(toDocument, copier, true, insertInBetween ? pageInsertIndex : -1);
                copiedPages.Add(newPage);
                page2page.Put(page, newPage);
                if (lastCopiedPageNum >= pageNum) {
                    rangesOfPagesWithIncreasingNumbers.Add(new Dictionary<PdfPage, PdfPage>());
                }
                int lastRangeInd = rangesOfPagesWithIncreasingNumbers.Count - 1;
                rangesOfPagesWithIncreasingNumbers[lastRangeInd].Put(page, newPage);
                pageInsertIndex++;
                if (toDocument.HasOutlines()) {
                    IList<PdfOutline> pageOutlines = page.GetOutlines(false);
                    if (pageOutlines != null) {
                        outlinesToCopy.AddAll(pageOutlines);
                    }
                }
                lastCopiedPageNum = (int)pageNum;
            }
            ResolveDestinations(toDocument, page2page);
            // Copying OCGs should go after copying LinkAnnotations
            if (GetCatalog() != null && GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties) != null) {
                OcgPropertiesCopier.CopyOCGProperties(this, toDocument, page2page);
            }
            // It's important to copy tag structure after link annotations were copied, because object content items in tag
            // structure are not copied in case if their's OBJ key is annotation and doesn't contain /P entry.
            if (toDocument.IsTagged()) {
                if (IsTagged()) {
                    try {
                        foreach (IDictionary<PdfPage, PdfPage> increasingPagesRange in rangesOfPagesWithIncreasingNumbers) {
                            if (insertInBetween) {
                                GetStructTreeRoot().CopyTo(toDocument, insertBeforePage, increasingPagesRange);
                            }
                            else {
                                GetStructTreeRoot().CopyTo(toDocument, increasingPagesRange);
                            }
                            insertBeforePage += increasingPagesRange.Count;
                        }
                        toDocument.GetTagStructureContext().NormalizeDocumentRootTag();
                    }
                    catch (Exception ex) {
                        throw new PdfException(KernelExceptionMessageConstant.TAG_STRUCTURE_COPYING_FAILED_IT_MIGHT_BE_CORRUPTED_IN_ONE_OF_THE_DOCUMENTS
                            , ex);
                    }
                    if (copier is IPdfPageFormCopier) {
                        ((IPdfPageFormCopier)copier).RecreateAcroformToProcessCopiedFields(toDocument);
                    }
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                    logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.NOT_TAGGED_PAGES_IN_TAGGED_DOCUMENT);
                }
            }
            if (catalog.IsOutlineMode()) {
                CopyOutlines(outlinesToCopy, toDocument, page2page);
            }
            return copiedPages;
        }

        /// <summary>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// appending copied pages to the end.
        /// </summary>
        /// <remarks>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// appending copied pages to the end.
        /// Use this method if you want to copy pages across tagged documents.
        /// This will keep resultant PDF structure consistent.
        /// <para />
        /// If outlines destination names are the same in different documents, all
        /// such outlines will lead to a single location in the resultant document.
        /// In this case iText will log a warning. This can be avoided by renaming
        /// destinations names in the source document.
        /// </remarks>
        /// <param name="pagesToCopy">list of pages to be copied.</param>
        /// <param name="toDocument">a document to copy pages to.</param>
        /// <returns>list of copied pages</returns>
        public virtual IList<PdfPage> CopyPagesTo(IList<int> pagesToCopy, iText.Kernel.Pdf.PdfDocument toDocument) {
            return CopyPagesTo(pagesToCopy, toDocument, null);
        }

        /// <summary>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// appending copied pages to the end.
        /// </summary>
        /// <remarks>
        /// Copies a range of pages from current document to
        /// <paramref name="toDocument"/>
        /// appending copied pages to the end.
        /// Use this method if you want to copy pages across tagged documents.
        /// This will keep resultant PDF structure consistent.
        /// <para />
        /// If outlines destination names are the same in different documents, all
        /// such outlines will lead to a single location in the resultant document.
        /// In this case iText will log a warning. This can be avoided by renaming
        /// destinations names in the source document.
        /// </remarks>
        /// <param name="pagesToCopy">list of pages to be copied.</param>
        /// <param name="toDocument">a document to copy pages to.</param>
        /// <param name="copier">
        /// a copier which bears a special copy logic. May be null.
        /// It is recommended to use the same instance of
        /// <see cref="IPdfPageExtraCopier"/>
        /// for the same output document.
        /// </param>
        /// <returns>list of copied pages</returns>
        public virtual IList<PdfPage> CopyPagesTo(IList<int> pagesToCopy, iText.Kernel.Pdf.PdfDocument toDocument, 
            IPdfPageExtraCopier copier) {
            return CopyPagesTo(pagesToCopy, toDocument, toDocument.GetNumberOfPages() + 1, copier);
        }

        /// <summary>Flush all copied objects and remove them from copied cache.</summary>
        /// <remarks>
        /// Flush all copied objects and remove them from copied cache.
        /// <para />
        /// Note, if you will copy objects from the same document, duplicated objects will be created.
        /// That's why usually this method is meant to be used when all copying from source document is finished.
        /// For other cases one can also consider other flushing mechanisms, e.g. pages-based flushing.
        /// </remarks>
        /// <param name="sourceDoc">source document</param>
        public virtual void FlushCopiedObjects(iText.Kernel.Pdf.PdfDocument sourceDoc) {
            if (GetWriter() != null) {
                GetWriter().FlushCopiedObjects(sourceDoc.GetDocumentId());
            }
        }

        /// <summary>
        /// Checks, whether
        /// <see cref="Close()"/>
        /// method will close associated PdfReader.
        /// </summary>
        /// <returns>
        /// true,
        /// <see cref="Close()"/>
        /// method is going to close associated PdfReader, otherwise false.
        /// </returns>
        public virtual bool IsCloseReader() {
            return closeReader;
        }

        /// <summary>
        /// Sets, whether
        /// <see cref="Close()"/>
        /// method shall close associated PdfReader.
        /// </summary>
        /// <param name="closeReader">
        /// true,
        /// <see cref="Close()"/>
        /// method shall close associated PdfReader, otherwise false.
        /// </param>
        public virtual void SetCloseReader(bool closeReader) {
            CheckClosingStatus();
            this.closeReader = closeReader;
        }

        /// <summary>
        /// Checks, whether
        /// <see cref="Close()"/>
        /// method will close associated PdfWriter.
        /// </summary>
        /// <returns>
        /// true,
        /// <see cref="Close()"/>
        /// method is going to close associated PdfWriter, otherwise false.
        /// </returns>
        public virtual bool IsCloseWriter() {
            return closeWriter;
        }

        /// <summary>
        /// Sets, whether
        /// <see cref="Close()"/>
        /// method shall close associated PdfWriter.
        /// </summary>
        /// <param name="closeWriter">
        /// true,
        /// <see cref="Close()"/>
        /// method shall close associated PdfWriter, otherwise false.
        /// </param>
        public virtual void SetCloseWriter(bool closeWriter) {
            CheckClosingStatus();
            this.closeWriter = closeWriter;
        }

        /// <summary>
        /// Checks, whether
        /// <see cref="Close()"/>
        /// will flush unused objects,
        /// e.g. unreachable from PDF Catalog.
        /// </summary>
        /// <remarks>
        /// Checks, whether
        /// <see cref="Close()"/>
        /// will flush unused objects,
        /// e.g. unreachable from PDF Catalog. By default - false.
        /// </remarks>
        /// <returns>
        /// false, if
        /// <see cref="Close()"/>
        /// shall not flush unused objects, otherwise true.
        /// </returns>
        public virtual bool IsFlushUnusedObjects() {
            return flushUnusedObjects;
        }

        /// <summary>
        /// Sets, whether
        /// <see cref="Close()"/>
        /// shall flush unused objects,
        /// e.g. unreachable from PDF Catalog.
        /// </summary>
        /// <param name="flushUnusedObjects">
        /// false, if
        /// <see cref="Close()"/>
        /// shall not flush unused objects, otherwise true.
        /// </param>
        public virtual void SetFlushUnusedObjects(bool flushUnusedObjects) {
            CheckClosingStatus();
            this.flushUnusedObjects = flushUnusedObjects;
        }

        /// <summary>This method returns a complete outline tree of the whole document.</summary>
        /// <param name="updateOutlines">
        /// if the flag is
        /// <see langword="true"/>
        /// , the method reads the whole document and creates outline tree.
        /// If the flag is
        /// <see langword="false"/>
        /// , the method gets cached outline tree
        /// (if it was cached via calling getOutlines method before).
        /// </param>
        /// <returns>
        /// fully initialize
        /// <see cref="PdfOutline"/>
        /// object.
        /// </returns>
        public virtual PdfOutline GetOutlines(bool updateOutlines) {
            CheckClosingStatus();
            return catalog.GetOutlines(updateOutlines);
        }

        /// <summary>This method initializes an outline tree of the document and sets outline mode to true.</summary>
        public virtual void InitializeOutlines() {
            CheckClosingStatus();
            GetOutlines(false);
        }

        /// <summary>This methods adds new name in the Dests NameTree.</summary>
        /// <remarks>This methods adds new name in the Dests NameTree. It throws an exception, if the name already exists.
        ///     </remarks>
        /// <param name="key">Name of the destination.</param>
        /// <param name="value">
        /// An object destination refers to. Must be an array or a dictionary with key /D and array.
        /// See ISO 32000-1 12.3.2.3 for more info.
        /// </param>
        public virtual void AddNamedDestination(String key, PdfObject value) {
            AddNamedDestination(new PdfString(key), value);
        }

        /// <summary>This methods adds new name in the Dests NameTree.</summary>
        /// <remarks>This methods adds new name in the Dests NameTree. It throws an exception, if the name already exists.
        ///     </remarks>
        /// <param name="key">Name of the destination.</param>
        /// <param name="value">
        /// An object destination refers to. Must be an array or a dictionary with key /D and array.
        /// See ISO 32000-1 12.3.2.3 for more info.
        /// </param>
        public virtual void AddNamedDestination(PdfString key, PdfObject value) {
            CheckClosingStatus();
            if (value.IsArray() && ((PdfArray)value).Get(0).IsNumber()) {
                ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                    .INVALID_DESTINATION_TYPE);
            }
            catalog.AddNamedDestination(key, value);
        }

        /// <summary>Gets static copy of cross reference table.</summary>
        /// <returns>a static copy of cross reference table</returns>
        public virtual IList<PdfIndirectReference> ListIndirectReferences() {
            CheckClosingStatus();
            IList<PdfIndirectReference> indRefs = new List<PdfIndirectReference>(xref.Size());
            for (int i = 0; i < xref.Size(); ++i) {
                PdfIndirectReference indref = xref.Get(i);
                if (indref != null) {
                    indRefs.Add(indref);
                }
            }
            return indRefs;
        }

        /// <summary>Gets document trailer.</summary>
        /// <returns>document trailer.</returns>
        public virtual PdfDictionary GetTrailer() {
            CheckClosingStatus();
            return trailer;
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfOutputIntent"/>
        /// that shall specify the colour characteristics of output devices
        /// on which the document might be rendered.
        /// </summary>
        /// <param name="outputIntent">
        /// 
        /// <see cref="PdfOutputIntent"/>
        /// to add.
        /// </param>
        /// <seealso cref="PdfOutputIntent"/>
        public virtual void AddOutputIntent(PdfOutputIntent outputIntent) {
            CheckClosingStatus();
            if (outputIntent == null) {
                return;
            }
            PdfArray outputIntents = catalog.GetPdfObject().GetAsArray(PdfName.OutputIntents);
            if (outputIntents == null) {
                outputIntents = new PdfArray();
                catalog.Put(PdfName.OutputIntents, outputIntents);
            }
            outputIntents.Add(outputIntent.GetPdfObject());
        }

        /// <summary>Checks whether PDF document conforms a specific standard.</summary>
        /// <remarks>
        /// Checks whether PDF document conforms a specific standard.
        /// Shall be override.
        /// </remarks>
        /// <param name="obj">An object to conform.</param>
        /// <param name="key">type of object to conform.</param>
        public virtual void CheckIsoConformance(Object obj, IsoKey key) {
        }

        /// <summary>Checks whether PDF document conforms a specific standard.</summary>
        /// <remarks>
        /// Checks whether PDF document conforms a specific standard.
        /// Shall be override.
        /// </remarks>
        /// <param name="obj">an object to conform.</param>
        /// <param name="key">type of object to conform.</param>
        /// <param name="resources">
        /// 
        /// <see cref="PdfResources"/>
        /// associated with an object to check.
        /// </param>
        /// <param name="contentStream">current content stream</param>
        public virtual void CheckIsoConformance(Object obj, IsoKey key, PdfResources resources, PdfStream contentStream
            ) {
        }

        /// <summary>Checks whether PDF document conforms a specific standard.</summary>
        /// <remarks>
        /// Checks whether PDF document conforms a specific standard.
        /// Shall be override.
        /// </remarks>
        /// <param name="gState">
        /// a
        /// <see cref="iText.Kernel.Pdf.Canvas.CanvasGraphicsState"/>
        /// object to conform.
        /// </param>
        /// <param name="resources">
        /// 
        /// <see cref="PdfResources"/>
        /// associated with an object to check.
        /// </param>
        public virtual void CheckShowTextIsoConformance(CanvasGraphicsState gState, PdfResources resources) {
        }

        /// <summary>Adds file attachment at document level.</summary>
        /// <param name="key">name of the destination.</param>
        /// <param name="fs">
        /// 
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
        /// object.
        /// </param>
        public virtual void AddFileAttachment(String key, PdfFileSpec fs) {
            CheckClosingStatus();
            catalog.AddNameToNameTree(new PdfString(key), fs.GetPdfObject(), PdfName.EmbeddedFiles);
        }

        /// <summary>Adds file associated with PDF document as a whole and identifies the relationship between them.</summary>
        /// <remarks>
        /// Adds file associated with PDF document as a whole and identifies the relationship between them.
        /// <para />
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method is very similar to
        /// <see cref="AddFileAttachment(System.String, iText.Kernel.Pdf.Filespec.PdfFileSpec)"/>.
        /// However, besides adding file description to Names tree, it adds file to array value of the AF key in the document
        /// catalog.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="description">the file description</param>
        /// <param name="fs">file specification dictionary of associated file</param>
        /// <seealso cref="AddFileAttachment(System.String, iText.Kernel.Pdf.Filespec.PdfFileSpec)"/>
        public virtual void AddAssociatedFile(String description, PdfFileSpec fs) {
            if (null == ((PdfDictionary)fs.GetPdfObject()).Get(PdfName.AFRelationship)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ASSOCIATED_FILE_SPEC_SHALL_INCLUDE_AFRELATIONSHIP);
            }
            PdfArray afArray = catalog.GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null) {
                afArray = (PdfArray)new PdfArray().MakeIndirect(this);
                catalog.Put(PdfName.AF, afArray);
            }
            afArray.Add(fs.GetPdfObject());
            AddFileAttachment(description, fs);
        }

        /// <summary>Returns files associated with PDF document.</summary>
        /// <returns>associated files array.</returns>
        public virtual PdfArray GetAssociatedFiles() {
            CheckClosingStatus();
            return catalog.GetPdfObject().GetAsArray(PdfName.AF);
        }

        /// <summary>
        /// Gets the encrypted payload of this document,
        /// or returns
        /// <see langword="null"/>
        /// if this document isn't an unencrypted wrapper document.
        /// </summary>
        /// <returns>encrypted payload of this document.</returns>
        public virtual PdfEncryptedPayloadDocument GetEncryptedPayloadDocument() {
            if (GetReader() != null && GetReader().IsEncrypted()) {
                return null;
            }
            PdfCollection collection = GetCatalog().GetCollection();
            if (collection != null && collection.IsViewHidden()) {
                PdfString documentName = collection.GetInitialDocument();
                PdfNameTree embeddedFiles = GetCatalog().GetNameTree(PdfName.EmbeddedFiles);
                PdfObject fileSpecObject = embeddedFiles.GetNames().Get(documentName);
                if (fileSpecObject != null && fileSpecObject.IsDictionary()) {
                    try {
                        PdfFileSpec fileSpec = PdfEncryptedPayloadFileSpecFactory.Wrap((PdfDictionary)fileSpecObject);
                        if (fileSpec != null) {
                            PdfDictionary embeddedDictionary = ((PdfDictionary)fileSpec.GetPdfObject()).GetAsDictionary(PdfName.EF);
                            PdfStream stream = embeddedDictionary.GetAsStream(PdfName.UF);
                            if (stream == null) {
                                stream = embeddedDictionary.GetAsStream(PdfName.F);
                            }
                            if (stream != null) {
                                String documentNameUnicode = documentName.ToUnicodeString();
                                return new PdfEncryptedPayloadDocument(stream, fileSpec, documentNameUnicode);
                            }
                        }
                    }
                    catch (PdfException e) {
                        ITextLogManager.GetLogger(GetType()).LogError(e.Message);
                    }
                }
            }
            return null;
        }

        /// <summary>Sets an encrypted payload, making this document an unencrypted wrapper document.</summary>
        /// <remarks>
        /// Sets an encrypted payload, making this document an unencrypted wrapper document.
        /// The file spec shall include the AFRelationship key with a value of EncryptedPayload,
        /// and shall include an encrypted payload dictionary.
        /// </remarks>
        /// <param name="fs">
        /// encrypted payload file spec.
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfEncryptedPayloadFileSpecFactory"/>
        /// can produce one.
        /// </param>
        public virtual void SetEncryptedPayload(PdfFileSpec fs) {
            if (GetWriter() == null) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_SET_ENCRYPTED_PAYLOAD_TO_DOCUMENT_OPENED_IN_READING_MODE
                    );
            }
            if (WriterHasEncryption()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_SET_ENCRYPTED_PAYLOAD_TO_ENCRYPTED_DOCUMENT);
            }
            if (!PdfName.EncryptedPayload.Equals(((PdfDictionary)fs.GetPdfObject()).Get(PdfName.AFRelationship))) {
                ITextLogManager.GetLogger(GetType()).LogError(iText.IO.Logs.IoLogMessageConstant.ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_HAVE_AFRELATIONSHIP_FILED_EQUAL_TO_ENCRYPTED_PAYLOAD
                    );
            }
            PdfEncryptedPayload encryptedPayload = PdfEncryptedPayload.ExtractFrom(fs);
            if (encryptedPayload == null) {
                throw new PdfException(KernelExceptionMessageConstant.ENCRYPTED_PAYLOAD_FILE_SPEC_DOES_NOT_HAVE_ENCRYPTED_PAYLOAD_DICTIONARY
                    );
            }
            PdfCollection collection = GetCatalog().GetCollection();
            if (collection != null) {
                ITextLogManager.GetLogger(GetType()).LogWarning(iText.IO.Logs.IoLogMessageConstant.COLLECTION_DICTIONARY_ALREADY_EXISTS_IT_WILL_BE_MODIFIED
                    );
            }
            else {
                collection = new PdfCollection();
                GetCatalog().SetCollection(collection);
            }
            collection.SetView(PdfCollection.HIDDEN);
            String displayName = PdfEncryptedPayloadFileSpecFactory.GenerateFileDisplay(encryptedPayload);
            collection.SetInitialDocument(displayName);
            AddAssociatedFile(displayName, fs);
        }

        /// <summary>This method retrieves the page labels from a document as an array of String objects.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// list of page labels if they were found, or
        /// <see langword="null"/>
        /// otherwise
        /// </returns>
        public virtual String[] GetPageLabels() {
            if (catalog.GetPageLabelsTree(false) == null) {
                return null;
            }
            IDictionary<int?, PdfObject> pageLabels = catalog.GetPageLabelsTree(false).GetNumbers();
            if (pageLabels.Count == 0) {
                return null;
            }
            String[] labelStrings = new String[GetNumberOfPages()];
            int pageCount = 1;
            String prefix = "";
            String type = "D";
            for (int i = 0; i < GetNumberOfPages(); i++) {
                if (pageLabels.ContainsKey(i)) {
                    PdfDictionary labelDictionary = (PdfDictionary)pageLabels.Get(i);
                    PdfNumber pageRange = labelDictionary.GetAsNumber(PdfName.St);
                    if (pageRange != null) {
                        pageCount = pageRange.IntValue();
                    }
                    else {
                        pageCount = 1;
                    }
                    PdfString p = labelDictionary.GetAsString(PdfName.P);
                    if (p != null) {
                        prefix = p.ToUnicodeString();
                    }
                    else {
                        prefix = "";
                    }
                    PdfName t = labelDictionary.GetAsName(PdfName.S);
                    if (t != null) {
                        type = t.GetValue();
                    }
                    else {
                        type = "e";
                    }
                }
                switch (type) {
                    case "R": {
                        labelStrings[i] = prefix + RomanNumbering.ToRomanUpperCase(pageCount);
                        break;
                    }

                    case "r": {
                        labelStrings[i] = prefix + RomanNumbering.ToRomanLowerCase(pageCount);
                        break;
                    }

                    case "A": {
                        labelStrings[i] = prefix + EnglishAlphabetNumbering.ToLatinAlphabetNumberUpperCase(pageCount);
                        break;
                    }

                    case "a": {
                        labelStrings[i] = prefix + EnglishAlphabetNumbering.ToLatinAlphabetNumberLowerCase(pageCount);
                        break;
                    }

                    case "e": {
                        labelStrings[i] = prefix;
                        break;
                    }

                    default: {
                        labelStrings[i] = prefix + pageCount;
                        break;
                    }
                }
                pageCount++;
            }
            return labelStrings;
        }

        /// <summary>Indicates if the document has any outlines</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// , if there are outlines and
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool HasOutlines() {
            return catalog.HasOutlines();
        }

        /// <summary>Sets the flag indicating the presence of structure elements that contain user properties attributes.
        ///     </summary>
        /// <param name="userProperties">the user properties flag</param>
        public virtual void SetUserProperties(bool userProperties) {
            PdfBoolean userPropsVal = userProperties ? PdfBoolean.TRUE : PdfBoolean.FALSE;
            UpdateValueInMarkInfoDict(PdfName.UserProperties, userPropsVal);
        }

        /// <summary>
        /// Create a new instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// or load already created one.
        /// </summary>
        /// <param name="dictionary">
        /// 
        /// <see cref="PdfDictionary"/>
        /// that presents
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </param>
        /// <returns>
        /// instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// <para />
        /// Note, PdfFont which created with
        /// <see cref="iText.Kernel.Font.PdfFontFactory.CreateFont(PdfDictionary)"/>
        /// won't be cached
        /// until it will be added to
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// or
        /// <see cref="PdfResources"/>.
        /// </returns>
        public virtual PdfFont GetFont(PdfDictionary dictionary) {
            PdfIndirectReference indirectReference = dictionary.GetIndirectReference();
            if (indirectReference != null && documentFonts.ContainsKey(indirectReference)) {
                return documentFonts.Get(indirectReference);
            }
            else {
                return AddFont(PdfFontFactory.CreateFont(dictionary));
            }
        }

        /// <summary>Gets default font for the document: Helvetica, WinAnsi.</summary>
        /// <remarks>
        /// Gets default font for the document: Helvetica, WinAnsi.
        /// One instance per document.
        /// </remarks>
        /// <returns>
        /// instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// or
        /// <see langword="null"/>
        /// on error.
        /// </returns>
        public virtual PdfFont GetDefaultFont() {
            if (defaultFont == null) {
                try {
                    defaultFont = PdfFontFactory.CreateFont();
                    if (writer != null) {
                        defaultFont.MakeIndirect(this);
                    }
                }
                catch (System.IO.IOException e) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                    logger.LogError(e, iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_CREATING_DEFAULT_FONT);
                    defaultFont = null;
                }
            }
            return defaultFont;
        }

        /// <summary>
        /// Adds a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// instance to this document so that this font is flushed automatically
        /// on document close.
        /// </summary>
        /// <remarks>
        /// Adds a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// instance to this document so that this font is flushed automatically
        /// on document close. As a side effect, the underlying font dictionary is made indirect if it wasn't the case yet
        /// </remarks>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// instance to add
        /// </param>
        /// <returns>the same PdfFont instance.</returns>
        public virtual PdfFont AddFont(PdfFont font) {
            font.MakeIndirect(this);
            // forbid release for font dictionaries that are stored in #documentFonts collection
            font.SetForbidRelease();
            documentFonts.Put(font.GetPdfObject().GetIndirectReference(), font);
            return font;
        }

        /// <summary>Registers a product for debugging purposes.</summary>
        /// <param name="productData">product to be registered.</param>
        /// <returns>true if the product hadn't been registered before.</returns>
        public virtual bool RegisterProduct(ProductData productData) {
            return this.fingerPrint.RegisterProduct(productData);
        }

        /// <summary>Returns the object containing the registered products.</summary>
        /// <returns>fingerprint object</returns>
        public virtual FingerPrint GetFingerPrint() {
            return fingerPrint;
        }

        /// <summary>
        /// Find
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// from loaded fonts with corresponding fontProgram and encoding or CMAP.
        /// </summary>
        /// <param name="fontProgram">a font name or path to a font program</param>
        /// <param name="encoding">an encoding or CMAP</param>
        /// <returns>the font instance, or null if font wasn't found</returns>
        public virtual PdfFont FindFont(String fontProgram, String encoding) {
            foreach (PdfFont font in documentFonts.Values) {
                if (!font.IsFlushed() && font.IsBuiltWith(fontProgram, encoding)) {
                    return font;
                }
            }
            return null;
        }

        /// <summary>Obtains numeric document id.</summary>
        /// <returns>document id</returns>
        public virtual long GetDocumentId() {
            return documentId.GetId();
        }

        /// <summary>
        /// Obtains document id as a
        /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>.
        /// </summary>
        /// <returns>document id</returns>
        public virtual SequenceId GetDocumentIdWrapper() {
            return documentId;
        }

        /// <summary>Gets a persistent XMP metadata serialization options.</summary>
        /// <returns>serialize options</returns>
        public virtual SerializeOptions GetSerializeOptions() {
            return this.serializeOptions;
        }

        /// <summary>Sets a persistent XMP metadata serialization options.</summary>
        /// <param name="serializeOptions">serialize options</param>
        public virtual void SetSerializeOptions(SerializeOptions serializeOptions) {
            this.serializeOptions = serializeOptions;
        }

        /// <summary>
        /// Initialize
        /// <see cref="iText.Kernel.Pdf.Tagutils.TagStructureContext"/>.
        /// </summary>
        protected internal virtual void InitTagStructureContext() {
            tagStructureContext = new TagStructureContext(this);
        }

        /// <summary>Save destinations in a temporary storage for further copying.</summary>
        /// <param name="destination">
        /// the
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// to be updated itself.
        /// </param>
        /// <param name="onPageAvailable">
        /// a destination consumer that will handle the copying when the
        /// destination still resolves, it gets the new destination as input
        /// </param>
        /// <param name="onPageNotAvailable">
        /// a destination consumer that will handle the copying when the
        /// destination is not available, it gets the original destination
        /// as input
        /// </param>
        protected internal virtual void StoreDestinationToReaddress(PdfDestination destination, Action<PdfDestination
            > onPageAvailable, Action<PdfDestination> onPageNotAvailable) {
            pendingDestinationMutations.Add(new PdfDocument.DestinationMutationInfo(destination, onPageAvailable, onPageNotAvailable
                ));
        }

        /// <summary>Checks whether PDF document conforms a specific standard.</summary>
        /// <remarks>
        /// Checks whether PDF document conforms a specific standard.
        /// Shall be override.
        /// </remarks>
        protected internal virtual void CheckIsoConformance() {
        }

        /// <summary>
        /// Mark an object with
        /// <see cref="PdfObject.MUST_BE_FLUSHED"/>.
        /// </summary>
        /// <param name="pdfObject">an object to mark.</param>
        protected internal virtual void MarkObjectAsMustBeFlushed(PdfObject pdfObject) {
            if (pdfObject.GetIndirectReference() != null) {
                pdfObject.GetIndirectReference().SetState(PdfObject.MUST_BE_FLUSHED);
            }
        }

        /// <summary>Flush an object.</summary>
        /// <param name="pdfObject">object to flush.</param>
        /// <param name="canBeInObjStm">indicates whether object can be placed into object stream.</param>
        protected internal virtual void FlushObject(PdfObject pdfObject, bool canBeInObjStm) {
            writer.FlushObject(pdfObject, canBeInObjStm);
        }

        /// <summary>Initializes document.</summary>
        /// <param name="newPdfVersion">
        /// new pdf version of the resultant file if stamper is used and the version needs to be
        /// changed,
        /// or
        /// <see langword="null"/>
        /// otherwise
        /// </param>
        protected internal virtual void Open(PdfVersion newPdfVersion) {
            this.fingerPrint = new FingerPrint();
            this.encryptedEmbeddedStreamsHandler = new EncryptedEmbeddedStreamsHandler(this);
            try {
                ITextCoreProductEvent @event = ITextCoreProductEvent.CreateProcessPdfEvent(this.GetDocumentIdWrapper(), properties
                    .metaInfo, writer == null ? EventConfirmationType.ON_DEMAND : EventConfirmationType.ON_CLOSE);
                EventManager.GetInstance().OnEvent(@event);
                bool embeddedStreamsSavedOnReading = false;
                if (reader != null) {
                    if (reader.pdfDocument != null) {
                        throw new PdfException(KernelExceptionMessageConstant.PDF_READER_HAS_BEEN_ALREADY_UTILIZED);
                    }
                    reader.pdfDocument = this;
                    memoryLimitsAwareHandler = reader.properties.memoryLimitsAwareHandler;
                    if (null == memoryLimitsAwareHandler) {
                        memoryLimitsAwareHandler = new MemoryLimitsAwareHandler(reader.tokens.GetSafeFile().Length());
                    }
                    xref.SetMemoryLimitsAwareHandler(memoryLimitsAwareHandler);
                    reader.ReadPdf();
                    if (reader.decrypt != null && reader.decrypt.IsEmbeddedFilesOnly()) {
                        encryptedEmbeddedStreamsHandler.StoreAllEmbeddedStreams();
                        embeddedStreamsSavedOnReading = true;
                    }
                    pdfVersion = reader.headerPdfVersion;
                    trailer = new PdfDictionary(reader.trailer);
                    ReadDocumentIds();
                    PdfDictionary catalogDictionary = (PdfDictionary)trailer.Get(PdfName.Root, true);
                    if (null == catalogDictionary) {
                        throw new PdfException(KernelExceptionMessageConstant.CORRUPTED_ROOT_ENTRY_IN_TRAILER);
                    }
                    catalog = new PdfCatalog(catalogDictionary);
                    UpdatePdfVersionFromCatalog();
                    PdfStream xmpMetadataStream = catalog.GetPdfObject().GetAsStream(PdfName.Metadata);
                    if (xmpMetadataStream != null) {
                        xmpMetadata = xmpMetadataStream.GetBytes();
                        if (!this.GetType().Equals(typeof(iText.Kernel.Pdf.PdfDocument))) {
                            // TODO DEVSIX-5292 If somebody extends PdfDocument we have to initialize document info
                            //  and conformance level to provide compatibility. This code block shall be removed
                            reader.GetPdfAConformanceLevel();
                            GetDocumentInfo();
                        }
                    }
                    PdfDictionary str = catalog.GetPdfObject().GetAsDictionary(PdfName.StructTreeRoot);
                    if (str != null) {
                        TryInitTagStructure(str);
                    }
                    if (properties.appendMode && (reader.HasRebuiltXref() || reader.HasFixedXref())) {
                        throw new PdfException(KernelExceptionMessageConstant.APPEND_MODE_REQUIRES_A_DOCUMENT_WITHOUT_ERRORS_EVEN_IF_RECOVERY_IS_POSSIBLE
                            );
                    }
                }
                xref.InitFreeReferencesList(this);
                if (writer != null) {
                    if (reader != null && reader.HasXrefStm() && writer.properties.isFullCompression == null) {
                        writer.properties.isFullCompression = true;
                    }
                    if (reader != null && !reader.IsOpenedWithFullPermission()) {
                        throw new BadPasswordException(BadPasswordException.PdfReaderNotOpenedWithOwnerPassword);
                    }
                    if (reader != null && properties.preserveEncryption) {
                        writer.crypto = reader.decrypt;
                    }
                    writer.document = this;
                    if (reader == null) {
                        catalog = new PdfCatalog(this);
                        info = new PdfDocumentInfo(this).AddCreationDate();
                    }
                    GetDocumentInfo().AddModDate();
                    if (trailer == null) {
                        trailer = new PdfDictionary();
                    }
                    // We keep the original trailer of the document to preserve the original document keys,
                    // but we have to remove all standard keys that can occur in the trailer to avoid invalid pdfs
                    if (trailer.Size() > 0) {
                        foreach (PdfName key in iText.Kernel.Pdf.PdfDocument.PDF_NAMES_TO_REMOVE_FROM_ORIGINAL_TRAILER) {
                            trailer.Remove(key);
                        }
                    }
                    trailer.Put(PdfName.Root, catalog.GetPdfObject().GetIndirectReference());
                    trailer.Put(PdfName.Info, GetDocumentInfo().GetPdfObject().GetIndirectReference());
                    if (reader != null) {
                        // If the reader's trailer contains an ID entry, let's copy it over to the new trailer
                        if (reader.trailer.ContainsKey(PdfName.ID)) {
                            trailer.Put(PdfName.ID, reader.trailer.Get(PdfName.ID));
                        }
                    }
                    if (writer.properties != null) {
                        PdfString readerModifiedId = modifiedDocumentId;
                        if (writer.properties.initialDocumentId != null && !(reader != null && reader.decrypt != null && (properties
                            .appendMode || properties.preserveEncryption))) {
                            originalDocumentId = writer.properties.initialDocumentId;
                        }
                        if (writer.properties.modifiedDocumentId != null) {
                            modifiedDocumentId = writer.properties.modifiedDocumentId;
                        }
                        if (originalDocumentId == null && modifiedDocumentId != null) {
                            originalDocumentId = modifiedDocumentId;
                        }
                        if (modifiedDocumentId == null) {
                            if (originalDocumentId == null) {
                                originalDocumentId = new PdfString(PdfEncryption.GenerateNewDocumentId());
                            }
                            modifiedDocumentId = originalDocumentId;
                        }
                        if (writer.properties.modifiedDocumentId == null && modifiedDocumentId.Equals(readerModifiedId)) {
                            modifiedDocumentId = new PdfString(PdfEncryption.GenerateNewDocumentId());
                        }
                    }
                    System.Diagnostics.Debug.Assert(originalDocumentId != null);
                    System.Diagnostics.Debug.Assert(modifiedDocumentId != null);
                }
                if (properties.appendMode) {
                    // Due to constructor reader and writer not null.
                    System.Diagnostics.Debug.Assert(reader != null);
                    RandomAccessFileOrArray file = reader.tokens.GetSafeFile();
                    int n;
                    byte[] buffer = new byte[8192];
                    while ((n = file.Read(buffer)) > 0) {
                        writer.Write(buffer, 0, n);
                    }
                    file.Close();
                    writer.Write((byte)'\n');
                    OverrideFullCompressionInWriterProperties(writer.properties, reader.HasXrefStm());
                    writer.crypto = reader.decrypt;
                    if (newPdfVersion != null) {
                        // In PDF 1.4, a PDF version can also be specified in the Version entry of the document catalog,
                        // essentially updating the version associated with the file by overriding the one specified in
                        // the file header
                        if (pdfVersion.CompareTo(PdfVersion.PDF_1_4) >= 0) {
                            // If the header specifies a later version, or if this entry is absent, the document conforms
                            // to the
                            // version specified in the header.
                            // So only update the version if it is older than the one in the header
                            if (newPdfVersion.CompareTo(reader.headerPdfVersion) > 0) {
                                catalog.Put(PdfName.Version, newPdfVersion.ToPdfName());
                                catalog.SetModified();
                                pdfVersion = newPdfVersion;
                            }
                        }
                    }
                }
                else {
                    // Formally we cannot update version in the catalog as it is not supported for the
                    // PDF version of the original document
                    if (writer != null) {
                        if (newPdfVersion != null) {
                            pdfVersion = newPdfVersion;
                        }
                        writer.WriteHeader();
                        if (writer.crypto == null) {
                            writer.InitCryptoIfSpecified(pdfVersion);
                        }
                        if (writer.crypto != null) {
                            if (!embeddedStreamsSavedOnReading && writer.crypto.IsEmbeddedFilesOnly()) {
                                encryptedEmbeddedStreamsHandler.StoreAllEmbeddedStreams();
                            }
                            if (writer.crypto.GetCryptoMode() < EncryptionConstants.ENCRYPTION_AES_256) {
                                VersionConforming.ValidatePdfVersionForDeprecatedFeatureLogWarn(this, PdfVersion.PDF_2_0, VersionConforming
                                    .DEPRECATED_ENCRYPTION_ALGORITHMS);
                            }
                            else {
                                if (writer.crypto.GetCryptoMode() == EncryptionConstants.ENCRYPTION_AES_256) {
                                    PdfNumber r = writer.crypto.GetPdfObject().GetAsNumber(PdfName.R);
                                    if (r != null && r.IntValue() == 5) {
                                        VersionConforming.ValidatePdfVersionForDeprecatedFeatureLogWarn(this, PdfVersion.PDF_2_0, VersionConforming
                                            .DEPRECATED_AES256_REVISION);
                                    }
                                }
                            }
                        }
                    }
                }
                if (EventConfirmationType.ON_DEMAND == @event.GetConfirmationType()) {
                    // Event confirmation: opening has passed successfully
                    EventManager.GetInstance().OnEvent(new ConfirmEvent(@event));
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_OPEN_DOCUMENT, e, this);
            }
        }

        /// <summary>Adds custom XMP metadata extension.</summary>
        /// <remarks>Adds custom XMP metadata extension. Useful for PDF/UA, ZUGFeRD, etc.</remarks>
        /// <param name="xmpMeta">
        /// 
        /// <see cref="iText.Kernel.XMP.XMPMeta"/>
        /// to add custom metadata to.
        /// </param>
        protected internal virtual void AddCustomMetadataExtensions(XMPMeta xmpMeta) {
        }

        /// <summary>Updates XMP metadata.</summary>
        /// <remarks>
        /// Updates XMP metadata.
        /// Shall be override.
        /// </remarks>
        protected internal virtual void UpdateXmpMetadata() {
            try {
                // We add PDF producer info in any case, and the valid way to do it for PDF 2.0 in only in metadata, not
                // in the info dictionary.
                if (xmpMetadata != null || writer.properties.addXmpMetadata || pdfVersion.CompareTo(PdfVersion.PDF_2_0) >=
                     0) {
                    SetXmpMetadata(UpdateDefaultXmpMetadata());
                }
            }
            catch (XMPException e) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                logger.LogError(e, iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA);
            }
        }

        /// <summary>
        /// Update XMP metadata values from
        /// <see cref="PdfDocumentInfo"/>.
        /// </summary>
        /// <returns>the XMPMetadata</returns>
        protected internal virtual XMPMeta UpdateDefaultXmpMetadata() {
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(GetXmpMetadata(true));
            XmpMetaInfoConverter.AppendDocumentInfoToMetadata(GetDocumentInfo(), xmpMeta);
            if (IsTagged() && writer.properties.addUAXmpMetadata && !IsXmpMetaHasProperty(xmpMeta, XMPConst.NS_PDFUA_ID
                , XMPConst.PART)) {
                xmpMeta.SetPropertyInteger(XMPConst.NS_PDFUA_ID, XMPConst.PART, 1, new PropertyOptions(PropertyOptions.SEPARATE_NODE
                    ));
            }
            return xmpMeta;
        }

        /// <summary>List all newly added or loaded fonts</summary>
        /// <returns>
        /// List of
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </returns>
        protected internal virtual ICollection<PdfFont> GetDocumentFonts() {
            return documentFonts.Values;
        }

        /// <summary>Flushes all newly added or loaded fonts.</summary>
        protected internal virtual void FlushFonts() {
            if (properties.appendMode) {
                foreach (PdfFont font in GetDocumentFonts()) {
                    if (font.GetPdfObject().CheckState(PdfObject.MUST_BE_INDIRECT) || font.GetPdfObject().GetIndirectReference
                        ().CheckState(PdfObject.MODIFIED)) {
                        font.Flush();
                    }
                }
            }
            else {
                foreach (PdfFont font in GetDocumentFonts()) {
                    font.Flush();
                }
            }
        }

        /// <summary>Checks page before adding and add.</summary>
        /// <param name="index">one-base index of the page.</param>
        /// <param name="page">
        /// 
        /// <see cref="PdfPage"/>
        /// to add.
        /// </param>
        protected internal virtual void CheckAndAddPage(int index, PdfPage page) {
            if (page.IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.FLUSHED_PAGE_CANNOT_BE_ADDED_OR_INSERTED, page);
            }
            if (page.GetDocument() != null && this != page.GetDocument()) {
                throw new PdfException(KernelExceptionMessageConstant.PAGE_CANNOT_BE_ADDED_TO_DOCUMENT_BECAUSE_IT_BELONGS_TO_ANOTHER_DOCUMENT
                    ).SetMessageParams(page.GetDocument(), page.GetDocument().GetPageNumber(page), this);
            }
            catalog.GetPageTree().AddPage(index, page);
        }

        /// <summary>Checks page before adding.</summary>
        /// <param name="page">
        /// 
        /// <see cref="PdfPage"/>
        /// to add.
        /// </param>
        protected internal virtual void CheckAndAddPage(PdfPage page) {
            if (page.IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.FLUSHED_PAGE_CANNOT_BE_ADDED_OR_INSERTED, page);
            }
            if (page.GetDocument() != null && this != page.GetDocument()) {
                throw new PdfException(KernelExceptionMessageConstant.PAGE_CANNOT_BE_ADDED_TO_DOCUMENT_BECAUSE_IT_BELONGS_TO_ANOTHER_DOCUMENT
                    ).SetMessageParams(page.GetDocument(), page.GetDocument().GetPageNumber(page), this);
            }
            catalog.GetPageTree().AddPage(page);
        }

        /// <summary>checks whether a method is invoked at the closed document</summary>
        protected internal virtual void CheckClosingStatus() {
            if (closed) {
                throw new PdfException(KernelExceptionMessageConstant.DOCUMENT_CLOSED_IT_IS_IMPOSSIBLE_TO_EXECUTE_ACTION);
            }
        }

        /// <summary>Returns the factory for creating page instances.</summary>
        /// <returns>
        /// implementation of
        /// <see cref="IPdfPageFactory"/>
        /// for current document
        /// </returns>
        protected internal virtual IPdfPageFactory GetPageFactory() {
            return pdfPageFactory;
        }

        /// <summary>
        /// Initializes the new instance of document's structure tree root
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructTreeRoot"/>.
        /// </summary>
        /// <remarks>
        /// Initializes the new instance of document's structure tree root
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructTreeRoot"/>.
        /// See ISO 32000-1, section 14.7.2 Structure Hierarchy.
        /// </remarks>
        /// <param name="str">dictionary to create structure tree root</param>
        protected internal virtual void TryInitTagStructure(PdfDictionary str) {
            try {
                structTreeRoot = new PdfStructTreeRoot(str, this);
                structParentIndex = GetStructTreeRoot().GetParentTreeNextKey();
            }
            catch (Exception ex) {
                structTreeRoot = null;
                structParentIndex = -1;
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                logger.LogError(ex, iText.IO.Logs.IoLogMessageConstant.TAG_STRUCTURE_INIT_FAILED);
            }
        }

        /// <summary>Gets list of indirect references.</summary>
        /// <returns>list of indirect references.</returns>
        internal virtual PdfXrefTable GetXref() {
            return xref;
        }

        internal virtual bool IsDocumentFont(PdfIndirectReference indRef) {
            return indRef != null && documentFonts.ContainsKey(indRef);
        }

        internal virtual bool DoesStreamBelongToEmbeddedFile(PdfStream stream) {
            return encryptedEmbeddedStreamsHandler.IsStreamStoredAsEmbedded(stream);
        }

        internal virtual bool HasAcroForm() {
            return GetCatalog().GetPdfObject().ContainsKey(PdfName.AcroForm);
        }

        private void TryFlushTagStructure(bool isAppendMode) {
            try {
                if (tagStructureContext != null) {
                    tagStructureContext.PrepareToDocumentClosing();
                }
                if (!isAppendMode || structTreeRoot.GetPdfObject().IsModified()) {
                    structTreeRoot.Flush();
                }
            }
            catch (Exception ex) {
                throw new PdfException(KernelExceptionMessageConstant.TAG_STRUCTURE_FLUSHING_FAILED_IT_MIGHT_BE_CORRUPTED, 
                    ex);
            }
        }

        private void UpdateValueInMarkInfoDict(PdfName key, PdfObject value) {
            PdfDictionary markInfo = catalog.GetPdfObject().GetAsDictionary(PdfName.MarkInfo);
            if (markInfo == null) {
                markInfo = new PdfDictionary();
                catalog.GetPdfObject().Put(PdfName.MarkInfo, markInfo);
            }
            markInfo.Put(key, value);
        }

        /// <summary>Removes all widgets associated with a given page from AcroForm structure.</summary>
        /// <remarks>Removes all widgets associated with a given page from AcroForm structure. Widgets can be either pure or merged.
        ///     </remarks>
        /// <param name="page">to remove from.</param>
        private void RemoveUnusedWidgetsFromFields(PdfPage page) {
            if (page.IsFlushed()) {
                return;
            }
            PdfDictionary acroForm = this.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm);
            PdfArray fields = acroForm == null ? null : acroForm.GetAsArray(PdfName.Fields);
            IList<PdfAnnotation> annots = page.GetAnnotations();
            foreach (PdfAnnotation annot in annots) {
                if (annot.GetSubtype().Equals(PdfName.Widget)) {
                    ((PdfWidgetAnnotation)annot).ReleaseFormFieldFromWidgetAnnotation();
                    if (fields != null) {
                        fields.Remove(annot.GetPdfObject());
                    }
                }
            }
        }

        private void ResolveDestinations(iText.Kernel.Pdf.PdfDocument toDocument, IDictionary<PdfPage, PdfPage> page2page
            ) {
            foreach (PdfDocument.DestinationMutationInfo mutation in pendingDestinationMutations) {
                PdfDestination copiedDest = null;
                copiedDest = GetCatalog().CopyDestination(mutation.GetOriginalDestination().GetPdfObject(), page2page, toDocument
                    );
                if (copiedDest == null) {
                    mutation.HandleDestinationUnavailable();
                }
                else {
                    mutation.HandleDestinationAvailable(copiedDest);
                }
            }
        }

        /// <summary>This method copies all given outlines</summary>
        /// <param name="outlines">outlines to be copied</param>
        /// <param name="toDocument">document where outlines should be copied</param>
        private void CopyOutlines(ICollection<PdfOutline> outlines, iText.Kernel.Pdf.PdfDocument toDocument, IDictionary
            <PdfPage, PdfPage> page2page) {
            ICollection<PdfOutline> outlinesToCopy = new HashSet<PdfOutline>();
            outlinesToCopy.AddAll(outlines);
            foreach (PdfOutline outline in outlines) {
                GetAllOutlinesToCopy(outline, outlinesToCopy);
            }
            PdfOutline rootOutline = toDocument.GetOutlines(false);
            if (rootOutline == null) {
                rootOutline = new PdfOutline(toDocument);
                rootOutline.SetTitle("Outlines");
            }
            CloneOutlines(outlinesToCopy, rootOutline, GetOutlines(false), page2page, toDocument);
        }

        /// <summary>This method gets all outlines to be copied including parent outlines</summary>
        /// <param name="outline">current outline</param>
        /// <param name="outlinesToCopy">a Set of outlines to be copied</param>
        private void GetAllOutlinesToCopy(PdfOutline outline, ICollection<PdfOutline> outlinesToCopy) {
            PdfOutline parent = outline.GetParent();
            //note there's no need to continue recursion if the current outline parent is root (first condition) or
            // if it is already in the Set of outlines to be copied (second condition)
            if ("Outlines".Equals(parent.GetTitle()) || outlinesToCopy.Contains(parent)) {
                return;
            }
            outlinesToCopy.Add(parent);
            GetAllOutlinesToCopy(parent, outlinesToCopy);
        }

        /// <summary>This method copies create new outlines in the Document to copy.</summary>
        /// <param name="outlinesToCopy">- Set of outlines to be copied</param>
        /// <param name="newParent">- new parent outline</param>
        /// <param name="oldParent">- old parent outline</param>
        private void CloneOutlines(ICollection<PdfOutline> outlinesToCopy, PdfOutline newParent, PdfOutline oldParent
            , IDictionary<PdfPage, PdfPage> page2page, iText.Kernel.Pdf.PdfDocument toDocument) {
            if (null == oldParent) {
                return;
            }
            foreach (PdfOutline outline in oldParent.GetAllChildren()) {
                if (outlinesToCopy.Contains(outline)) {
                    PdfDestination copiedDest = null;
                    if (null != outline.GetDestination()) {
                        PdfObject destObjToCopy = outline.GetDestination().GetPdfObject();
                        copiedDest = GetCatalog().CopyDestination(destObjToCopy, page2page, toDocument);
                    }
                    PdfOutline child = newParent.AddOutline(outline.GetTitle());
                    if (copiedDest != null) {
                        child.AddDestination(copiedDest);
                    }
                    int? copiedStyle = outline.GetStyle();
                    if (copiedStyle != null) {
                        child.SetStyle(copiedStyle.Value);
                    }
                    Color copiedColor = outline.GetColor();
                    if (copiedColor != null) {
                        child.SetColor(copiedColor);
                    }
                    child.SetOpen(outline.IsOpen());
                    CloneOutlines(outlinesToCopy, child, outline, page2page, toDocument);
                }
            }
        }

        private void EnsureTreeRootAddedToNames(PdfObject treeRoot, PdfName treeType) {
            PdfDictionary names = catalog.GetPdfObject().GetAsDictionary(PdfName.Names);
            if (names == null) {
                names = new PdfDictionary();
                catalog.Put(PdfName.Names, names);
                names.MakeIndirect(this);
            }
            names.Put(treeType, treeRoot);
            names.SetModified();
        }

        private bool WriterHasEncryption() {
            return writer.properties.IsStandardEncryptionUsed() || writer.properties.IsPublicKeyEncryptionUsed();
        }

        private void UpdatePdfVersionFromCatalog() {
            if (catalog.GetPdfObject().ContainsKey(PdfName.Version)) {
                // The version of the PDF specification to which the document conforms (for example, 1.4)
                // if later than the version specified in the file's header
                try {
                    PdfVersion catalogVersion = PdfVersion.FromPdfName(catalog.GetPdfObject().GetAsName(PdfName.Version));
                    if (catalogVersion.CompareTo(pdfVersion) > 0) {
                        pdfVersion = catalogVersion;
                    }
                }
                catch (ArgumentException) {
                    ProcessReadingError(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_VERSION_IN_CATALOG_CORRUPTED);
                }
            }
        }

        private void ReadDocumentIds() {
            PdfArray id = reader.trailer.GetAsArray(PdfName.ID);
            if (id != null) {
                if (id.Size() == 2) {
                    originalDocumentId = id.GetAsString(0);
                    modifiedDocumentId = id.GetAsString(1);
                }
                if (originalDocumentId == null || modifiedDocumentId == null) {
                    ProcessReadingError(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_IDS_ARE_CORRUPTED);
                }
            }
        }

        private void ProcessReadingError(String errorMessage) {
            if (PdfReader.StrictnessLevel.CONSERVATIVE.IsStricter(reader.GetStrictnessLevel())) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                logger.LogError(errorMessage);
            }
            else {
                throw new PdfException(errorMessage);
            }
        }

        private static void OverrideFullCompressionInWriterProperties(WriterProperties properties, bool readerHasXrefStream
            ) {
            if (true == properties.isFullCompression && !readerHasXrefStream) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                logger.LogWarning(KernelLogMessageConstant.FULL_COMPRESSION_APPEND_MODE_XREF_TABLE_INCONSISTENCY);
            }
            else {
                if (false == properties.isFullCompression && readerHasXrefStream) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfDocument));
                    logger.LogWarning(KernelLogMessageConstant.FULL_COMPRESSION_APPEND_MODE_XREF_STREAM_INCONSISTENCY);
                }
            }
            properties.isFullCompression = readerHasXrefStream;
        }

        private static bool IsXmpMetaHasProperty(XMPMeta xmpMeta, String schemaNS, String propName) {
            return xmpMeta.GetProperty(schemaNS, propName) != null;
        }

        private class DestinationMutationInfo {
            private readonly PdfDestination originalDestination;

            private readonly Action<PdfDestination> onDestinationAvailable;

            private readonly Action<PdfDestination> onDestinationNotAvailable;

            public DestinationMutationInfo(PdfDestination originalDestination, Action<PdfDestination> onDestinationAvailable
                , Action<PdfDestination> onDestinationNotAvailable) {
                this.originalDestination = originalDestination;
                this.onDestinationAvailable = onDestinationAvailable;
                this.onDestinationNotAvailable = onDestinationNotAvailable;
            }

            public virtual void HandleDestinationAvailable(PdfDestination newDestination) {
                onDestinationAvailable(newDestination);
            }

            public virtual void HandleDestinationUnavailable() {
                onDestinationNotAvailable(originalDestination);
            }

            public virtual PdfDestination GetOriginalDestination() {
                return originalDestination;
            }
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
