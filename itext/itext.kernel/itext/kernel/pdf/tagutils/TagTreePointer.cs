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
using iText.IO.Log;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// <c>TagTreePointer</c>
    /// class is used to modify the document's tag tree. At any given moment, instance of this class
    /// 'points' at the specific position in the tree (at the specific tag). For the current tag you can add new tags, modify
    /// it's role and properties, etc. Also, using instance of this class, you can change tag position in the tag structure,
    /// you can flush current tag or remove it.
    /// <br/><br/>
    /// <p>
    /// There could be any number of the instances of this class, simultaneously pointing to different (or the same) parts of
    /// the tag structure. Because of this, you can for example remove the tag at which another instance is currently pointing.
    /// In this case, this another instance becomes invalid, and invocation of any method on it will result in exception. To make
    /// given instance valid again, use
    /// <see cref="MoveToRoot()"/>
    /// method.
    /// </summary>
    public class TagTreePointer {
        private TagStructureContext tagStructureContext;

        private PdfStructElem currentStructElem;

        private PdfPage currentPage;

        private PdfStream contentStream;

        private int nextNewKidIndex = -1;

        /// <summary>
        /// Creates
        /// <c>TagTreePointer</c>
        /// instance. After creation
        /// <c>TagTreePointer</c>
        /// points at the root tag.
        /// </summary>
        /// <param name="document">the document, at which tag structure this instance will point.</param>
        public TagTreePointer(PdfDocument document) {
            // '-1' value of this field means that next new kid will be the last element in the kids array
            tagStructureContext = document.GetTagStructureContext();
            SetCurrentStructElem(tagStructureContext.GetRootTag());
        }

        /// <summary>A copy constructor.</summary>
        /// <param name="tagPointer">
        /// the
        /// <c>TagTreePointer</c>
        /// from which current position and page are copied.
        /// </param>
        public TagTreePointer(iText.Kernel.Pdf.Tagutils.TagTreePointer tagPointer) {
            this.tagStructureContext = tagPointer.tagStructureContext;
            SetCurrentStructElem(tagPointer.GetCurrentStructElem());
            this.currentPage = tagPointer.currentPage;
            this.contentStream = tagPointer.contentStream;
        }

        internal TagTreePointer(PdfStructElem structElem) {
            PdfDocument doc = structElem.GetPdfObject().GetIndirectReference().GetDocument();
            tagStructureContext = doc.GetTagStructureContext();
            SetCurrentStructElem(structElem);
        }

        /// <summary>
        /// Sets a page which content will be tagged with this instance of
        /// <c>TagTreePointer</c>
        /// .
        /// To tag page content:
        /// <ol>
        /// <li>Set pointer position to the tag which will be the parent of the page content item;</li>
        /// <li>Call
        /// <see cref="GetTagReference()"/>
        /// to obtain the reference to the current tag;</li>
        /// <li>Pass
        /// <see cref="TagReference"/>
        /// to the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas.OpenTag(TagReference)"/>
        /// method of the page's
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// to start marked content item;</li>
        /// <li>Draw content on
        /// <c>PdfCanvas</c>
        /// ;</li>
        /// <li>Use
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas.CloseTag()"/>
        /// to finish marked content item.</li>
        /// </ol>
        /// </summary>
        /// <param name="page">
        /// the page which content will be tagged with this instance of
        /// <c>TagTreePointer</c>
        /// .
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetPageForTagging(PdfPage page) {
            if (page.IsFlushed()) {
                throw new PdfException(PdfException.PageAlreadyFlushed);
            }
            this.currentPage = page;
            return this;
        }

        /// <returns>
        /// a page which content will be tagged with this instance of
        /// <c>TagTreePointer</c>
        /// .
        /// </returns>
        public virtual PdfPage GetCurrentPage() {
            return currentPage;
        }

        /// <summary>
        /// Sometimes, tags are desired to be connected with the content that resides not in the page's content stream,
        /// but rather in the some appearance stream or in the form xObject stream.
        /// </summary>
        /// <remarks>
        /// Sometimes, tags are desired to be connected with the content that resides not in the page's content stream,
        /// but rather in the some appearance stream or in the form xObject stream. In that case, to have a valid tag structure,
        /// one shall set not only the page, on which the content will be rendered, but also the content stream in which
        /// the tagged content will reside.
        /// <br /><br />
        /// NOTE: It's important to set a
        /// <see langword="null"/>
        /// for this value, when tagging of this stream content is finished.
        /// </remarks>
        /// <param name="contentStream">
        /// the content stream which content will be tagged with this instance of
        /// <c>TagTreePointer</c>
        /// or
        /// <see langword="null"/>
        /// if content stream tagging is finished.
        /// </param>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetContentStreamForTagging(PdfStream contentStream
            ) {
            this.contentStream = contentStream;
            return this;
        }

        /// <returns>
        /// the content stream which content will be tagged with this instance of
        /// <c>TagTreePointer</c>
        /// .
        /// </returns>
        public virtual PdfStream GetCurrentContentStream() {
            return contentStream;
        }

        /// <returns>the document, at which tag structure this instance points.</returns>
        public virtual PdfDocument GetDocument() {
            return tagStructureContext.GetDocument();
        }

        /// <summary>Adds a new tag with given role to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag with given role to the tag structure.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the added kid.
        /// </remarks>
        /// <param name="role">role of the new tag.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(PdfName role) {
            AddTag(-1, role);
            return this;
        }

        /// <summary>Adds a new tag with given role to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag with given role to the tag structure.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the added kid.
        /// <br/>
        /// This call is equivalent of calling sequentially
        /// <see cref="SetNextNewKidIndex(int)"/>
        /// and
        /// <see cref="AddTag(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        /// <param name="index">zero-based index in kids array of parent tag at which new tag will be added.</param>
        /// <param name="role">role of the new tag.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(int index, PdfName role) {
            AddTag(index, new DummyAccessibleElement(role, null));
            return this;
        }

        /// <summary>Adds a new tag to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag to the tag structure.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the added kid.
        /// <br/>
        /// New tag will have a role and attributes defined by the given IAccessibleElement.
        /// </remarks>
        /// <param name="element">accessible element which represents a new tag.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(IAccessibleElement element) {
            AddTag(element, false);
            return this;
        }

        /// <summary>Adds a new tag to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag to the tag structure.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the added kid.
        /// <br/>
        /// New tag will have a role and attributes defined by the given IAccessibleElement.
        /// <br /><br />
        /// If <i>keepConnectedToTag</i> is true then a newly created tag will retain the connection with given
        /// accessible element. See
        /// <see cref="MoveToTag(IAccessibleElement)"/>
        /// for more explanations about tag connections concept.
        /// <br/><br/>
        /// If the same accessible element is connected to the tag and is added twice to the same parent -
        /// this
        /// <c>TagTreePointer</c>
        /// instance would move to connected kid instead of creating tag twice.
        /// But if it is added to some other parent, then connection will be removed.
        /// </remarks>
        /// <param name="element">accessible element which represents a new tag.</param>
        /// <param name="keepConnectedToTag">defines if to retain the connection between accessible element and the tag.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(IAccessibleElement element, bool keepConnectedToTag
            ) {
            AddTag(-1, element, keepConnectedToTag);
            return this;
        }

        /// <summary>Adds a new tag to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag to the tag structure.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the added kid.
        /// <br/>
        /// New tag will have a role and attributes defined by the given IAccessibleElement.
        /// This call is equivalent of calling sequentially
        /// <see cref="SetNextNewKidIndex(int)"/>
        /// and
        /// <see cref="AddTag(IAccessibleElement)"/>
        /// .
        /// </remarks>
        /// <param name="index">zero-based index in kids array of parent tag at which new tag will be added.</param>
        /// <param name="element">accessible element which represents a new tag.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(int index, IAccessibleElement element) {
            AddTag(index, element, false);
            return this;
        }

        /// <summary>Adds a new tag to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag to the tag structure.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the added kid.
        /// <br/>
        /// New tag will have a role and attributes defined by the given IAccessibleElement.
        /// <br /><br />
        /// If
        /// 
        /// is true then a newly created tag will retain the connection with given
        /// accessible element. See
        /// <see cref="MoveToTag(IAccessibleElement)"/>
        /// for more explanations about tag connections concept.
        /// <br/><br/>
        /// If the same accessible element is connected to the tag and is added twice to the same parent -
        /// this
        /// <c>TagTreePointer</c>
        /// instance would move to connected kid instead of creating tag twice.
        /// But if it is added to some other parent, then connection will be removed.
        /// <p>
        /// <br/><br/>
        /// This call is equivalent of calling sequentially
        /// <see cref="SetNextNewKidIndex(int)"/>
        /// and
        /// <see cref="AddTag(IAccessibleElement, bool)"/>
        /// .
        /// </remarks>
        /// <param name="index">zero-based index in kids array of parent tag at which new tag will be added.</param>
        /// <param name="element">accessible element which represents a new tag.</param>
        /// <param name="keepConnectedToTag">defines if to retain the connection between accessible element and the tag.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(int index, IAccessibleElement element, bool
             keepConnectedToTag) {
            tagStructureContext.ThrowExceptionIfRoleIsInvalid(element.GetRole());
            if (!tagStructureContext.IsElementConnectedToTag(element)) {
                SetNextNewKidIndex(index);
                SetCurrentStructElem(AddNewKid(element, keepConnectedToTag));
            }
            else {
                PdfStructElem connectedStruct = tagStructureContext.GetStructConnectedToModel(element);
                if (connectedStruct.GetParent() != null && GetCurrentStructElem().GetPdfObject() == ((PdfStructElem)connectedStruct
                    .GetParent()).GetPdfObject()) {
                    SetCurrentStructElem(connectedStruct);
                }
                else {
                    tagStructureContext.RemoveElementConnectionToTag(element);
                    SetNextNewKidIndex(index);
                    SetCurrentStructElem(AddNewKid(element, keepConnectedToTag));
                }
            }
            return this;
        }

        /// <summary>
        /// Adds a new content item for the given
        /// <c>PdfAnnotation</c>
        /// under the current tag.
        /// <br/><br/>
        /// By default, when annotation is added to the page it is automatically tagged with auto tagging pointer
        /// (see
        /// <see cref="TagStructureContext.GetAutoTaggingPointer()"/>
        /// ). If you want to add annotation tag manually, be sure to use
        /// <see cref="iText.Kernel.Pdf.PdfPage.AddAnnotation(int, iText.Kernel.Pdf.Annot.PdfAnnotation, bool)"/>
        /// method with <i>false</i> for boolean flag.
        /// </summary>
        /// <param name="annotation">
        /// 
        /// <c>PdfAnnotation</c>
        /// to be tagged.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddAnnotationTag(PdfAnnotation annotation) {
            ThrowExceptionIfCurrentPageIsNotInited();
            PdfObjRef kid = new PdfObjRef(annotation, GetCurrentStructElem());
            if (!EnsureElementPageEqualsKidPage(GetCurrentStructElem(), currentPage.GetPdfObject())) {
                ((PdfDictionary)kid.GetPdfObject()).Put(PdfName.Pg, currentPage.GetPdfObject());
            }
            AddNewKid(kid);
            return this;
        }

        /// <summary>Sets index of the next added to the current tag kid, which could be another tag or content item.</summary>
        /// <remarks>
        /// Sets index of the next added to the current tag kid, which could be another tag or content item.
        /// By default, new tag is added at the end of the parent kids array. This property affects only the next added tag,
        /// all tags added after will be added with the default behaviour.
        /// <br/><br/>
        /// This method could be used with any overload of
        /// <see cref="AddTag(iText.Kernel.Pdf.PdfName)"/>
        /// method,
        /// with
        /// <see cref="RelocateKid(int, TagTreePointer)"/>
        /// and
        /// <see cref="AddAnnotationTag(iText.Kernel.Pdf.Annot.PdfAnnotation)"/>
        /// .
        /// <br/>
        /// Keep in mind, that this method set property to the
        /// <c>TagTreePointer</c>
        /// and not to the tag itself, which means
        /// that if you would move the pointer, this property would be applied to the new current tag.
        /// </remarks>
        /// <param name="nextNewKidIndex">index of the next added kid.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetNextNewKidIndex(int nextNewKidIndex) {
            if (nextNewKidIndex > -1) {
                this.nextNewKidIndex = nextNewKidIndex;
            }
            return this;
        }

        /// <summary>
        /// Checks if given
        /// <c>IAccessibleElement</c>
        /// is connected to some tag.
        /// See
        /// <see cref="MoveToTag(IAccessibleElement)"/>
        /// for more explanations about tag connections concept.
        /// </summary>
        /// <param name="element">element to check if it has a connected tag.</param>
        /// <returns>true, if there is a tag which retains the connection to the given accessible element.</returns>
        public virtual bool IsElementConnectedToTag(IAccessibleElement element) {
            return tagStructureContext.IsElementConnectedToTag(element);
        }

        /// <summary>Destroys the connection between the given accessible element and the tag to which this element is connected to.
        ///     </summary>
        /// <remarks>
        /// Destroys the connection between the given accessible element and the tag to which this element is connected to.
        /// See
        /// <see cref="MoveToTag(IAccessibleElement)"/>
        /// for more explanations about tag connections concept.
        /// </remarks>
        /// <param name="element">
        /// 
        /// <c>IAccessibleElement</c>
        /// which connection to the tag (if there is one) will be removed.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual TagStructureContext RemoveElementConnectionToTag(IAccessibleElement element) {
            return tagStructureContext.RemoveElementConnectionToTag(element);
        }

        /// <summary>Removes the current tag.</summary>
        /// <remarks>
        /// Removes the current tag. If it has kids, they will become kids of the current tag parent.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the current tag parent.
        /// <br/><br/>
        /// You cannot remove root tag, and also you cannot remove any tag if document's tag structure was partially flushed;
        /// in this two cases an exception will be thrown.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer RemoveTag() {
            IPdfStructElem parentElem = GetCurrentStructElem().GetParent();
            if (parentElem is PdfStructTreeRoot) {
                throw new PdfException(PdfException.CannotRemoveDocumentRootTag);
            }
            IList<IPdfStructElem> kids = GetCurrentStructElem().GetKids();
            PdfStructElem parent = (PdfStructElem)parentElem;
            if (parent.IsFlushed()) {
                throw new PdfException(PdfException.CannotRemoveTagBecauseItsParentIsFlushed);
            }
            int removedKidIndex = parent.RemoveKid(GetCurrentStructElem());
            GetCurrentStructElem().GetPdfObject().GetIndirectReference().SetFree();
            foreach (IPdfStructElem kid in kids) {
                if (kid is PdfStructElem) {
                    parent.AddKid(removedKidIndex++, (PdfStructElem)kid);
                }
                else {
                    PdfMcr mcr = PrepareMcrForMovingToNewParent((PdfMcr)kid, parent);
                    parent.AddKid(removedKidIndex++, mcr);
                }
            }
            SetCurrentStructElem(parent);
            return this;
        }

        /// <summary>
        /// Moves kid of the current tag to the tag at which given
        /// <c>TagTreePointer</c>
        /// points.
        /// This method doesn't change pointerToNewParent position.
        /// </summary>
        /// <param name="kidIndex">zero-based index of the current tag's kid to be relocated.</param>
        /// <param name="pointerToNewParent">
        /// the
        /// <c>TagTreePointer</c>
        /// which is positioned at the tag which will become kid's new parent.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer RelocateKid(int kidIndex, iText.Kernel.Pdf.Tagutils.TagTreePointer
             pointerToNewParent) {
            if (GetDocument() != pointerToNewParent.GetDocument()) {
                throw new PdfException(PdfException.TagCannotBeMovedToTheAnotherDocumentsTagStructure);
            }
            IPdfStructElem removedKid = GetCurrentStructElem().RemoveKid(kidIndex);
            if (removedKid is PdfStructElem) {
                pointerToNewParent.AddNewKid((PdfStructElem)removedKid);
            }
            else {
                if (removedKid is PdfMcr) {
                    PdfMcr mcrKid = PrepareMcrForMovingToNewParent((PdfMcr)removedKid, pointerToNewParent.GetCurrentStructElem
                        ());
                    pointerToNewParent.AddNewKid(mcrKid);
                }
            }
            return this;
        }

        /// <summary>Creates a reference to the current tag, which could be used to associate a content on the PdfCanvas with current tag.
        ///     </summary>
        /// <remarks>
        /// Creates a reference to the current tag, which could be used to associate a content on the PdfCanvas with current tag.
        /// See
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas.OpenTag(TagReference)"/>
        /// and
        /// <see cref="SetPageForTagging(iText.Kernel.Pdf.PdfPage)"/>
        /// .
        /// </remarks>
        /// <returns>the reference to the current tag.</returns>
        public virtual TagReference GetTagReference() {
            return GetTagReference(-1);
        }

        /// <summary>Creates a reference to the current tag, which could be used to associate a content on the PdfCanvas with current tag.
        ///     </summary>
        /// <remarks>
        /// Creates a reference to the current tag, which could be used to associate a content on the PdfCanvas with current tag.
        /// See
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas.OpenTag(TagReference)"/>
        /// and
        /// <see cref="SetPageForTagging(iText.Kernel.Pdf.PdfPage)"/>
        /// .
        /// </remarks>
        /// <param name="index">zero-based index in kids array of tag. These indexes define the logical order of the content on the page.
        ///     </param>
        /// <returns>the reference to the current tag.</returns>
        public virtual TagReference GetTagReference(int index) {
            return new TagReference(GetCurrentStructElem(), this, index);
        }

        /// <summary>
        /// Moves this
        /// <c>TagTreePointer</c>
        /// instance to the document root tag.
        /// </summary>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToRoot() {
            SetCurrentStructElem(tagStructureContext.GetRootTag());
            return this;
        }

        /// <summary>
        /// Moves this
        /// <c>TagTreePointer</c>
        /// instance to the parent of the current tag.
        /// </summary>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToParent() {
            if (GetCurrentStructElem().GetPdfObject() == tagStructureContext.GetRootTag().GetPdfObject()) {
                throw new PdfException(PdfException.CannotMoveToParentCurrentElementIsRoot);
            }
            IPdfStructElem parent = GetCurrentStructElem().GetParent();
            if (parent == null) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.TagTreePointer));
                logger.Warn(iText.IO.LogMessageConstant.ATTEMPT_TO_MOVE_TO_FLUSHED_PARENT);
                MoveToRoot();
            }
            else {
                SetCurrentStructElem((PdfStructElem)parent);
            }
            return this;
        }

        /// <summary>
        /// Moves this
        /// <c>TagTreePointer</c>
        /// instance to the kid of the current tag.
        /// </summary>
        /// <param name="kidIndex">zero-based index of the current tag kid to which pointer will be moved.</param>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToKid(int kidIndex) {
            IPdfStructElem kid = GetCurrentStructElem().GetKids()[kidIndex];
            if (kid is PdfStructElem) {
                SetCurrentStructElem((PdfStructElem)kid);
            }
            else {
                if (kid is PdfMcr) {
                    throw new PdfException(PdfException.CannotMoveToMarkedContentReference);
                }
                else {
                    throw new PdfException(PdfException.CannotMoveToFlushedKid);
                }
            }
            return this;
        }

        /// <summary>
        /// Moves this
        /// <c>TagTreePointer</c>
        /// instance to the kid of the current tag.
        /// </summary>
        /// <param name="role">
        /// role of the current tag kid to which pointer will be moved.
        /// If there is several kids with this role, pointer will be moved to the first kid with such role.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToKid(PdfName role) {
            MoveToKid(0, role);
            return this;
        }

        /// <summary>
        /// Moves this
        /// <c>TagTreePointer</c>
        /// instance to the kid of the current tag.
        /// </summary>
        /// <param name="n">
        /// if there is several kids with the given role, pointer will be moved to the kid
        /// which is the n'th if you count kids with such role.
        /// </param>
        /// <param name="role">role of the current tag kid to which pointer will be moved.</param>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToKid(int n, PdfName role) {
            if (PdfName.MCR.Equals(role)) {
                throw new PdfException(PdfException.CannotMoveToMarkedContentReference);
            }
            IList<IPdfStructElem> kids = GetCurrentStructElem().GetKids();
            int k = 0;
            for (int i = 0; i < kids.Count; ++i) {
                if (kids[i] == null) {
                    continue;
                }
                if (kids[i].GetRole().Equals(role) && !(kids[i] is PdfMcr) && k++ == n) {
                    MoveToKid(i);
                    return this;
                }
            }
            throw new PdfException(PdfException.NoKidWithSuchRole);
        }

        /// <summary>
        /// Moves this
        /// <c>TagTreePointer</c>
        /// instance to a tag, which is connected with the given accessible element.
        /// <p>
        /// <br/><br/>
        /// The connection between the tag and the accessible element instance is used as a sign that tag is not yet finished
        /// and therefore should not be flushed or removed if page tags are flushed or removed. Also, any
        /// <c>TagTreePointer</c>
        /// could be immediately moved to the tag with connection via it's connected element by using this method. If accessible
        /// element is connected to the tag, then all changes of the role or properties of the element will affect the connected
        /// tag role and properties.
        /// <br/>
        /// For any existing not connected tag the connection could be created using
        /// <see cref="GetConnectedElement(bool)"/>
        /// with <i>true</i> as parameter.
        /// </summary>
        /// <param name="element">an element which has a connection with some tag.</param>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToTag(IAccessibleElement element) {
            tagStructureContext.MoveTagPointerToTag(element, this);
            return this;
        }

        /// <summary>Gets current element kids roles.</summary>
        /// <remarks>
        /// Gets current element kids roles.
        /// If certain kid is already flushed, at its position there will be a
        /// <see langword="null"/>
        /// .
        /// If kid is content item, at its position there will be "MCR" (Marked Content Reference).
        /// </remarks>
        /// <returns>current element kids roles</returns>
        public virtual IList<PdfName> GetKidsRoles() {
            IList<PdfName> roles = new List<PdfName>();
            IList<IPdfStructElem> kids = GetCurrentStructElem().GetKids();
            foreach (IPdfStructElem kid in kids) {
                if (kid == null) {
                    roles.Add(null);
                }
                else {
                    if (kid is PdfStructElem) {
                        roles.Add(kid.GetRole());
                    }
                    else {
                        roles.Add(PdfName.MCR);
                    }
                }
            }
            return roles;
        }

        /// <summary>Flushes the current tag and all it's descenders.</summary>
        /// <remarks>
        /// Flushes the current tag and all it's descenders.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the current tag parent.
        /// <p>
        /// <br /><br />
        /// If some of the tags to be flushed are still connected to the accessible elements, then these tags are considered
        /// as not yet finished ones, and they won't be flushed immediately, but they will be flushed, when the connection
        /// is removed.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer FlushTag() {
            if (GetCurrentStructElem().GetPdfObject() == tagStructureContext.GetRootTag().GetPdfObject()) {
                throw new PdfException(PdfException.CannotFlushDocumentRootTagBeforeDocumentIsClosed);
            }
            IPdfStructElem parent = tagStructureContext.FlushTag(GetCurrentStructElem());
            if (parent != null) {
                // parent is not flushed
                SetCurrentStructElem((PdfStructElem)parent);
            }
            else {
                SetCurrentStructElem(tagStructureContext.GetRootTag());
            }
            return this;
        }

        /// <summary>Gets connected accessible element for the current tag.</summary>
        /// <remarks>
        /// Gets connected accessible element for the current tag. If tag is not connected to element, behaviour is defined
        /// by the createIfNotExist flag.
        /// See
        /// <see cref="MoveToTag(IAccessibleElement)"/>
        /// for more explanations about tag connections concept.
        /// </remarks>
        /// <param name="createIfNotExist">
        /// if <i>true</i>, creates an
        /// <c>IAccessibleElement</c>
        /// and connects it to the tag.
        /// </param>
        /// <returns>
        /// connected
        /// <c>IAccessibleElement</c>
        /// if there is one (or if it is created), otherwise null.
        /// </returns>
        public virtual IAccessibleElement GetConnectedElement(bool createIfNotExist) {
            IAccessibleElement element;
            element = tagStructureContext.GetModelConnectedToStruct(GetCurrentStructElem());
            if (element == null && createIfNotExist) {
                element = new DummyAccessibleElement(GetRole(), GetProperties());
                tagStructureContext.SaveConnectionBetweenStructAndModel(element, GetCurrentStructElem());
            }
            return element;
        }

        /// <summary>Gets accessibility properties of the current tag.</summary>
        /// <returns>accessibility properties of the current tag.</returns>
        public virtual AccessibilityProperties GetProperties() {
            PdfStructElem currElem = GetCurrentStructElem();
            IAccessibleElement model = tagStructureContext.GetModelConnectedToStruct(currElem);
            if (model != null) {
                return model.GetAccessibilityProperties();
            }
            else {
                return new BackedAccessibleProperties(currElem);
            }
        }

        /// <summary>Gets current tag role.</summary>
        /// <returns>current tag role.</returns>
        public virtual PdfName GetRole() {
            IAccessibleElement model = tagStructureContext.GetModelConnectedToStruct(GetCurrentStructElem());
            if (model != null) {
                return model.GetRole();
            }
            else {
                return GetCurrentStructElem().GetRole();
            }
        }

        /// <summary>Sets new role to the current tag.</summary>
        /// <param name="role">new role to be set.</param>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetRole(PdfName role) {
            IAccessibleElement connectedElement = tagStructureContext.GetModelConnectedToStruct(GetCurrentStructElem()
                );
            if (connectedElement != null) {
                connectedElement.SetRole(role);
            }
            else {
                GetCurrentStructElem().SetRole(role);
            }
            return this;
        }

        internal virtual int CreateNextMcidForStructElem(PdfStructElem elem, int index) {
            ThrowExceptionIfCurrentPageIsNotInited();
            PdfMcr mcr;
            if (!MarkedContentNotInPageStream() && EnsureElementPageEqualsKidPage(elem, currentPage.GetPdfObject())) {
                mcr = new PdfMcrNumber(currentPage, elem);
            }
            else {
                mcr = new PdfMcrDictionary(currentPage, elem);
                if (MarkedContentNotInPageStream()) {
                    ((PdfDictionary)mcr.GetPdfObject()).Put(PdfName.Stm, contentStream);
                }
            }
            elem.AddKid(index, mcr);
            return mcr.GetMcid();
        }

        internal virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetCurrentStructElem(PdfStructElem structElem) {
            currentStructElem = structElem;
            return this;
        }

        internal virtual PdfStructElem GetCurrentStructElem() {
            if (currentStructElem.IsFlushed()) {
                throw new PdfException(PdfException.TagTreePointerIsInInvalidStateItPointsAtFlushedElementUseMoveToRoot);
            }
            if (currentStructElem.GetPdfObject().GetIndirectReference() == null) {
                // is removed
                throw new PdfException(PdfException.TagTreePointerIsInInvalidStateItPointsAtRemovedElementUseMoveToRoot);
            }
            return currentStructElem;
        }

        private int GetNextNewKidPosition() {
            int nextPos = nextNewKidIndex;
            nextNewKidIndex = -1;
            return nextPos;
        }

        private PdfStructElem AddNewKid(IAccessibleElement element, bool keepConnectedToTag) {
            PdfStructElem kid = new PdfStructElem(GetDocument(), element.GetRole());
            if (keepConnectedToTag) {
                tagStructureContext.SaveConnectionBetweenStructAndModel(element, kid);
            }
            if (!keepConnectedToTag && element.GetAccessibilityProperties() != null) {
                element.GetAccessibilityProperties().SetToStructElem(kid);
            }
            return AddNewKid(kid);
        }

        private PdfStructElem AddNewKid(PdfStructElem kid) {
            return GetCurrentStructElem().AddKid(GetNextNewKidPosition(), kid);
        }

        private PdfMcr AddNewKid(PdfMcr kid) {
            return GetCurrentStructElem().AddKid(GetNextNewKidPosition(), kid);
        }

        private PdfMcr PrepareMcrForMovingToNewParent(PdfMcr mcrKid, PdfStructElem newParent) {
            PdfObject mcrObject = mcrKid.GetPdfObject();
            PdfDictionary mcrPage = mcrKid.GetPageObject();
            PdfDictionary mcrDict = null;
            if (!mcrObject.IsNumber()) {
                mcrDict = (PdfDictionary)mcrObject;
            }
            if (mcrDict == null || !mcrDict.ContainsKey(PdfName.Pg)) {
                if (!EnsureElementPageEqualsKidPage(newParent, mcrPage)) {
                    if (mcrDict == null) {
                        mcrDict = new PdfDictionary();
                        mcrDict.Put(PdfName.Type, PdfName.MCR);
                        mcrDict.Put(PdfName.MCID, mcrKid.GetPdfObject());
                    }
                    mcrDict.Put(PdfName.Pg, mcrPage);
                }
            }
            if (mcrDict != null) {
                if (PdfName.MCR.Equals(mcrDict.Get(PdfName.Type))) {
                    mcrKid = new PdfMcrDictionary(mcrDict, newParent);
                }
                else {
                    if (PdfName.OBJR.Equals(mcrDict.Get(PdfName.Type))) {
                        mcrKid = new PdfObjRef(mcrDict, newParent);
                    }
                }
            }
            else {
                mcrKid = new PdfMcrNumber((PdfNumber)mcrObject, newParent);
            }
            return mcrKid;
        }

        private bool EnsureElementPageEqualsKidPage(PdfStructElem elem, PdfDictionary kidPage) {
            PdfObject pageObject = elem.GetPdfObject().Get(PdfName.Pg);
            if (pageObject == null) {
                pageObject = kidPage;
                elem.GetPdfObject().Put(PdfName.Pg, kidPage);
                elem.SetModified();
            }
            return kidPage.Equals(pageObject);
        }

        private bool MarkedContentNotInPageStream() {
            return contentStream != null;
        }

        private void ThrowExceptionIfCurrentPageIsNotInited() {
            if (currentPage == null) {
                throw new PdfException(PdfException.PageIsNotSetForThePdfTagStructure);
            }
        }
    }
}
