/*

This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using Common.Logging;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Collection;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Navigation;

namespace iText.Kernel.Pdf {
    /// <summary>The root of a document’s object hierarchy.</summary>
    public class PdfCatalog : PdfObjectWrapper<PdfDictionary> {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfCatalog));

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

        private const String OutlineRoot = "Outlines";

        private PdfOutline outlines;

        //This HashMap contents all pages of the document and outlines associated to them
        private IDictionary<PdfObject, IList<PdfOutline>> pagesWithOutlines = new Dictionary<PdfObject, IList<PdfOutline
            >>();

        //This flag determines if Outline tree of the document has been built via calling getOutlines method. If this flag is false all outline operations will be ignored
        private bool outlineMode;

        private static readonly ICollection<PdfName> PAGE_MODES = JavaCollectionsUtil.UnmodifiableSet(new HashSet<
            PdfName>(JavaUtil.ArraysAsList(PdfName.UseNone, PdfName.UseOutlines, PdfName.UseThumbs, PdfName.FullScreen
            , PdfName.UseOC, PdfName.UseAttachments)));

        private static readonly ICollection<PdfName> PAGE_LAYOUTS = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <PdfName>(JavaUtil.ArraysAsList(PdfName.SinglePage, PdfName.OneColumn, PdfName.TwoColumnLeft, PdfName.
            TwoColumnRight, PdfName.TwoPageLeft, PdfName.TwoPageRight)));

        /// <summary>
        /// Create
        /// <see cref="PdfCatalog"/>
        /// dictionary.
        /// </summary>
        /// <param name="pdfObject">the dictionary to be wrapped</param>
        protected internal PdfCatalog(PdfDictionary pdfObject)
            : base(pdfObject) {
            if (pdfObject == null) {
                throw new PdfException(PdfException.DocumentHasNoPdfCatalogObject);
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
        /// , etc),
        /// then you should address directly those objects, e.g.:
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
            ILog logger = LogManager.GetLogger(typeof(PdfDocument));
            logger.Warn("PdfCatalog cannot be flushed manually");
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

        /// <summary>Get page mode of the document.</summary>
        /// <returns>
        /// current instance of
        /// <see cref="PdfCatalog"/>
        /// </returns>
        public virtual PdfName GetPageMode() {
            return GetPdfObject().GetAsName(PdfName.PageMode);
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

        /// <summary>Get page layout of the document.</summary>
        /// <returns>name object of page layout that shall be used when document is opened</returns>
        public virtual PdfName GetPageLayout() {
            return GetPdfObject().GetAsName(PdfName.PageLayout);
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

        /// <summary>Get natural language.</summary>
        /// <returns>natural language</returns>
        public virtual PdfString GetLang() {
            return GetPdfObject().GetAsString(PdfName.Lang);
        }

        /// <summary>
        /// Add an extensions dictionary containing developer prefix identification and version
        /// numbers for developer extensions that occur in this document.
        /// </summary>
        /// <remarks>
        /// Add an extensions dictionary containing developer prefix identification and version
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
            else {
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
            }
            extensions.Put(extension.GetPrefix(), extension.GetDeveloperExtensions());
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

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>
        /// True indicates that getOCProperties() was called, may have been modified,
        /// and thus its dictionary needs to be reconstructed.
        /// </summary>
        /// <returns>boolean indicating if the dictionary needs to be reconstructed</returns>
        protected internal virtual bool IsOCPropertiesMayHaveChanged() {
            return ocProperties != null;
        }

        internal virtual PdfPagesTree GetPageTree() {
            return pageTree;
        }

        /// <summary>this method return map containing all pages of the document with associated outlines.</summary>
        /// <returns>map containing all pages of the document with associated outlines</returns>
        internal virtual IDictionary<PdfObject, IList<PdfOutline>> GetPagesWithOutlines() {
            return pagesWithOutlines;
        }

        /// <summary>This methods adds new name to the Dests NameTree.</summary>
        /// <remarks>This methods adds new name to the Dests NameTree. It throws an exception, if the name already exists.
        ///     </remarks>
        /// <param name="key">Name of the destination.</param>
        /// <param name="value">
        /// An object destination refers to. Must be an array or a dictionary with key /D and array.
        /// See ISO 32000-1 12.3.2.3 for more info.
        /// </param>
        internal virtual void AddNamedDestination(String key, PdfObject value) {
            AddNameToNameTree(key, value, PdfName.Dests);
        }

        /// <summary>This methods adds a new name to the specified NameTree.</summary>
        /// <remarks>This methods adds a new name to the specified NameTree. It throws an exception, if the name already exists.
        ///     </remarks>
        /// <param name="key">key in the name tree</param>
        /// <param name="value">value in the name tree</param>
        /// <param name="treeType">type of the tree (Dests, AP, EmbeddedFiles etc).</param>
        internal virtual void AddNameToNameTree(String key, PdfObject value, PdfName treeType) {
            GetNameTree(treeType).AddEntry(key, value);
        }

        /// <summary>This method returns a complete outline tree of the whole document.</summary>
        /// <param name="updateOutlines">
        /// if the flag is true, the method read the whole document and creates outline tree.
        /// If false the method gets cached outline tree (if it was cached via calling getOutlines method before).
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
                ConstructOutlines(outlineRoot, destsTree.GetNames());
            }
            return outlines;
        }

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

        /// <summary>This flag determines if Outline tree of the document has been built via calling getOutlines method.
        ///     </summary>
        /// <remarks>This flag determines if Outline tree of the document has been built via calling getOutlines method. If this flag is false all outline operations will be ignored
        ///     </remarks>
        /// <returns>state of outline mode.</returns>
        internal virtual bool IsOutlineMode() {
            return outlineMode;
        }

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

        internal virtual PdfDestination CopyDestination(PdfObject dest, IDictionary<PdfPage, PdfPage> page2page, PdfDocument
             toDocument) {
            if (dest == null) return null;
            PdfDestination d = null;
            if (dest.IsArray()) {
                PdfObject pageObject = ((PdfArray)dest).Get(0);
                foreach (PdfPage oldPage in page2page.Keys) {
                    if (oldPage.GetPdfObject() == pageObject) {
                        // in the copiedArray old page ref will be correctly replaced by the new page ref as this page is already copied
                        PdfArray copiedArray = (PdfArray)dest.CopyTo(toDocument, false);
                        d = new PdfExplicitDestination(copiedArray);
                        break;
                    }
                }
            }
            else {
                if (dest.IsString() || dest.IsName()) {
                    PdfNameTree destsTree = GetNameTree(PdfName.Dests);
                    IDictionary<String, PdfObject> dests = destsTree.GetNames();
                    String srcDestName = dest.IsString() ? ((PdfString)dest).ToUnicodeString() : ((PdfName)dest).GetValue();
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
                                    // in the copiedArray old page ref will be correctly replaced by the new page ref as this page is already copied
                                    PdfArray copiedArray = (PdfArray)srcDestArray.CopyTo(toDocument, false);
                                    // here we can safely replace first item of the array because array of NamedDestination or StringDestination
                                    // never refers to page in another document via PdfNumber, but should always refer to page within current document
                                    // via page object reference.
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

        private bool IsEqualSameNameDestExist(IDictionary<PdfPage, PdfPage> page2page, PdfDocument toDocument, String
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

        private void AddOutlineToPage(PdfOutline outline, IDictionary<String, PdfObject> names) {
            PdfObject pageObj = outline.GetDestination().GetDestinationPage(names);
            if (pageObj is PdfNumber) {
                int pageNumber = ((PdfNumber)pageObj).IntValue() + 1;
                try {
                    pageObj = GetDocument().GetPage(pageNumber).GetPdfObject();
                }
                catch (IndexOutOfRangeException) {
                    pageObj = null;
                    LOGGER.Warn(MessageFormatUtil.Format(iText.IO.LogMessageConstant.OUTLINE_DESTINATION_PAGE_NUMBER_IS_OUT_OF_BOUNDS
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

        /// <summary>Get the next outline of the current node in the outline tree by looking for a child or sibling node.
        ///     </summary>
        /// <remarks>
        /// Get the next outline of the current node in the outline tree by looking for a child or sibling node.
        /// If there is no child or sibling of the current node
        /// <see cref="GetParentNextOutline(PdfDictionary)"/>
        /// is called to get a hierarchical parent's next node.
        /// <see langword="null"/>
        /// is returned if one does not exist.
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="PdfDictionary"/>
        /// object of the next outline if one exists,
        /// <see langword="null"/>
        /// otherwise.
        /// </returns>
        private PdfDictionary GetNextOutline(PdfDictionary first, PdfDictionary next, PdfDictionary parent) {
            if (first != null) {
                return first;
            }
            else {
                if (next != null) {
                    return next;
                }
                else {
                    return GetParentNextOutline(parent);
                }
            }
        }

        /// <summary>Gets the parent's next outline of the current node.</summary>
        /// <remarks>
        /// Gets the parent's next outline of the current node.
        /// If the parent does not have a next we look at the grand parent, great-grand parent, etc until we find a next node or reach the root at which point
        /// <see langword="null"/>
        /// is returned to signify there is no next node present.
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="PdfDictionary"/>
        /// object of the next outline if one exists,
        /// <see langword="null"/>
        /// otherwise.
        /// </returns>
        private PdfDictionary GetParentNextOutline(PdfDictionary parent) {
            if (parent == null) {
                return null;
            }
            PdfDictionary current = null;
            while (current == null) {
                current = parent.GetAsDictionary(PdfName.Next);
                if (current == null) {
                    parent = parent.GetAsDictionary(PdfName.Parent);
                    if (parent == null) {
                        return null;
                    }
                }
            }
            return current;
        }

        private void AddOutlineToPage(PdfOutline outline, PdfDictionary item, IDictionary<String, PdfObject> names
            ) {
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

        /// <summary>
        /// Constructs
        /// <see cref="outlines"/>
        /// iteratively
        /// </summary>
        private void ConstructOutlines(PdfDictionary outlineRoot, IDictionary<String, PdfObject> names) {
            if (outlineRoot == null) {
                return;
            }
            PdfDictionary first = outlineRoot.GetAsDictionary(PdfName.First);
            PdfDictionary current = first;
            PdfDictionary next;
            PdfDictionary parent;
            Dictionary<PdfDictionary, PdfOutline> parentOutlineMap = new Dictionary<PdfDictionary, PdfOutline>();
            outlines = new PdfOutline(OutlineRoot, outlineRoot, GetDocument());
            PdfOutline parentOutline = outlines;
            parentOutlineMap.Put(outlineRoot, parentOutline);
            while (current != null) {
                first = current.GetAsDictionary(PdfName.First);
                next = current.GetAsDictionary(PdfName.Next);
                parent = current.GetAsDictionary(PdfName.Parent);
                parentOutline = parentOutlineMap.Get(parent);
                PdfOutline currentOutline = new PdfOutline(current.GetAsString(PdfName.Title).ToUnicodeString(), current, 
                    parentOutline);
                AddOutlineToPage(currentOutline, current, names);
                parentOutline.GetAllChildren().Add(currentOutline);
                if (first != null) {
                    parentOutlineMap.Put(current, currentOutline);
                }
                current = GetNextOutline(first, next, parent);
            }
        }
    }
}
