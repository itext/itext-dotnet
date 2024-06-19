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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// <see cref="TagTreePointer"/>
    /// class is used to modify the document's tag tree.
    /// </summary>
    /// <remarks>
    /// <see cref="TagTreePointer"/>
    /// class is used to modify the document's tag tree. At any given moment, instance of this class
    /// 'points' at the specific position in the tree (at the specific tag), however every instance can be freely moved around
    /// the tree primarily using
    /// <see cref="MoveToKid(int)"/>
    /// and
    /// <see cref="MoveToParent()"/>
    /// methods. For the current tag you can add new tags,
    /// modify it's role and properties, etc. Also, using instance of this class, you can change tag position in the tag structure,
    /// you can flush current tag or remove it.
    /// <para />
    /// There could be any number of the instances of this class, simultaneously pointing to different (or the same) parts of
    /// the tag structure. Because of this, you can for example remove the tag at which another instance is currently pointing.
    /// In this case, this another instance becomes invalid, and invocation of any method on it will result in exception. To make
    /// given instance valid again, use
    /// <see cref="MoveToRoot()"/>
    /// method.
    /// </remarks>
    public class TagTreePointer {
        private const String MCR_MARKER = "MCR";

        private TagStructureContext tagStructureContext;

        private PdfStructElem currentStructElem;

        private PdfPage currentPage;

        private PdfStream contentStream;

        private PdfNamespace currentNamespace;

        // '-1' value of this field means that next new kid will be the last element in the kids array
        private int nextNewKidIndex = -1;

        /// <summary>
        /// Creates
        /// <c>TagTreePointer</c>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <c>TagTreePointer</c>
        /// instance. After creation
        /// <c>TagTreePointer</c>
        /// points at the root tag.
        /// <para />
        /// The
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// for the new tags, which don't explicitly define namespace by the means of
        /// <see cref="DefaultAccessibilityProperties.SetNamespace(iText.Kernel.Pdf.Tagging.PdfNamespace)"/>
        /// , is set to the value returned by
        /// <see cref="TagStructureContext.GetDocumentDefaultNamespace()"/>
        /// on
        /// <see cref="TagTreePointer"/>
        /// creation.
        /// See also
        /// <see cref="SetNamespaceForNewTags(iText.Kernel.Pdf.Tagging.PdfNamespace)"/>.
        /// </remarks>
        /// <param name="document">the document, at which tag structure this instance will point.</param>
        public TagTreePointer(PdfDocument document) {
            tagStructureContext = document.GetTagStructureContext();
            SetCurrentStructElem(tagStructureContext.GetRootTag());
            SetNamespaceForNewTags(tagStructureContext.GetDocumentDefaultNamespace());
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
            this.currentNamespace = tagPointer.currentNamespace;
        }

//\cond DO_NOT_DOCUMENT
        internal TagTreePointer(PdfStructElem structElem, PdfDocument document) {
            tagStructureContext = document.GetTagStructureContext();
            SetCurrentStructElem(structElem);
        }
//\endcond

        /// <summary>
        /// Sets a page which content will be tagged with this instance of
        /// <c>TagTreePointer</c>.
        /// </summary>
        /// <remarks>
        /// Sets a page which content will be tagged with this instance of
        /// <c>TagTreePointer</c>.
        /// To tag page content:
        /// <list type="number">
        /// <item><description>Set pointer position to the tag which will be the parent of the page content item;
        /// </description></item>
        /// <item><description>Call
        /// <see cref="GetTagReference()"/>
        /// to obtain the reference to the current tag;
        /// </description></item>
        /// <item><description>Pass
        /// <see cref="TagReference"/>
        /// to the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas.OpenTag(TagReference)"/>
        /// method of the page's
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// to start marked content item;
        /// </description></item>
        /// <item><description>Draw content on
        /// <c>PdfCanvas</c>
        /// ;
        /// </description></item>
        /// <item><description>Use
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas.CloseTag()"/>
        /// to finish marked content item.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="page">
        /// the page which content will be tagged with this instance of
        /// <c>TagTreePointer</c>.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetPageForTagging(PdfPage page) {
            if (page.IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.PAGE_ALREADY_FLUSHED);
            }
            this.currentPage = page;
            return this;
        }

        /// <returns>
        /// a page which content will be tagged with this instance of
        /// <c>TagTreePointer</c>.
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
        /// if content stream tagging is finished
        /// </param>
        /// <returns>
        /// current
        /// <see cref="TagTreePointer"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetContentStreamForTagging(PdfStream contentStream
            ) {
            this.contentStream = contentStream;
            return this;
        }

        /// <returns>
        /// the content stream which content will be tagged with this instance of
        /// <c>TagTreePointer</c>.
        /// </returns>
        public virtual PdfStream GetCurrentContentStream() {
            return contentStream;
        }

        /// <returns>
        /// the
        /// <see cref="TagStructureContext"/>
        /// associated with the document to which this pointer belongs.
        /// </returns>
        public virtual TagStructureContext GetContext() {
            return tagStructureContext;
        }

        /// <returns>the document, at which tag structure this instance points.</returns>
        public virtual PdfDocument GetDocument() {
            return tagStructureContext.GetDocument();
        }

        /// <summary>
        /// Sets a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which will be set to every new tag created by this
        /// <see cref="TagTreePointer"/>
        /// instance
        /// if this tag doesn't explicitly define namespace by the means of
        /// <see cref="DefaultAccessibilityProperties.SetNamespace(iText.Kernel.Pdf.Tagging.PdfNamespace)"/>.
        /// </summary>
        /// <remarks>
        /// Sets a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which will be set to every new tag created by this
        /// <see cref="TagTreePointer"/>
        /// instance
        /// if this tag doesn't explicitly define namespace by the means of
        /// <see cref="DefaultAccessibilityProperties.SetNamespace(iText.Kernel.Pdf.Tagging.PdfNamespace)"/>.
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// <para />
        /// It's highly recommended to acquire
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// class instances via
        /// <see cref="TagStructureContext.FetchNamespace(System.String)"/>.
        /// </remarks>
        /// <param name="namespace">
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// to be set for the new tags created. If set to null - new tags will have
        /// a namespace set only if it is defined in the corresponding
        /// <see cref="AccessibilityProperties"/>.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        /// <seealso cref="TagStructureContext.FetchNamespace(System.String)"/>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetNamespaceForNewTags(PdfNamespace @namespace) {
            this.currentNamespace = @namespace;
            return this;
        }

        /// <summary>
        /// Gets a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which will be set to every new tag created by this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which is to be set for the new tags created, or null if one is not defined.
        /// </returns>
        /// <seealso cref="SetNamespaceForNewTags(iText.Kernel.Pdf.Tagging.PdfNamespace)"/>
        public virtual PdfNamespace GetNamespaceForNewTags() {
            return this.currentNamespace;
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
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(String role) {
            AddTag(-1, role);
            return this;
        }

        /// <summary>Adds a new tag with given role to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag with given role to the tag structure.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the added kid.
        /// <br />
        /// This call is equivalent of calling sequentially
        /// <see cref="SetNextNewKidIndex(int)"/>
        /// and
        /// <see cref="AddTag(System.String)"/>.
        /// </remarks>
        /// <param name="index">zero-based index in kids array of parent tag at which new tag will be added.</param>
        /// <param name="role">role of the new tag.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(int index, String role) {
            tagStructureContext.ThrowExceptionIfRoleIsInvalid(role, currentNamespace);
            SetNextNewKidIndex(index);
            SetCurrentStructElem(AddNewKid(role));
            return this;
        }

        /// <summary>Adds a new tag to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag to the tag structure.
        /// This method call moves this
        /// <see cref="TagTreePointer"/>
        /// to the added kid.
        /// <br />
        /// New tag will have a role and attributes defined by the given
        /// <see cref="AccessibilityProperties"/>.
        /// </remarks>
        /// <param name="properties">accessibility properties which define a new tag role and other properties.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(AccessibilityProperties properties) {
            AddTag(-1, properties);
            return this;
        }

        /// <summary>Adds a new tag to the tag structure.</summary>
        /// <remarks>
        /// Adds a new tag to the tag structure.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the added kid.
        /// <br />
        /// New tag will have a role and attributes defined by the given
        /// <see cref="AccessibilityProperties"/>.
        /// This call is equivalent of calling sequentially
        /// <see cref="SetNextNewKidIndex(int)"/>
        /// and
        /// <see cref="AddTag(AccessibilityProperties)"/>.
        /// </remarks>
        /// <param name="index">zero-based index in kids array of parent tag at which new tag will be added.</param>
        /// <param name="properties">accessibility properties which define a new tag role and other properties.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer AddTag(int index, AccessibilityProperties properties
            ) {
            tagStructureContext.ThrowExceptionIfRoleIsInvalid(properties, currentNamespace);
            SetNextNewKidIndex(index);
            SetCurrentStructElem(AddNewKid(properties));
            return this;
        }

        /// <summary>
        /// Adds a new content item for the given
        /// <c>PdfAnnotation</c>
        /// under the current tag.
        /// </summary>
        /// <remarks>
        /// Adds a new content item for the given
        /// <c>PdfAnnotation</c>
        /// under the current tag.
        /// <br /><br />
        /// By default, when annotation is added to the page it is automatically tagged with auto tagging pointer
        /// (see
        /// <see cref="TagStructureContext.GetAutoTaggingPointer()"/>
        /// ). If you want to add annotation tag manually, be sure to use
        /// <see cref="iText.Kernel.Pdf.PdfPage.AddAnnotation(int, iText.Kernel.Pdf.Annot.PdfAnnotation, bool)"/>
        /// method with <i>false</i> for boolean flag.
        /// </remarks>
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
            // Sometimes the merged field is split into a form field and an annotation, so we should add the annotation
            // instead of the merged field in the tag structure. So the annotation already contains the merged field's
            // StructParent in its dictionary, which we need to take into account. Otherwise, getNextStructParentIndex()
            // will increment the structParentIndex counter and the annotation will be added to the end, but the merged
            // field's StructParent index will disappear from the number tree of the tag structure.
            PdfNumber structParentIndex = annotation.GetPdfObject().GetAsNumber(PdfName.StructParent);
            PdfObjRef kid = new PdfObjRef(annotation, GetCurrentStructElem(), structParentIndex != null ? structParentIndex
                .IntValue() : GetDocument().GetNextStructParentIndex());
            if (!EnsureElementPageEqualsKidPage(GetCurrentStructElem(), currentPage.GetPdfObject())) {
                // Explicitly using object indirect reference here in order to correctly process released objects.
                ((PdfDictionary)kid.GetPdfObject()).Put(PdfName.Pg, currentPage.GetPdfObject().GetIndirectReference());
            }
            AddNewKid(kid);
            return this;
        }

        /// <summary>Sets index of the next added to the current tag kid, which could be another tag or content item.</summary>
        /// <remarks>
        /// Sets index of the next added to the current tag kid, which could be another tag or content item.
        /// By default, new tag is added at the end of the parent kids array. This property affects only the next added tag,
        /// all tags added after will be added with the default behaviour.
        /// <br /><br />
        /// This method could be used with any overload of
        /// <see cref="AddTag(System.String)"/>
        /// method,
        /// with
        /// <see cref="RelocateKid(int, TagTreePointer)"/>
        /// and
        /// <see cref="AddAnnotationTag(iText.Kernel.Pdf.Annot.PdfAnnotation)"/>.
        /// <br />
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

        /// <summary>Removes the current tag.</summary>
        /// <remarks>
        /// Removes the current tag. If it has kids, they will become kids of the current tag parent.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the current tag parent.
        /// <br /><br />
        /// You cannot remove root tag, and also you cannot remove the tag if it's parent is already flushed;
        /// in this two cases an exception will be thrown.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer RemoveTag() {
            PdfStructElem currentStructElem = GetCurrentStructElem();
            IStructureNode parentElem = currentStructElem.GetParent();
            if (parentElem is PdfStructTreeRoot) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_REMOVE_DOCUMENT_ROOT_TAG);
            }
            IList<IStructureNode> kids = currentStructElem.GetKids();
            PdfStructElem parent = (PdfStructElem)parentElem;
            if (parent.IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_REMOVE_TAG_BECAUSE_ITS_PARENT_IS_FLUSHED);
            }
            // remove waiting tag state if tag is removed
            Object objForStructDict = tagStructureContext.GetWaitingTagsManager().GetObjForStructDict(currentStructElem
                .GetPdfObject());
            tagStructureContext.GetWaitingTagsManager().RemoveWaitingState(objForStructDict);
            int removedKidIndex = parent.RemoveKid(currentStructElem);
            PdfIndirectReference indRef = currentStructElem.GetPdfObject().GetIndirectReference();
            if (indRef != null) {
                // TODO DEVSIX-5472 need to clean references to structure element from
                //  other structure elements /Ref entries and structure destinations
                indRef.SetFree();
            }
            foreach (IStructureNode kid in kids) {
                if (kid is PdfStructElem) {
                    parent.AddKid(removedKidIndex++, (PdfStructElem)kid);
                }
                else {
                    PdfMcr mcr = PrepareMcrForMovingToNewParent((PdfMcr)kid, parent);
                    parent.AddKid(removedKidIndex++, mcr);
                }
            }
            currentStructElem.GetPdfObject().Clear();
            SetCurrentStructElem(parent);
            return this;
        }

        /// <summary>
        /// Moves kid of the current tag to the tag at which given
        /// <c>TagTreePointer</c>
        /// points.
        /// </summary>
        /// <remarks>
        /// Moves kid of the current tag to the tag at which given
        /// <c>TagTreePointer</c>
        /// points.
        /// This method doesn't change neither this instance nor pointerToNewParent position.
        /// </remarks>
        /// <param name="kidIndex">zero-based index of the current tag's kid to be relocated.</param>
        /// <param name="pointerToNewParent">
        /// the
        /// <c>TagTreePointer</c>
        /// which is positioned at the tag which will become kid's new parent.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer RelocateKid(int kidIndex, iText.Kernel.Pdf.Tagutils.TagTreePointer
             pointerToNewParent) {
            if (GetDocument() != pointerToNewParent.GetDocument()) {
                throw new PdfException(KernelExceptionMessageConstant.TAG_CANNOT_BE_MOVED_TO_THE_ANOTHER_DOCUMENTS_TAG_STRUCTURE
                    );
            }
            if (GetCurrentStructElem().IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_RELOCATE_TAG_WHICH_PARENT_IS_ALREADY_FLUSHED);
            }
            if (IsPointingToSameTag(pointerToNewParent)) {
                if (kidIndex == pointerToNewParent.nextNewKidIndex) {
                    return this;
                }
                else {
                    if (kidIndex < pointerToNewParent.nextNewKidIndex) {
                        pointerToNewParent.SetNextNewKidIndex(pointerToNewParent.nextNewKidIndex - 1);
                    }
                }
            }
            if (GetCurrentStructElem().GetKids()[kidIndex] == null) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_RELOCATE_TAG_WHICH_IS_ALREADY_FLUSHED);
            }
            IStructureNode removedKid = GetCurrentStructElem().RemoveKid(kidIndex, true);
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

        /// <summary>
        /// Moves current tag to the tag at which given
        /// <c>TagTreePointer</c>
        /// points.
        /// </summary>
        /// <remarks>
        /// Moves current tag to the tag at which given
        /// <c>TagTreePointer</c>
        /// points.
        /// This method doesn't change either this instance or pointerToNewParent position.
        /// </remarks>
        /// <param name="pointerToNewParent">
        /// the
        /// <c>TagTreePointer</c>
        /// which is positioned at the tag
        /// which will become current tag new parent.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer Relocate(iText.Kernel.Pdf.Tagutils.TagTreePointer 
            pointerToNewParent) {
            if (GetCurrentStructElem().GetPdfObject() == tagStructureContext.GetRootTag().GetPdfObject()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_RELOCATE_ROOT_TAG);
            }
            if (GetCurrentStructElem().IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_RELOCATE_TAG_WHICH_IS_ALREADY_FLUSHED);
            }
            int i = GetIndexInParentKidsList();
            if (i < 0) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_RELOCATE_TAG_WHICH_PARENT_IS_ALREADY_FLUSHED);
            }
            new iText.Kernel.Pdf.Tagutils.TagTreePointer(this).MoveToParent().RelocateKid(i, pointerToNewParent);
            return this;
        }

        /// <summary>Creates a reference to the current tag, which could be used to associate a content on the PdfCanvas with current tag.
        ///     </summary>
        /// <remarks>
        /// Creates a reference to the current tag, which could be used to associate a content on the PdfCanvas with current tag.
        /// See
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas.OpenTag(TagReference)"/>
        /// and
        /// <see cref="SetPageForTagging(iText.Kernel.Pdf.PdfPage)"/>.
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
        /// <see cref="SetPageForTagging(iText.Kernel.Pdf.PdfPage)"/>.
        /// </remarks>
        /// <param name="index">zero-based index in kids array of tag. These indexes define the logical order of the content on the page.
        ///     </param>
        /// <returns>the reference to the current tag.</returns>
        public virtual TagReference GetTagReference(int index) {
            return new TagReference(GetCurrentElemEnsureIndirect(), this, index);
        }

        /// <summary>
        /// Moves this
        /// <c>TagTreePointer</c>
        /// instance to the document root tag.
        /// </summary>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToRoot() {
            SetCurrentStructElem(tagStructureContext.GetRootTag());
            return this;
        }

        /// <summary>
        /// Moves this
        /// <see cref="TagTreePointer"/>
        /// instance to the parent of the current tag.
        /// </summary>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToParent() {
            if (GetCurrentStructElem().GetPdfObject() == tagStructureContext.GetRootTag().GetPdfObject()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_MOVE_TO_PARENT_CURRENT_ELEMENT_IS_ROOT);
            }
            PdfStructElem parent = (PdfStructElem)GetCurrentStructElem().GetParent();
            if (parent.IsFlushed()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.TagTreePointer));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_TO_MOVE_TO_FLUSHED_PARENT);
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
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToKid(int kidIndex) {
            IStructureNode kid = GetCurrentStructElem().GetKids()[kidIndex];
            if (kid is PdfStructElem) {
                SetCurrentStructElem((PdfStructElem)kid);
            }
            else {
                if (kid is PdfMcr) {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_MOVE_TO_MARKED_CONTENT_REFERENCE);
                }
                else {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_MOVE_TO_FLUSHED_KID);
                }
            }
            return this;
        }

        /// <summary>
        /// Moves this
        /// <see cref="TagTreePointer"/>
        /// instance to the first descendant of the current tag which has the given role.
        /// </summary>
        /// <remarks>
        /// Moves this
        /// <see cref="TagTreePointer"/>
        /// instance to the first descendant of the current tag which has the given role.
        /// If there are no direct kids of the tag with such role, further descendants are checked in BFS order.
        /// </remarks>
        /// <param name="role">
        /// role of the current tag descendant to which pointer will be moved.
        /// If there are several descendants with this role, pointer will be moved
        /// to the first kid with such role in BFS order.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToKid(String role) {
            MoveToKid(0, role);
            return this;
        }

        /// <summary>
        /// Moves this
        /// <see cref="TagTreePointer"/>
        /// instance to the nth descendant of the current tag which has the given role.
        /// </summary>
        /// <remarks>
        /// Moves this
        /// <see cref="TagTreePointer"/>
        /// instance to the nth descendant of the current tag which has the given role.
        /// If there are no direct kids of the tag with such role, further descendants are checked in BFS order.
        /// </remarks>
        /// <param name="n">
        /// if there are several descendants with the given role, pointer will be moved to the descendant
        /// which has zero-based index <em>n</em> if you count only the descendants with the given role in BFS order.
        /// </param>
        /// <param name="role">role of the current tag descendant to which pointer will be moved.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToKid(int n, String role) {
            // MCR literal could be returned in a list of kid names (see #getKidsRoles())
            if (MCR_MARKER.Equals(role)) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_MOVE_TO_MARKED_CONTENT_REFERENCE);
            }
            IList<IStructureNode> descendants = new List<IStructureNode>(GetCurrentStructElem().GetKids());
            int k = 0;
            for (int i = 0; i < descendants.Count; ++i) {
                if (descendants[i] == null || descendants[i] is PdfMcr) {
                    continue;
                }
                String descendantRole = descendants[i].GetRole().GetValue();
                if (descendantRole.Equals(role) && k++ == n) {
                    SetCurrentStructElem((PdfStructElem)descendants[i]);
                    return this;
                }
                else {
                    descendants.AddAll(descendants[i].GetKids());
                }
            }
            throw new PdfException(KernelExceptionMessageConstant.NO_KID_WITH_SUCH_ROLE);
        }

        /// <summary>Gets current tag kids roles.</summary>
        /// <remarks>
        /// Gets current tag kids roles.
        /// If certain kid is already flushed, at its position there will be a
        /// <see langword="null"/>.
        /// If kid is a content item, at it's position there will be "MCR" string literal (stands for Marked Content Reference).
        /// </remarks>
        /// <returns>current tag kids roles</returns>
        public virtual IList<String> GetKidsRoles() {
            IList<String> roles = new List<String>();
            IList<IStructureNode> kids = GetCurrentStructElem().GetKids();
            foreach (IStructureNode kid in kids) {
                if (kid == null) {
                    roles.Add(null);
                }
                else {
                    if (kid is PdfStructElem) {
                        roles.Add(kid.GetRole().GetValue());
                    }
                    else {
                        roles.Add(MCR_MARKER);
                    }
                }
            }
            return roles;
        }

        /// <summary>Flushes current tag and all it's descendants.</summary>
        /// <remarks>
        /// Flushes current tag and all it's descendants.
        /// This method call moves this
        /// <c>TagTreePointer</c>
        /// to the current tag parent.
        /// <para />
        /// If some of the descendant tags of the current tag have waiting state (see
        /// <see cref="WaitingTagsManager"/>
        /// ),
        /// then these tags are considered as not yet finished ones, and they won't be flushed immediately,
        /// but they will be flushed, when waiting state is removed.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer FlushTag() {
            if (GetCurrentStructElem().GetPdfObject() == tagStructureContext.GetRootTag().GetPdfObject()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_FLUSH_DOCUMENT_ROOT_TAG_BEFORE_DOCUMENT_IS_CLOSED
                    );
            }
            IStructureNode parent = tagStructureContext.GetWaitingTagsManager().FlushTag(GetCurrentStructElem());
            if (parent != null) {
                // parent is not flushed
                SetCurrentStructElem((PdfStructElem)parent);
            }
            else {
                SetCurrentStructElem(tagStructureContext.GetRootTag());
            }
            return this;
        }

        /// <summary>
        /// For current tag and all of it's parents consequentially checks if the following constraints apply,
        /// and flushes the tag if they do or stops if they don't:
        /// <list type="bullet">
        /// <item><description>tag is not already flushed;
        /// </description></item>
        /// <item><description>tag is not in waiting state (see
        /// <see cref="WaitingTagsManager"/>
        /// );
        /// </description></item>
        /// <item><description>tag is not the root tag;
        /// </description></item>
        /// <item><description>tag has no kids or all of the kids are either flushed themselves or
        /// (if they are a marked content reference) belong to the flushed page.
        /// </summary>
        /// <remarks>
        /// For current tag and all of it's parents consequentially checks if the following constraints apply,
        /// and flushes the tag if they do or stops if they don't:
        /// <list type="bullet">
        /// <item><description>tag is not already flushed;
        /// </description></item>
        /// <item><description>tag is not in waiting state (see
        /// <see cref="WaitingTagsManager"/>
        /// );
        /// </description></item>
        /// <item><description>tag is not the root tag;
        /// </description></item>
        /// <item><description>tag has no kids or all of the kids are either flushed themselves or
        /// (if they are a marked content reference) belong to the flushed page.
        /// </description></item>
        /// </list>
        /// It makes sense to use this method in conjunction with
        /// <see cref="TagStructureContext.FlushPageTags(iText.Kernel.Pdf.PdfPage)"/>
        /// for the tags which have just lost their waiting state and might be not flushed only because they had one.
        /// This helps to eliminate hanging (not flushed) tags when they don't have waiting state anymore.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer FlushParentsIfAllKidsFlushed() {
            GetContext().FlushParentIfBelongsToPage(GetCurrentStructElem(), null);
            return this;
        }

        /// <summary>Gets accessibility properties of the current tag.</summary>
        /// <returns>accessibility properties of the current tag.</returns>
        public virtual AccessibilityProperties GetProperties() {
            return new BackedAccessibilityProperties(this);
        }

        /// <summary>Gets current tag role.</summary>
        /// <returns>current tag role.</returns>
        public virtual String GetRole() {
            return GetCurrentStructElem().GetRole().GetValue();
        }

        /// <summary>Sets new role to the current tag.</summary>
        /// <param name="role">new role to be set.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetRole(String role) {
            GetCurrentStructElem().SetRole(PdfStructTreeRoot.ConvertRoleToPdfName(role));
            return this;
        }

        /// <summary>Defines index of the current tag in the parent's kids list.</summary>
        /// <returns>
        /// returns index of the current tag in the parent's kids list, or -1
        /// if either current tag is a root tag, parent is flushed or it wasn't possible to define index.
        /// </returns>
        public virtual int GetIndexInParentKidsList() {
            if (GetCurrentStructElem().GetPdfObject() == tagStructureContext.GetRootTag().GetPdfObject()) {
                return -1;
            }
            PdfStructElem parent = (PdfStructElem)GetCurrentStructElem().GetParent();
            if (parent.IsFlushed()) {
                return -1;
            }
            PdfObject k = parent.GetK();
            if (k == GetCurrentStructElem().GetPdfObject()) {
                return 0;
            }
            if (k.IsArray()) {
                PdfArray kidsArr = (PdfArray)k;
                return kidsArr.IndexOf(GetCurrentStructElem().GetPdfObject());
            }
            return -1;
        }

        /// <summary>
        /// Moves this
        /// <see cref="TagTreePointer"/>
        /// instance to the tag at which given
        /// <see cref="TagTreePointer"/>
        /// instance is pointing.
        /// </summary>
        /// <param name="tagTreePointer">
        /// a
        /// <see cref="TagTreePointer"/>
        /// that points at the tag which will become the current tag
        /// of this instance.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="TagTreePointer"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreePointer MoveToPointer(iText.Kernel.Pdf.Tagutils.TagTreePointer
             tagTreePointer) {
            this.currentStructElem = tagTreePointer.currentStructElem;
            return this;
        }

        /// <summary>
        /// Checks if this
        /// <see cref="TagTreePointer"/>
        /// is pointing at the same tag as the giving
        /// <see cref="TagTreePointer"/>.
        /// </summary>
        /// <param name="otherPointer">
        /// a
        /// <see cref="TagTreePointer"/>
        /// which is checked against this instance on whether they point
        /// at the same tag.
        /// </param>
        /// <returns>
        /// true if both
        /// <see cref="TagTreePointer"/>
        /// instances point at the same tag, false otherwise.
        /// </returns>
        public virtual bool IsPointingToSameTag(iText.Kernel.Pdf.Tagutils.TagTreePointer otherPointer) {
            return GetCurrentStructElem().GetPdfObject().Equals(otherPointer.GetCurrentStructElem().GetPdfObject());
        }

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.Kernel.Pdf.Tagutils.TagTreePointer SetCurrentStructElem(PdfStructElem structElem) {
            if (structElem.GetParent() == null) {
                throw new PdfException(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_SHALL_CONTAIN_PARENT_OBJECT);
            }
            currentStructElem = structElem;
            return this;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual PdfStructElem GetCurrentStructElem() {
            if (currentStructElem.IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.TAG_TREE_POINTER_IS_IN_INVALID_STATE_IT_POINTS_AT_FLUSHED_ELEMENT_USE_MOVE_TO_ROOT
                    );
            }
            PdfIndirectReference indRef = currentStructElem.GetPdfObject().GetIndirectReference();
            if (indRef != null && indRef.IsFree()) {
                // is removed
                throw new PdfException(KernelExceptionMessageConstant.TAG_TREE_POINTER_IS_IN_INVALID_STATE_IT_POINTS_AT_REMOVED_ELEMENT_USE_MOVE_TO_ROOT
                    );
            }
            return currentStructElem;
        }
