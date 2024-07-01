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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>Represents a wrapper-class for structure tree root dictionary.</summary>
    /// <remarks>Represents a wrapper-class for structure tree root dictionary. See ISO-32000-1 "14.7.2 Structure hierarchy".
    ///     </remarks>
    public class PdfStructTreeRoot : PdfObjectWrapper<PdfDictionary>, IStructureNode {
        private PdfDocument document;

        private ParentTreeHandler parentTreeHandler;

        private PdfStructIdTree idTree = null;

        private static IDictionary<String, PdfName> staticRoleNames = new ConcurrentDictionary<String, PdfName>();

        /// <summary>Creates a new structure tree root instance, this initializes empty logical structure in the document.
        ///     </summary>
        /// <remarks>
        /// Creates a new structure tree root instance, this initializes empty logical structure in the document.
        /// This class also handles global state of parent tree, so it's not expected to create multiple instances
        /// of this class. Instead, use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetStructTreeRoot()"/>.
        /// </remarks>
        /// <param name="document">a document to which new instance of struct tree root will be bound</param>
        public PdfStructTreeRoot(PdfDocument document)
            : this((PdfDictionary)new PdfDictionary().MakeIndirect(document), document) {
            GetPdfObject().Put(PdfName.Type, PdfName.StructTreeRoot);
        }

        /// <summary>Creates wrapper instance for already existing logical structure tree root in the document.</summary>
        /// <remarks>
        /// Creates wrapper instance for already existing logical structure tree root in the document.
        /// This class also handles global state of parent tree, so it's not expected to create multiple instances
        /// of this class. Instead, use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetStructTreeRoot()"/>.
        /// </remarks>
        /// <param name="structTreeRootDict">a dictionary that defines document structure tree root</param>
        /// <param name="document">a document, which contains given structure tree root dictionary</param>
        public PdfStructTreeRoot(PdfDictionary structTreeRootDict, PdfDocument document)
            : base(structTreeRootDict) {
            this.document = document;
            if (this.document == null) {
                EnsureObjectIsAddedToDocument(structTreeRootDict);
                this.document = structTreeRootDict.GetIndirectReference().GetDocument();
            }
            SetForbidRelease();
            parentTreeHandler = new ParentTreeHandler(this);
            // Always init role map dictionary in order to avoid inconsistency, because
            // iText often initializes it during role mapping resolution anyway.
            // In future, better way might be to not write it to the document needlessly
            // and avoid possible redundant modifications in append mode.
            GetRoleMap();
        }

        public static PdfName ConvertRoleToPdfName(String role) {
            PdfName name = PdfName.staticNames.Get(role);
            if (name != null) {
                return name;
            }
            name = staticRoleNames.Get(role);
            if (name != null) {
                return name;
            }
            name = new PdfName(role);
            staticRoleNames.Put(role, name);
            return name;
        }

        public virtual PdfStructElem AddKid(PdfStructElem structElem) {
            return AddKid(-1, structElem);
        }

        public virtual PdfStructElem AddKid(int index, PdfStructElem structElem) {
            AddKidObject(index, structElem.GetPdfObject());
            return structElem;
        }

        public virtual IStructureNode GetParent() {
            return null;
        }

        /// <summary>Gets list of the direct kids of StructTreeRoot.</summary>
        /// <remarks>
        /// Gets list of the direct kids of StructTreeRoot.
        /// If certain kid is flushed, there will be a
        /// <see langword="null"/>
        /// in the list on it's place.
        /// </remarks>
        /// <returns>list of the direct kids of StructTreeRoot.</returns>
        public virtual IList<IStructureNode> GetKids() {
            PdfObject k = GetPdfObject().Get(PdfName.K);
            IList<IStructureNode> kids = new List<IStructureNode>();
            if (k != null) {
                if (k.IsArray()) {
                    PdfArray a = (PdfArray)k;
                    for (int i = 0; i < a.Size(); i++) {
                        IfKidIsStructElementAddToList(a.Get(i), kids);
                    }
                }
                else {
                    IfKidIsStructElementAddToList(k, kids);
                }
            }
            return kids;
        }

        public virtual PdfArray GetKidsObject() {
            PdfArray k = null;
            PdfObject kObj = GetPdfObject().Get(PdfName.K);
            if (kObj != null && kObj.IsArray()) {
                k = (PdfArray)kObj;
            }
            if (k == null) {
                k = new PdfArray();
                GetPdfObject().Put(PdfName.K, k);
                SetModified();
                if (kObj != null) {
                    k.Add(kObj);
                }
            }
            return k;
        }

        public virtual void AddRoleMapping(String fromRole, String toRole) {
            PdfDictionary roleMap = GetRoleMap();
            PdfObject prevVal = roleMap.Put(ConvertRoleToPdfName(fromRole), ConvertRoleToPdfName(toRole));
            if (prevVal != null && prevVal is PdfName) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagging.PdfStructTreeRoot));
                logger.LogWarning(String.Format(iText.IO.Logs.IoLogMessageConstant.MAPPING_IN_STRUCT_ROOT_OVERWRITTEN, fromRole
                    , prevVal, toRole));
            }
            if (roleMap.IsIndirect()) {
                roleMap.SetModified();
            }
            else {
                SetModified();
            }
        }

        public virtual PdfDictionary GetRoleMap() {
            PdfDictionary roleMap = GetPdfObject().GetAsDictionary(PdfName.RoleMap);
            if (roleMap == null) {
                roleMap = new PdfDictionary();
                GetPdfObject().Put(PdfName.RoleMap, roleMap);
                SetModified();
            }
            return roleMap;
        }

        /// <summary>Gets namespaces used within the document.</summary>
        /// <remarks>
        /// Gets namespaces used within the document. Essentially this method returns value of
        /// <see cref="GetNamespacesObject()"/>
        /// wrapped in the
        /// <see cref="PdfNamespace"/>
        /// and
        /// <see cref="System.Collections.IList{E}"/>
        /// classes. Therefore limitations of the referred method are
        /// applied to this method too.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="PdfNamespace"/>
        /// s used within the document.
        /// </returns>
        public virtual IList<PdfNamespace> GetNamespaces() {
            PdfArray namespacesArray = GetPdfObject().GetAsArray(PdfName.Namespaces);
            if (namespacesArray == null) {
                return JavaCollectionsUtil.EmptyList<PdfNamespace>();
            }
            else {
                IList<PdfNamespace> namespacesList = new List<PdfNamespace>(namespacesArray.Size());
                for (int i = 0; i < namespacesArray.Size(); ++i) {
                    namespacesList.Add(new PdfNamespace(namespacesArray.GetAsDictionary(i)));
                }
                return namespacesList;
            }
        }

        /// <summary>
        /// Adds a
        /// <see cref="PdfNamespace"/>
        /// to the list of the namespaces used within the document.
        /// </summary>
        /// <remarks>
        /// Adds a
        /// <see cref="PdfNamespace"/>
        /// to the list of the namespaces used within the document.
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// </remarks>
        /// <param name="namespace">
        /// a
        /// <see cref="PdfNamespace"/>
        /// to be added.
        /// </param>
        public virtual void AddNamespace(PdfNamespace @namespace) {
            GetNamespacesObject().Add(@namespace.GetPdfObject());
            SetModified();
        }

        /// <summary>An array of namespaces used within the document.</summary>
        /// <remarks>
        /// An array of namespaces used within the document. This value, however, is not automatically updated while
        /// the document is processed. It identifies only the namespaces that were in the document at the moment of it's
        /// opening.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of namespaces used within the document.
        /// </returns>
        public virtual PdfArray GetNamespacesObject() {
            PdfArray namespacesArray = GetPdfObject().GetAsArray(PdfName.Namespaces);
            if (namespacesArray == null) {
                namespacesArray = new PdfArray();
                VersionConforming.ValidatePdfVersionForDictEntry(GetDocument(), PdfVersion.PDF_2_0, PdfName.Namespaces, PdfName
                    .StructTreeRoot);
                GetPdfObject().Put(PdfName.Namespaces, namespacesArray);
                SetModified();
            }
            return namespacesArray;
        }

        /// <summary>
        /// A
        /// <see cref="System.Collections.IList{E}"/>
        /// containing one or more
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
        /// objects, where each specified file
        /// is a pronunciation lexicon, which is an XML file conforming to the Pronunciation Lexicon Specification (PLS) Version 1.0.
        /// </summary>
        /// <remarks>
        /// A
        /// <see cref="System.Collections.IList{E}"/>
        /// containing one or more
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
        /// objects, where each specified file
        /// is a pronunciation lexicon, which is an XML file conforming to the Pronunciation Lexicon Specification (PLS) Version 1.0.
        /// These pronunciation lexicons may be used as pronunciation hints when the document’s content is presented via
        /// text-to-speech. Where two or more pronunciation lexicons apply to the same text, the first match – as defined by
        /// the order of entries in the array and the order of entries inside the pronunciation lexicon file – should be used.
        /// <para />
        /// See ISO 32000-2 14.9.6, "Pronunciation hints".
        /// </remarks>
        /// <returns>
        /// A
        /// <see cref="System.Collections.IList{E}"/>
        /// containing one or more
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>.
        /// </returns>
        public virtual IList<PdfFileSpec> GetPronunciationLexiconsList() {
            PdfArray pronunciationLexicons = GetPdfObject().GetAsArray(PdfName.PronunciationLexicon);
            if (pronunciationLexicons == null) {
                return JavaCollectionsUtil.EmptyList<PdfFileSpec>();
            }
            else {
                IList<PdfFileSpec> lexiconsList = new List<PdfFileSpec>(pronunciationLexicons.Size());
                for (int i = 0; i < pronunciationLexicons.Size(); ++i) {
                    lexiconsList.Add(PdfFileSpec.WrapFileSpecObject(pronunciationLexicons.Get(i)));
                }
                return lexiconsList;
            }
        }

        /// <summary>
        /// Adds a single
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
        /// object, which specifies XML file conforming to PLS.
        /// </summary>
        /// <remarks>
        /// Adds a single
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
        /// object, which specifies XML file conforming to PLS.
        /// For more info see
        /// <see cref="GetPronunciationLexiconsList()"/>.
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// </remarks>
        /// <param name="pronunciationLexiconFileSpec">
        /// a
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
        /// object, which specifies XML file conforming to PLS.
        /// </param>
        public virtual void AddPronunciationLexicon(PdfFileSpec pronunciationLexiconFileSpec) {
            PdfArray pronunciationLexicons = GetPdfObject().GetAsArray(PdfName.PronunciationLexicon);
            if (pronunciationLexicons == null) {
                pronunciationLexicons = new PdfArray();
                VersionConforming.ValidatePdfVersionForDictEntry(GetDocument(), PdfVersion.PDF_2_0, PdfName.PronunciationLexicon
                    , PdfName.StructTreeRoot);
                GetPdfObject().Put(PdfName.PronunciationLexicon, pronunciationLexicons);
            }
            pronunciationLexicons.Add(pronunciationLexiconFileSpec.GetPdfObject());
            SetModified();
        }

        /// <summary>Creates and flushes parent tree entry for the page.</summary>
        /// <remarks>
        /// Creates and flushes parent tree entry for the page.
        /// Effectively this means that new content mustn't be added to the page.
        /// </remarks>
        /// <param name="page">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// for which to create parent tree entry. Typically this page is flushed after this call.
        /// </param>
        public virtual void CreateParentTreeEntryForPage(PdfPage page) {
            GetParentTreeHandler().CreateParentTreeEntryForPage(page);
        }

        public virtual void SavePageStructParentIndexIfNeeded(PdfPage page) {
            GetParentTreeHandler().SavePageStructParentIndexIfNeeded(page);
        }

        /// <summary>Gets an unmodifiable collection of marked content references on page.</summary>
        /// <remarks>
        /// Gets an unmodifiable collection of marked content references on page.
        /// NOTE: Do not remove tags when iterating over returned collection, this could
        /// lead to the ConcurrentModificationException, because returned collection is backed by the internal list of the
        /// actual page tags.
        /// </remarks>
        /// <param name="page">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// to obtain unmodifiable collection of marked content references
        /// </param>
        /// <returns>the unmodifiable collection of marked content references on page, if no Mcrs defined returns null
        ///     </returns>
        public virtual ICollection<PdfMcr> GetPageMarkedContentReferences(PdfPage page) {
            ParentTreeHandler.PageMcrsContainer pageMcrs = GetParentTreeHandler().GetPageMarkedContentReferences(page);
            return pageMcrs != null ? JavaCollectionsUtil.UnmodifiableCollection(pageMcrs.GetAllMcrsAsCollection()) : 
                null;
        }

        public virtual PdfMcr FindMcrByMcid(PdfDictionary pageDict, int mcid) {
            return GetParentTreeHandler().FindMcrByMcid(pageDict, mcid);
        }

        public virtual PdfMcr FindMcrByMcid(PdfDocument document, int mcid) {
            int amountOfPages = document.GetNumberOfPages();
            for (int i = 1; i <= amountOfPages; ++i) {
                PdfPage page = document.GetPage(i);
                PdfMcr mcr = FindMcrByMcid(page.GetPdfObject(), mcid);
                if (mcr != null) {
                    return mcr;
                }
            }
            return null;
        }

        public virtual PdfObjRef FindObjRefByStructParentIndex(PdfDictionary pageDict, int structParentIndex) {
            return GetParentTreeHandler().FindObjRefByStructParentIndex(pageDict, structParentIndex);
        }

        public virtual PdfName GetRole() {
            return null;
        }

        public override void Flush() {
            for (int i = 0; i < GetDocument().GetNumberOfPages(); ++i) {
                CreateParentTreeEntryForPage(GetDocument().GetPage(i + 1));
            }
            GetPdfObject().Put(PdfName.ParentTree, GetParentTreeHandler().BuildParentTree());
            GetPdfObject().Put(PdfName.ParentTreeNextKey, new PdfNumber((int)GetDocument().GetNextStructParentIndex())
                );
            if (this.idTree != null && this.idTree.IsModified()) {
                GetPdfObject().Put(PdfName.IDTree, this.idTree.BuildTree().MakeIndirect(GetDocument()));
            }
            if (!GetDocument().IsAppendMode()) {
                iText.Kernel.Pdf.Tagging.PdfStructTreeRoot.FlushAllKids(this);
            }
            base.Flush();
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>.
        /// </summary>
        /// <remarks>
        /// Copies structure to a
        /// <paramref name="destDocument"/>.
        /// NOTE: Works only for
        /// <see cref="PdfStructTreeRoot"/>
        /// that is read from the document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </remarks>
        /// <param name="destDocument">document to copy structure to. Shall not be current document.</param>
        /// <param name="page2page">association between original page and copied page.</param>
        public virtual void CopyTo(PdfDocument destDocument, IDictionary<PdfPage, PdfPage> page2page) {
            StructureTreeCopier.CopyTo(destDocument, page2page, GetDocument());
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>
        /// and insert it in a specified position in the document.
        /// </summary>
        /// <remarks>
        /// Copies structure to a
        /// <paramref name="destDocument"/>
        /// and insert it in a specified position in the document.
        /// NOTE: Works only for
        /// <see cref="PdfStructTreeRoot"/>
        /// that is read from the document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </remarks>
        /// <param name="destDocument">document to copy structure to.</param>
        /// <param name="insertBeforePage">indicates where the structure to be inserted.</param>
        /// <param name="page2page">association between original page and copied page.</param>
        public virtual void CopyTo(PdfDocument destDocument, int insertBeforePage, IDictionary<PdfPage, PdfPage> page2page
            ) {
            StructureTreeCopier.CopyTo(destDocument, insertBeforePage, page2page, GetDocument());
        }

        /// <summary>Moves structure associated with specified page and insert it in a specified position in the document.
        ///     </summary>
        /// <remarks>
        /// Moves structure associated with specified page and insert it in a specified position in the document.
        /// <para />
        /// NOTE: Works only for document with not flushed pages.
        /// </remarks>
        /// <param name="fromPage">page which tag structure will be moved</param>
        /// <param name="insertBeforePage">indicates before tags of which page tag structure will be moved to</param>
        public virtual void Move(PdfPage fromPage, int insertBeforePage) {
            for (int i = 1; i <= GetDocument().GetNumberOfPages(); ++i) {
                if (GetDocument().GetPage(i).IsFlushed()) {
                    throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_MOVE_PAGES_IN_PARTLY_FLUSHED_DOCUMENT
                        , i));
                }
            }
            StructureTreeCopier.Move(GetDocument(), fromPage, insertBeforePage);
        }

        public virtual int GetParentTreeNextKey() {
            // /ParentTreeNextKey entry is always inited on ParentTreeHandler initialization
            return GetPdfObject().GetAsNumber(PdfName.ParentTreeNextKey).IntValue();
        }

        public virtual int GetNextMcidForPage(PdfPage page) {
            return GetParentTreeHandler().GetNextMcidForPage(page);
        }

        public virtual PdfDocument GetDocument() {
            return document;
        }

        /// <summary>Adds file associated with structure tree root and identifies the relationship between them.</summary>
        /// <remarks>
        /// Adds file associated with structure tree root and identifies the relationship between them.
        /// <para />
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the structure tree root dictionary.
        /// If description is provided, it also will add file description to catalog Names tree.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="description">the file description</param>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(String description, PdfFileSpec fs) {
            if (null == ((PdfDictionary)fs.GetPdfObject()).Get(PdfName.AFRelationship)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagging.PdfStructTreeRoot));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ASSOCIATED_FILE_SPEC_SHALL_INCLUDE_AFRELATIONSHIP);
            }
            if (null != description) {
                GetDocument().GetCatalog().GetNameTree(PdfName.EmbeddedFiles).AddEntry(description, fs.GetPdfObject());
            }
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null) {
                afArray = new PdfArray();
                GetPdfObject().Put(PdfName.AF, afArray);
            }
            afArray.Add(fs.GetPdfObject());
        }

        /// <summary>
        /// <para />
        /// Adds file associated with structure tree root and identifies the relationship between them.
        /// </summary>
        /// <remarks>
        /// <para />
        /// Adds file associated with structure tree root and identifies the relationship between them.
        /// <para />
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the structure tree root dictionary.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(PdfFileSpec fs) {
            AddAssociatedFile(null, fs);
        }

        /// <summary>Returns files associated with structure tree root.</summary>
        /// <param name="create">defines whether AF arrays will be created if it doesn't exist</param>
        /// <returns>associated files array</returns>
        public virtual PdfArray GetAssociatedFiles(bool create) {
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null && create) {
                afArray = new PdfArray();
                GetPdfObject().Put(PdfName.AF, afArray);
            }
            return afArray;
        }

        /// <summary>
        /// Returns the document's structure element ID tree wrapped in a
        /// <see cref="PdfStructIdTree"/>
        /// object.
        /// </summary>
        /// <remarks>
        /// Returns the document's structure element ID tree wrapped in a
        /// <see cref="PdfStructIdTree"/>
        /// object. If no such tree exists, it is initialized. The initialization happens lazily,
        /// and does not trigger any PDF object changes unless populated.
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="PdfStructIdTree"/>
        /// of the document
        /// </returns>
        public virtual PdfStructIdTree GetIdTree() {
            if (this.idTree == null) {
                // Attempt to parse the ID tree in the document if there is one
                PdfDictionary idTreeDict = this.GetPdfObject().GetAsDictionary(PdfName.IDTree);
                if (idTreeDict == null) {
                    // No tree found -> initialise one
                    // Don't call setModified() here, registering the first ID will
                    // take care of that for us.
                    // The ID tree will be registered at flush time.
                    this.idTree = new PdfStructIdTree(document);
                }
                else {
                    this.idTree = PdfStructIdTree.ReadFromDictionary(document, idTreeDict);
                }
            }
            return this.idTree;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual ParentTreeHandler GetParentTreeHandler() {
            return parentTreeHandler;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddKidObject(int index, PdfDictionary structElem) {
            if (index == -1) {
                GetKidsObject().Add(structElem);
            }
            else {
                GetKidsObject().Add(index, structElem);
            }
            if (PdfStructElem.IsStructElem(structElem)) {
                if (GetPdfObject().GetIndirectReference() == null) {
                    throw new PdfException(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_DICTIONARY_SHALL_BE_AN_INDIRECT_OBJECT_IN_ORDER_TO_HAVE_CHILDREN
                        );
                }
                structElem.Put(PdfName.P, GetPdfObject());
            }
            SetModified();
        }
//\endcond

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private static void FlushAllKids(iText.Kernel.Pdf.Tagging.PdfStructTreeRoot elem) {
            TagTreeIterator iterator = new TagTreeIterator(elem, new TagTreeIteratorAvoidDuplicatesApprover(), TagTreeIterator.TreeTraversalOrder
                .POST_ORDER);
            iterator.AddHandler(new TagTreeIteratorFlusher());
            iterator.Traverse();
        }

        private void IfKidIsStructElementAddToList(PdfObject kid, IList<IStructureNode> kids) {
            if (kid.IsFlushed()) {
                kids.Add(null);
            }
            else {
                if (kid.IsDictionary() && PdfStructElem.IsStructElem((PdfDictionary)kid)) {
                    kids.Add(new PdfStructElem((PdfDictionary)kid));
                }
            }
        }
    }
}
