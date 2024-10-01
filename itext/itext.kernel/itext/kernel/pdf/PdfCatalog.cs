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
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Collection;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>The root of a document’s object hierarchy.</summary>
    public class PdfCatalog : PdfObjectWrapper<PdfDictionary> {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfCatalog));

        private const String ROOT_OUTLINE_TITLE = "Outlines";

        private static readonly ICollection<PdfName> PAGE_MODES = JavaCollectionsUtil.UnmodifiableSet(new HashSet<
            PdfName>(JavaUtil.ArraysAsList(PdfName.UseNone, PdfName.UseOutlines, PdfName.UseThumbs, PdfName.FullScreen
            , PdfName.UseOC, PdfName.UseAttachments)));

        private static readonly ICollection<PdfName> PAGE_LAYOUTS = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <PdfName>(JavaUtil.ArraysAsList(PdfName.SinglePage, PdfName.OneColumn, PdfName.TwoColumnLeft, PdfName.
            TwoColumnRight, PdfName.TwoPageLeft, PdfName.TwoPageRight)));

        private readonly PdfPagesTree pageTree;

        /// <summary>
        /// Map of the
        /// <see cref="PdfNameTree"/>.
        /// </summary>
        /// <remarks>
        /// Map of the
        /// <see cref="PdfNameTree"/>
        /// . Used for creation
        /// <c>name tree</c>
        /// dictionary.
        /// </remarks>
        protected internal IDictionary<PdfName, PdfNameTree> nameTrees = new LinkedDictionary<PdfName, PdfNameTree
            >();

        /// <summary>Defining the page labelling for the document.</summary>
        protected internal PdfNumTree pageLabels;

        /// <summary>The document’s optional content properties dictionary.</summary>
        protected internal PdfOCProperties ocProperties;

        private PdfOutline outlines;

        //This HashMap contents all pages of the document and outlines associated to them
        private readonly IDictionary<PdfObject, IList<PdfOutline>> pagesWithOutlines = new Dictionary<PdfObject, IList
            <PdfOutline>>();

        //This flag determines if Outline tree of the document has been built via calling getOutlines method.
        // If this flag is false all outline operations will be ignored
        private bool outlineMode;

        private bool ocgCopied = false;

        /// <summary>
        /// Create
        /// <see cref="PdfCatalog"/>
        /// dictionary.
        /// </summary>
        /// <param name="pdfObject">the dictionary to be wrapped</param>
        protected internal PdfCatalog(PdfDictionary pdfObject)
            : base(pdfObject) {
            if (pdfObject == null) {
                throw new PdfException(KernelExceptionMessageConstant.DOCUMENT_HAS_NO_PDF_CATALOG_OBJECT);
            }
            EnsureObjectIsAddedToDocument(pdfObject);
            GetPdfObject().Put(PdfName.Type, PdfName.Catalog);
            SetForbidRelease();
            pageTree = new PdfPagesTree(this);
        }

        /// <summary>
        /// Create
        /// <see cref="PdfCatalog"/>
        /// to
        /// <see cref="PdfDocument"/>.
        /// </summary>
        /// <param name="pdfDocument">
        /// A
        /// <see cref="PdfDocument"/>
        /// object representing the document
        /// to which redaction applies
        /// </param>
        protected internal PdfCatalog(PdfDocument pdfDocument)
            : this((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDocument)) {
        }

        /// <summary>Use this method to get the <b>Optional Content Properties Dictionary</b>.</summary>
        /// <remarks>
        /// Use this method to get the <b>Optional Content Properties Dictionary</b>.
        /// Note that if you call this method, then the
        /// <see cref="PdfDictionary"/>
        /// with OCProperties will be
        /// generated from
        /// <see cref="iText.Kernel.Pdf.Layer.PdfOCProperties"/>
        /// object right before closing the
        /// <see cref="PdfDocument"/>
        /// ,
        /// so if you want to make low-level changes in Pdf structures themselves (
        /// <see cref="PdfArray"/>
        /// ,
        /// <see cref="PdfDictionary"/>
        /// , etc), then you should address directly those objects, e.g.:
        /// <c>
        /// PdfCatalog pdfCatalog = pdfDoc.getCatalog();
        /// PdfDictionary ocProps = pdfCatalog.getAsDictionary(PdfName.OCProperties);
        /// // manipulate with ocProps.
        /// </c>
        /// Also note that this method is implicitly called when creating a new PdfLayer instance,
        /// so you should either use hi-level logic of operating with layers,
        /// or manipulate low-level Pdf objects by yourself.
        /// </remarks>
        /// <param name="createIfNotExists">
        /// true to create new /OCProperties entry in catalog if not exists,
        /// false to return null if /OCProperties entry in catalog is not present.
        /// </param>
        /// <returns>the Optional Content Properties Dictionary</returns>
        public virtual PdfOCProperties GetOCProperties(bool createIfNotExists) {
            if (ocProperties != null) {
                return ocProperties;
            }
            else {
                PdfDictionary ocPropertiesDict = GetPdfObject().GetAsDictionary(PdfName.OCProperties);
                if (ocPropertiesDict != null) {
                    if (GetDocument().GetWriter() != null) {
                        ocPropertiesDict.MakeIndirect(GetDocument());
                    }
                    ocProperties = new PdfOCProperties(ocPropertiesDict);
                }
                else {
                    if (createIfNotExists) {
                        ocProperties = new PdfOCProperties(GetDocument());
                    }
                }
            }
            return ocProperties;
        }

        /// <summary>
        /// Get
        /// <see cref="PdfDocument"/>
        /// with indirect reference associated with the object.
        /// </summary>
        /// <returns>the resultant dictionary</returns>
        public virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        /// <summary>PdfCatalog will be flushed in PdfDocument.close().</summary>
        /// <remarks>PdfCatalog will be flushed in PdfDocument.close(). User mustn't flush PdfCatalog!</remarks>
        public override void Flush() {
            ILogger logger = ITextLogManager.GetLogger(typeof(PdfDocument));
            logger.LogWarning("PdfCatalog cannot be flushed manually");
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>A value specifying a destination that shall be displayed when the document is opened.</summary>
        /// <remarks>
        /// A value specifying a destination that shall be displayed when the document is opened.
        /// See ISO 32000-1, Table 28 – Entries in the catalog dictionary.
        /// </remarks>
        /// <param name="destination">
        /// instance of
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>.
        /// </param>
        /// <returns>destination</returns>
        public virtual iText.Kernel.Pdf.PdfCatalog SetOpenAction(PdfDestination destination) {
            return Put(PdfName.OpenAction, destination.GetPdfObject());
        }

        /// <summary>A value specifying an action that shall be performed when the document is opened.</summary>
        /// <remarks>
        /// A value specifying an action that shall be performed when the document is opened.
        /// See ISO 32000-1, Table 28 – Entries in the catalog dictionary.
        /// </remarks>
        /// <param name="action">
        /// instance of
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>.
        /// </param>
        /// <returns>action</returns>
        public virtual iText.Kernel.Pdf.PdfCatalog SetOpenAction(PdfAction action) {
            return Put(PdfName.OpenAction, action.GetPdfObject());
        }

        /// <summary>The actions that shall be taken in response to various trigger events affecting the document as a whole.
        ///     </summary>
        /// <remarks>
        /// The actions that shall be taken in response to various trigger events affecting the document as a whole.
        /// See ISO 32000-1, Table 28 – Entries in the catalog dictionary.
        /// </remarks>
        /// <param name="key">the key of which the associated value needs to be returned</param>
        /// <param name="action">
        /// instance of
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>.
        /// </param>
        /// <returns>additional action</returns>
        public virtual iText.Kernel.Pdf.PdfCatalog SetAdditionalAction(PdfName key, PdfAction action) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        /// <summary>Get page mode of the document.</summary>
        /// <returns>
        /// current instance of
        /// <see cref="PdfCatalog"/>
        /// </returns>
        public virtual PdfName GetPageMode() {
            return GetPdfObject().GetAsName(PdfName.PageMode);
        }

        /// <summary>This method sets a page mode of the document.</summary>
        /// <remarks>
        /// This method sets a page mode of the document.
        /// <br />
        /// Valid values are:
        /// <c>PdfName.UseNone</c>
        /// ,
        /// <c>PdfName.UseOutlines</c>
        /// ,
        /// <c>PdfName.UseThumbs</c>
        /// ,
        /// <c>PdfName.FullScreen</c>
        /// ,
        /// <c>PdfName.UseOC</c>
        /// ,
        /// <c>PdfName.UseAttachments</c>.
        /// </remarks>
        /// <param name="pageMode">page mode.</param>
        /// <returns>current instance of PdfCatalog</returns>
        public virtual iText.Kernel.Pdf.PdfCatalog SetPageMode(PdfName pageMode) {
            if (PAGE_MODES.Contains(pageMode)) {
                return Put(PdfName.PageMode, pageMode);
            }
            return this;
        }

        /// <summary>Get page layout of the document.</summary>
        /// <returns>name object of page layout that shall be used when document is opened</returns>
        public virtual PdfName GetPageLayout() {
            return GetPdfObject().GetAsName(PdfName.PageLayout);
        }

        /// <summary>This method sets a page layout of the document</summary>
        /// <param name="pageLayout">page layout of the document</param>
        /// <returns>
        /// 
        /// <see cref="PdfCatalog"/>
        /// instance with applied page layout
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfCatalog SetPageLayout(PdfName pageLayout) {
            if (PAGE_LAYOUTS.Contains(pageLayout)) {
                return Put(PdfName.PageLayout, pageLayout);
            }
            return this;
        }

        /// <summary>Get viewer preferences of the document.</summary>
        /// <returns>dictionary of viewer preferences</returns>
        public virtual PdfViewerPreferences GetViewerPreferences() {
            PdfDictionary viewerPreferences = GetPdfObject().GetAsDictionary(PdfName.ViewerPreferences);
            if (viewerPreferences != null) {
                return new PdfViewerPreferences(viewerPreferences);
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// This method sets the document viewer preferences, specifying the way the document shall be displayed on the
        /// screen
        /// </summary>
        /// <param name="preferences">
        /// document's
        /// <see cref="PdfViewerPreferences">viewer preferences</see>
        /// </param>
        /// <returns>
        /// 
        /// <see cref="PdfCatalog"/>
        /// instance with applied viewer preferences
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfCatalog SetViewerPreferences(PdfViewerPreferences preferences) {
            return Put(PdfName.ViewerPreferences, preferences.GetPdfObject());
        }

        /// <summary>This method gets Names tree from the catalog.</summary>
        /// <param name="treeType">type of the tree (Dests, AP, EmbeddedFiles etc).</param>
        /// <returns>
        /// returns
        /// <see cref="PdfNameTree"/>
        /// </returns>
        public virtual PdfNameTree GetNameTree(PdfName treeType) {
            PdfNameTree tree = nameTrees.Get(treeType);
            if (tree == null) {
                tree = new PdfNameTree(this, treeType);
                nameTrees.Put(treeType, tree);
            }
            return tree;
        }

        /// <summary>This method checks Names tree for specified tree type.</summary>
        /// <param name="treeType">type of tree which existence should be checked</param>
        /// <returns>true if such tree exists, false otherwise</returns>
        public virtual bool NameTreeContainsKey(PdfName treeType) {
            return nameTrees.ContainsKey(treeType);
        }

        /// <summary>This method returns the NumberTree of Page Labels</summary>
        /// <param name="createIfNotExists">
        /// defines whether the NumberTree of Page Labels should be created
        /// if it didn't exist before
        /// </param>
        /// <returns>
        /// returns
        /// <see cref="PdfNumTree"/>
        /// </returns>
        public virtual PdfNumTree GetPageLabelsTree(bool createIfNotExists) {
            if (pageLabels == null && (GetPdfObject().ContainsKey(PdfName.PageLabels) || createIfNotExists)) {
                pageLabels = new PdfNumTree(this, PdfName.PageLabels);
            }
            return pageLabels;
        }

        /// <summary>Get natural language.</summary>
        /// <returns>natural language</returns>
        public virtual PdfString GetLang() {
            return GetPdfObject().GetAsString(PdfName.Lang);
        }

        /// <summary>An entry specifying the natural language, and optionally locale.</summary>
        /// <remarks>
        /// An entry specifying the natural language, and optionally locale. Use this
        /// to specify the Language attribute on a Tagged Pdf element.
        /// For the content usage dictionary, use PdfName.Language
        /// </remarks>
        /// <param name="lang">
        /// 
        /// <see cref="PdfString">language</see>
        /// to be set
        /// </param>
        public virtual void SetLang(PdfString lang) {
            Put(PdfName.Lang, lang);
        }

        /// <summary>
        /// Adds an extensions dictionary containing developer prefix identification and version
        /// numbers for developer extensions that occur in this document.
        /// </summary>
        /// <remarks>
        /// Adds an extensions dictionary containing developer prefix identification and version
        /// numbers for developer extensions that occur in this document.
        /// See ISO 32000-1, Table 28 – Entries in the catalog dictionary.
        /// </remarks>
        /// <param name="extension">
        /// enables developers to identify their own extension
        /// relative to a base version of PDF
        /// </param>
        public virtual void AddDeveloperExtension(PdfDeveloperExtension extension) {
            PdfDictionary extensions = GetPdfObject().GetAsDictionary(PdfName.Extensions);
            if (extensions == null) {
                extensions = new PdfDictionary();
                Put(PdfName.Extensions, extensions);
            }
            if (extension.IsMultiValued()) {
                // for multivalued extensions, we only check whether one of the same level is present or not
                // (main use case: ISO extensions)
                PdfArray existingExtensionArray = extensions.GetAsArray(extension.GetPrefix());
                if (existingExtensionArray == null) {
                    existingExtensionArray = new PdfArray();
                    extensions.Put(extension.GetPrefix(), existingExtensionArray);
                }
                else {
                    for (int i = 0; i < existingExtensionArray.Size(); i++) {
                        PdfDictionary pdfDict = existingExtensionArray.GetAsDictionary(i);
                        // for array-based extensions, we check for membership only, since comparison doesn't make sense
                        if (pdfDict.GetAsNumber(PdfName.ExtensionLevel).IntValue() == extension.GetExtensionLevel()) {
                            return;
                        }
                    }
                }
                existingExtensionArray.Add(extension.GetDeveloperExtensions());
                existingExtensionArray.SetModified();
            }
            else {
                // for single-valued extensions, we compare against the existing extension level
                PdfDictionary existingExtensionDict = extensions.GetAsDictionary(extension.GetPrefix());
                if (existingExtensionDict != null) {
                    int diff = extension.GetBaseVersion().CompareTo(existingExtensionDict.GetAsName(PdfName.BaseVersion));
                    if (diff < 0) {
                        return;
                    }
                    diff = extension.GetExtensionLevel() - existingExtensionDict.GetAsNumber(PdfName.ExtensionLevel).IntValue(
                        );
                    if (diff <= 0) {
                        return;
                    }
                }
                extensions.Put(extension.GetPrefix(), extension.GetDeveloperExtensions());
            }
        }

        /// <summary>
        /// Removes an extensions dictionary containing developer prefix identification and version
        /// numbers for developer extensions that do not occur in this document.
        /// </summary>
        /// <remarks>
        /// Removes an extensions dictionary containing developer prefix identification and version
        /// numbers for developer extensions that do not occur in this document.
        /// See ISO 32000-1, Table 28 – Entries in the catalog dictionary.
        /// </remarks>
        /// <param name="extension">developer extension to be removed from the document</param>
        public virtual void RemoveDeveloperExtension(PdfDeveloperExtension extension) {
            PdfDictionary extensions = GetPdfObject().GetAsDictionary(PdfName.Extensions);
            if (extensions == null) {
                return;
            }
            if (extension.IsMultiValued()) {
                PdfArray existingExtensionArray = extensions.GetAsArray(extension.GetPrefix());
                if (existingExtensionArray == null) {
                    return;
                }
                for (int i = 0; i < existingExtensionArray.Size(); i++) {
                    PdfDictionary pdfDict = existingExtensionArray.GetAsDictionary(i);
                    // for array-based extensions, we check for membership only, since comparison doesn't make sense
                    if (pdfDict.GetAsNumber(PdfName.ExtensionLevel).IntValue() == extension.GetExtensionLevel()) {
                        existingExtensionArray.Remove(i);
                        existingExtensionArray.SetModified();
                        return;
                    }
                }
            }
            else {
                extensions.Remove(extension.GetPrefix());
            }
        }

        /// <summary>
        /// Gets collection dictionary that a conforming reader shall use to enhance the presentation of file attachments
        /// stored in the PDF document.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Collection.PdfCollection"/>
        /// wrapper of collection dictionary.
        /// </returns>
        public virtual PdfCollection GetCollection() {
            PdfDictionary collectionDictionary = GetPdfObject().GetAsDictionary(PdfName.Collection);
            if (collectionDictionary != null) {
                return new PdfCollection(collectionDictionary);
            }
            return null;
        }

        /// <summary>
        /// Sets collection dictionary that a conforming reader shall use to enhance the presentation of file attachments
        /// stored in the PDF document.
        /// </summary>
        /// <param name="collection">
        /// 
        /// <see cref="iText.Kernel.Pdf.Collection.PdfCollection">dictionary</see>
        /// </param>
        /// <returns>
        /// 
        /// <see cref="PdfCatalog"/>
        /// instance with applied collection dictionary
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfCatalog SetCollection(PdfCollection collection) {
            Put(PdfName.Collection, collection.GetPdfObject());
            return this;
        }

        /// <summary>
        /// Add key and value to
        /// <see cref="PdfCatalog"/>
        /// dictionary.
        /// </summary>
        /// <param name="key">the dictionary key corresponding with the PDF object</param>
        /// <param name="value">the value of key</param>
        /// <returns>the key and value</returns>
        public virtual iText.Kernel.Pdf.PdfCatalog Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        /// <summary>Remove key from catalog dictionary.</summary>
        /// <param name="key">the dictionary key corresponding with the PDF object</param>
        /// <returns>the key</returns>
        public virtual iText.Kernel.Pdf.PdfCatalog Remove(PdfName key) {
            GetPdfObject().Remove(key);
            SetModified();
            return this;
        }

        /// <summary>
        /// True indicates that getOCProperties() was called, may have been modified,
        /// and thus its dictionary needs to be reconstructed.
        /// </summary>
        /// <returns>boolean indicating if the dictionary needs to be reconstructed</returns>
        protected internal virtual bool IsOCPropertiesMayHaveChanged() {
            return ocProperties != null || ocgCopied;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void SetOcgCopied(bool ocgCopied) {
            this.ocgCopied = ocgCopied;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual PdfPagesTree GetPageTree() {
            return pageTree;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>this method return map containing all pages of the document with associated outlines.</summary>
        /// <returns>map containing all pages of the document with associated outlines</returns>
        internal virtual IDictionary<PdfObject, IList<PdfOutline>> GetPagesWithOutlines() {
            return pagesWithOutlines;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>This methods adds new name to the Dests NameTree.</summary>
        /// <remarks>This methods adds new name to the Dests NameTree. It throws an exception, if the name already exists.
        ///     </remarks>
        /// <param name="key">Name of the destination.</param>
        /// <param name="value">
        /// An object destination refers to. Must be an array or a dictionary with key /D and array.
        /// See ISO 32000-1 12.3.2.3 for more info.
        /// </param>
        internal virtual void AddNamedDestination(PdfString key, PdfObject value) {
            AddNameToNameTree(key, value, PdfName.Dests);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>This methods adds a new name to the specified NameTree.</summary>
        /// <remarks>This methods adds a new name to the specified NameTree. It throws an exception, if the name already exists.
        ///     </remarks>
        /// <param name="key">key in the name tree</param>
        /// <param name="value">value in the name tree</param>
        /// <param name="treeType">type of the tree (Dests, AP, EmbeddedFiles etc).</param>
        internal virtual void AddNameToNameTree(PdfString key, PdfObject value, PdfName treeType) {
            GetNameTree(treeType).AddEntry(key, value);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>This method returns a complete outline tree of the whole document.</summary>
        /// <param name="updateOutlines">
        /// if the flag is true, the method read the whole document and creates outline tree.
        /// If false the method gets cached outline tree (if it was cached via calling
        /// getOutlines method before).
        /// </param>
        /// <returns>
        /// fully initialized
        /// <see cref="PdfOutline"/>
        /// object.
        /// </returns>
        internal virtual PdfOutline GetOutlines(bool updateOutlines) {
            if (outlines != null && !updateOutlines) {
                return outlines;
            }
            if (outlines != null) {
                outlines.Clear();
                pagesWithOutlines.Clear();
            }
            outlineMode = true;
            PdfNameTree destsTree = GetNameTree(PdfName.Dests);
            PdfDictionary outlineRoot = GetPdfObject().GetAsDictionary(PdfName.Outlines);
            if (outlineRoot == null) {
                if (null == GetDocument().GetWriter()) {
                    return null;
                }
                outlines = new PdfOutline(GetDocument());
            }
            else {
                ConstructOutlines(outlineRoot, destsTree);
            }
            return outlines;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Indicates if the catalog has any outlines</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// , if there are outlines and
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        internal virtual bool HasOutlines() {
            return GetPdfObject().ContainsKey(PdfName.Outlines);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>This flag determines if Outline tree of the document has been built via calling getOutlines method.
        ///     </summary>
        /// <remarks>
        /// This flag determines if Outline tree of the document has been built via calling getOutlines method.
        /// If this flag is false all outline operations will be ignored
        /// </remarks>
        /// <returns>state of outline mode.</returns>
        internal virtual bool IsOutlineMode() {
            return outlineMode;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>This method removes all outlines associated with a given page</summary>
        /// <param name="page">the page to remove outlines</param>
        internal virtual void RemoveOutlines(PdfPage page) {
            if (GetDocument().GetWriter() == null) {
                return;
            }
            if (HasOutlines()) {
                GetOutlines(false);
                if (pagesWithOutlines.Count > 0) {
                    if (pagesWithOutlines.Get(page.GetPdfObject()) != null) {
                        foreach (PdfOutline outline in pagesWithOutlines.Get(page.GetPdfObject())) {
                            outline.RemoveOutline();
                        }
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>This method sets the root outline element in the catalog.</summary>
        /// <param name="outline">the outline dictionary that shall be the root of the document’s outline hierarchy</param>
        internal virtual void AddRootOutline(PdfOutline outline) {
            if (!outlineMode) {
                return;
            }
            if (pagesWithOutlines.Count == 0) {
                Put(PdfName.Outlines, outline.GetContent());
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Construct
        /// <see cref="PdfCatalog">dictionary</see>
        /// iteratively.
        /// </summary>
        /// <remarks>
        /// Construct
        /// <see cref="PdfCatalog">dictionary</see>
        /// iteratively. Invalid pdf documents will be processed depending on
        /// <see cref="StrictnessLevel"/>
        /// , if it set to lenient, we will ignore and process invalid outline structure, otherwise
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>
        /// will be thrown.
        /// </remarks>
        /// <param name="outlineRoot">
        /// 
        /// <see cref="PdfOutline">dictionary</see>
        /// root.
        /// </param>
        /// <param name="names">map containing the PdfObjects stored in the tree.</param>
        internal virtual void ConstructOutlines(PdfDictionary outlineRoot, IPdfNameTreeAccess names) {
            if (outlineRoot == null) {
                return;
            }
            PdfReader reader = GetDocument().GetReader();
            bool isLenientLevel = reader == null || PdfReader.StrictnessLevel.CONSERVATIVE.IsStricter(reader.GetStrictnessLevel
                ());
            PdfDictionary current = outlineRoot.GetAsDictionary(PdfName.First);
            outlines = new PdfOutline(ROOT_OUTLINE_TITLE, outlineRoot, GetDocument());
            PdfOutline parentOutline = outlines;
            IDictionary<PdfOutline, PdfDictionary> nextUnprocessedChildForParentMap = new Dictionary<PdfOutline, PdfDictionary
                >();
            ICollection<PdfDictionary> alreadyVisitedOutlinesSet = new HashSet<PdfDictionary>();
            while (current != null) {
                PdfDictionary parent = current.GetAsDictionary(PdfName.Parent);
                if (null == parent && !isLenientLevel) {
                    throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.CORRUPTED_OUTLINE_NO_PARENT_ENTRY
                        , current.indirectReference));
                }
                PdfString title = current.GetAsString(PdfName.Title);
                if (null == title) {
                    throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.CORRUPTED_OUTLINE_NO_TITLE_ENTRY
                        , current.indirectReference));
                }
                PdfOutline currentOutline = new PdfOutline(title.ToUnicodeString(), current, parentOutline);
                alreadyVisitedOutlinesSet.Add(current);
                AddOutlineToPage(currentOutline, current, names);
                parentOutline.GetAllChildren().Add(currentOutline);
                PdfDictionary first = current.GetAsDictionary(PdfName.First);
                PdfDictionary next = current.GetAsDictionary(PdfName.Next);
                if (first != null) {
                    if (alreadyVisitedOutlinesSet.Contains(first)) {
                        if (!isLenientLevel) {
                            throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.CORRUPTED_OUTLINE_DICTIONARY_HAS_INFINITE_LOOP
                                , first));
                        }
                        LOGGER.LogWarning(MessageFormatUtil.Format(KernelLogMessageConstant.CORRUPTED_OUTLINE_DICTIONARY_HAS_INFINITE_LOOP
                            , first));
                        return;
                    }
                    // Down in hierarchy; when returning up, process `next`.
                    nextUnprocessedChildForParentMap.Put(parentOutline, next);
                    parentOutline = currentOutline;
                    current = first;
                }
                else {
                    if (next != null) {
                        if (alreadyVisitedOutlinesSet.Contains(next)) {
                            if (!isLenientLevel) {
                                throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.CORRUPTED_OUTLINE_DICTIONARY_HAS_INFINITE_LOOP
                                    , next));
                            }
                            LOGGER.LogWarning(MessageFormatUtil.Format(KernelLogMessageConstant.CORRUPTED_OUTLINE_DICTIONARY_HAS_INFINITE_LOOP
                                , next));
                            return;
                        }
                        // Next sibling in hierarchy
                        current = next;
                    }
                    else {
                        // Up in hierarchy using 'nextUnprocessedChildForParentMap'.
                        current = null;
                        while (current == null && parentOutline != null) {
                            parentOutline = parentOutline.GetParent();
                            if (parentOutline != null) {
                                current = nextUnprocessedChildForParentMap.Get(parentOutline);
                            }
                        }
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual PdfDestination CopyDestination(PdfObject dest, IDictionary<PdfPage, PdfPage> page2page, PdfDocument
             toDocument) {
            if (null == dest) {
                return null;
            }
            PdfDestination d = null;
            if (dest.IsArray()) {
                PdfObject pageObject = ((PdfArray)dest).Get(0);
                foreach (PdfPage oldPage in page2page.Keys) {
                    if (oldPage.GetPdfObject() == pageObject) {
                        // in the copiedArray old page ref will be correctly replaced by the new page ref
                        // as this page is already copied
                        PdfArray copiedArray = (PdfArray)dest.CopyTo(toDocument, false, NullCopyFilter.GetInstance());
                        d = new PdfExplicitDestination(copiedArray);
                        break;
                    }
                }
            }
            else {
                if (dest.IsString() || dest.IsName()) {
                    PdfNameTree destsTree = GetNameTree(PdfName.Dests);
                    IDictionary<PdfString, PdfObject> dests = destsTree.GetNames();
                    PdfString srcDestName = dest.IsString() ? (PdfString)dest : new PdfString(((PdfName)dest).GetValue());
                    PdfArray srcDestArray = (PdfArray)dests.Get(srcDestName);
                    if (srcDestArray != null) {
                        PdfObject pageObject = srcDestArray.Get(0);
                        if (pageObject is PdfNumber) {
                            pageObject = GetDocument().GetPage(((PdfNumber)pageObject).IntValue() + 1).GetPdfObject();
                        }
                        foreach (PdfPage oldPage in page2page.Keys) {
                            if (oldPage.GetPdfObject() == pageObject) {
                                d = new PdfStringDestination(srcDestName);
                                if (!IsEqualSameNameDestExist(page2page, toDocument, srcDestName, srcDestArray, oldPage)) {
                                    // in the copiedArray old page ref will be correctly replaced by the new page ref as this
                                    // page is already copied
                                    PdfArray copiedArray = (PdfArray)srcDestArray.CopyTo(toDocument, false);
                                    // here we can safely replace first item of the array because array of NamedDestination or
                                    // StringDestination never refers to page in another document via PdfNumber, but should
                                    // always refer to page within current document via page object reference.
                                    copiedArray.Set(0, page2page.Get(oldPage).GetPdfObject());
                                    toDocument.AddNamedDestination(srcDestName, copiedArray);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return d;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual PdfDictionary FillAndGetOcPropertiesDictionary() {
            if (ocProperties != null) {
                ocProperties.FillDictionary(false);
                GetPdfObject().Put(PdfName.OCProperties, ocProperties.GetPdfObject());
                ocProperties = null;
            }
            if (GetPdfObject().GetAsDictionary(PdfName.OCProperties) == null) {
                PdfDictionary pdfDictionary = new PdfDictionary();
                pdfDictionary.MakeIndirect(GetDocument());
                GetDocument().GetCatalog().GetPdfObject().Put(PdfName.OCProperties, pdfDictionary);
            }
            return GetPdfObject().GetAsDictionary(PdfName.OCProperties);
        }
//\endcond

        private bool IsEqualSameNameDestExist(IDictionary<PdfPage, PdfPage> page2page, PdfDocument toDocument, PdfString
             srcDestName, PdfArray srcDestArray, PdfPage oldPage) {
            PdfArray sameNameDest = (PdfArray)toDocument.GetCatalog().GetNameTree(PdfName.Dests).GetNames().Get(srcDestName
                );
            bool equalSameNameDestExists = false;
            if (sameNameDest != null && sameNameDest.GetAsDictionary(0) != null) {
                PdfIndirectReference existingDestPageRef = sameNameDest.GetAsDictionary(0).GetIndirectReference();
                PdfIndirectReference newDestPageRef = page2page.Get(oldPage).GetPdfObject().GetIndirectReference();
                if (equalSameNameDestExists = existingDestPageRef.Equals(newDestPageRef) && sameNameDest.Size() == srcDestArray
                    .Size()) {
                    for (int i = 1; i < sameNameDest.Size(); ++i) {
                        equalSameNameDestExists = equalSameNameDestExists && sameNameDest.Get(i).Equals(srcDestArray.Get(i));
                    }
                }
            }
            return equalSameNameDestExists;
        }

        private void AddOutlineToPage(PdfOutline outline, IPdfNameTreeAccess names) {
            PdfObject pageObj = outline.GetDestination().GetDestinationPage(names);
            if (pageObj is PdfNumber) {
                int pageNumber = ((PdfNumber)pageObj).IntValue() + 1;
                try {
                    pageObj = GetDocument().GetPage(pageNumber).GetPdfObject();
                }
                catch (IndexOutOfRangeException) {
                    pageObj = null;
                    LOGGER.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OUTLINE_DESTINATION_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                        , pageNumber));
                }
            }
            if (pageObj != null) {
                IList<PdfOutline> outs = pagesWithOutlines.Get(pageObj);
                if (outs == null) {
                    outs = new List<PdfOutline>();
                    pagesWithOutlines.Put(pageObj, outs);
                }
                outs.Add(outline);
            }
        }

        private void AddOutlineToPage(PdfOutline outline, PdfDictionary item, IPdfNameTreeAccess names) {
            PdfObject dest = item.Get(PdfName.Dest);
            if (dest != null) {
                PdfDestination destination = PdfDestination.MakeDestination(dest);
                outline.SetDestination(destination);
                AddOutlineToPage(outline, names);
            }
            else {
                //Take into account outlines that specify their destination through an action
                PdfDictionary action = item.GetAsDictionary(PdfName.A);
                if (action != null) {
                    PdfName actionType = action.GetAsName(PdfName.S);
                    //Check if it is a go to action
                    if (PdfName.GoTo.Equals(actionType)) {
                        //Retrieve destination if it is.
                        PdfObject destObject = action.Get(PdfName.D);
                        if (destObject != null) {
                            //Page is always the first object
                            PdfDestination destination = PdfDestination.MakeDestination(destObject);
                            outline.SetDestination(destination);
                            AddOutlineToPage(outline, names);
                        }
                    }
                }
            }
        }
    }
}