//\endcond

        /// <summary>Applies properties to the current tag.</summary>
        /// <remarks>
        /// Applies properties to the current tag.
        /// <para />
        /// </remarks>
        /// <param name="properties">the properties to be applied to the current tag.</param>
        public virtual void ApplyProperties(AccessibilityProperties properties) {
            AccessibilityPropertiesToStructElem.Apply(properties, GetCurrentStructElem());
        }

        private int GetNextNewKidPosition() {
            int nextPos = nextNewKidIndex;
            nextNewKidIndex = -1;
            return nextPos;
        }

        private PdfStructElem AddNewKid(String role) {
            PdfStructElem kid = new PdfStructElem(GetDocument(), PdfStructTreeRoot.ConvertRoleToPdfName(role));
            ProcessKidNamespace(kid);
            return AddNewKid(kid);
        }

        private PdfStructElem AddNewKid(AccessibilityProperties properties) {
            PdfStructElem kid = new PdfStructElem(GetDocument(), PdfStructTreeRoot.ConvertRoleToPdfName(properties.GetRole
                ()));
            AccessibilityPropertiesToStructElem.Apply(properties, kid);
            ProcessKidNamespace(kid);
            return AddNewKid(kid);
        }

        private void ProcessKidNamespace(PdfStructElem kid) {
            PdfNamespace kidNamespace = kid.GetNamespace();
            if (currentNamespace != null && kidNamespace == null) {
                kid.SetNamespace(currentNamespace);
                kidNamespace = currentNamespace;
            }
            tagStructureContext.EnsureNamespaceRegistered(kidNamespace);
        }

        private PdfStructElem AddNewKid(PdfStructElem kid) {
            return GetCurrentElemEnsureIndirect().AddKid(GetNextNewKidPosition(), kid);
        }

        private PdfMcr AddNewKid(PdfMcr kid) {
            return GetCurrentElemEnsureIndirect().AddKid(GetNextNewKidPosition(), kid);
        }

        private PdfStructElem GetCurrentElemEnsureIndirect() {
            PdfStructElem currentStructElem = GetCurrentStructElem();
            if (currentStructElem.GetPdfObject().GetIndirectReference() == null) {
                currentStructElem.MakeIndirect(GetDocument());
            }
            return currentStructElem;
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
                    // Explicitly using object indirect reference here in order to correctly process released objects.
                    mcrDict.Put(PdfName.Pg, mcrPage.GetIndirectReference());
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
                // Explicitly using object indirect reference here in order to correctly process released objects.
                elem.Put(PdfName.Pg, kidPage.GetIndirectReference());
            }
            return kidPage.Equals(pageObject);
        }

        private bool MarkedContentNotInPageStream() {
            return contentStream != null;
        }

        private void ThrowExceptionIfCurrentPageIsNotInited() {
            if (currentPage == null) {
                throw new PdfException(KernelExceptionMessageConstant.PAGE_IS_NOT_SET_FOR_THE_PDF_TAG_STRUCTURE);
            }
        }
    }
}
