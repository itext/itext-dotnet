/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.IO;
using iText.IO;
using iText.IO.Log;
using iText.Kernel;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.Pdf {
    public class PdfPage : PdfObjectWrapper<PdfDictionary> {
        private PdfResources resources = null;

        private int mcid = -1;

        private int structParents = -1;

        internal PdfPages parentPages;

        private IList<PdfName> excludedKeys = new List<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList(PdfName.Parent
            , PdfName.Annots, PdfName.StructParents, PdfName.B));

        /// <summary>Automatically rotate new content if the page has a rotation ( is disabled by default )</summary>
        private bool ignorePageRotationForContent = false;

        /// <summary>
        /// See
        /// <see cref="IsPageRotationInverseMatrixWritten()"/>
        /// .
        /// </summary>
        private bool pageRotationInverseMatrixWritten = false;

        protected internal PdfPage(PdfDictionary pdfObject)
            : base(pdfObject) {
            // This key contains reference to all articles, while this articles could reference to lots of pages.
            // See DEVSIX-191
            SetForbidRelease();
            EnsureObjectIsAddedToDocument(pdfObject);
        }

        protected internal PdfPage(PdfDocument pdfDocument, PageSize pageSize)
            : this(((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDocument))) {
            PdfStream contentStream = ((PdfStream)new PdfStream().MakeIndirect(pdfDocument));
            GetPdfObject().Put(PdfName.Contents, contentStream);
            GetPdfObject().Put(PdfName.Type, PdfName.Page);
            GetPdfObject().Put(PdfName.MediaBox, new PdfArray(pageSize));
            GetPdfObject().Put(PdfName.TrimBox, new PdfArray(pageSize));
            if (pdfDocument.IsTagged()) {
                structParents = (int)pdfDocument.GetNextStructParentIndex();
                GetPdfObject().Put(PdfName.StructParents, new PdfNumber(structParents));
            }
        }

        protected internal PdfPage(PdfDocument pdfDocument)
            : this(pdfDocument, pdfDocument.GetDefaultPageSize()) {
        }

        public virtual Rectangle GetPageSize() {
            return GetMediaBox();
        }

        /// <summary>Gets the rotated page.</summary>
        /// <returns>the rotated rectangle</returns>
        public virtual Rectangle GetPageSizeWithRotation() {
            PageSize rect = new PageSize(GetPageSize());
            int rotation = GetRotation();
            while (rotation > 0) {
                rect = rect.Rotate();
                rotation -= 90;
            }
            return rect;
        }

        public virtual int GetRotation() {
            PdfNumber rotate = GetPdfObject().GetAsNumber(PdfName.Rotate);
            if (rotate == null) {
                return 0;
            }
            else {
                int n = rotate.IntValue();
                n %= 360;
                return n < 0 ? n + 360 : n;
            }
        }

        public virtual iText.Kernel.Pdf.PdfPage SetRotation(int degAngle) {
            GetPdfObject().Put(PdfName.Rotate, new PdfNumber(degAngle));
            return this;
        }

        public virtual PdfStream GetContentStream(int index) {
            int count = GetContentStreamCount();
            if (index >= count) {
                throw new IndexOutOfRangeException(String.Format("Index: {0}, Size: {1}", index, count));
            }
            PdfObject contents = GetPdfObject().Get(PdfName.Contents);
            if (contents is PdfStream) {
                return (PdfStream)contents;
            }
            else {
                if (contents is PdfArray) {
                    PdfArray a = (PdfArray)contents;
                    return (PdfStream)a.Get(index);
                }
                else {
                    return null;
                }
            }
        }

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

        public virtual PdfStream GetFirstContentStream() {
            if (GetContentStreamCount() > 0) {
                return GetContentStream(0);
            }
            return null;
        }

        public virtual PdfStream GetLastContentStream() {
            int count = GetContentStreamCount();
            if (count > 0) {
                return GetContentStream(count - 1);
            }
            return null;
        }

        public virtual PdfStream NewContentStreamBefore() {
            return NewContentStream(true);
        }

        public virtual PdfStream NewContentStreamAfter() {
            return NewContentStream(false);
        }

        public virtual PdfResources GetResources() {
            if (this.resources == null) {
                bool readOnly = false;
                PdfDictionary resources = GetPdfObject().GetAsDictionary(PdfName.Resources);
                if (resources == null) {
                    InitParentPages();
                    resources = (PdfDictionary)GetParentValue(this.parentPages, PdfName.Resources);
                    if (resources != null) {
                        readOnly = true;
                    }
                }
                if (resources == null) {
                    resources = new PdfDictionary();
                    GetPdfObject().Put(PdfName.Resources, resources);
                }
                this.resources = new PdfResources(resources);
                this.resources.SetReadOnly(readOnly);
            }
            return this.resources;
        }

        public virtual iText.Kernel.Pdf.PdfPage SetResources(PdfResources pdfResources) {
            GetPdfObject().Put(PdfName.Resources, pdfResources.GetPdfObject());
            this.resources = pdfResources;
            return this;
        }

        /// <summary>Use this method to set the XMP Metadata for each page.</summary>
        /// <param name="xmpMetadata">The xmpMetadata to set.</param>
        /// <exception cref="System.IO.IOException"/>
        public virtual void SetXmpMetadata(byte[] xmpMetadata) {
            PdfStream xmp = ((PdfStream)new PdfStream().MakeIndirect(GetDocument()));
            xmp.GetOutputStream().Write(xmpMetadata);
            xmp.Put(PdfName.Type, PdfName.Metadata);
            xmp.Put(PdfName.Subtype, PdfName.XML);
            GetPdfObject().Put(PdfName.Metadata, xmp);
        }

        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual void SetXmpMetadata(XMPMeta xmpMeta, SerializeOptions serializeOptions) {
            SetXmpMetadata(XMPMetaFactory.SerializeToBuffer(xmpMeta, serializeOptions));
        }

        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual void SetXmpMetadata(XMPMeta xmpMeta) {
            SerializeOptions serializeOptions = new SerializeOptions();
            serializeOptions.SetPadding(2000);
            SetXmpMetadata(xmpMeta, serializeOptions);
        }

        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        public virtual PdfStream GetXmpMetadata() {
            return GetPdfObject().GetAsStream(PdfName.Metadata);
        }

        /// <summary>Copies page to the specified document.</summary>
        /// <remarks>
        /// Copies page to the specified document.
        /// <br/><br/>
        /// NOTE: Works only for pages from the document opened in reading mode, otherwise an exception is thrown.
        /// </remarks>
        /// <param name="toDocument">a document to copy page to.</param>
        /// <returns>copied page.</returns>
        public virtual iText.Kernel.Pdf.PdfPage CopyTo(PdfDocument toDocument) {
            return CopyTo(toDocument, null);
        }

        /// <summary>Copies page to the specified document.</summary>
        /// <remarks>
        /// Copies page to the specified document.
        /// <br/><br/>
        /// NOTE: Works only for pages from the document opened in reading mode, otherwise an exception is thrown.
        /// </remarks>
        /// <param name="toDocument">a document to copy page to.</param>
        /// <param name="copier">a copier which bears a specific copy logic. May be NULL</param>
        /// <returns>copied page.</returns>
        public virtual iText.Kernel.Pdf.PdfPage CopyTo(PdfDocument toDocument, IPdfPageExtraCopier copier) {
            PdfDictionary dictionary = GetPdfObject().CopyTo(toDocument, excludedKeys, true);
            iText.Kernel.Pdf.PdfPage page = new iText.Kernel.Pdf.PdfPage(dictionary);
            CopyInheritedProperties(page, toDocument);
            foreach (PdfAnnotation annot in GetAnnotations()) {
                if (annot.GetSubtype().Equals(PdfName.Link)) {
                    GetDocument().StoreLinkAnnotation(this, (PdfLinkAnnotation)annot);
                }
                else {
                    page.AddAnnotation(-1, PdfAnnotation.MakeAnnotation(((PdfDictionary)annot.GetPdfObject().CopyTo(toDocument
                        , true))), false);
                }
            }
            if (toDocument.IsTagged()) {
                page.structParents = (int)toDocument.GetNextStructParentIndex();
                page.GetPdfObject().Put(PdfName.StructParents, new PdfNumber(page.structParents));
            }
            if (copier != null) {
                copier.Copy(this, page);
            }
            else {
                if (!toDocument.GetWriter().isUserWarnedAboutAcroFormCopying && GetDocument().GetCatalog().GetPdfObject().
                    ContainsKey(PdfName.AcroForm)) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.PdfPage));
                    logger.Warn(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY);
                    toDocument.GetWriter().isUserWarnedAboutAcroFormCopying = true;
                }
            }
            return page;
        }

        /// <summary>Copies page as FormXObject to the specified document.</summary>
        /// <param name="toDocument">a document to copy to.</param>
        /// <returns>resultant XObject.</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual PdfFormXObject CopyAsFormXObject(PdfDocument toDocument) {
            PdfFormXObject xObject = new PdfFormXObject(GetCropBox());
            IList<PdfName> excludedKeys = new List<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList(PdfName.MediaBox, PdfName
                .CropBox, PdfName.Contents));
            excludedKeys.AddAll(this.excludedKeys);
            PdfDictionary dictionary = GetPdfObject().CopyTo(toDocument, excludedKeys, true);
            xObject.GetPdfObject().GetOutputStream().Write(GetContentBytes());
            xObject.GetPdfObject().MergeDifferent(dictionary);
            return xObject;
        }

        public virtual PdfDocument GetDocument() {
            if (GetPdfObject().GetIndirectReference() != null) {
                return GetPdfObject().GetIndirectReference().GetDocument();
            }
            return null;
        }

        /// <summary>Flushes page and it's content stream.</summary>
        /// <remarks>
        /// Flushes page and it's content stream.
        /// <br />
        /// <br />
        /// If the page belongs to the document which is tagged, page flushing also triggers flushing of the tags,
        /// which are considered to belong to the page. The logic that defines if the given tag (structure element) belongs
        /// to the page is the following: if all the marked content references (dictionary or number references), that are the
        /// descenders of the given structure element, belong to the current page - the tag is considered
        /// to belong to the page. If tag has descenders from several pages - it is flushed, if all other pages except the
        /// current one are flushed.
        /// </remarks>
        public override void Flush() {
            Flush(false);
        }

        /// <summary>Flushes page and its content stream.</summary>
        /// <remarks>
        /// Flushes page and its content stream. If <code>flushContentStreams</code> is true, all content streams that are
        /// rendered on this page (like FormXObjects, annotation appearance streams, patterns) and also all images associated
        /// with this page will also be flushed.
        /// <br />
        /// For notes about tag structure flushing see
        /// <see cref="Flush()">PdfPage#flush() method</see>
        /// .
        /// <br />
        /// <br />
        /// If <code>PdfADocument</code> is used, flushing will be applied only if <code>flushContentStreams</code> is true.
        /// </remarks>
        /// <param name="flushContentStreams">
        /// if true all content streams that are rendered on this page (like form xObjects,
        /// annotation appearance streams, patterns) and also all images associated with this page
        /// will be flushed.
        /// </param>
        public virtual void Flush(bool flushContentStreams) {
            // TODO log warning in case of failed flush in pdfa document case
            if (IsFlushed()) {
                return;
            }
            GetDocument().DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.END_PAGE, this));
            if (GetDocument().IsTagged() && !GetDocument().GetStructTreeRoot().IsFlushed()) {
                TryFlushPageTags();
            }
            if (flushContentStreams) {
                GetDocument().CheckIsoConformance(this, IsoKey.PAGE);
                FlushContentStreams();
            }
            int contentStreamCount = GetContentStreamCount();
            for (int i = 0; i < contentStreamCount; i++) {
                GetContentStream(i).Flush(false);
            }
            if (resources != null) {
                if (resources.IsReadOnly() && !resources.IsModified()) {
                    GetPdfObject().Remove(PdfName.Resources);
                }
            }
            resources = null;
            base.Flush();
        }

        public virtual Rectangle GetMediaBox() {
            InitParentPages();
            PdfArray mediaBox = GetPdfObject().GetAsArray(PdfName.MediaBox);
            if (mediaBox == null) {
                mediaBox = (PdfArray)GetParentValue(parentPages, PdfName.MediaBox);
            }
            if (mediaBox == null) {
                throw new PdfException(PdfException.CannotRetrieveMediaBoxAttribute);
            }
            if (mediaBox.Size() != 4) {
                throw new PdfException(PdfException.WrongMediaBoxSize).SetMessageParams(mediaBox.Size());
            }
            PdfNumber llx = mediaBox.GetAsNumber(0);
            PdfNumber lly = mediaBox.GetAsNumber(1);
            PdfNumber urx = mediaBox.GetAsNumber(2);
            PdfNumber ury = mediaBox.GetAsNumber(3);
            if (llx == null || lly == null || urx == null || ury == null) {
                throw new PdfException(PdfException.InvalidMediaBoxValue);
            }
            return new Rectangle(Math.Min(llx.FloatValue(), urx.FloatValue()), Math.Min(lly.FloatValue(), ury.FloatValue
                ()), Math.Abs(urx.FloatValue() - llx.FloatValue()), Math.Abs(ury.FloatValue() - lly.FloatValue()));
        }

        public virtual iText.Kernel.Pdf.PdfPage SetMediaBox(Rectangle rectangle) {
            GetPdfObject().Put(PdfName.MediaBox, new PdfArray(rectangle));
            return this;
        }

        public virtual Rectangle GetCropBox() {
            InitParentPages();
            PdfArray cropBox = GetPdfObject().GetAsArray(PdfName.CropBox);
            if (cropBox == null) {
                cropBox = (PdfArray)GetParentValue(parentPages, PdfName.CropBox);
                if (cropBox == null) {
                    return GetMediaBox();
                }
            }
            return cropBox.ToRectangle();
        }

        public virtual iText.Kernel.Pdf.PdfPage SetCropBox(Rectangle rectangle) {
            GetPdfObject().Put(PdfName.CropBox, new PdfArray(rectangle));
            return this;
        }

        public virtual iText.Kernel.Pdf.PdfPage SetArtBox(Rectangle rectangle) {
            if (GetPdfObject().GetAsRectangle(PdfName.TrimBox) != null) {
                GetPdfObject().Remove(PdfName.TrimBox);
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.PdfPage));
                logger.Warn(LogMessageConstant.ONLY_ONE_OF_ARTBOX_OR_TRIMBOX_CAN_EXIST_IN_THE_PAGE);
            }
            GetPdfObject().Put(PdfName.ArtBox, new PdfArray(rectangle));
            return this;
        }

        public virtual Rectangle GetArtBox() {
            return GetPdfObject().GetAsRectangle(PdfName.ArtBox);
        }

        public virtual iText.Kernel.Pdf.PdfPage SetTrimBox(Rectangle rectangle) {
            if (GetPdfObject().GetAsRectangle(PdfName.ArtBox) != null) {
                GetPdfObject().Remove(PdfName.ArtBox);
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.PdfPage));
                logger.Warn(LogMessageConstant.ONLY_ONE_OF_ARTBOX_OR_TRIMBOX_CAN_EXIST_IN_THE_PAGE);
            }
            GetPdfObject().Put(PdfName.TrimBox, new PdfArray(rectangle));
            return this;
        }

        public virtual Rectangle GetTrimBox() {
            return GetPdfObject().GetAsRectangle(PdfName.TrimBox);
        }

        /// <summary>Get decoded bytes for the whole page content.</summary>
        /// <returns>byte array.</returns>
        /// <exception cref="iText.Kernel.PdfException">in case any @see IOException.</exception>
        public virtual byte[] GetContentBytes() {
            try {
                MemoryStream baos = new MemoryStream();
                int streamCount = GetContentStreamCount();
                byte[] streamBytes;
                for (int i = 0; i < streamCount; i++) {
                    streamBytes = GetStreamBytes(i);
                    baos.Write(streamBytes);
                    if (0 != streamBytes.Length && !char.IsWhiteSpace((char)streamBytes[streamBytes.Length - 1])) {
                        baos.Write('\n');
                    }
                }
                return baos.ToArray();
            }
            catch (System.IO.IOException ioe) {
                throw new PdfException(PdfException.CannotGetContentBytes, ioe, this);
            }
        }

        /// <summary>Gets decoded bytes of a certain stream of a page content.</summary>
        /// <param name="index">index of stream inside Content.</param>
        /// <returns>byte array.</returns>
        /// <exception cref="iText.Kernel.PdfException">in case any @see IOException.</exception>
        public virtual byte[] GetStreamBytes(int index) {
            return GetContentStream(index).GetBytes();
        }

        /// <summary>Calculates and returns next available MCID reference.</summary>
        /// <returns>calculated MCID reference.</returns>
        /// <exception cref="iText.Kernel.PdfException"/>
        public virtual int GetNextMcid() {
            if (!GetDocument().IsTagged()) {
                throw new PdfException(PdfException.MustBeATaggedDocument);
            }
            if (mcid == -1) {
                PdfStructTreeRoot structTreeRoot = GetDocument().GetStructTreeRoot();
                mcid = structTreeRoot.GetNextMcidForPage(this);
            }
            return mcid++;
        }

        public virtual int? GetStructParentIndex() {
            if (structParents == -1) {
                PdfNumber n = GetPdfObject().GetAsNumber(PdfName.StructParents);
                if (n != null) {
                    structParents = n.IntValue();
                }
                else {
                    structParents = (int)GetDocument().GetNextStructParentIndex();
                }
            }
            return structParents;
        }

        public virtual iText.Kernel.Pdf.PdfPage SetAdditionalAction(PdfName key, PdfAction action) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        public virtual IList<PdfAnnotation> GetAnnotations() {
            IList<PdfAnnotation> annotations = new List<PdfAnnotation>();
            PdfArray annots = GetPdfObject().GetAsArray(PdfName.Annots);
            if (annots != null) {
                for (int i = 0; i < annots.Size(); i++) {
                    PdfDictionary annot = annots.GetAsDictionary(i);
                    annotations.Add(PdfAnnotation.MakeAnnotation(annot).SetPage(this));
                }
            }
            return annotations;
        }

        public virtual bool ContainsAnnotation(PdfAnnotation annotation) {
            foreach (PdfAnnotation a in GetAnnotations()) {
                if (a.GetPdfObject().Equals(annotation.GetPdfObject())) {
                    return true;
                }
            }
            return false;
        }

        public virtual iText.Kernel.Pdf.PdfPage AddAnnotation(PdfAnnotation annotation) {
            return AddAnnotation(-1, annotation, true);
        }

        public virtual iText.Kernel.Pdf.PdfPage AddAnnotation(int index, PdfAnnotation annotation, bool tagAnnotation
            ) {
            if (GetDocument().IsTagged() && tagAnnotation) {
                TagTreePointer tagPointer = GetDocument().GetTagStructureContext().GetAutoTaggingPointer();
                iText.Kernel.Pdf.PdfPage prevPage = tagPointer.GetCurrentPage();
                // TODO what about if current tagging stream is set
                tagPointer.SetPageForTagging(this).AddAnnotationTag(annotation);
                if (prevPage != null) {
                    tagPointer.SetPageForTagging(prevPage);
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
                SetModified();
            }
            return this;
        }

        /// <summary>Removes an annotation from the page.</summary>
        /// <remarks>
        /// Removes an annotation from the page.
        /// <br /><br />
        /// NOTE: If document is tagged, PdfDocument's PdfTagStructure instance will point at annotation tag parent after method call.
        /// </remarks>
        /// <param name="annotation">an annotation to be removed.</param>
        /// <returns>this PdfPage instance.</returns>
        public virtual iText.Kernel.Pdf.PdfPage RemoveAnnotation(PdfAnnotation annotation) {
            PdfArray annots = GetAnnots(false);
            if (annots != null) {
                if (annots.Contains(annotation.GetPdfObject())) {
                    annots.Remove(annotation.GetPdfObject());
                }
                else {
                    annots.Remove(annotation.GetPdfObject().GetIndirectReference());
                }
                if (annots.IsEmpty()) {
                    GetPdfObject().Remove(PdfName.Annots);
                }
            }
            if (GetDocument().IsTagged()) {
                TagTreePointer tagPointer = GetDocument().GetTagStructureContext().RemoveAnnotationTag(annotation);
                if (tagPointer != null) {
                    bool standardAnnotTagRole = tagPointer.GetRole().Equals(PdfName.Annot) || tagPointer.GetRole().Equals(PdfName
                        .Form);
                    if (tagPointer.GetKidsRoles().Count == 0 && standardAnnotTagRole) {
                        tagPointer.RemoveTag();
                    }
                }
            }
            return this;
        }

        public virtual int GetAnnotsSize() {
            PdfArray annots = GetAnnots(false);
            if (annots == null) {
                return 0;
            }
            return annots.Size();
        }

        /// <summary>This method gets outlines of a current page</summary>
        /// <param name="updateOutlines"/>
        /// <returns>return all outlines of a current page</returns>
        /// <exception cref="iText.Kernel.PdfException"/>
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
        /// <see langword="false"/>
        /// .
        /// </remarks>
        /// <param name="ignorePageRotationForContent">- true to ignore rotation of the new content on the rotated page.
        ///     </param>
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
        /// <returns/>
        public virtual iText.Kernel.Pdf.PdfPage SetPageLabel(PageLabelNumberingStyleConstants numberingStyle, String
             labelPrefix) {
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
        /// <returns/>
        public virtual iText.Kernel.Pdf.PdfPage SetPageLabel(PageLabelNumberingStyleConstants numberingStyle, String
             labelPrefix, int firstPage) {
            if (firstPage < 1) {
                throw new PdfException(PdfException.InAPageLabelThePageNumbersMustBeGreaterOrEqualTo1);
            }
            PdfDictionary pageLabel = new PdfDictionary();
            if (numberingStyle != null) {
                switch (numberingStyle) {
                    case PageLabelNumberingStyleConstants.DECIMAL_ARABIC_NUMERALS: {
                        pageLabel.Put(PdfName.S, PdfName.D);
                        break;
                    }

                    case PageLabelNumberingStyleConstants.UPPERCASE_ROMAN_NUMERALS: {
                        pageLabel.Put(PdfName.S, PdfName.R);
                        break;
                    }

                    case PageLabelNumberingStyleConstants.LOWERCASE_ROMAN_NUMERALS: {
                        pageLabel.Put(PdfName.S, PdfName.r);
                        break;
                    }

                    case PageLabelNumberingStyleConstants.UPPERCASE_LETTERS: {
                        pageLabel.Put(PdfName.S, PdfName.A);
                        break;
                    }

                    case PageLabelNumberingStyleConstants.LOWERCASE_LETTERS: {
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

        public virtual iText.Kernel.Pdf.PdfPage Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        /// <summary>
        /// This flag is meaningful for the case, when page rotation is applied and ignorePageRotationForContent
        /// is set to true.
        /// </summary>
        /// <remarks>
        /// This flag is meaningful for the case, when page rotation is applied and ignorePageRotationForContent
        /// is set to true. NOTE: It is needed for the internal usage.
        /// <br/><br/>
        /// This flag defines if inverse matrix (which rotates content into the opposite direction from page rotation
        /// direction in order to give the impression of the not rotated text) is already applied to the page content stream.
        /// See
        /// <see cref="SetIgnorePageRotationForContent(bool)"/>
        /// </remarks>
        /// <returns>true, if inverse matrix is already applied, false otherwise.</returns>
        public virtual bool IsPageRotationInverseMatrixWritten() {
            return pageRotationInverseMatrixWritten;
        }

        /// <summary>NOTE: For internal usage! Use this method only if you know what you are doing.</summary>
        /// <remarks>
        /// NOTE: For internal usage! Use this method only if you know what you are doing.
        /// <br/><br/>
        /// This method is called when inverse matrix (which rotates content into the opposite direction from page rotation
        /// direction in order to give the impression of the not rotated text) is applied to the page content stream.
        /// See
        /// <see cref="SetIgnorePageRotationForContent(bool)"/>
        /// </remarks>
        public virtual void SetPageRotationInverseMatrixWritten() {
            // this method specifically return void to discourage it's unintended usage
            pageRotationInverseMatrixWritten = true;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private PdfArray GetAnnots(bool create) {
            PdfArray annots = GetPdfObject().GetAsArray(PdfName.Annots);
            if (annots == null && create) {
                annots = new PdfArray();
                Put(PdfName.Annots, annots);
                SetModified();
            }
            return annots;
        }

        private PdfObject GetParentValue(PdfPages parentPages, PdfName pdfName) {
            if (parentPages != null) {
                PdfDictionary parentDictionary = parentPages.GetPdfObject();
                PdfObject value = parentDictionary.Get(pdfName);
                if (value != null) {
                    return value;
                }
                else {
                    return GetParentValue(parentPages.GetParent(), pdfName);
                }
            }
            return null;
        }

        private PdfStream NewContentStream(bool before) {
            PdfObject contents = GetPdfObject().Get(PdfName.Contents);
            PdfArray array;
            if (contents is PdfStream) {
                array = new PdfArray();
                array.Add(contents);
                GetPdfObject().Put(PdfName.Contents, array);
                SetModified();
            }
            else {
                if (contents is PdfArray) {
                    array = (PdfArray)contents;
                }
                else {
                    throw new PdfException(PdfException.PdfPageShallHaveContent);
                }
            }
            PdfStream contentStream = ((PdfStream)new PdfStream().MakeIndirect(GetDocument()));
            if (before) {
                array.Add(0, contentStream);
            }
            else {
                array.Add(contentStream);
            }
            if (null != array.GetIndirectReference()) {
                array.SetModified();
            }
            else {
                SetModified();
            }
            return contentStream;
        }

        private void TryFlushPageTags() {
            try {
                GetDocument().GetTagStructureContext().FlushPageTags(this);
                GetDocument().GetStructTreeRoot().CreateParentTreeEntryForPage(this);
            }
            catch (Exception ex) {
                throw new PdfException(PdfException.TagStructureFlushingFailedItMightBeCorrupted, ex);
            }
        }

        private void FlushContentStreams() {
            FlushContentStreams(GetResources().GetPdfObject());
            PdfArray annots = GetAnnots(false);
            if (annots != null) {
                for (int i = 0; i < annots.Size(); ++i) {
                    PdfDictionary apDict = annots.GetAsDictionary(i).GetAsDictionary(PdfName.AP);
                    if (apDict != null) {
                        FlushAppearanceStreams(apDict);
                    }
                }
            }
        }

        private void FlushContentStreams(PdfDictionary resources) {
            if (resources != null) {
                FlushWithResources(resources.GetAsDictionary(PdfName.XObject));
                FlushWithResources(resources.GetAsDictionary(PdfName.Pattern));
                FlushWithResources(resources.GetAsDictionary(PdfName.Shading));
            }
        }

        private void FlushWithResources(PdfDictionary objsCollection) {
            if (objsCollection == null) {
                return;
            }
            foreach (PdfObject obj in objsCollection.DirectValues()) {
                if (obj.IsFlushed()) {
                    continue;
                }
                FlushContentStreams(((PdfDictionary)obj).GetAsDictionary(PdfName.Resources));
                FlushMustBeIndirectObject(obj);
            }
        }

        private void FlushAppearanceStreams(PdfDictionary appearanceStreamsDict) {
            foreach (PdfObject val in appearanceStreamsDict.DirectValues()) {
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

        /*
        * initialization <code>parentPages</code> if needed
        */
        private void InitParentPages() {
            if (this.parentPages == null) {
                this.parentPages = GetDocument().GetCatalog().GetPageTree().FindPageParent(this);
            }
        }

        private void CopyInheritedProperties(iText.Kernel.Pdf.PdfPage copyPdfPage, PdfDocument pdfDocument) {
            if (copyPdfPage.GetPdfObject().Get(PdfName.Resources) == null) {
                PdfObject copyResource = pdfDocument.GetWriter().CopyObject(GetResources().GetPdfObject(), pdfDocument, false
                    );
                copyPdfPage.GetPdfObject().Put(PdfName.Resources, copyResource);
            }
            if (copyPdfPage.GetPdfObject().Get(PdfName.MediaBox) == null) {
                copyPdfPage.SetMediaBox(GetMediaBox());
            }
            if (copyPdfPage.GetPdfObject().Get(PdfName.CropBox) == null) {
                InitParentPages();
                PdfArray cropBox = (PdfArray)GetParentValue(parentPages, PdfName.CropBox);
                if (cropBox != null) {
                    copyPdfPage.SetCropBox(cropBox.ToRectangle());
                }
            }
        }
    }
}
