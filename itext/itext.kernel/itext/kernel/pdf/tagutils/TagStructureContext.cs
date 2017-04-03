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
using System;
using System.Collections.Generic;
using iText.IO.Log;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// <c>TagStructureContext</c>
    /// class is used to track necessary information of document's tag structure.
    /// It is also used to make some global modifications of the tag tree like removing or flushing page tags, however
    /// these two methods and also others are called automatically and are for the most part for internal usage.
    /// <br/><br/>
    /// There shall be only one instance of this class per
    /// <c>PdfDocument</c>
    /// . To obtain instance of this class use
    /// <see cref="iText.Kernel.Pdf.PdfDocument.GetTagStructureContext()"/>
    /// .
    /// </summary>
    public class TagStructureContext {
        private static readonly ICollection<PdfName> allowedRootTagRoles = new HashSet<PdfName>();

        static TagStructureContext() {
            allowedRootTagRoles.Add(PdfName.Document);
            allowedRootTagRoles.Add(PdfName.Part);
            allowedRootTagRoles.Add(PdfName.Art);
            allowedRootTagRoles.Add(PdfName.Sect);
            allowedRootTagRoles.Add(PdfName.Div);
        }

        private PdfDocument document;

        private PdfStructElem rootTagElement;

        protected internal TagTreePointer autoTaggingPointer;

        private PdfVersion tagStructureTargetVersion;

        private bool forbidUnknownRoles;

        /// <summary>
        /// These two fields define the connections between tags (
        /// <c>PdfStructElem</c>
        /// ) and
        /// layout model elements (
        /// <see cref="IAccessibleElement"/>
        /// ). This connection is used as
        /// a sign that tag is not yet finished and therefore should not be flushed or removed
        /// if page tags are flushed or removed. Also, any
        /// <see cref="TagTreePointer"/>
        /// could be
        /// immediately moved to the tag with connection via it's connected element
        /// <see cref="TagTreePointer.MoveToTag(IAccessibleElement)"/>
        /// .
        /// When connection is removed, accessible element role and properties are set to the structure element.
        /// </summary>
        private IDictionary<IAccessibleElement, PdfStructElem> connectedModelToStruct;

        private IDictionary<PdfDictionary, IAccessibleElement> connectedStructToModel;

        private ICollection<PdfDictionary> namespaces;

        private IDictionary<PdfString, PdfNamespace> nameToNamespace;

        private PdfNamespace documentDefaultNamespace;

        /// <summary>
        /// Do not use this constructor, instead use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetTagStructureContext()"/>
        /// method.
        /// <br/><br/>
        /// Creates
        /// <c>TagStructureContext</c>
        /// for document. There shall be only one instance of this
        /// class per
        /// <c>PdfDocument</c>
        /// .
        /// </summary>
        /// <param name="document">the document which tag structure will be manipulated with this class.</param>
        public TagStructureContext(PdfDocument document)
            : this(document, document.GetPdfVersion()) {
        }

        /// <summary>
        /// Do not use this constructor, instead use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetTagStructureContext()"/>
        /// method.
        /// <br/><br/>
        /// Creates
        /// <c>TagStructureContext</c>
        /// for document. There shall be only one instance of this
        /// class per
        /// <c>PdfDocument</c>
        /// .
        /// </summary>
        /// <param name="document">the document which tag structure will be manipulated with this class.</param>
        /// <param name="tagStructureTargetVersion">the version of the pdf standard to which the tag structure shall adhere.
        ///     </param>
        public TagStructureContext(PdfDocument document, PdfVersion tagStructureTargetVersion) {
            this.document = document;
            if (!document.IsTagged()) {
                throw new PdfException(PdfException.MustBeATaggedDocument);
            }
            connectedModelToStruct = new Dictionary<IAccessibleElement, PdfStructElem>();
            connectedStructToModel = new Dictionary<PdfDictionary, IAccessibleElement>();
            namespaces = new LinkedHashSet<PdfDictionary>();
            nameToNamespace = new Dictionary<PdfString, PdfNamespace>();
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

        public virtual PdfVersion GetTagStructureTargetVersion() {
            return tagStructureTargetVersion;
        }

        /// <summary>
        /// All document auto tagging logic uses
        /// <see cref="TagTreePointer"/>
        /// returned by this method to manipulate tag structure.
        /// Typically it points at the root tag. This pointer also could be used to tweak auto tagging process
        /// (e.g. move this pointer to the Sect tag, which would result in placing all automatically tagged content
        /// under Sect tag).
        /// </summary>
        /// <returns>
        /// the
        /// <c>TagTreePointer</c>
        /// which is used for all auto tagging of the document.
        /// </returns>
        public virtual TagTreePointer GetAutoTaggingPointer() {
            if (autoTaggingPointer == null) {
                autoTaggingPointer = new TagTreePointer(document);
            }
            return autoTaggingPointer;
        }

        /// <summary>
        /// A namespace that is used as a default value for the tagging for any new
        /// <see cref="TagTreePointer"/>
        /// created
        /// (including the pointer returned by
        /// <see cref="GetAutoTaggingPointer()"/>
        /// , which implies that automatically
        /// created tag structure will be in this namespace by default).
        /// <p>
        /// By default, this value is defined based on the PDF document version and the existing tag structure inside
        /// a document. For the new empty PDF 2.0 documents this namespace is set to
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardStructureNamespace._2_0"/>
        /// .
        /// </p>
        /// <p>This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.</p>
        /// </summary>
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
        /// See
        /// <see cref="GetDocumentDefaultNamespace()"/>
        /// for more info.
        /// <p>
        /// Be careful when changing this property value. It is most recommended to do it right after the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// was
        /// created, before any content was added. Changing this value after any content was added might result in the mingled
        /// tag structure from the namespaces point of view. So in order to maintain the document consistent but in the namespace
        /// different from default, set this value before any modifications to the document were made and before
        /// <see cref="GetAutoTaggingPointer()"/>
        /// method was called for the first time.
        /// </p>
        /// <p>This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.</p>
        /// </summary>
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
        /// <p>
        /// Returns either a wrapper over an already existing namespace dictionary in the document or over a new one
        /// if such namespace wasn't encountered before. Calling this method is considered as encountering a namespace,
        /// i.e. two sequential calls on this method will return the same namespace instance (which is not true in general case
        /// of two method calls, for instance if several namespace instances with the same name are created via
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// constructors and set to the elements of the tag structure, then the last encountered one
        /// will be returned by this method). However encountered namespaces will not be added to the document's structure tree root
        /// <see cref="iText.Kernel.Pdf.PdfName.Namespaces">/Namespaces</see>
        /// array unless they were set to the certain element of the tag structure.
        /// </p>
        /// </summary>
        /// <param name="namespaceName">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// defining the namespace name (conventionally a uniform resource identifier, or URI).
        /// </param>
        /// <returns>
        /// 
        /// <see>PdfNamespace) wrapper over either already existing namespace object or over the new one.</see>
        /// </returns>
        public virtual PdfNamespace FetchNamespace(PdfString namespaceName) {
            PdfNamespace ns = nameToNamespace.Get(namespaceName);
            if (ns == null) {
                ns = new PdfNamespace(namespaceName);
                nameToNamespace[namespaceName] = ns;
            }
            return ns;
        }

        /// <summary>
        /// Gets an instance of the
        /// <see cref="IRoleMappingResolver"/>
        /// corresponding to the current tag structure target version.
        /// This method implies that role is in the default standard structure namespace.
        /// </summary>
        /// <param name="role">a role in the default standard structure namespace which mapping is to be resolved.</param>
        /// <returns>
        /// a
        /// <see cref="IRoleMappingResolver"/>
        /// instance, with the giving role as current.
        /// </returns>
        public virtual IRoleMappingResolver GetRoleMappingResolver(PdfName role) {
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
        public virtual IRoleMappingResolver GetRoleMappingResolver(PdfName role, PdfNamespace @namespace) {
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
        public virtual bool CheckIfRoleShallBeMappedToStandardRole(PdfName role, PdfNamespace @namespace) {
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
        /// methods calls which correspond to the mapping of the given role; or null if the given role is not mapped to the standard or domain-specific one.
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
        public virtual IRoleMappingResolver ResolveMappingToStandardOrDomainSpecificRole(PdfName role, PdfNamespace
             @namespace) {
            IRoleMappingResolver mappingResolver = GetRoleMappingResolver(role, @namespace);
            mappingResolver.ResolveNextMapping();
            int i = 0;
            // reasonably large arbitrary number that will help to avoid a possible infinite loop
            int maxIters = 100;
            while (mappingResolver.CurrentRoleShallBeMappedToStandard()) {
                if (++i > maxIters) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.TagStructureContext));
                    logger.Error(ComposeTooMuchTransitiveMappingsException(role, @namespace));
                    return null;
                }
                if (!mappingResolver.ResolveNextMapping()) {
                    return null;
                }
            }
            return mappingResolver;
        }

        /// <summary>
        /// Checks if given
        /// <c>IAccessibleElement</c>
        /// is connected to some tag.
        /// </summary>
        /// <param name="element">element to check if it has a connected tag.</param>
        /// <returns>true, if there is a tag which retains the connection to the given accessible element.</returns>
        public virtual bool IsElementConnectedToTag(IAccessibleElement element) {
            return connectedModelToStruct.ContainsKey(element);
        }

        /// <summary>Destroys the connection between the given accessible element and the tag to which this element is connected to.
        ///     </summary>
        /// <param name="element">
        /// 
        /// <c>IAccessibleElement</c>
        /// which connection to the tag (if there is one) will be removed.
        /// </param>
        /// <returns>
        /// current
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagStructureContext RemoveElementConnectionToTag(IAccessibleElement
             element) {
            PdfStructElem structElem = connectedModelToStruct.JRemove(element);
            RemoveStructToModelConnection(structElem);
            return this;
        }

        /// <summary>Removes annotation content item from the tag structure.</summary>
        /// <remarks>
        /// Removes annotation content item from the tag structure.
        /// If annotation is not added to the document or is not tagged, nothing will happen.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="TagTreePointer"/>
        /// instance which points at annotation tag parent if annotation was removed,
        /// otherwise returns null.
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
        /// <br/>
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
        /// <see cref="FlushPageTags(iText.Kernel.Pdf.PdfPage)"/>
        /// .
        /// </remarks>
        /// <param name="page">page that defines which tags are to be removed</param>
        /// <returns>
        /// current
        /// <see cref="TagStructureContext"/>
        /// instance.
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

        /// <summary>
        /// Sets the tag, which is connected with the given accessible element, as a current tag for the given
        /// <see cref="TagTreePointer"/>
        /// . An exception will be thrown, if given accessible element is not connected to any tag.
        /// </summary>
        /// <param name="element">an element which has a connection with some tag.</param>
        /// <param name="tagPointer">
        /// 
        /// <see cref="TagTreePointer"/>
        /// which will be moved to the tag connected to the given accessible element.
        /// </param>
        /// <returns>
        /// current
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagStructureContext MoveTagPointerToTag(IAccessibleElement element
            , TagTreePointer tagPointer) {
            PdfStructElem connectedStructElem = connectedModelToStruct.Get(element);
            if (connectedStructElem == null) {
                throw new PdfException(PdfException.GivenAccessibleElementIsNotConnectedToAnyTag);
            }
            tagPointer.SetCurrentStructElem(connectedStructElem);
            return this;
        }

        /// <summary>Destroys all the retained connections.</summary>
        /// <returns>
        /// current
        /// <see cref="TagStructureContext"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagStructureContext RemoveAllConnectionsToTags() {
            foreach (PdfStructElem structElem in connectedModelToStruct.Values) {
                RemoveStructToModelConnection(structElem);
            }
            connectedModelToStruct.Clear();
            return this;
        }

        /// <summary>Flushes the tags which are considered to belong to the given page.</summary>
        /// <remarks>
        /// Flushes the tags which are considered to belong to the given page.
        /// The logic that defines if the given tag (structure element) belongs to the page is the following:
        /// if all the marked content references (dictionary or number references), that are the
        /// descenders of the given structure element, belong to the current page - the tag is considered
        /// to belong to the page. If tag has descenders from several pages - it is flushed, if all other pages except the
        /// current one are flushed.
        /// <br /><br />
        /// If some of the page's tags are still connected to the accessible elements, in this case these tags are considered
        /// as not yet finished ones, and they won't be flushed.
        /// </remarks>
        /// <param name="page">a page which tags will be flushed.</param>
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

        /// <summary>Transforms root tags in a way that complies with the PDF References.</summary>
        /// <remarks>
        /// Transforms root tags in a way that complies with the PDF References.
        /// <br/><br/>
        /// PDF Reference
        /// 10.7.3 Grouping Elements:
        /// <br/><br/>
        /// For most content extraction formats, the document must be a tree with a single top-level element;
        /// the structure tree root (identified by the StructTreeRoot entry in the document catalog) must have
        /// only one child in its K (kids) array. If the PDF file contains a complete document, the structure
        /// type Document is recommended for this top-level element in the logical structure hierarchy. If the
        /// file contains a well-formed document fragment, one of the structure types Part, Art, Sect, or Div
        /// may be used instead.
        /// </remarks>
        public virtual void NormalizeDocumentRootTag() {
            // in this method we could deal with existing document, so we don't won't to throw exceptions here
            bool forbid = forbidUnknownRoles;
            forbidUnknownRoles = false;
            IList<IPdfStructElem> rootKids = document.GetStructTreeRoot().GetKids();
            IRoleMappingResolver resolvedMapping = null;
            if (rootKids.Count > 0) {
                PdfStructElem firstKid = (PdfStructElem)rootKids[0];
                resolvedMapping = ResolveMappingToStandardOrDomainSpecificRole(firstKid.GetRole(), firstKid.GetNamespace()
                    );
            }
            if (rootKids.Count == 1 && resolvedMapping != null && resolvedMapping.CurrentRoleIsStandard() && IsRoleAllowedToBeRoot
                (resolvedMapping.GetRole())) {
                rootTagElement = (PdfStructElem)rootKids[0];
            }
            else {
                PdfStructElem prevRootTag = rootTagElement;
                document.GetStructTreeRoot().GetPdfObject().Remove(PdfName.K);
                if (prevRootTag == null) {
                    rootTagElement = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
                    if (TargetTagStructureVersionIs2()) {
                        PdfNamespace ns = GetDocumentDefaultNamespace();
                        rootTagElement.SetNamespace(ns);
                        EnsureNamespaceRegistered(ns);
                    }
                }
                else {
                    document.GetStructTreeRoot().AddKid(rootTagElement);
                    resolvedMapping = ResolveMappingToStandardOrDomainSpecificRole(rootTagElement.GetRole(), rootTagElement.GetNamespace
                        ());
                    if (resolvedMapping.CurrentRoleIsStandard() && !PdfName.Document.Equals(resolvedMapping.GetRole())) {
                        WrapAllKidsInTag(rootTagElement, rootTagElement.GetRole(), rootTagElement.GetNamespace());
                        rootTagElement.SetRole(PdfName.Document);
                    }
                }
                int originalRootKidsIndex = 0;
                bool isBeforeOriginalRoot = true;
                foreach (IPdfStructElem elem in rootKids) {
                    // StructTreeRoot kids are always PdfStructElem, so we are save here to cast it
                    PdfStructElem kid = (PdfStructElem)elem;
                    if (kid.GetPdfObject() == rootTagElement.GetPdfObject()) {
                        isBeforeOriginalRoot = false;
                        continue;
                    }
                    // This boolean is used to "flatten" possible deep "stacking" of the tag structure in case of the multiple pages copying operations.
                    // This could happen due to the wrapping of all the kids in the "(prevRootTag == null)" if-clause above.
                    // And therefore, we don't need here to resolve mappings, because we exactly know which role we set.
                    bool kidIsDocument = PdfName.Document.Equals(kid.GetRole());
                    if (kidIsDocument && kid.GetNamespace() != null && TargetTagStructureVersionIs2()) {
                        // we flatten only tags of document role in standard structure namespace
                        PdfString kidNamespaceName = kid.GetNamespace().GetNamespaceName();
                        kidIsDocument = StandardStructureNamespace._1_7.Equals(kidNamespaceName) || StandardStructureNamespace._2_0
                            .Equals(kidNamespaceName);
                    }
                    if (isBeforeOriginalRoot) {
                        rootTagElement.AddKid(originalRootKidsIndex, kid);
                        originalRootKidsIndex += kidIsDocument ? kid.GetKids().Count : 1;
                    }
                    else {
                        rootTagElement.AddKid(kid);
                    }
                    if (kidIsDocument) {
                        RemoveOldRoot(kid);
                    }
                }
            }
            forbidUnknownRoles = forbid;
        }

        /// <summary>
        /// A utility method that prepares the current instance of the
        /// <see cref="TagStructureContext"/>
        /// for
        /// the closing of document. Essentially it flushes all the "hanging" information to the document.
        /// </summary>
        public virtual void PrepareToDocumentClosing() {
            RemoveAllConnectionsToTags();
            ActualizeNamespacesInStructTreeRoot();
        }

        /// <summary>Method for internal usages.</summary>
        /// <remarks>
        /// Method for internal usages.
        /// Essentially, all it does is just making sure that for connected tags the properties are
        /// up to date with the connected accessible elements properties.
        /// </remarks>
        public virtual void ActualizeTagsProperties() {
            foreach (KeyValuePair<IAccessibleElement, PdfStructElem> structToModel in connectedModelToStruct) {
                IAccessibleElement element = structToModel.Key;
                PdfStructElem structElem = structToModel.Value;
                structElem.SetRole(element.GetRole());
                if (element.GetAccessibilityProperties() != null) {
                    element.GetAccessibilityProperties().SetToStructElem(structElem);
                    EnsureNamespaceRegistered(element.GetAccessibilityProperties().GetNamespace());
                }
            }
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

        internal virtual PdfStructElem GetStructConnectedToModel(IAccessibleElement element) {
            return connectedModelToStruct.Get(element);
        }

        internal virtual IAccessibleElement GetModelConnectedToStruct(PdfStructElem @struct) {
            return connectedStructToModel.Get(@struct.GetPdfObject());
        }

        internal virtual void EnsureNamespaceRegistered(PdfNamespace @namespace) {
            if (@namespace != null) {
                PdfDictionary namespaceObj = @namespace.GetPdfObject();
                if (!namespaces.Contains(namespaceObj)) {
                    namespaces.Add(namespaceObj);
                }
                nameToNamespace[@namespace.GetNamespaceName()] = @namespace;
            }
        }

        internal virtual void ThrowExceptionIfRoleIsInvalid(IAccessibleElement element, PdfNamespace pointerCurrentNamespace
            ) {
            AccessibilityProperties properties = element.GetAccessibilityProperties();
            PdfNamespace @namespace = properties != null ? properties.GetNamespace() : null;
            if (@namespace == null) {
                @namespace = pointerCurrentNamespace;
            }
            if (!CheckIfRoleShallBeMappedToStandardRole(element.GetRole(), @namespace)) {
                String exMessage = ComposeInvalidRoleException(element, @namespace);
                if (forbidUnknownRoles) {
                    throw new PdfException(exMessage);
                }
                else {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.TagStructureContext));
                    logger.Warn(exMessage);
                }
            }
        }

        internal virtual void SaveConnectionBetweenStructAndModel(IAccessibleElement element, PdfStructElem structElem
            ) {
            connectedModelToStruct[element] = structElem;
            connectedStructToModel[structElem.GetPdfObject()] = element;
        }

        /// <returns>parent of the flushed tag</returns>
        internal virtual IPdfStructElem FlushTag(PdfStructElem tagStruct) {
            IAccessibleElement modelElement = connectedStructToModel.JRemove(tagStruct.GetPdfObject());
            if (modelElement != null) {
                connectedModelToStruct.JRemove(modelElement);
            }
            IPdfStructElem parent = tagStruct.GetParent();
            FlushStructElementAndItKids(tagStruct);
            return parent;
        }

        private bool TargetTagStructureVersionIs2() {
            return PdfVersion.PDF_2_0.CompareTo(tagStructureTargetVersion) <= 0;
        }

        private bool IsRoleAllowedToBeRoot(PdfName role) {
            if (TargetTagStructureVersionIs2()) {
                return PdfName.Document.Equals(role);
            }
            else {
                return allowedRootTagRoles.Contains(role);
            }
        }

        private void SetNamespaceForNewTagsBasedOnExistingRoot() {
            IList<IPdfStructElem> rootKids = document.GetStructTreeRoot().GetKids();
            if (rootKids.Count > 0) {
                PdfStructElem firstKid = (PdfStructElem)rootKids[0];
                IRoleMappingResolver resolvedMapping = ResolveMappingToStandardOrDomainSpecificRole(firstKid.GetRole(), firstKid
                    .GetNamespace());
                if (resolvedMapping == null || !resolvedMapping.CurrentRoleIsStandard()) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.TagStructureContext));
                    String nsStr;
                    if (firstKid.GetNamespace() != null) {
                        nsStr = firstKid.GetNamespace().GetNamespaceName().ToUnicodeString();
                    }
                    else {
                        nsStr = StandardStructureNamespace.GetDefault().ToUnicodeString();
                    }
                    logger.Warn(String.Format(iText.IO.LogMessageConstant.EXISTING_TAG_STRUCTURE_ROOT_IS_NOT_STANDARD, firstKid
                        .GetRole().GetValue(), nsStr));
                }
                if (resolvedMapping == null || !StandardStructureNamespace._1_7.Equals(resolvedMapping.GetNamespace().GetNamespaceName
                    ())) {
                    documentDefaultNamespace = FetchNamespace(StandardStructureNamespace._2_0);
                }
            }
            else {
                documentDefaultNamespace = FetchNamespace(StandardStructureNamespace._2_0);
            }
        }

        private String ComposeInvalidRoleException(IAccessibleElement element, PdfNamespace @namespace) {
            return ComposeExceptionBasedOnNamespacePresence(element.GetRole().ToString(), @namespace, PdfException.RoleIsNotMappedToAnyStandardRole
                , PdfException.RoleInNamespaceIsNotMappedToAnyStandardRole);
        }

        private String ComposeTooMuchTransitiveMappingsException(PdfName role, PdfNamespace @namespace) {
            return ComposeExceptionBasedOnNamespacePresence(role.ToString(), @namespace, iText.IO.LogMessageConstant.CANNOT_RESOLVE_ROLE_TOO_MUCH_TRANSITIVE_MAPPINGS
                , iText.IO.LogMessageConstant.CANNOT_RESOLVE_ROLE_IN_NAMESPACE_TOO_MUCH_TRANSITIVE_MAPPINGS);
        }

        private void InitRegisteredNamespaces() {
            PdfStructTreeRoot structTreeRoot = document.GetStructTreeRoot();
            foreach (PdfNamespace @namespace in structTreeRoot.GetNamespaces()) {
                namespaces.Add(@namespace.GetPdfObject());
                nameToNamespace[@namespace.GetNamespaceName()] = @namespace;
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

        private void RemoveStructToModelConnection(PdfStructElem structElem) {
            if (structElem != null) {
                IAccessibleElement element = connectedStructToModel.JRemove(structElem.GetPdfObject());
                structElem.SetRole(element.GetRole());
                if (element.GetAccessibilityProperties() != null) {
                    element.GetAccessibilityProperties().SetToStructElem(structElem);
                    EnsureNamespaceRegistered(element.GetAccessibilityProperties().GetNamespace());
                }
                if (structElem.GetParent() == null) {
                    // is flushed
                    FlushStructElementAndItKids(structElem);
                }
            }
        }

        private void RemovePageTagFromParent(IPdfStructElem pageTag, IPdfStructElem parent) {
            if (parent is PdfStructElem) {
                PdfStructElem structParent = (PdfStructElem)parent;
                if (!structParent.IsFlushed()) {
                    structParent.RemoveKid(pageTag);
                    PdfDictionary parentObject = structParent.GetPdfObject();
                    if (!connectedStructToModel.ContainsKey(parentObject) && parent.GetKids().Count == 0 && parentObject != GetRootTag
                        ().GetPdfObject()) {
                        RemovePageTagFromParent(structParent, parent.GetParent());
                        parentObject.GetIndirectReference().SetFree();
                    }
                }
                else {
                    if (pageTag is PdfMcr) {
                        throw new PdfException(PdfException.CannotRemoveTagBecauseItsParentIsFlushed);
                    }
                }
            }
        }

        // it is StructTreeRoot
        // should never happen as we always should have only one root tag and we don't remove it
        private void FlushParentIfBelongsToPage(PdfStructElem parent, PdfPage currentPage) {
            if (parent.IsFlushed() || connectedStructToModel.ContainsKey(parent.GetPdfObject()) || parent.GetPdfObject
                () == GetRootTag().GetPdfObject()) {
                return;
            }
            IList<IPdfStructElem> kids = parent.GetKids();
            bool allKidsBelongToPage = true;
            foreach (IPdfStructElem kid in kids) {
                if (kid is PdfMcr) {
                    PdfDictionary kidPage = ((PdfMcr)kid).GetPageObject();
                    if (!kidPage.IsFlushed() && !kidPage.Equals(currentPage.GetPdfObject())) {
                        allKidsBelongToPage = false;
                        break;
                    }
                }
                else {
                    if (kid is PdfStructElem) {
                        // If kid is structElem and was already flushed then in kids list there will be null for it instead of
                        // PdfStructElem. And therefore if we get into this if-clause it means that some StructElem wasn't flushed.
                        allKidsBelongToPage = false;
                        break;
                    }
                }
            }
            if (allKidsBelongToPage) {
                IPdfStructElem parentsParent = parent.GetParent();
                parent.Flush();
                if (parentsParent is PdfStructElem) {
                    FlushParentIfBelongsToPage((PdfStructElem)parentsParent, currentPage);
                }
            }
            return;
        }

        private void FlushStructElementAndItKids(PdfStructElem elem) {
            if (connectedStructToModel.ContainsKey(elem.GetPdfObject())) {
                return;
            }
            foreach (IPdfStructElem kid in elem.GetKids()) {
                if (kid is PdfStructElem) {
                    FlushStructElementAndItKids((PdfStructElem)kid);
                }
            }
            elem.Flush();
        }

        private void WrapAllKidsInTag(PdfStructElem parent, PdfName wrapTagRole, PdfNamespace wrapTagNs) {
            int kidsNum = parent.GetKids().Count;
            TagTreePointer tagPointer = new TagTreePointer(parent);
            tagPointer.AddTag(0, wrapTagRole);
            if (TargetTagStructureVersionIs2()) {
                tagPointer.GetProperties().SetNamespace(wrapTagNs);
            }
            TagTreePointer newParentOfKids = new TagTreePointer(tagPointer);
            tagPointer.MoveToParent();
            for (int i = 0; i < kidsNum; ++i) {
                tagPointer.RelocateKid(1, newParentOfKids);
            }
        }

        private void RemoveOldRoot(PdfStructElem oldRoot) {
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetCurrentStructElem(oldRoot).RemoveTag();
        }

        private String ComposeExceptionBasedOnNamespacePresence(String role, PdfNamespace @namespace, String withoutNsEx
            , String withNsEx) {
            if (@namespace == null) {
                return String.Format(withoutNsEx, role);
            }
            else {
                String nsName = @namespace.GetNamespaceName().ToUnicodeString();
                PdfIndirectReference @ref = @namespace.GetPdfObject().GetIndirectReference();
                if (@ref != null) {
                    nsName = nsName + " (" + iText.IO.Util.JavaUtil.IntegerToString(@ref.GetObjNumber()) + " " + iText.IO.Util.JavaUtil.IntegerToString
                        (@ref.GetGenNumber()) + " obj)";
                }
                return String.Format(withNsEx, role, nsName);
            }
        }
    }
}
