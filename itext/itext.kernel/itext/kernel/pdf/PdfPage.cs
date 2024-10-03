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
using iText.Kernel.Events;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Kernel.Validation.Context;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.Pdf {
    public class PdfPage : PdfObjectWrapper<PdfDictionary> {
        private PdfResources resources = null;

        private int mcid = -1;

//\cond DO_NOT_DOCUMENT
        internal PdfPages parentPages;
//\endcond

        private static readonly IList<PdfName> PAGE_EXCLUDED_KEYS = new List<PdfName>(JavaUtil.ArraysAsList(PdfName
            .Parent, PdfName.Annots, PdfName.StructParents, PdfName.B));

        // This key contains reference to all articles, while this articles could reference to lots of pages.
        // See DEVSIX-191
        private static readonly IList<PdfName> XOBJECT_EXCLUDED_KEYS;

        static PdfPage() {
            XOBJECT_EXCLUDED_KEYS = new List<PdfName>(JavaUtil.ArraysAsList(PdfName.MediaBox, PdfName.CropBox, PdfName
                .TrimBox, PdfName.Contents));
            XOBJECT_EXCLUDED_KEYS.AddAll(PAGE_EXCLUDED_KEYS);
        }

        /// <summary>Automatically rotate new content if the page has a rotation ( is disabled by default )</summary>
        private bool ignorePageRotationForContent = false;

        /// <summary>
        /// See
        /// <see cref="IsPageRotationInverseMatrixWritten()"/>.
        /// </summary>
        private bool pageRotationInverseMatrixWritten = false;

        protected internal PdfPage(PdfDictionary pdfObject)
            : base(pdfObject) {
            SetForbidRelease();
            EnsureObjectIsAddedToDocument(pdfObject);
        }

        protected internal PdfPage(PdfDocument pdfDocument, PageSize pageSize)
            : this((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDocument)) {
            PdfStream contentStream = (PdfStream)new PdfStream().MakeIndirect(pdfDocument);
            GetPdfObject().Put(PdfName.Contents, contentStream);
            GetPdfObject().Put(PdfName.Type, PdfName.Page);
            GetPdfObject().Put(PdfName.MediaBox, new PdfArray(pageSize));
            GetPdfObject().Put(PdfName.TrimBox, new PdfArray(pageSize));
            if (pdfDocument.IsTagged()) {
                SetTabOrder(PdfName.S);
            }
        }

        protected internal PdfPage(PdfDocument pdfDocument)
            : this(pdfDocument, pdfDocument.GetDefaultPageSize()) {
        }

        /// <summary>Gets page size, defined by media box object.</summary>
        /// <remarks>Gets page size, defined by media box object. This method doesn't take page rotation into account.
        ///     </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// that specify page size.
        /// </returns>
        public virtual Rectangle GetPageSize() {
            return GetMediaBox();
        }

        /// <summary>Gets page size, considering page rotation.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// that specify size of rotated page.
        /// </returns>
        public virtual Rectangle GetPageSizeWithRotation() {
            PageSize rect = new PageSize(GetPageSize());
            int rotation = GetRotation();
            while (rotation > 0) {
                rect = rect.Rotate();
                rotation -= 90;
            }
            return rect;
        }

        /// <summary>Gets the number of degrees by which the page shall be rotated clockwise when displayed or printed.
        ///     </summary>
        /// <remarks>
        /// Gets the number of degrees by which the page shall be rotated clockwise when displayed or printed.
        /// Shall be a multiple of 90.
        /// </remarks>
        /// <returns>
        /// 
        /// <c>int</c>
        /// number of degrees. Default value: 0
        /// </returns>
        public virtual int GetRotation() {
            PdfNumber rotate = GetPdfObject().GetAsNumber(PdfName.Rotate);
            int rotateValue = 0;
            if (rotate == null) {
                rotate = (PdfNumber)GetInheritedValue(PdfName.Rotate, PdfObject.NUMBER);
            }
            if (rotate != null) {
                rotateValue = rotate.IntValue();
            }
            rotateValue %= 360;
            return rotateValue < 0 ? rotateValue + 360 : rotateValue;
        }

        /// <summary>Sets the page rotation.</summary>
        /// <param name="degAngle">
        /// the
        /// <c>int</c>
        /// number of degrees by which the page shall be rotated clockwise
        /// when displayed or printed. Shall be a multiple of 90.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetRotation(int degAngle) {
            Put(PdfName.Rotate, new PdfNumber(degAngle));
            return this;
        }

        /// <summary>
        /// Gets the content stream at specified 0-based index in the Contents object
        /// <see cref="PdfArray"/>.
        /// </summary>
        /// <remarks>
        /// Gets the content stream at specified 0-based index in the Contents object
        /// <see cref="PdfArray"/>.
        /// The situation when Contents object is a
        /// <see cref="PdfStream"/>
        /// is treated like a one element array.
        /// </remarks>
        /// <param name="index">
        /// the
        /// <c>int</c>
        /// index of returned
        /// <see cref="PdfStream"/>.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="PdfStream"/>
        /// object at specified index;
        /// will return null in case page dictionary doesn't adhere to the specification, meaning that the document is an invalid PDF.
        /// </returns>
        public virtual PdfStream GetContentStream(int index) {
            int count = GetContentStreamCount();
            if (index >= count || index < 0) {
                throw new IndexOutOfRangeException(MessageFormatUtil.Format("Index: {0}, Size: {1}", index, count));
            }
            PdfObject contents = GetPdfObject().Get(PdfName.Contents);
            if (contents is PdfStream) {
                return (PdfStream)contents;
            }
            else {
                if (contents is PdfArray) {
                    PdfArray a = (PdfArray)contents;
                    return a.GetAsStream(index);
                }
                else {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the size of Contents object
        /// <see cref="PdfArray"/>.
        /// </summary>
        /// <remarks>
        /// Gets the size of Contents object
        /// <see cref="PdfArray"/>.
        /// The situation when Contents object is a
        /// <see cref="PdfStream"/>
        /// is treated like a one element array.
        /// </remarks>
        /// <returns>
        /// the
        /// <c>int</c>
        /// size of Contents object, or 1 if Contents object is a
        /// <see cref="PdfStream"/>.
        /// </returns>
        public virtual int GetContentStreamCount() {
            PdfObject contents = GetPdfObject().Get(PdfName.Contents);
            if (contents is PdfStream) {
                return 1;
            }
            else {
                if (contents is PdfArray) {
                    return ((PdfArray)contents).Size();
                }
                else {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns the Contents object if it is
        /// <see cref="PdfStream"/>
        /// , or first stream in the array if it is
        /// <see cref="PdfArray"/>.
        /// </summary>
        /// <returns>
        /// first
        /// <see cref="PdfStream"/>
        /// in Contents object, or
        /// <see langword="null"/>
        /// if Contents is empty.
        /// </returns>
        public virtual PdfStream GetFirstContentStream() {
            if (GetContentStreamCount() > 0) {
                return GetContentStream(0);
            }
            return null;
        }

        /// <summary>
        /// Returns the Contents object if it is
        /// <see cref="PdfStream"/>
        /// , or last stream in the array if it is
        /// <see cref="PdfArray"/>.
        /// </summary>
        /// <returns>
        /// first
        /// <see cref="PdfStream"/>
        /// in Contents object, or
        /// <see langword="null"/>
        /// if Contents is empty.
        /// </returns>
        public virtual PdfStream GetLastContentStream() {
            int count = GetContentStreamCount();
            if (count > 0) {
                return GetContentStream(count - 1);
            }
            return null;
        }

        /// <summary>
        /// Creates new
        /// <see cref="PdfStream"/>
        /// object and puts it at the beginning of Contents array
        /// (if Contents object is
        /// <see cref="PdfStream"/>
        /// it will be replaced with one-element array).
        /// </summary>
        /// <returns>
        /// Created
        /// <see cref="PdfStream"/>
        /// object.
        /// </returns>
        public virtual PdfStream NewContentStreamBefore() {
            return NewContentStream(true);
        }

        /// <summary>
        /// Creates new
        /// <see cref="PdfStream"/>
        /// object and puts it at the end of
        /// <c>Contents</c>
        /// array
        /// (if Contents object is
        /// <see cref="PdfStream"/>
        /// it will be replaced with one-element array).
        /// </summary>
        /// <returns>
        /// Created
        /// <see cref="PdfStream"/>
        /// object.
        /// </returns>
        public virtual PdfStream NewContentStreamAfter() {
            return NewContentStream(false);
        }

        /// <summary>
        /// Gets the
        /// <see cref="PdfResources"/>
        /// wrapper object for this page resources.
        /// </summary>
        /// <remarks>
        /// Gets the
        /// <see cref="PdfResources"/>
        /// wrapper object for this page resources.
        /// If page doesn't have resource object, then it will be inherited from page's parents.
        /// If neither parents nor page has the resource object, then the new one is created and added to page dictionary.
        /// <br /><br />
        /// NOTE: If you'll try to modify the inherited resources, then the new resources object will be created,
        /// so you won't change the parent's resources.
        /// This new object under the wrapper will be added to page dictionary on
        /// <see cref="Flush()"/>
        /// ,
        /// or you can add it manually with this line, if needed:<br />
        /// <c>getPdfObject().put(PdfName.Resources, getResources().getPdfObject());</c>
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="PdfResources"/>
        /// wrapper of the page.
        /// </returns>
        public virtual PdfResources GetResources() {
            return GetResources(true);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual PdfResources GetResources(bool initResourcesField) {
            if (this.resources == null && initResourcesField) {
                InitResources(true);
            }
            return this.resources;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual PdfDictionary InitResources(bool initResourcesField) {
            bool readOnly = false;
            PdfDictionary resources = GetPdfObject().GetAsDictionary(PdfName.Resources);
            if (resources == null) {
                resources = (PdfDictionary)GetInheritedValue(PdfName.Resources, PdfObject.DICTIONARY);
                if (resources != null) {
                    readOnly = true;
                }
            }
            if (resources == null) {
                resources = new PdfDictionary();
                // not marking page as modified because of this change
                GetPdfObject().Put(PdfName.Resources, resources);
            }
            if (initResourcesField) {
                this.resources = new PdfResources(resources);
                this.resources.SetReadOnly(readOnly);
            }
            return resources;
        }
//\endcond

        /// <summary>
        /// Sets
        /// <see cref="PdfResources"/>
        /// object.
        /// </summary>
        /// <param name="pdfResources">
        /// 
        /// <see cref="PdfResources"/>
        /// to set.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetResources(PdfResources pdfResources) {
            Put(PdfName.Resources, pdfResources.GetPdfObject());
            this.resources = pdfResources;
            return this;
        }

        /// <summary>Sets the XMP Metadata.</summary>
        /// <param name="xmpMetadata">
        /// the
        /// <c>byte[]</c>
        /// of XMP Metadata to set.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetXmpMetadata(byte[] xmpMetadata) {
            PdfStream xmp = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            xmp.GetOutputStream().Write(xmpMetadata);
            xmp.Put(PdfName.Type, PdfName.Metadata);
            xmp.Put(PdfName.Subtype, PdfName.XML);
            Put(PdfName.Metadata, xmp);
            return this;
        }

        /// <summary>Serializes XMP Metadata to byte array and sets it.</summary>
        /// <param name="xmpMeta">
        /// the
        /// <see cref="iText.Kernel.XMP.XMPMeta"/>
        /// object to set.
        /// </param>
        /// <param name="serializeOptions">
        /// the
        /// <see cref="iText.Kernel.XMP.Options.SerializeOptions"/>
        /// used while serialization.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetXmpMetadata(XMPMeta xmpMeta, SerializeOptions serializeOptions) {
            return SetXmpMetadata(XMPMetaFactory.SerializeToBuffer(xmpMeta, serializeOptions));
        }

        /// <summary>Serializes XMP Metadata to byte array and sets it.</summary>
        /// <remarks>Serializes XMP Metadata to byte array and sets it. Uses padding equals to 2000.</remarks>
        /// <param name="xmpMeta">
        /// the
        /// <see cref="iText.Kernel.XMP.XMPMeta"/>
        /// object to set.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetXmpMetadata(XMPMeta xmpMeta) {
            SerializeOptions serializeOptions = new SerializeOptions();
            serializeOptions.SetPadding(2000);
            return SetXmpMetadata(xmpMeta, serializeOptions);
        }

        /// <summary>Gets the XMP Metadata object.</summary>
        /// <returns>
        /// 
        /// <see cref="PdfStream"/>
        /// object, that represent XMP Metadata.
        /// </returns>
        public virtual PdfStream GetXmpMetadata() {
            return GetPdfObject().GetAsStream(PdfName.Metadata);
        }

        /// <summary>Copies page to the specified document.</summary>
        /// <remarks>
        /// Copies page to the specified document.
        /// <br /><br />
        /// NOTE: Works only for pages from the document opened in reading mode, otherwise an exception is thrown.
        /// </remarks>
        /// <param name="toDocument">a document to copy page to.</param>
        /// <returns>
        /// copied
        /// <see cref="PdfPage"/>.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage CopyTo(PdfDocument toDocument) {
            return CopyTo(toDocument, null);
        }

        /// <summary>Copies page to the specified document.</summary>
        /// <remarks>
        /// Copies page to the specified document.
        /// <br /><br />
        /// NOTE: Works only for pages from the document opened in reading mode, otherwise an exception is thrown.
        /// </remarks>
        /// <param name="toDocument">a document to copy page to.</param>
        /// <param name="copier">
        /// a copier which bears a special copy logic. May be null.
        /// It is recommended to use the same instance of
        /// <see cref="IPdfPageExtraCopier"/>
        /// for the same output document.
        /// </param>
        /// <returns>
        /// copied
        /// <see cref="PdfPage"/>.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage CopyTo(PdfDocument toDocument, IPdfPageExtraCopier copier) {
            return CopyTo(toDocument, copier, false, -1);
        }

        /// <summary>Copies page and adds it to the specified document to the end or by index if the corresponding parameter is true.
        ///     </summary>
        /// <remarks>
        /// Copies page and adds it to the specified document to the end or by index if the corresponding parameter is true.
        /// <br /><br />
        /// NOTE: Works only for pages from the document opened in reading mode, otherwise an exception is thrown.
        /// NOTE: If both documents (from which and to which the copy is made) are tagged, you must additionally call the
        /// <see cref="IPdfPageFormCopier.RecreateAcroformToProcessCopiedFields(PdfDocument)"/>
        /// method after copying the
        /// tag structure to process copied fields, like add them to the document and merge fields with the same names.
        /// </remarks>
        /// <param name="toDocument">a document to copy page to.</param>
        /// <param name="copier">
        /// a copier which bears a special copy logic. May be null.
        /// It is recommended to use the same instance of
        /// <see cref="IPdfPageExtraCopier"/>
        /// for the same output document.
        /// </param>
        /// <param name="addPageToDocument">true if page should be added to document.</param>
        /// <param name="pageInsertIndex">
        /// position to add the page to, if -1 page will be added to the end of the document,
        /// will be ignored if addPageToDocument is false.
        /// </param>
        /// <returns>
        /// copied
        /// <see cref="PdfPage"/>.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage CopyTo(PdfDocument toDocument, IPdfPageExtraCopier copier, bool addPageToDocument
            , int pageInsertIndex) {
            ICopyFilter copyFilter = new DestinationResolverCopyFilter(this.GetDocument(), toDocument);
            PdfDictionary dictionary = GetPdfObject().CopyTo(toDocument, PAGE_EXCLUDED_KEYS, true, copyFilter);
            iText.Kernel.Pdf.PdfPage page = GetDocument().GetPageFactory().CreatePdfPage(dictionary);
            if (addPageToDocument) {
                if (pageInsertIndex == -1) {
                    toDocument.AddPage(page);
                }
                else {
                    toDocument.AddPage(pageInsertIndex, page);
                }
            }
            return CopyTo(page, toDocument, copier);
        }

        /// <summary>Get all pdf layers stored under this page's annotations/xobjects/resources.</summary>
        /// <remarks>
        /// Get all pdf layers stored under this page's annotations/xobjects/resources.
        /// Note that it will include all layers, even those already stored under /OCProperties entry in catalog.
        /// To get only unique layers, you can simply exclude ocgs, which already present in catalog.
        /// </remarks>
        /// <returns>set of pdf layers, associated with this page.</returns>
        public virtual ICollection<PdfLayer> GetPdfLayers() {
            ICollection<PdfIndirectReference> ocgs = OcgPropertiesCopier.GetOCGsFromPage(this);
            ICollection<PdfLayer> result = new LinkedHashSet<PdfLayer>();
            foreach (PdfIndirectReference ocg in ocgs) {
                if (ocg.GetRefersTo() != null && ocg.GetRefersTo().IsDictionary()) {
                    result.Add(new PdfLayer((PdfDictionary)ocg.GetRefersTo()));
                }
            }
            return result;
        }

        /// <summary>Copies page as FormXObject to the specified document.</summary>
        /// <param name="toDocument">a document to copy to.</param>
        /// <returns>
        /// copied
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// object.
        /// </returns>
        public virtual PdfFormXObject CopyAsFormXObject(PdfDocument toDocument) {
            PdfFormXObject xObject = new PdfFormXObject(GetCropBox());
            foreach (PdfName key in GetPdfObject().KeySet()) {
                if (XOBJECT_EXCLUDED_KEYS.Contains(key)) {
                    continue;
                }
                PdfObject obj = GetPdfObject().Get(key);
                if (!xObject.GetPdfObject().ContainsKey(key)) {
                    PdfObject copyObj = obj.CopyTo(toDocument, false, NullCopyFilter.GetInstance());
                    xObject.GetPdfObject().Put(key, copyObj);
                }
            }
            xObject.GetPdfObject().GetOutputStream().Write(GetContentBytes());
            //Copy inherited resources
            if (!xObject.GetPdfObject().ContainsKey(PdfName.Resources)) {
                PdfObject copyResource = GetResources().GetPdfObject().CopyTo(toDocument, true, NullCopyFilter.GetInstance
                    ());
                xObject.GetPdfObject().Put(PdfName.Resources, copyResource);
            }
            return xObject;
        }

        /// <summary>
        /// Gets the
        /// <see cref="PdfDocument"/>
        /// that owns that page, or
        /// <see langword="null"/>
        /// if such document isn't exist.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfDocument"/>
        /// that owns that page, or
        /// <see langword="null"/>
        /// if such document isn't exist.
        /// </returns>
        public virtual PdfDocument GetDocument() {
            if (GetPdfObject().GetIndirectReference() != null) {
                return GetPdfObject().GetIndirectReference().GetDocument();
            }
            return null;
        }

        /// <summary>Flushes page dictionary, its content streams, annotations and thumb image.</summary>
        /// <remarks>
        /// Flushes page dictionary, its content streams, annotations and thumb image.
        /// <para />
        /// If the page belongs to the document which is tagged, page flushing also triggers flushing of the tags,
        /// which are considered to belong to the page. The logic that defines if the given tag (structure element) belongs
        /// to the page is the following: if all the marked content references (dictionary or number references), that are the
        /// descendants of the given structure element, belong to the current page - the tag is considered
        /// to belong to the page. If tag has descendants from several pages - it is flushed, if all other pages except the
        /// current one are flushed.
        /// </remarks>
        public override void Flush() {
            Flush(false);
        }

        /// <summary>Flushes page dictionary, its content streams, annotations and thumb image.</summary>
        /// <remarks>
        /// Flushes page dictionary, its content streams, annotations and thumb image. If <c>flushResourcesContentStreams</c> is true,
        /// all content streams that are rendered on this page (like FormXObjects, annotation appearance streams, patterns)
        /// and also all images associated with this page will also be flushed.
        /// <para />
        /// For notes about tag structure flushing see
        /// <see cref="Flush()">PdfPage#flush() method</see>.
        /// <para />
        /// If <c>PdfADocument</c> is used, flushing will be applied only if <c>flushResourcesContentStreams</c> is true.
        /// <para />
        /// Be careful with handling document in which some of the pages are flushed. Keep in mind that flushed objects are
        /// finalized and are completely written to the output stream. This frees their memory but makes
        /// it impossible to modify or read data from them. Whenever there is an attempt to modify or to fetch
        /// flushed object inner contents an exception will be thrown. Flushing is only possible for objects in the writing
        /// and stamping modes, also its possible to flush modified objects in append mode.
        /// </remarks>
        /// <param name="flushResourcesContentStreams">
        /// if true all content streams that are rendered on this page (like form xObjects,
        /// annotation appearance streams, patterns) and also all images associated with this page
        /// will be flushed.
        /// </param>
        public virtual void Flush(bool flushResourcesContentStreams) {
            if (IsFlushed()) {
                return;
            }
            GetDocument().DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.END_PAGE, this));
            if (GetDocument().IsTagged() && !GetDocument().GetStructTreeRoot().IsFlushed()) {
                TryFlushPageTags();
            }
            if (resources == null) {
                // ensure that either resources are inherited or add empty resources dictionary
                InitResources(false);
            }
            else {
                if (resources.IsModified() && !resources.IsReadOnly()) {
                    Put(PdfName.Resources, resources.GetPdfObject());
                }
            }
            if (flushResourcesContentStreams) {
                GetDocument().CheckIsoConformance(new PdfPageValidationContext(this));
                FlushResourcesContentStreams();
            }
            PdfArray annots = GetAnnots(false);
            if (annots != null && !annots.IsFlushed()) {
                for (int i = 0; i < annots.Size(); ++i) {
                    PdfObject a = annots.Get(i);
                    if (a != null) {
                        a.MakeIndirect(GetDocument()).Flush();
                    }
                }
            }
            PdfStream thumb = GetPdfObject().GetAsStream(PdfName.Thumb);
            if (thumb != null) {
                thumb.Flush();
            }
            PdfObject contentsObj = GetPdfObject().Get(PdfName.Contents);
            // avoid trying to operate with flushed /Contents array
            if (contentsObj != null && !contentsObj.IsFlushed()) {
                int contentStreamCount = GetContentStreamCount();
                for (int i = 0; i < contentStreamCount; i++) {
                    PdfStream contentStream = GetContentStream(i);
                    if (contentStream != null) {
                        contentStream.Flush(false);
                    }
                }
            }
            ReleaseInstanceFields();
            base.Flush();
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by page's Media Box, that defines the boundaries of the physical medium
        /// on which the page shall be displayed or printed
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by page Media Box, expressed in default user space units.
        /// </returns>
        public virtual Rectangle GetMediaBox() {
            PdfArray mediaBox = GetPdfObject().GetAsArray(PdfName.MediaBox);
            if (mediaBox == null) {
                mediaBox = (PdfArray)GetInheritedValue(PdfName.MediaBox, PdfObject.ARRAY);
            }
            if (mediaBox == null) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_RETRIEVE_MEDIA_BOX_ATTRIBUTE);
            }
            int mediaBoxSize;
            if ((mediaBoxSize = mediaBox.Size()) != 4) {
                if (mediaBoxSize > 4) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfPage));
                    if (logger.IsEnabled(LogLevel.Error)) {
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.WRONG_MEDIABOX_SIZE_TOO_MANY_ARGUMENTS
                            , mediaBoxSize));
                    }
                }
                if (mediaBoxSize < 4) {
                    throw new PdfException(KernelExceptionMessageConstant.WRONG_MEDIA_BOX_SIZE_TOO_FEW_ARGUMENTS).SetMessageParams
                        (mediaBox.Size());
                }
            }
            PdfNumber llx = mediaBox.GetAsNumber(0);
            PdfNumber lly = mediaBox.GetAsNumber(1);
            PdfNumber urx = mediaBox.GetAsNumber(2);
            PdfNumber ury = mediaBox.GetAsNumber(3);
            if (llx == null || lly == null || urx == null || ury == null) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_MEDIA_BOX_VALUE);
            }
            return new Rectangle(Math.Min(llx.FloatValue(), urx.FloatValue()), Math.Min(lly.FloatValue(), ury.FloatValue
                ()), Math.Abs(urx.FloatValue() - llx.FloatValue()), Math.Abs(ury.FloatValue() - lly.FloatValue()));
        }

        /// <summary>
        /// Sets the Media Box object, that defines the boundaries of the physical medium
        /// on which the page shall be displayed or printed.
        /// </summary>
        /// <param name="rectangle">
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object to set, expressed in default user space units.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetMediaBox(Rectangle rectangle) {
            Put(PdfName.MediaBox, new PdfArray(rectangle));
            return this;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// specified by page's CropBox, that defines the visible region of default user space.
        /// </summary>
        /// <remarks>
        /// Gets the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// specified by page's CropBox, that defines the visible region of default user space.
        /// When the page is displayed or printed, its contents shall be clipped (cropped) to this rectangle
        /// and then shall be imposed on the output medium in some implementation-defined manner.
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by pages's CropBox, expressed in default user space units.
        /// MediaBox by default.
        /// </returns>
        public virtual Rectangle GetCropBox() {
            PdfArray cropBox = GetPdfObject().GetAsArray(PdfName.CropBox);
            if (cropBox == null) {
                cropBox = (PdfArray)GetInheritedValue(PdfName.CropBox, PdfObject.ARRAY);
                if (cropBox == null) {
                    return GetMediaBox();
                }
            }
            return cropBox.ToRectangle();
        }

        /// <summary>Sets the CropBox object, that defines the visible region of default user space.</summary>
        /// <remarks>
        /// Sets the CropBox object, that defines the visible region of default user space.
        /// When the page is displayed or printed, its contents shall be clipped (cropped) to this rectangle
        /// and then shall be imposed on the output medium in some implementation-defined manner.
        /// </remarks>
        /// <param name="rectangle">
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object to set, expressed in default user space units.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetCropBox(Rectangle rectangle) {
            Put(PdfName.CropBox, new PdfArray(rectangle));
            return this;
        }

        /// <summary>
        /// Sets the BleedBox object, that defines the region to which the contents of the page shall be clipped
        /// when output in a production environment.
        /// </summary>
        /// <param name="rectangle">
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object to set, expressed in default user space units.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetBleedBox(Rectangle rectangle) {
            Put(PdfName.BleedBox, new PdfArray(rectangle));
            return this;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by page's BleedBox, that define the region to which the
        /// contents of the page shall be clipped when output in a production environment.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by page's BleedBox, expressed in default user space units.
        /// CropBox by default.
        /// </returns>
        public virtual Rectangle GetBleedBox() {
            Rectangle bleedBox = GetPdfObject().GetAsRectangle(PdfName.BleedBox);
            return bleedBox == null ? GetCropBox() : bleedBox;
        }

        /// <summary>
        /// Sets the ArtBox object, that define the extent of the page’s meaningful content
        /// (including potential white space) as intended by the page’s creator.
        /// </summary>
        /// <param name="rectangle">
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object to set, expressed in default user space units.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetArtBox(Rectangle rectangle) {
            Put(PdfName.ArtBox, new PdfArray(rectangle));
            return this;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by page's ArtBox, that define the extent of the page’s
        /// meaningful content (including potential white space) as intended by the page’s creator.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by page's ArtBox, expressed in default user space units.
        /// CropBox by default.
        /// </returns>
        public virtual Rectangle GetArtBox() {
            Rectangle artBox = GetPdfObject().GetAsRectangle(PdfName.ArtBox);
            return artBox == null ? GetCropBox() : artBox;
        }

        /// <summary>Sets the TrimBox object, that define the intended dimensions of the finished page after trimming.
        ///     </summary>
        /// <param name="rectangle">
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object to set, expressed in default user space units.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetTrimBox(Rectangle rectangle) {
            Put(PdfName.TrimBox, new PdfArray(rectangle));
            return this;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by page's TrimBox object,
        /// that define the intended dimensions of the finished page after trimming.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object specified by page's TrimBox, expressed in default user space units.
        /// CropBox by default.
        /// </returns>
        public virtual Rectangle GetTrimBox() {
            Rectangle trimBox = GetPdfObject().GetAsRectangle(PdfName.TrimBox);
            return trimBox == null ? GetCropBox() : trimBox;
        }

        /// <summary>Get decoded bytes for the whole page content.</summary>
        /// <returns>byte array.</returns>
        public virtual byte[] GetContentBytes() {
            try {
                MemoryLimitsAwareHandler handler = GetDocument().memoryLimitsAwareHandler;
                long usedMemory = null == handler ? -1 : handler.GetAllMemoryUsedForDecompression();
                MemoryLimitsAwareOutputStream baos = new MemoryLimitsAwareOutputStream();
                int streamCount = GetContentStreamCount();
                byte[] streamBytes;
                for (int i = 0; i < streamCount; i++) {
                    streamBytes = GetStreamBytes(i);
                    // usedMemory has changed, that means that some of currently processed pdf streams are suspicious
                    if (null != handler && usedMemory < handler.GetAllMemoryUsedForDecompression()) {
                        baos.SetMaxStreamSize(handler.GetMaxSizeOfSingleDecompressedPdfStream());
                    }
                    baos.Write(streamBytes);
                    if (0 != streamBytes.Length && !iText.IO.Util.TextUtil.IsWhiteSpace((char)streamBytes[streamBytes.Length -
                         1])) {
                        baos.Write('\n');
                    }
                }
                return baos.ToArray();
            }
            catch (System.IO.IOException ioe) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_GET_CONTENT_BYTES, ioe, this);
            }
        }

        /// <summary>Gets decoded bytes of a certain stream of a page content.</summary>
        /// <param name="index">index of stream inside Content.</param>
        /// <returns>byte array.</returns>
        public virtual byte[] GetStreamBytes(int index) {
            return GetContentStream(index).GetBytes();
        }

        /// <summary>Calculates and returns the next available for this page's content stream MCID reference.</summary>
        /// <returns>calculated MCID reference.</returns>
        public virtual int GetNextMcid() {
            if (!GetDocument().IsTagged()) {
                throw new PdfException(KernelExceptionMessageConstant.MUST_BE_A_TAGGED_DOCUMENT);
            }
            if (mcid == -1) {
                PdfStructTreeRoot structTreeRoot = GetDocument().GetStructTreeRoot();
                mcid = structTreeRoot.GetNextMcidForPage(this);
            }
            return mcid++;
        }

        /// <summary>Gets the key of the page’s entry in the structural parent tree.</summary>
        /// <returns>
        /// the key of the page’s entry in the structural parent tree.
        /// If page has no entry in the structural parent tree, returned value is -1.
        /// </returns>
        public virtual int GetStructParentIndex() {
            return GetPdfObject().GetAsNumber(PdfName.StructParents) != null ? GetPdfObject().GetAsNumber(PdfName.StructParents
                ).IntValue() : -1;
        }

        /// <summary>Helper method to add an additional action to this page.</summary>
        /// <remarks>
        /// Helper method to add an additional action to this page.
        /// May be used in chain.
        /// </remarks>
        /// <param name="key">
        /// a
        /// <see cref="PdfName"/>
        /// specifying the name of an additional action
        /// </param>
        /// <param name="action">
        /// the
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to add as an additional action
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetAdditionalAction(PdfName key, PdfAction action) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        /// <summary>
        /// Gets array of annotation dictionaries that shall contain indirect references
        /// to all annotations associated with the page.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="System.Collections.IList{E}"/>
        /// &lt;
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// &gt; containing all page's annotations.
        /// </returns>
        public virtual IList<PdfAnnotation> GetAnnotations() {
            IList<PdfAnnotation> annotations = new List<PdfAnnotation>();
            PdfArray annots = GetPdfObject().GetAsArray(PdfName.Annots);
            if (annots != null) {
                for (int i = 0; i < annots.Size(); i++) {
                    PdfDictionary annot = annots.GetAsDictionary(i);
                    if (annot == null) {
                        continue;
                    }
                    PdfAnnotation annotation = PdfAnnotation.MakeAnnotation(annot);
                    if (annotation == null) {
                        continue;
                    }
                    bool hasBeenNotModified = annot.GetIndirectReference() != null && !annot.GetIndirectReference().CheckState
                        (PdfObject.MODIFIED);
                    annotations.Add(annotation.SetPage(this));
                    if (hasBeenNotModified) {
                        annot.GetIndirectReference().ClearState(PdfObject.MODIFIED);
                        annot.ClearState(PdfObject.FORBID_RELEASE);
                    }
                }
            }
            return annotations;
        }

        /// <summary>Checks if page contains the specified annotation.</summary>
        /// <param name="annotation">
        /// the
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// to check.
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if page contains specified annotation and
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool ContainsAnnotation(PdfAnnotation annotation) {
            foreach (PdfAnnotation a in GetAnnotations()) {
                if (a.GetPdfObject().Equals(annotation.GetPdfObject())) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Adds specified annotation to the end of annotations array and tagged it.</summary>
        /// <remarks>
        /// Adds specified annotation to the end of annotations array and tagged it.
        /// May be used in chain.
        /// </remarks>
        /// <param name="annotation">
        /// the
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// to add.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage AddAnnotation(PdfAnnotation annotation) {
            return AddAnnotation(-1, annotation, true);
        }

        /// <summary>
        /// Adds specified
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// to specified index in annotations array with or without autotagging.
        /// </summary>
        /// <remarks>
        /// Adds specified
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// to specified index in annotations array with or without autotagging.
        /// May be used in chain.
        /// </remarks>
        /// <param name="index">
        /// the index at which specified annotation will be added. If
        /// <c>-1</c>
        /// then annotation will be added
        /// to the end of array.
        /// </param>
        /// <param name="annotation">
        /// the
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// to add.
        /// </param>
        /// <param name="tagAnnotation">
        /// if
        /// <see langword="true"/>
        /// the added annotation will be autotagged. <para />
        /// (see
        /// <see cref="iText.Kernel.Pdf.Tagutils.TagStructureContext.GetAutoTaggingPointer()"/>
        /// )
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage AddAnnotation(int index, PdfAnnotation annotation, bool tagAnnotation
            ) {
            if (GetDocument().IsTagged()) {
                if (tagAnnotation) {
                    TagTreePointer tagPointer = GetDocument().GetTagStructureContext().GetAutoTaggingPointer();
                    if (!StandardRoles.ANNOT.Equals(tagPointer.GetRole()) && PdfVersion.PDF_1_4
                                        // "Annot" tag was added starting from PDF 1.5
                                        .CompareTo(GetDocument().GetPdfVersion()) < 0) {
                        if (PdfVersion.PDF_2_0.CompareTo(GetDocument().GetPdfVersion()) > 0) {
                            if (!(annotation is PdfWidgetAnnotation) && !(annotation is PdfLinkAnnotation) && !(annotation is PdfPrinterMarkAnnotation
                                )) {
                                tagPointer.AddTag(StandardRoles.ANNOT);
                            }
                        }
                        else {
                            if (annotation is PdfMarkupAnnotation) {
                                tagPointer.AddTag(StandardRoles.ANNOT);
                            }
                        }
                    }
                    iText.Kernel.Pdf.PdfPage prevPage = tagPointer.GetCurrentPage();
                    tagPointer.SetPageForTagging(this).AddAnnotationTag(annotation);
                    if (prevPage != null) {
                        tagPointer.SetPageForTagging(prevPage);
                    }
                }
                if (GetTabOrder() == null) {
                    SetTabOrder(PdfName.S);
                }
            }
            PdfArray annots = GetAnnots(true);
            if (index == -1) {
                annots.Add(annotation.SetPage(this).GetPdfObject());
            }
            else {
                annots.Add(index, annotation.SetPage(this).GetPdfObject());
            }
            if (annots.GetIndirectReference() == null) {
                //Annots are not indirect so page needs to be marked as modified
                SetModified();
            }
            else {
                //Annots are indirect so need to be marked as modified
                annots.SetModified();
            }
            return this;
        }

        /// <summary>Removes an annotation from the page.</summary>
        /// <remarks>
        /// Removes an annotation from the page.
        /// <para />
        /// When document is tagged a corresponding logical structure content item for this annotation
        /// will be removed; its immediate structure element parent will be removed as well if the following
        /// conditions are met: annotation content item was its single child and structure element role
        /// is either Annot or Form.
        /// </remarks>
        /// <param name="annotation">an annotation to be removed</param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage RemoveAnnotation(PdfAnnotation annotation) {
            return RemoveAnnotation(annotation, false);
        }

        /// <summary>Removes an annotation from the page.</summary>
        /// <remarks>
        /// Removes an annotation from the page.
        /// <para />
        /// When document is tagged a corresponding logical structure content item for this annotation
        /// will be removed; its immediate structure element parent will be removed as well if the following
        /// conditions are met: annotation content item was its single child and structure element role
        /// is either Annot or Form.
        /// </remarks>
        /// <param name="annotation">an annotation to be removed</param>
        /// <param name="rememberTagPointer">
        /// if set to true, the
        /// <see cref="iText.Kernel.Pdf.Tagutils.TagStructureContext.GetAutoTaggingPointer()"/>
        /// instance of
        /// <see cref="iText.Kernel.Pdf.Tagutils.TagTreePointer"/>
        /// will be moved to the parent of the removed
        /// annotation tag. Can be used to add a new annotation to the same place in the
        /// tag structure. (E.g. when merged Acroform field is split into a field and
        /// a pure widget, the page annotation needs to be replaced by the new one)
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage RemoveAnnotation(PdfAnnotation annotation, bool rememberTagPointer
            ) {
            PdfArray annots = GetAnnots(false);
            if (annots != null) {
                annots.Remove(annotation.GetPdfObject());
                if (annots.IsEmpty()) {
                    Remove(PdfName.Annots);
                }
                else {
                    if (annots.GetIndirectReference() == null) {
                        SetModified();
                    }
                    else {
                        annots.SetModified();
                    }
                }
            }
            if (GetDocument().IsTagged()) {
                TagTreePointer tagPointer = GetDocument().GetTagStructureContext().RemoveAnnotationTag(annotation, rememberTagPointer
                    );
                if (tagPointer != null) {
                    bool standardAnnotTagRole = StandardRoles.ANNOT.Equals(tagPointer.GetRole()) || StandardRoles.FORM.Equals(
                        tagPointer.GetRole());
                    if (tagPointer.GetKidsRoles().IsEmpty() && standardAnnotTagRole) {
                        tagPointer.RemoveTag();
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Gets the number of
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// associated with this page.
        /// </summary>
        /// <returns>
        /// the
        /// <c>int</c>
        /// number of
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// associated with this page.
        /// </returns>
        public virtual int GetAnnotsSize() {
            PdfArray annots = GetAnnots(false);
            if (annots == null) {
                return 0;
            }
            return annots.Size();
        }

        /// <summary>This method gets outlines of a current page</summary>
        /// <param name="updateOutlines">
        /// if the flag is
        /// <see langword="true"/>
        /// , the method reads the whole document and creates outline tree.
        /// If the flag is
        /// <see langword="false"/>
        /// , the method gets cached outline tree
        /// (if it was cached via calling getOutlines method before).
        /// </param>
        /// <returns>return all outlines of a current page</returns>
        public virtual IList<PdfOutline> GetOutlines(bool updateOutlines) {
            GetDocument().GetOutlines(updateOutlines);
            return GetDocument().GetCatalog().GetPagesWithOutlines().Get(GetPdfObject());
        }

        /// <returns>
        /// true - if in case the page has a rotation, then new content will be automatically rotated in the
        /// opposite direction. On the rotated page this would look like if new content ignores page rotation.
        /// </returns>
        public virtual bool IsIgnorePageRotationForContent() {
            return ignorePageRotationForContent;
        }

        /// <summary>
        /// If true - defines that in case the page has a rotation, then new content will be automatically rotated in the
        /// opposite direction.
        /// </summary>
        /// <remarks>
        /// If true - defines that in case the page has a rotation, then new content will be automatically rotated in the
        /// opposite direction. On the rotated page this would look like if new content ignores page rotation.
        /// Default value -
        /// <see langword="false"/>.
        /// </remarks>
        /// <param name="ignorePageRotationForContent">- true to ignore rotation of the new content on the rotated page.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetIgnorePageRotationForContent(bool ignorePageRotationForContent) {
            this.ignorePageRotationForContent = ignorePageRotationForContent;
            return this;
        }

        /// <summary>This method adds or replaces a page label.</summary>
        /// <param name="numberingStyle">
        /// The numbering style that shall be used for the numeric portion of each page label.
        /// May be NULL
        /// </param>
        /// <param name="labelPrefix">The label prefix for page labels in this range. May be NULL</param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetPageLabel(PageLabelNumberingStyle? numberingStyle, String labelPrefix
            ) {
            return SetPageLabel(numberingStyle, labelPrefix, 1);
        }

        /// <summary>This method adds or replaces a page label.</summary>
        /// <param name="numberingStyle">
        /// The numbering style that shall be used for the numeric portion of each page label.
        /// May be NULL
        /// </param>
        /// <param name="labelPrefix">The label prefix for page labels in this range. May be NULL</param>
        /// <param name="firstPage">
        /// The value of the numeric portion for the first page label in the range. Must be greater or
        /// equal 1.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetPageLabel(PageLabelNumberingStyle? numberingStyle, String labelPrefix
            , int firstPage) {
            if (firstPage < 1) {
                throw new PdfException(KernelExceptionMessageConstant.IN_A_PAGE_LABEL_THE_PAGE_NUMBERS_MUST_BE_GREATER_OR_EQUAL_TO_1
                    );
            }
            PdfDictionary pageLabel = new PdfDictionary();
            if (numberingStyle != null) {
                switch (numberingStyle) {
                    case PageLabelNumberingStyle.DECIMAL_ARABIC_NUMERALS: {
                        pageLabel.Put(PdfName.S, PdfName.D);
                        break;
                    }

                    case PageLabelNumberingStyle.UPPERCASE_ROMAN_NUMERALS: {
                        pageLabel.Put(PdfName.S, PdfName.R);
                        break;
                    }

                    case PageLabelNumberingStyle.LOWERCASE_ROMAN_NUMERALS: {
                        pageLabel.Put(PdfName.S, PdfName.r);
                        break;
                    }

                    case PageLabelNumberingStyle.UPPERCASE_LETTERS: {
                        pageLabel.Put(PdfName.S, PdfName.A);
                        break;
                    }

                    case PageLabelNumberingStyle.LOWERCASE_LETTERS: {
                        pageLabel.Put(PdfName.S, PdfName.a);
                        break;
                    }

                    default: {
                        break;
                    }
                }
            }
            if (labelPrefix != null) {
                pageLabel.Put(PdfName.P, new PdfString(labelPrefix));
            }
            if (firstPage != 1) {
                pageLabel.Put(PdfName.St, new PdfNumber(firstPage));
            }
            GetDocument().GetCatalog().GetPageLabelsTree(true).AddEntry(GetDocument().GetPageNumber(this) - 1, pageLabel
                );
            return this;
        }

        /// <summary>Sets a name specifying the tab order that shall be used for annotations on the page.</summary>
        /// <remarks>
        /// Sets a name specifying the tab order that shall be used for annotations on the page.
        /// The possible values are
        /// <see cref="PdfName.R"/>
        /// (row order),
        /// <see cref="PdfName.C"/>
        /// (column order), and
        /// <see cref="PdfName.S"/>
        /// (structure order).
        /// Beginning with PDF 2.0, the possible values also include
        /// <see cref="PdfName.A"/>
        /// (annotations array order) and
        /// <see cref="PdfName.W"/>
        /// (widget order).
        /// See ISO 32000 12.5, "Annotations" for details.
        /// </remarks>
        /// <param name="tabOrder">
        /// a
        /// <see cref="PdfName"/>
        /// specifying the annotations tab order. See method description for the allowed values.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetTabOrder(PdfName tabOrder) {
            Put(PdfName.Tabs, tabOrder);
            return this;
        }

        /// <summary>Gets a name specifying the tab order that shall be used for annotations on the page.</summary>
        /// <remarks>
        /// Gets a name specifying the tab order that shall be used for annotations on the page.
        /// The possible values are
        /// <see cref="PdfName.R"/>
        /// (row order),
        /// <see cref="PdfName.C"/>
        /// (column order), and
        /// <see cref="PdfName.S"/>
        /// (structure order).
        /// Beginning with PDF 2.0, the possible values also include
        /// <see cref="PdfName.A"/>
        /// (annotations array order) and
        /// <see cref="PdfName.W"/>
        /// (widget order).
        /// See ISO 32000 12.5, "Annotations" for details.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="PdfName"/>
        /// specifying the annotations tab order or null if tab order is not defined.
        /// </returns>
        public virtual PdfName GetTabOrder() {
            return GetPdfObject().GetAsName(PdfName.Tabs);
        }

        /// <summary>Sets a stream object that shall define the page’s thumbnail image.</summary>
        /// <remarks>
        /// Sets a stream object that shall define the page’s thumbnail image. Thumbnail images represent the contents of
        /// its pages in miniature form
        /// </remarks>
        /// <param name="thumb">the thumbnail image</param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// object
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage SetThumbnailImage(PdfImageXObject thumb) {
            return Put(PdfName.Thumb, thumb.GetPdfObject());
        }

        /// <summary>Sets a stream object that shall define the page’s thumbnail image.</summary>
        /// <remarks>
        /// Sets a stream object that shall define the page’s thumbnail image. Thumbnail images represent the contents of
        /// its pages in miniature form
        /// </remarks>
        /// <returns>the thumbnail image, or <c>null</c> if it is not present</returns>
        public virtual PdfImageXObject GetThumbnailImage() {
            PdfStream thumbStream = GetPdfObject().GetAsStream(PdfName.Thumb);
            return thumbStream != null ? new PdfImageXObject(thumbStream) : null;
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfOutputIntent"/>
        /// that shall specify the colour characteristics of output devices
        /// on which the page might be rendered.
        /// </summary>
        /// <param name="outputIntent">
        /// 
        /// <see cref="PdfOutputIntent"/>
        /// to add.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// object
        /// </returns>
        /// <seealso cref="PdfOutputIntent"/>
        public virtual iText.Kernel.Pdf.PdfPage AddOutputIntent(PdfOutputIntent outputIntent) {
            if (outputIntent == null) {
                return this;
            }
            PdfArray outputIntents = GetPdfObject().GetAsArray(PdfName.OutputIntents);
            if (outputIntents == null) {
                outputIntents = new PdfArray();
                Put(PdfName.OutputIntents, outputIntents);
            }
            outputIntents.Add(outputIntent.GetPdfObject());
            return this;
        }

        /// <summary>
        /// Helper method that associates specified value with the specified key in the underlying
        /// <see cref="PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Helper method that associates specified value with the specified key in the underlying
        /// <see cref="PdfDictionary"/>
        /// . Can be used in method chaining.
        /// </remarks>
        /// <param name="key">
        /// the
        /// <see cref="PdfName"/>
        /// key with which the specified value is to be associated
        /// </param>
        /// <param name="value">
        /// the
        /// <see cref="PdfObject"/>
        /// value to be associated with the specified key.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// object.
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        /// <summary>
        /// Helper method that removes the value associated with the specified key
        /// from the underlying
        /// <see cref="PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Helper method that removes the value associated with the specified key
        /// from the underlying
        /// <see cref="PdfDictionary"/>
        /// . Can be used in method chaining.
        /// </remarks>
        /// <param name="key">
        /// the
        /// <see cref="PdfName"/>
        /// key for which associated value is to be removed
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfPage"/>
        /// object
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfPage Remove(PdfName key) {
            GetPdfObject().Remove(key);
            SetModified();
            return this;
        }

        /// <summary>Adds file associated with PDF page and identifies the relationship between them.</summary>
        /// <remarks>
        /// Adds file associated with PDF page and identifies the relationship between them.
        /// <para />
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the page dictionary.
        /// If description is provided, it also will add file description to catalog Names tree.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="description">the file description</param>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(String description, PdfFileSpec fs) {
            if (null == ((PdfDictionary)fs.GetPdfObject()).Get(PdfName.AFRelationship)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfPage));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ASSOCIATED_FILE_SPEC_SHALL_INCLUDE_AFRELATIONSHIP);
            }
            if (null != description) {
                PdfString key = new PdfString(description);
                GetDocument().GetCatalog().AddNameToNameTree(key, fs.GetPdfObject(), PdfName.EmbeddedFiles);
            }
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null) {
                afArray = new PdfArray();
                Put(PdfName.AF, afArray);
            }
            afArray.Add(fs.GetPdfObject());
        }

        /// <summary>
        /// <para />
        /// Adds file associated with PDF page and identifies the relationship between them.
        /// </summary>
        /// <remarks>
        /// <para />
        /// Adds file associated with PDF page and identifies the relationship between them.
        /// <para />
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the page dictionary.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(PdfFileSpec fs) {
            AddAssociatedFile(null, fs);
        }

        /// <summary>Returns files associated with PDF page.</summary>
        /// <param name="create">defines whether AF arrays will be created if it doesn't exist</param>
        /// <returns>associated files array</returns>
        public virtual PdfArray GetAssociatedFiles(bool create) {
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null && create) {
                afArray = new PdfArray();
                Put(PdfName.AF, afArray);
            }
            return afArray;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void TryFlushPageTags() {
            try {
                if (!GetDocument().isClosing) {
                    GetDocument().GetTagStructureContext().FlushPageTags(this);
                }
                GetDocument().GetStructTreeRoot().SavePageStructParentIndexIfNeeded(this);
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.TAG_STRUCTURE_FLUSHING_FAILED_IT_MIGHT_BE_CORRUPTED, 
                    e);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ReleaseInstanceFields() {
            resources = null;
            parentPages = null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks if page rotation inverse matrix (which rotates content into the opposite direction from page rotation
        /// direction in order to give the impression of the not rotated text) is already applied to the page content stream.
        /// </summary>
        /// <remarks>
        /// Checks if page rotation inverse matrix (which rotates content into the opposite direction from page rotation
        /// direction in order to give the impression of the not rotated text) is already applied to the page content stream.
        /// See
        /// <see cref="SetIgnorePageRotationForContent(bool)"/>
        /// and
        /// <see cref="PageContentRotationHelper"/>.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if inverse matrix is already applied,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        internal virtual bool IsPageRotationInverseMatrixWritten() {
            return pageRotationInverseMatrixWritten;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Specifies that page rotation inverse matrix (which rotates content into the opposite direction from page rotation
        /// direction in order to give the impression of the not rotated text) is applied to the page content stream.
        /// </summary>
        /// <remarks>
        /// Specifies that page rotation inverse matrix (which rotates content into the opposite direction from page rotation
        /// direction in order to give the impression of the not rotated text) is applied to the page content stream.
        /// See
        /// <see cref="SetIgnorePageRotationForContent(bool)"/>
        /// and
        /// <see cref="PageContentRotationHelper"/>.
        /// </remarks>
        internal virtual void SetPageRotationInverseMatrixWritten() {
            pageRotationInverseMatrixWritten = true;
        }
//\endcond

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private iText.Kernel.Pdf.PdfPage CopyTo(iText.Kernel.Pdf.PdfPage page, PdfDocument toDocument, IPdfPageExtraCopier
             copier) {
            ICopyFilter copyFilter = new DestinationResolverCopyFilter(this.GetDocument(), toDocument);
            CopyInheritedProperties(page, toDocument, NullCopyFilter.GetInstance());
            CopyAnnotations(toDocument, page, copyFilter);
            if (copier != null) {
                copier.Copy(this, page);
            }
            else {
                if (!toDocument.GetWriter().isUserWarnedAboutAcroFormCopying && GetDocument().HasAcroForm()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfPage));
                    logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY);
                    toDocument.GetWriter().isUserWarnedAboutAcroFormCopying = true;
                }
            }
            return page;
        }

        private PdfArray GetAnnots(bool create) {
            PdfArray annots = GetPdfObject().GetAsArray(PdfName.Annots);
            if (annots == null && create) {
                annots = new PdfArray();
                Put(PdfName.Annots, annots);
            }
            return annots;
        }

        private PdfObject GetInheritedValue(PdfName pdfName, int type) {
            if (this.parentPages == null) {
                this.parentPages = GetDocument().GetCatalog().GetPageTree().FindPageParent(this);
            }
            PdfObject val = GetInheritedValue(this.parentPages, pdfName);
            return val != null && val.GetObjectType() == type ? val : null;
        }

        private static PdfObject GetInheritedValue(PdfPages parentPages, PdfName pdfName) {
            if (parentPages != null) {
                PdfDictionary parentDictionary = parentPages.GetPdfObject();
                PdfObject value = parentDictionary.Get(pdfName);
                if (value != null) {
                    return value;
                }
                else {
                    return GetInheritedValue(parentPages.GetParent(), pdfName);
                }
            }
            return null;
        }

        private PdfStream NewContentStream(bool before) {
            PdfObject contents = GetPdfObject().Get(PdfName.Contents);
            PdfArray array;
            if (contents is PdfStream) {
                array = new PdfArray();
                if (contents.GetIndirectReference() != null) {
                    // Explicitly using object indirect reference here in order to correctly process released objects.
                    array.Add(contents.GetIndirectReference());
                }
                else {
                    array.Add(contents);
                }
                Put(PdfName.Contents, array);
            }
            else {
                if (contents is PdfArray) {
                    array = (PdfArray)contents;
                }
                else {
                    array = null;
                }
            }
            PdfStream contentStream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            if (array != null) {
                if (before) {
                    array.Add(0, contentStream);
                }
                else {
                    array.Add(contentStream);
                }
                if (array.GetIndirectReference() != null) {
                    array.SetModified();
                }
                else {
                    SetModified();
                }
            }
            else {
                Put(PdfName.Contents, contentStream);
            }
            return contentStream;
        }

        private void CopyAnnotations(PdfDocument toDocument, iText.Kernel.Pdf.PdfPage page, ICopyFilter copyFilter
            ) {
            foreach (PdfAnnotation annot in GetAnnotations()) {
                if (copyFilter.ShouldProcess(page.GetPdfObject(), null, annot.GetPdfObject())) {
                    PdfAnnotation newAnnot = PdfAnnotation.MakeAnnotation(annot.GetPdfObject().CopyTo(toDocument, JavaUtil.ArraysAsList
                        (PdfName.P, PdfName.Parent), true, copyFilter));
                    if (PdfName.Widget.Equals(annot.GetSubtype())) {
                        RebuildFormFieldParent(annot.GetPdfObject(), newAnnot.GetPdfObject(), toDocument);
                    }
                    // P will be set in PdfPage#addAnnotation; Parent will be regenerated in PdfPageExtraCopier.
                    page.AddAnnotation(-1, newAnnot, false);
                }
            }
        }

        private void FlushResourcesContentStreams() {
            FlushResourcesContentStreams(GetResources().GetPdfObject());
            PdfArray annots = GetAnnots(false);
            if (annots != null && !annots.IsFlushed()) {
                for (int i = 0; i < annots.Size(); ++i) {
                    PdfDictionary apDict = annots.GetAsDictionary(i).GetAsDictionary(PdfName.AP);
                    if (apDict != null) {
                        FlushAppearanceStreams(apDict);
                    }
                }
            }
        }

        private void FlushResourcesContentStreams(PdfDictionary resources) {
            if (resources != null && !resources.IsFlushed()) {
                FlushWithResources(resources.GetAsDictionary(PdfName.XObject));
                FlushWithResources(resources.GetAsDictionary(PdfName.Pattern));
                FlushWithResources(resources.GetAsDictionary(PdfName.Shading));
            }
        }

        private void FlushWithResources(PdfDictionary objsCollection) {
            if (objsCollection == null || objsCollection.IsFlushed()) {
                return;
            }
            foreach (PdfObject obj in objsCollection.Values()) {
                if (obj.IsFlushed()) {
                    continue;
                }
                FlushResourcesContentStreams(((PdfDictionary)obj).GetAsDictionary(PdfName.Resources));
                FlushMustBeIndirectObject(obj);
            }
        }

        private void FlushAppearanceStreams(PdfDictionary appearanceStreamsDict) {
            if (appearanceStreamsDict.IsFlushed()) {
                return;
            }
            foreach (PdfObject val in appearanceStreamsDict.Values()) {
                if (val is PdfDictionary) {
                    PdfDictionary ap = (PdfDictionary)val;
                    if (ap.IsDictionary()) {
                        FlushAppearanceStreams(ap);
                    }
                    else {
                        if (ap.IsStream()) {
                            FlushMustBeIndirectObject(ap);
                        }
                    }
                }
            }
        }

        private void FlushMustBeIndirectObject(PdfObject obj) {
            // TODO DEVSIX-744
            obj.MakeIndirect(GetDocument()).Flush();
        }

        private void CopyInheritedProperties(iText.Kernel.Pdf.PdfPage copyPdfPage, PdfDocument pdfDocument, ICopyFilter
             copyFilter) {
            if (copyPdfPage.GetPdfObject().Get(PdfName.Resources) == null) {
                PdfObject copyResource = pdfDocument.GetWriter().CopyObject(GetResources().GetPdfObject(), pdfDocument, false
                    , copyFilter);
                copyPdfPage.GetPdfObject().Put(PdfName.Resources, copyResource);
            }
            if (copyPdfPage.GetPdfObject().Get(PdfName.MediaBox) == null) {
                //media box shall be in any case
                copyPdfPage.SetMediaBox(GetMediaBox());
            }
            if (copyPdfPage.GetPdfObject().Get(PdfName.CropBox) == null) {
                //original pdfObject don't have CropBox, otherwise copyPdfPage will contain it
                PdfArray cropBox = (PdfArray)GetInheritedValue(PdfName.CropBox, PdfObject.ARRAY);
                //crop box is optional, we shall not set default value.
                if (cropBox != null) {
                    copyPdfPage.Put(PdfName.CropBox, cropBox.CopyTo(pdfDocument));
                }
            }
            if (copyPdfPage.GetPdfObject().Get(PdfName.Rotate) == null) {
                //original pdfObject don't have Rotate, otherwise copyPdfPage will contain it
                PdfNumber rotate = (PdfNumber)GetInheritedValue(PdfName.Rotate, PdfObject.NUMBER);
                //rotate is optional, we shall not set default value.
                if (rotate != null) {
                    copyPdfPage.Put(PdfName.Rotate, rotate.CopyTo(pdfDocument));
                }
            }
        }

        private void RebuildFormFieldParent(PdfDictionary field, PdfDictionary newField, PdfDocument toDocument) {
            if (newField.ContainsKey(PdfName.Parent)) {
                return;
            }
            PdfDictionary oldParent = field.GetAsDictionary(PdfName.Parent);
            if (oldParent != null) {
                PdfDictionary newParent = oldParent.CopyTo(toDocument, JavaUtil.ArraysAsList(PdfName.P, PdfName.Kids, PdfName
                    .Parent), false, NullCopyFilter.GetInstance());
                if (newParent.IsFlushed()) {
                    newParent = oldParent.CopyTo(toDocument, JavaUtil.ArraysAsList(PdfName.P, PdfName.Kids, PdfName.Parent), true
                        , NullCopyFilter.GetInstance());
                }
                if (oldParent == oldParent.GetAsDictionary(PdfName.Parent)) {
                    return;
                }
                RebuildFormFieldParent(oldParent, newParent, toDocument);
                PdfArray kids = newParent.GetAsArray(PdfName.Kids);
                if (kids == null) {
                    // no kids are added here, since we do not know at this point which pages are to be copied,
                    // hence we do not know which annotations we should copy
                    newParent.Put(PdfName.Kids, new PdfArray());
                }
                newField.Put(PdfName.Parent, newParent);
            }
        }
    }
}
