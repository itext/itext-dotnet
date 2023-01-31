/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// <c>TagStructureContext</c>
    /// class is used to track necessary information of document's tag structure.
    /// </summary>
    /// <remarks>
    /// <c>TagStructureContext</c>
    /// class is used to track necessary information of document's tag structure.
    /// It is also used to make some global modifications of the tag tree like removing or flushing page tags, however
    /// these two methods and also others are called automatically and are for the most part for internal usage.
    /// <br />
    /// There shall be only one instance of this class per
    /// <c>PdfDocument</c>
    /// . To obtain instance of this class use
    /// <see cref="iText.Kernel.Pdf.PdfDocument.GetTagStructureContext()"/>.
    /// </remarks>
    public class TagStructureContext {
        private static readonly ICollection<String> ALLOWED_ROOT_TAG_ROLES;

        static TagStructureContext() {
            // HashSet is required in order to autoport correctly in .Net
            HashSet<String> tempSet = new HashSet<String>();
            tempSet.Add(StandardRoles.DOCUMENT);
            tempSet.Add(StandardRoles.PART);
            tempSet.Add(StandardRoles.ART);
            tempSet.Add(StandardRoles.SECT);
            tempSet.Add(StandardRoles.DIV);
            ALLOWED_ROOT_TAG_ROLES = JavaCollectionsUtil.UnmodifiableSet(tempSet);
        }

        private PdfDocument document;

        private PdfStructElem rootTagElement;

        protected internal TagTreePointer autoTaggingPointer;

        private PdfVersion tagStructureTargetVersion;

        private bool forbidUnknownRoles;

        private WaitingTagsManager waitingTagsManager;

        private ICollection<PdfDictionary> namespaces;

        private IDictionary<String, PdfNamespace> nameToNamespace;

        private PdfNamespace documentDefaultNamespace;

        /// <summary>
        /// Do not use this constructor, instead use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetTagStructureContext()"/>
        /// method.
        /// </summary>
        /// <remarks>
        /// Do not use this constructor, instead use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetTagStructureContext()"/>
        /// method.
        /// <br />
        /// Creates
        /// <c>TagStructureContext</c>
        /// for document. There shall be only one instance of this
        /// class per
        /// <c>PdfDocument</c>.
        /// </remarks>
        /// <param name="document">the document which tag structure will be manipulated with this class.</param>
        public TagStructureContext(PdfDocument document)
            : this(document, document.GetPdfVersion()) {
        }

        /// <summary>
        /// Do not use this constructor, instead use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetTagStructureContext()"/>
        /// method.
        /// </summary>
        /// <remarks>
        /// Do not use this constructor, instead use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetTagStructureContext()"/>
        /// method.
        /// <para />
        /// Creates
        /// <c>TagStructureContext</c>
        /// for document. There shall be only one instance of this
        /// class per
        /// <c>PdfDocument</c>.
        /// </remarks>
        /// <param name="document">the document which tag structure will be manipulated with this class.</param>
        /// <param name="tagStructureTargetVersion">the version of the pdf standard to which the tag structure shall adhere.
        ///     </param>
        public TagStructureContext(PdfDocument document, PdfVersion tagStructureTargetVersion) {
            this.document = document;
            if (!document.IsTagged()) {
                throw new PdfException(KernelExceptionMessageConstant.MUST_BE_A_TAGGED_DOCUMENT);
            }
            waitingTagsManager = new WaitingTagsManager();
            namespaces = new LinkedHashSet<PdfDictionary>();
            nameToNamespace = new Dictionary<String, PdfNamespace>();
            this.tagStructureTargetVersion = tagStructureTargetVersion;
            forbidUnknownRoles = true;
            if (TargetTagStructureVersionIs2()) {
                InitRegisteredNamespaces();
                SetNamespaceForNewTagsBasedOnExistingRoot();
            }
        }

        /// <summary>
        /// If forbidUnknownRoles is set to true, then if you would try to add new tag which has not a standard role and
        /// it's role is not mapped through RoleMap, an exception will be raised.
        /// </summary>
        /// <remarks>
        /// If forbidUnknownRoles is set to true, then if you would try to add new tag which has not a standard role and
        /// it's role is not mapped through RoleMap, an exception will be raised.
        /// Default value - true.
        /// </remarks>
        /// <param name="forbidUnknownRoles">new value of the flag</param>
        /// <returns>
        /// current
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagStructureContext SetForbidUnknownRoles(bool forbidUnknownRoles
            ) {
            this.forbidUnknownRoles = forbidUnknownRoles;
            return this;
        }

        /// <summary>Gets the version of the PDF standard to which the tag structure shall adhere.</summary>
        /// <returns>the tag structure target version</returns>
        public virtual PdfVersion GetTagStructureTargetVersion() {
            return tagStructureTargetVersion;
        }

        /// <summary>
        /// All tagging logic performed by iText automatically (along with addition of content, annotations etc)
        /// uses
        /// <see cref="TagTreePointer"/>
        /// returned by this method to manipulate the tag structure.
        /// </summary>
        /// <remarks>
        /// All tagging logic performed by iText automatically (along with addition of content, annotations etc)
        /// uses
        /// <see cref="TagTreePointer"/>
        /// returned by this method to manipulate the tag structure.
        /// Typically it points at the root tag. This pointer also could be used to tweak auto tagging process
        /// (e.g. move this pointer to the Section tag, which would result in placing all automatically tagged content
        /// under Section tag).
        /// </remarks>
        /// <returns>
        /// the
        /// <c>TagTreePointer</c>
        /// which is used for all automatic tagging of the document.
        /// </returns>
        public virtual TagTreePointer GetAutoTaggingPointer() {
            if (autoTaggingPointer == null) {
                autoTaggingPointer = new TagTreePointer(document);
            }
            return autoTaggingPointer;
        }

        /// <summary>
        /// Gets
        /// <see cref="WaitingTagsManager"/>
        /// for the current document.
        /// </summary>
        /// <remarks>
        /// Gets
        /// <see cref="WaitingTagsManager"/>
        /// for the current document. It allows to mark tags as waiting,
        /// which would indicate that they are incomplete and are not ready to be flushed.
        /// </remarks>
        /// <returns>
        /// document's
        /// <see cref="WaitingTagsManager"/>
        /// class instance.
        /// </returns>
        public virtual WaitingTagsManager GetWaitingTagsManager() {
            return waitingTagsManager;
        }

        /// <summary>
        /// A namespace that is used as a default value for the tagging for any new
        /// <see cref="TagTreePointer"/>
        /// created
        /// (including the pointer returned by
        /// <see cref="GetAutoTaggingPointer()"/>
        /// , which implies that automatically
        /// created tag structure will be in this namespace by default).
        /// </summary>
        /// <remarks>
        /// A namespace that is used as a default value for the tagging for any new
        /// <see cref="TagTreePointer"/>
        /// created
        /// (including the pointer returned by
        /// <see cref="GetAutoTaggingPointer()"/>
        /// , which implies that automatically
        /// created tag structure will be in this namespace by default).
        /// <para />
        /// By default, this value is defined based on the PDF document version and the existing tag structure inside
        /// a document. For the new empty PDF 2.0 documents this namespace is set to
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardNamespaces.PDF_2_0"/>.
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which is used as a default value for the document tagging.
        /// </returns>
        public virtual PdfNamespace GetDocumentDefaultNamespace() {
            return documentDefaultNamespace;
        }

        /// <summary>
        /// Sets a namespace that will be used as a default value for the tagging for any new
        /// <see cref="TagTreePointer"/>
        /// created.
        /// </summary>
        /// <remarks>
        /// Sets a namespace that will be used as a default value for the tagging for any new
        /// <see cref="TagTreePointer"/>
        /// created.
        /// See
        /// <see cref="GetDocumentDefaultNamespace()"/>
        /// for more info.
        /// <para />
        /// Be careful when changing this property value. It is most recommended to do it right after the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// was
        /// created, before any content was added. Changing this value after any content was added might result in the mingled
        /// tag structure from the namespaces point of view. So in order to maintain the document consistent but in the namespace
        /// different from default, set this value before any modifications to the document were made and before
        /// <see cref="GetAutoTaggingPointer()"/>
        /// method was called for the first time.
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// </remarks>
        /// <param name="namespace">
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which is to be used as a default value for the document tagging.
        /// </param>
        /// <returns>
        /// current
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagStructureContext SetDocumentDefaultNamespace(PdfNamespace @namespace
            ) {
            this.documentDefaultNamespace = @namespace;
            return this;
        }

        /// <summary>
        /// This method defines a recommended way to obtain
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// class instances.
        /// </summary>
        /// <remarks>
        /// This method defines a recommended way to obtain
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// class instances.
        /// <para />
        /// Returns either a wrapper over an already existing namespace dictionary in the document or over a new one
        /// if such namespace wasn't encountered before. Calling this method is considered as encountering a namespace,
        /// i.e. two sequential calls on this method will return the same namespace instance (which is not true in general case
        /// of two method calls, for instance if several namespace instances with the same name are created via
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// constructors and set to the elements of the tag structure, then the last encountered one
        /// will be returned by this method). However encountered namespaces will not be added to the document's structure tree root
        /// <see cref="iText.Kernel.Pdf.PdfName.Namespaces">/Namespaces</see>
        /// array unless they were set to the certain element of the tag structure.
        /// </remarks>
        /// <param name="namespaceName">
        /// a
        /// <see cref="System.String"/>
        /// defining the namespace name (conventionally a uniform resource identifier, or URI).
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// wrapper over either already existing namespace object or over the new one.
        /// </returns>
        public virtual PdfNamespace FetchNamespace(String namespaceName) {
            PdfNamespace ns = nameToNamespace.Get(namespaceName);
            if (ns == null) {
                ns = new PdfNamespace(namespaceName);
                nameToNamespace.Put(namespaceName, ns);
            }
            return ns;
        }

        /// <summary>
        /// Gets an instance of the
        /// <see cref="IRoleMappingResolver"/>
        /// corresponding to the current tag structure target version.
        /// </summary>
        /// <remarks>
        /// Gets an instance of the
        /// <see cref="IRoleMappingResolver"/>
        /// corresponding to the current tag structure target version.
        /// This method implies that role is in the default standard structure namespace.
        /// </remarks>
        /// <param name="role">a role in the default standard structure namespace which mapping is to be resolved.</param>
        /// <returns>
        /// a
        /// <see cref="IRoleMappingResolver"/>
        /// instance, with the giving role as current.
        /// </returns>
        public virtual IRoleMappingResolver GetRoleMappingResolver(String role) {
            return GetRoleMappingResolver(role, null);
        }

        /// <summary>
        /// Gets an instance of the
        /// <see cref="IRoleMappingResolver"/>
        /// corresponding to the current tag structure target version.
        /// </summary>
        /// <param name="role">a role in the given namespace which mapping is to be resolved.</param>
        /// <param name="namespace">
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which this role belongs to.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="IRoleMappingResolver"/>
        /// instance, with the giving role in the given
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// as current.
        /// </returns>
        public virtual IRoleMappingResolver GetRoleMappingResolver(String role, PdfNamespace @namespace) {
            if (TargetTagStructureVersionIs2()) {
                return new RoleMappingResolverPdf2(role, @namespace, GetDocument());
            }
            else {
                return new RoleMappingResolver(role, GetDocument());
            }
        }

        /// <summary>
        /// Checks if the given role and namespace are specified to be obligatory mapped to the standard structure namespace
        /// in order to be a valid role in the Tagged PDF.
        /// </summary>
        /// <param name="role">a role in the given namespace which mapping necessity is to be checked.</param>
        /// <param name="namespace">
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which this role belongs to, null value refers to the default standard
        /// structure namespace.
        /// </param>
        /// <returns>
        /// true, if the given role in the given namespace is either mapped to the standard structure role or doesn't
        /// have to; otherwise false.
        /// </returns>
        public virtual bool CheckIfRoleShallBeMappedToStandardRole(String role, PdfNamespace @namespace) {
            return ResolveMappingToStandardOrDomainSpecificRole(role, @namespace) != null;
        }

        /// <summary>
        /// Gets an instance of the
        /// <see cref="IRoleMappingResolver"/>
        /// which is already in the "resolved" state: it returns
        /// role in the standard or domain-specific namespace for the
        /// <see cref="IRoleMappingResolver.GetRole()"/>
        /// and
        /// <see cref="IRoleMappingResolver.GetNamespace()"/>
        /// methods calls which correspond to the mapping of the given role;
        /// or null if the given role is not mapped to the standard or domain-specific one.
        /// </summary>
        /// <param name="role">a role in the given namespace which mapping is to be resolved.</param>
        /// <param name="namespace">
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// which this role belongs to.
        /// </param>
        /// <returns>
        /// an instance of the
        /// <see cref="IRoleMappingResolver"/>
        /// which returns false
        /// for the
        /// <see cref="IRoleMappingResolver.CurrentRoleShallBeMappedToStandard()"/>
        /// method call; if mapping cannot be resolved
        /// to this state, this method returns null, which means that the given role
        /// in the specified namespace is not mapped to the standard role in the standard namespace.
        /// </returns>
        public virtual IRoleMappingResolver ResolveMappingToStandardOrDomainSpecificRole(String role, PdfNamespace
             @namespace) {
            IRoleMappingResolver mappingResolver = GetRoleMappingResolver(role, @namespace);
            mappingResolver.ResolveNextMapping();
            int i = 0;
            // reasonably large arbitrary number that will help to avoid a possible infinite loop
            int maxIters = 100;
            while (mappingResolver.CurrentRoleShallBeMappedToStandard()) {
                if (++i > maxIters) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.TagStructureContext));
                    logger.LogError(ComposeTooMuchTransitiveMappingsException(role, @namespace));
                    return null;
                }
                if (!mappingResolver.ResolveNextMapping()) {
                    return null;
                }
            }
            return mappingResolver;
        }

        /// <summary>Removes annotation content item from the tag structure.</summary>
        /// <remarks>
        /// Removes annotation content item from the tag structure.
        /// If annotation is not added to the document or is not tagged, nothing will happen.
        /// </remarks>
        /// <param name="annotation">
        /// the
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// that will be removed from the tag structure
        /// </param>
        /// <returns>
        /// 
        /// <see cref="TagTreePointer"/>
        /// instance which points at annotation tag parent if annotation was removed,
        /// otherwise returns null
        /// </returns>
        public virtual TagTreePointer RemoveAnnotationTag(PdfAnnotation annotation) {
            PdfStructElem structElem = null;
            PdfDictionary annotDic = annotation.GetPdfObject();
            PdfNumber structParentIndex = (PdfNumber)annotDic.Get(PdfName.StructParent);
            if (structParentIndex != null) {
                PdfObjRef objRef = document.GetStructTreeRoot().FindObjRefByStructParentIndex(annotDic.GetAsDictionary(PdfName
                    .P), structParentIndex.IntValue());
                if (objRef != null) {
                    PdfStructElem parent = (PdfStructElem)objRef.GetParent();
                    parent.RemoveKid(objRef);
                    structElem = parent;
                }
            }
            annotDic.Remove(PdfName.StructParent);
            annotDic.SetModified();
            if (structElem != null) {
                return new TagTreePointer(document).SetCurrentStructElem(structElem);
            }
            return null;
        }

        /// <summary>Removes content item from the tag structure.</summary>
        /// <remarks>
        /// Removes content item from the tag structure.
        /// <br />
        /// Nothing happens if there is no such mcid on given page.
        /// </remarks>
        /// <param name="page">page, which contains this content item</param>
        /// <param name="mcid">marked content id of this content item</param>
        /// <returns>
        /// 
        /// <c>TagTreePointer</c>
        /// which points at the parent of the removed content item, or null if there is no
        /// such mcid on given page.
        /// </returns>
        public virtual TagTreePointer RemoveContentItem(PdfPage page, int mcid) {
            PdfMcr mcr = document.GetStructTreeRoot().FindMcrByMcid(page.GetPdfObject(), mcid);
            if (mcr == null) {
                return null;
            }
            PdfStructElem parent = (PdfStructElem)mcr.GetParent();
            parent.RemoveKid(mcr);
            return new TagTreePointer(document).SetCurrentStructElem(parent);
        }

        /// <summary>Removes all tags that belong only to this page.</summary>
        /// <remarks>
        /// Removes all tags that belong only to this page. The logic which defines if tag belongs to the page is described
        /// at
        /// <see cref="FlushPageTags(iText.Kernel.Pdf.PdfPage)"/>.
        /// </remarks>
        /// <param name="page">page that defines which tags are to be removed</param>
        /// <returns>
        /// current
        /// <see cref="TagStructureContext"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagStructureContext RemovePageTags(PdfPage page) {
            PdfStructTreeRoot structTreeRoot = document.GetStructTreeRoot();
            ICollection<PdfMcr> pageMcrs = structTreeRoot.GetPageMarkedContentReferences(page);
            if (pageMcrs != null) {
                // We create a copy here, because pageMcrs is backed by the internal collection which is changed when mcrs are removed.
                IList<PdfMcr> mcrsList = new List<PdfMcr>(pageMcrs);
                foreach (PdfMcr mcr in mcrsList) {
                    RemovePageTagFromParent(mcr, mcr.GetParent());
                }
            }
            return this;
        }

        /// <summary>Flushes the tags which are considered to belong to the given page.</summary>
        /// <remarks>
        /// Flushes the tags which are considered to belong to the given page.
        /// The logic that defines if the given tag (structure element) belongs to the page is the following:
        /// if all the marked content references (dictionary or number references), that are the
        /// descendants of the given structure element, belong to the current page - the tag is considered
        /// to belong to the page. If tag has descendants from several pages - it is flushed, if all other pages except the
        /// current one are flushed.
        /// <br /><br />
        /// If some of the page's tags have waiting state (see
        /// <see cref="WaitingTagsManager"/>
        /// these tags are considered
        /// as not yet finished ones, and they and their children won't be flushed.
        /// </remarks>
        /// <param name="page">a page which tags will be flushed</param>
        /// <returns>
        /// current
        /// <see cref="TagStructureContext"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagStructureContext FlushPageTags(PdfPage page) {
            PdfStructTreeRoot structTreeRoot = document.GetStructTreeRoot();
            ICollection<PdfMcr> pageMcrs = structTreeRoot.GetPageMarkedContentReferences(page);
            if (pageMcrs != null) {
                foreach (PdfMcr mcr in pageMcrs) {
                    PdfStructElem parent = (PdfStructElem)mcr.GetParent();
                    FlushParentIfBelongsToPage(parent, page);
                }
            }
            return this;
        }

        /// <summary>Transforms root tags in a way that complies with the tagged PDF specification.</summary>
        /// <remarks>
        /// Transforms root tags in a way that complies with the tagged PDF specification.
        /// Depending on PDF version behaviour may differ.
        /// <br />
        /// ISO 32000-1 (PDF 1.7 and lower)
        /// 14.8.4.2 Grouping Elements
        /// <br />
        /// "In a tagged PDF document, the structure tree shall contain a single top-level element; that is,
        /// the structure tree root (identified by the StructTreeRoot entry in the document catalogue) shall
        /// have only one child in its K (kids) array. If the PDF file contains a complete document, the structure
        /// type Document should be used for this top-level element in the logical structure hierarchy. If the file
        /// contains a well-formed document fragment, one of the structure types Part, Art, Sect, or Div may be used instead."
        /// <br />
        /// For PDF 2.0 and higher root tag is allowed to have only the Document role.
        /// </remarks>
        public virtual void NormalizeDocumentRootTag() {
            // in this method we could deal with existing document, so we don't won't to throw exceptions here
            bool forbid = forbidUnknownRoles;
            forbidUnknownRoles = false;
            IList<IStructureNode> rootKids = document.GetStructTreeRoot().GetKids();
            IRoleMappingResolver mapping = null;
            if (rootKids.Count > 0) {
                PdfStructElem firstKid = (PdfStructElem)rootKids[0];
                mapping = ResolveMappingToStandardOrDomainSpecificRole(firstKid.GetRole().GetValue(), firstKid.GetNamespace
                    ());
            }
            if (rootKids.Count == 1 && mapping != null && mapping.CurrentRoleIsStandard() && IsRoleAllowedToBeRoot(mapping
                .GetRole())) {
                rootTagElement = (PdfStructElem)rootKids[0];
            }
            else {
                document.GetStructTreeRoot().GetPdfObject().Remove(PdfName.K);
                rootTagElement = new RootTagNormalizer(this, rootTagElement, document).MakeSingleStandardRootTag(rootKids);
            }
            forbidUnknownRoles = forbid;
        }

        /// <summary>
        /// A utility method that prepares the current instance of the
        /// <see cref="TagStructureContext"/>
        /// for
        /// the closing of document.
        /// </summary>
        /// <remarks>
        /// A utility method that prepares the current instance of the
        /// <see cref="TagStructureContext"/>
        /// for
        /// the closing of document. Essentially it flushes all the "hanging" information to the document.
        /// </remarks>
        public virtual void PrepareToDocumentClosing() {
            waitingTagsManager.RemoveAllWaitingStates();
            ActualizeNamespacesInStructTreeRoot();
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// at which
        /// <see cref="TagTreePointer"/>
        /// points.
        /// </summary>
        /// <remarks>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// at which
        /// <see cref="TagTreePointer"/>
        /// points.
        /// <para />
        /// NOTE: Be aware that
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// is a low level class, use it carefully,
        /// especially in conjunction with high level
        /// <see cref="TagTreePointer"/>
        /// and
        /// <see cref="TagStructureContext"/>
        /// classes.
        /// </remarks>
        /// <param name="pointer">
        /// a
        /// <see cref="TagTreePointer"/>
        /// which points at desired
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// at which given
        /// <see cref="TagTreePointer"/>
        /// points.
        /// </returns>
        public virtual PdfStructElem GetPointerStructElem(TagTreePointer pointer) {
            return pointer.GetCurrentStructElem();
        }

        /// <summary>Retrieve a pointer to a structure element by ID.</summary>
        /// <param name="id">the ID of the element to retrieve</param>
        /// <returns>
        /// a
        /// <see cref="TagTreePointer"/>
        /// to the element in question, or null if there is none.
        /// </returns>
        public virtual TagTreePointer GetTagPointerById(byte[] id) {
            PdfStructElem elem = document.GetStructTreeRoot().GetIdTree().GetStructElemById(id);
            return elem == null ? null : new TagTreePointer(document).SetCurrentStructElem(elem);
        }

        /// <summary>Retrieve a pointer to a structure element by ID.</summary>
        /// <remarks>
        /// Retrieve a pointer to a structure element by ID. * The ID will be encoded as a
        /// UTF-8 string and passed to
        /// <see cref="GetTagPointerById(byte[])"/>.
        /// </remarks>
        /// <param name="id">the ID of the element to retrieve</param>
        /// <returns>
        /// a
        /// <see cref="TagTreePointer"/>
        /// to the element in question, or null if there is none.
        /// </returns>
        public virtual TagTreePointer GetTagPointerByIdString(String id) {
            return this.GetTagPointerById(id.GetBytes(System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TagTreePointer"/>
        /// which points at given
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>.
        /// </summary>
        /// <param name="structElem">
        /// a
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// for which
        /// <see cref="TagTreePointer"/>
        /// will be created.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="TagTreePointer"/>.
        /// </returns>
        public virtual TagTreePointer CreatePointerForStructElem(PdfStructElem structElem) {
            return new TagTreePointer(structElem, document);
        }

        internal virtual PdfStructElem GetRootTag() {
            if (rootTagElement == null) {
                NormalizeDocumentRootTag();
            }
            return rootTagElement;
        }

        internal virtual PdfDocument GetDocument() {
            return document;
        }

        internal virtual void EnsureNamespaceRegistered(PdfNamespace @namespace) {
            if (@namespace != null) {
                PdfDictionary namespaceObj = @namespace.GetPdfObject();
                if (!namespaces.Contains(namespaceObj)) {
                    namespaces.Add(namespaceObj);
                }
                nameToNamespace.Put(@namespace.GetNamespaceName(), @namespace);
            }
        }

        internal virtual void ThrowExceptionIfRoleIsInvalid(AccessibilityProperties properties, PdfNamespace pointerCurrentNamespace
            ) {
            PdfNamespace @namespace = properties.GetNamespace();
            if (@namespace == null) {
                @namespace = pointerCurrentNamespace;
            }
            ThrowExceptionIfRoleIsInvalid(properties.GetRole(), @namespace);
        }

        internal virtual void ThrowExceptionIfRoleIsInvalid(String role, PdfNamespace @namespace) {
            if (!CheckIfRoleShallBeMappedToStandardRole(role, @namespace)) {
                String exMessage = ComposeInvalidRoleException(role, @namespace);
                if (forbidUnknownRoles) {
                    throw new PdfException(exMessage);
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.TagStructureContext));
                    logger.LogWarning(exMessage);
                }
            }
        }

        internal virtual bool TargetTagStructureVersionIs2() {
            return PdfVersion.PDF_2_0.CompareTo(tagStructureTargetVersion) <= 0;
        }

        internal virtual void FlushParentIfBelongsToPage(PdfStructElem parent, PdfPage currentPage) {
            if (parent.IsFlushed() || waitingTagsManager.GetObjForStructDict(parent.GetPdfObject()) != null || parent.
                GetParent() is PdfStructTreeRoot) {
                return;
            }
            IList<IStructureNode> kids = parent.GetKids();
            bool readyToBeFlushed = true;
            foreach (IStructureNode kid in kids) {
                if (kid is PdfMcr) {
                    PdfDictionary kidPage = ((PdfMcr)kid).GetPageObject();
                    if (!kidPage.IsFlushed() && (currentPage == null || !kidPage.Equals(currentPage.GetPdfObject()))) {
                        readyToBeFlushed = false;
                        break;
                    }
                }
                else {
                    if (kid is PdfStructElem) {
                        // If kid is structElem and was already flushed then in kids list there will be null for it instead of
                        // PdfStructElement. And therefore if we get into this if-clause it means that some StructElem wasn't flushed.
                        readyToBeFlushed = false;
                        break;
                    }
                }
            }
            if (readyToBeFlushed) {
                IStructureNode parentsParent = parent.GetParent();
                parent.Flush();
                if (parentsParent is PdfStructElem) {
                    FlushParentIfBelongsToPage((PdfStructElem)parentsParent, currentPage);
                }
            }
        }

        private bool IsRoleAllowedToBeRoot(String role) {
            if (TargetTagStructureVersionIs2()) {
                return StandardRoles.DOCUMENT.Equals(role);
            }
            else {
                return ALLOWED_ROOT_TAG_ROLES.Contains(role);
            }
        }

        private void SetNamespaceForNewTagsBasedOnExistingRoot() {
            IList<IStructureNode> rootKids = document.GetStructTreeRoot().GetKids();
            if (rootKids.Count > 0) {
                PdfStructElem firstKid = (PdfStructElem)rootKids[0];
                IRoleMappingResolver resolvedMapping = ResolveMappingToStandardOrDomainSpecificRole(firstKid.GetRole().GetValue
                    (), firstKid.GetNamespace());
                if (resolvedMapping == null || !resolvedMapping.CurrentRoleIsStandard()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.TagStructureContext));
                    String nsStr;
                    if (firstKid.GetNamespace() != null) {
                        nsStr = firstKid.GetNamespace().GetNamespaceName();
                    }
                    else {
                        nsStr = StandardNamespaces.GetDefault();
                    }
                    logger.LogWarning(String.Format(iText.IO.Logs.IoLogMessageConstant.EXISTING_TAG_STRUCTURE_ROOT_IS_NOT_STANDARD
                        , firstKid.GetRole().GetValue(), nsStr));
                }
                if (resolvedMapping == null || !StandardNamespaces.PDF_1_7.Equals(resolvedMapping.GetNamespace().GetNamespaceName
                    ())) {
                    documentDefaultNamespace = FetchNamespace(StandardNamespaces.PDF_2_0);
                }
            }
            else {
                documentDefaultNamespace = FetchNamespace(StandardNamespaces.PDF_2_0);
            }
        }

        private String ComposeInvalidRoleException(String role, PdfNamespace @namespace) {
            return ComposeExceptionBasedOnNamespacePresence(role, @namespace, KernelExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE);
        }

        private String ComposeTooMuchTransitiveMappingsException(String role, PdfNamespace @namespace) {
            return ComposeExceptionBasedOnNamespacePresence(role, @namespace, iText.IO.Logs.IoLogMessageConstant.CANNOT_RESOLVE_ROLE_TOO_MUCH_TRANSITIVE_MAPPINGS
                , iText.IO.Logs.IoLogMessageConstant.CANNOT_RESOLVE_ROLE_IN_NAMESPACE_TOO_MUCH_TRANSITIVE_MAPPINGS);
        }

        private void InitRegisteredNamespaces() {
            PdfStructTreeRoot structTreeRoot = document.GetStructTreeRoot();
            foreach (PdfNamespace @namespace in structTreeRoot.GetNamespaces()) {
                namespaces.Add(@namespace.GetPdfObject());
                nameToNamespace.Put(@namespace.GetNamespaceName(), @namespace);
            }
        }

        private void ActualizeNamespacesInStructTreeRoot() {
            if (namespaces.Count > 0) {
                PdfStructTreeRoot structTreeRoot = GetDocument().GetStructTreeRoot();
                PdfArray rootNamespaces = structTreeRoot.GetNamespacesObject();
                ICollection<PdfDictionary> newNamespaces = new LinkedHashSet<PdfDictionary>(namespaces);
                for (int i = 0; i < rootNamespaces.Size(); ++i) {
                    newNamespaces.Remove(rootNamespaces.GetAsDictionary(i));
                }
                foreach (PdfDictionary newNs in newNamespaces) {
                    rootNamespaces.Add(newNs);
                }
                if (!newNamespaces.IsEmpty()) {
                    structTreeRoot.SetModified();
                }
            }
        }

        private void RemovePageTagFromParent(IStructureNode pageTag, IStructureNode parent) {
            if (parent is PdfStructElem) {
                PdfStructElem structParent = (PdfStructElem)parent;
                if (!structParent.IsFlushed()) {
                    structParent.RemoveKid(pageTag);
                    PdfDictionary parentStructDict = structParent.GetPdfObject();
                    if (waitingTagsManager.GetObjForStructDict(parentStructDict) == null && parent.GetKids().Count == 0 && !(structParent
                        .GetParent() is PdfStructTreeRoot)) {
                        RemovePageTagFromParent(structParent, parent.GetParent());
                        PdfIndirectReference indRef = parentStructDict.GetIndirectReference();
                        if (indRef != null) {
                            // TODO DEVSIX-5472 need to clean references to structure element from
                            //  other structure elements /Ref entries and structure destinations
                            indRef.SetFree();
                        }
                    }
                }
                else {
                    if (pageTag is PdfMcr) {
                        throw new PdfException(KernelExceptionMessageConstant.CANNOT_REMOVE_TAG_BECAUSE_ITS_PARENT_IS_FLUSHED);
                    }
                }
            }
        }

        // it is StructTreeRoot
        // should never happen as we always should have only one root tag and we don't remove it
        private String ComposeExceptionBasedOnNamespacePresence(String role, PdfNamespace @namespace, String withoutNsEx
            , String withNsEx) {
            if (@namespace == null) {
                return String.Format(withoutNsEx, role);
            }
            else {
                String nsName = @namespace.GetNamespaceName();
                PdfIndirectReference @ref = @namespace.GetPdfObject().GetIndirectReference();
                if (@ref != null) {
                    nsName = nsName + " (" + JavaUtil.IntegerToString(@ref.GetObjNumber()) + " " + JavaUtil.IntegerToString(@ref
                        .GetGenNumber()) + " obj)";
                }
                return String.Format(withNsEx, role, nsName);
            }
        }
    }
}
