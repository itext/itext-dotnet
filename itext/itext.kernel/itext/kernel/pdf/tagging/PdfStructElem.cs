/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Validation.Context;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>A wrapper for structure element dictionaries (ISO-32000 14.7.2 "Structure Hierarchy").</summary>
    /// <remarks>
    /// A wrapper for structure element dictionaries (ISO-32000 14.7.2 "Structure Hierarchy").
    /// <para />
    /// The logical structure of a document shall be described by a hierarchy of objects called
    /// the structure hierarchy or structure tree. At the root of the hierarchy shall be a dictionary object
    /// called the structure tree root (see
    /// <see cref="PdfStructTreeRoot"/>
    /// ). Immediate children of the structure tree root
    /// are structure elements. Structure elements are other structure elements or content items.
    /// </remarks>
    public class PdfStructElem : PdfObjectWrapper<PdfDictionary>, IStructureNode {
        public PdfStructElem(PdfDictionary pdfObject)
            : base(pdfObject) {
            SetForbidRelease();
        }

        public PdfStructElem(PdfDocument document, PdfName role, PdfPage page)
            : this(document, role) {
            // Explicitly using object indirect reference here in order to correctly process released objects.
            GetPdfObject().Put(PdfName.Pg, page.GetPdfObject().GetIndirectReference());
        }

        public PdfStructElem(PdfDocument document, PdfName role, PdfAnnotation annot)
            : this(document, role) {
            if (annot.GetPage() == null) {
                throw new PdfException(KernelExceptionMessageConstant.ANNOTATION_SHALL_HAVE_REFERENCE_TO_PAGE);
            }
            // Explicitly using object indirect reference here in order to correctly process released objects.
            GetPdfObject().Put(PdfName.Pg, annot.GetPage().GetPdfObject().GetIndirectReference());
        }

        public PdfStructElem(PdfDocument document, PdfName role)
            : this((PdfDictionary)new PdfDictionary().MakeIndirect(document)) {
            GetPdfObject().Put(PdfName.Type, PdfName.StructElem);
            GetPdfObject().Put(PdfName.S, role);
        }

        /// <summary>Method to distinguish struct elements from other elements of the logical tree (like mcr or struct tree root).
        ///     </summary>
        /// <param name="dictionary">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// to check on containing struct elements
        /// </param>
        /// <returns>
        /// if the type of
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// is StructElem or
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// contains the required key S
        /// then true, otherwise false
        /// </returns>
        public static bool IsStructElem(PdfDictionary dictionary) {
            // S is required key of the struct elem
            return (PdfName.StructElem.Equals(dictionary.GetAsName(PdfName.Type)) || dictionary.ContainsKey(PdfName.S)
                );
        }

        /// <summary>Gets attributes object.</summary>
        /// <param name="createNewIfNull">
        /// sometimes attributes object may not exist.
        /// Pass
        /// <see langword="true"/>
        /// if you want to create empty dictionary in such case.
        /// The attributes dictionary will be stored inside element.
        /// </param>
        /// <returns>attributes dictionary.</returns>
        public virtual PdfObject GetAttributes(bool createNewIfNull) {
            PdfObject attributes = GetPdfObject().Get(PdfName.A);
            if (attributes == null && createNewIfNull) {
                attributes = new PdfDictionary();
                SetAttributes(attributes);
            }
            return attributes;
        }

        public virtual void SetAttributes(PdfObject attributes) {
            Put(PdfName.A, attributes);
        }

        public virtual PdfString GetLang() {
            return GetPdfObject().GetAsString(PdfName.Lang);
        }

        public virtual void SetLang(PdfString lang) {
            Put(PdfName.Lang, lang);
        }

        public virtual PdfString GetAlt() {
            return GetPdfObject().GetAsString(PdfName.Alt);
        }

        public virtual void SetAlt(PdfString alt) {
            Put(PdfName.Alt, alt);
        }

        public virtual PdfString GetActualText() {
            return GetPdfObject().GetAsString(PdfName.ActualText);
        }

        public virtual void SetActualText(PdfString actualText) {
            Put(PdfName.ActualText, actualText);
        }

        public virtual PdfString GetE() {
            return GetPdfObject().GetAsString(PdfName.E);
        }

        public virtual void SetE(PdfString e) {
            Put(PdfName.E, e);
        }

        /// <summary>Gets the structure element's ID string, if it has one.</summary>
        /// <returns>the structure element's ID string, or null if there is none</returns>
        public virtual PdfString GetStructureElementId() {
            return GetPdfObject().GetAsString(PdfName.ID);
        }

        /// <summary>Sets the structure element's ID string.</summary>
        /// <remarks>
        /// Sets the structure element's ID string.
        /// This value can be used by other structure elements to reference this one.
        /// </remarks>
        /// <param name="id">the element's ID string to be set</param>
        public virtual void SetStructureElementId(PdfString id) {
            PdfStructIdTree idTree = GetDocument().GetStructTreeRoot().GetIdTree();
            if (id == null) {
                PdfObject orig = GetPdfObject().Remove(PdfName.ID);
                if (orig is PdfString) {
                    idTree.RemoveEntry((PdfString)orig);
                }
            }
            else {
                PdfObject orig = GetPdfObject().Get(PdfName.ID);
                if (id.Equals(orig)) {
                    // nothing to do, the ID is already set to the appropriate value
                    return;
                }
                if (orig is PdfString) {
                    idTree.RemoveEntry((PdfString)orig);
                }
                idTree.AddEntry(id, this.GetPdfObject());
                GetPdfObject().Put(PdfName.ID, id);
            }
        }

        public virtual PdfName GetRole() {
            return GetPdfObject().GetAsName(PdfName.S);
        }

        public virtual void SetRole(PdfName role) {
            Put(PdfName.S, role);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfStructElem AddKid(iText.Kernel.Pdf.Tagging.PdfStructElem kid) {
            return AddKid(-1, kid);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfStructElem AddKid(int index, iText.Kernel.Pdf.Tagging.PdfStructElem
             kid) {
            AddKidObject(GetPdfObject(), index, kid.GetPdfObject());
            return kid;
        }

        public virtual PdfMcr AddKid(PdfMcr kid) {
            return AddKid(-1, kid);
        }

        public virtual PdfMcr AddKid(int index, PdfMcr kid) {
            GetDocEnsureIndirectForKids().GetStructTreeRoot().GetParentTreeHandler().RegisterMcr(kid);
            AddKidObject(GetPdfObject(), index, kid.GetPdfObject());
            return kid;
        }

        public virtual IStructureNode RemoveKid(int index) {
            return RemoveKid(index, false);
        }

        public virtual IStructureNode RemoveKid(int index, bool prepareForReAdding) {
            PdfObject k = GetK();
            if (k == null || !k.IsArray() && index != 0) {
                throw new IndexOutOfRangeException();
            }
            if (k.IsArray()) {
                PdfArray kidsArray = (PdfArray)k;
                k = kidsArray.Get(index);
                kidsArray.Remove(index);
                if (kidsArray.IsEmpty()) {
                    GetPdfObject().Remove(PdfName.K);
                }
            }
            else {
                GetPdfObject().Remove(PdfName.K);
            }
            SetModified();
            IStructureNode removedKid = ConvertPdfObjectToIPdfStructElem(k);
            PdfDocument doc = GetDocument();
            if (removedKid is PdfMcr && doc != null && !prepareForReAdding) {
                doc.GetStructTreeRoot().GetParentTreeHandler().UnregisterMcr((PdfMcr)removedKid);
            }
            return removedKid;
        }

        public virtual int RemoveKid(IStructureNode kid) {
            if (kid is PdfMcr) {
                PdfMcr mcr = (PdfMcr)kid;
                PdfDocument doc = GetDocument();
                if (doc != null) {
                    doc.GetStructTreeRoot().GetParentTreeHandler().UnregisterMcr(mcr);
                }
                return RemoveKidObject(mcr.GetPdfObject());
            }
            else {
                if (kid is iText.Kernel.Pdf.Tagging.PdfStructElem) {
                    return RemoveKidObject(((iText.Kernel.Pdf.Tagging.PdfStructElem)kid).GetPdfObject());
                }
            }
            return -1;
        }

        /// <returns>parent of the current structure element. Returns null if parent isn't set or if either current element or parent are invalid.
        ///     </returns>
        public virtual IStructureNode GetParent() {
            PdfDictionary parent = GetPdfObject().GetAsDictionary(PdfName.P);
            if (parent == null) {
                return null;
            }
            if (parent.IsFlushed()) {
                PdfDocument pdfDoc = GetDocument();
                if (pdfDoc == null) {
                    return null;
                }
                PdfStructTreeRoot structTreeRoot = pdfDoc.GetStructTreeRoot();
                return structTreeRoot.GetPdfObject() == parent ? (IStructureNode)structTreeRoot : new iText.Kernel.Pdf.Tagging.PdfStructElem
                    (parent);
            }
            if (IsStructElem(parent)) {
                return new iText.Kernel.Pdf.Tagging.PdfStructElem(parent);
            }
            else {
                PdfDocument pdfDoc = GetDocument();
                bool parentIsRoot = pdfDoc != null && PdfName.StructTreeRoot.Equals(parent.GetAsName(PdfName.Type));
                parentIsRoot = parentIsRoot || pdfDoc != null && pdfDoc.GetStructTreeRoot().GetPdfObject() == parent;
                if (parentIsRoot) {
                    return pdfDoc.GetStructTreeRoot();
                }
                else {
                    return null;
                }
            }
        }

        /// <summary>Gets list of the direct kids of structure element.</summary>
        /// <remarks>
        /// Gets list of the direct kids of structure element.
        /// If certain kid is flushed, there will be a
        /// <see langword="null"/>
        /// in the list on it's place.
        /// </remarks>
        /// <returns>list of the direct kids of structure element.</returns>
        public virtual IList<IStructureNode> GetKids() {
            PdfObject k = GetK();
            IList<IStructureNode> kids = new List<IStructureNode>();
            if (k != null) {
                if (k.IsArray()) {
                    PdfArray a = (PdfArray)k;
                    for (int i = 0; i < a.Size(); i++) {
                        AddKidObjectToStructElemList(a.Get(i), kids);
                    }
                }
                else {
                    AddKidObjectToStructElemList(k, kids);
                }
            }
            return kids;
        }

        public virtual PdfObject GetK() {
            return GetPdfObject().Get(PdfName.K);
        }

        /// <summary>
        /// A
        /// <see cref="iText.Kernel.Pdf.PdfName.Ref"/>
        /// identifies the structure element or elements to which the item of content, contained
        /// within this structure element, refers (e.g. footnotes, endnotes, sidebars, etc.).
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IList{E}"/>
        /// &lt;
        /// <see cref="PdfStructElem"/>
        /// &gt; containing zero, one or more structure elements.
        /// </returns>
        public virtual IList<iText.Kernel.Pdf.Tagging.PdfStructElem> GetRefsList() {
            PdfArray refsArray = GetPdfObject().GetAsArray(PdfName.Ref);
            if (refsArray == null) {
                return JavaCollectionsUtil.EmptyList<iText.Kernel.Pdf.Tagging.PdfStructElem>();
            }
            else {
                IList<iText.Kernel.Pdf.Tagging.PdfStructElem> refs = new List<iText.Kernel.Pdf.Tagging.PdfStructElem>(refsArray
                    .Size());
                for (int i = 0; i < refsArray.Size(); ++i) {
                    refs.Add(new iText.Kernel.Pdf.Tagging.PdfStructElem(refsArray.GetAsDictionary(i)));
                }
                return refs;
            }
        }

        /// <summary>
        /// A
        /// <see cref="iText.Kernel.Pdf.PdfName.Ref"/>
        /// identifies the structure element to which the item of content, contained
        /// within this structure element, refers (e.g. footnotes, endnotes, sidebars, etc.).
        /// </summary>
        /// <remarks>
        /// A
        /// <see cref="iText.Kernel.Pdf.PdfName.Ref"/>
        /// identifies the structure element to which the item of content, contained
        /// within this structure element, refers (e.g. footnotes, endnotes, sidebars, etc.).
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// </remarks>
        /// <param name="ref">
        /// a
        /// <see cref="PdfStructElem"/>
        /// to which the item of content, contained within this structure element, refers.
        /// </param>
        public virtual void AddRef(iText.Kernel.Pdf.Tagging.PdfStructElem @ref) {
            if (!@ref.GetPdfObject().IsIndirect()) {
                throw new PdfException(KernelExceptionMessageConstant.REF_ARRAY_ITEMS_IN_STRUCTURE_ELEMENT_DICTIONARY_SHALL_BE_INDIRECT_OBJECTS
                    );
            }
            VersionConforming.ValidatePdfVersionForDictEntry(GetDocument(), PdfVersion.PDF_2_0, PdfName.Ref, PdfName.StructElem
                );
            PdfArray refsArray = GetPdfObject().GetAsArray(PdfName.Ref);
            if (refsArray == null) {
                refsArray = new PdfArray();
                Put(PdfName.Ref, refsArray);
            }
            refsArray.Add(@ref.GetPdfObject());
            SetModified();
        }

        /// <summary>A namespace this element belongs to (see ISO 32000-2 14.7.4, "Namespaces").</summary>
        /// <remarks>
        /// A namespace this element belongs to (see ISO 32000-2 14.7.4, "Namespaces"). If not present, the
        /// element shall be considered to be in the default standard structure namespace.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="PdfNamespace"/>
        /// this element belongs to.
        /// </returns>
        public virtual PdfNamespace GetNamespace() {
            PdfDictionary nsDict = GetPdfObject().GetAsDictionary(PdfName.NS);
            return nsDict != null ? new PdfNamespace(nsDict) : null;
        }

        /// <summary>A namespace this element belongs to (see ISO 32000-2 14.7.4, "Namespaces").</summary>
        /// <remarks>
        /// A namespace this element belongs to (see ISO 32000-2 14.7.4, "Namespaces").
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// </remarks>
        /// <param name="namespace">
        /// a
        /// <see cref="PdfNamespace"/>
        /// this element belongs to, or null if element is desired to be considered
        /// in the default standard structure namespace.
        /// </param>
        public virtual void SetNamespace(PdfNamespace @namespace) {
            VersionConforming.ValidatePdfVersionForDictEntry(GetDocument(), PdfVersion.PDF_2_0, PdfName.NS, PdfName.StructElem
                );
            if (@namespace != null) {
                Put(PdfName.NS, @namespace.GetPdfObject());
            }
            else {
                GetPdfObject().Remove(PdfName.NS);
                SetModified();
            }
        }

        /// <summary>Attribute for a structure element that may be used as pronunciation hint.</summary>
        /// <remarks>
        /// Attribute for a structure element that may be used as pronunciation hint. It is an exact replacement for content
        /// enclosed by the structure element and its children.
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// </remarks>
        /// <param name="elementPhoneme">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which defines an exact replacement for content enclosed by the structure
        /// element and its children. This value is to be interpreted based on the PhoneticAlphabet attribute in effect.
        /// </param>
        public virtual void SetPhoneme(PdfString elementPhoneme) {
            VersionConforming.ValidatePdfVersionForDictEntry(GetDocument(), PdfVersion.PDF_2_0, PdfName.Phoneme, PdfName
                .StructElem);
            Put(PdfName.Phoneme, elementPhoneme);
        }

        /// <summary>Attribute for a structure element that may be used as pronunciation hint.</summary>
        /// <remarks>
        /// Attribute for a structure element that may be used as pronunciation hint. It is an exact replacement for content
        /// enclosed by the structure element and its children.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which defines an exact replacement for content enclosed by the structure
        /// element and its children. This value is to be interpreted based on the PhoneticAlphabet attribute in effect.
        /// </returns>
        public virtual PdfString GetPhoneme() {
            return GetPdfObject().GetAsString(PdfName.Phoneme);
        }

        /// <summary>
        /// Attribute for a structure element that indicates the phonetic alphabet used by a
        /// <see cref="iText.Kernel.Pdf.PdfName.Phoneme"/>
        /// attribute.
        /// </summary>
        /// <remarks>
        /// Attribute for a structure element that indicates the phonetic alphabet used by a
        /// <see cref="iText.Kernel.Pdf.PdfName.Phoneme"/>
        /// attribute.
        /// Applies to the structure element and its children, except where overridden by a child structure element.
        /// <para />
        /// This value has meaning only for the PDF documents of version <b>2.0 and higher</b>.
        /// </remarks>
        /// <param name="phoneticAlphabet">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// which defines phonetic alphabet used by a
        /// <see cref="iText.Kernel.Pdf.PdfName.Phoneme"/>
        /// attribute. Possible values are:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.ipa"/>
        /// for the International Phonetic Alphabet by the International Phonetic Association;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.x_sampa"/>
        /// for Extended Speech Assessment Methods Phonetic Alphabet (X-SAMPA);
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.zh_Latn_pinyin"/>
        /// for Pinyin Latin romanization (Mandarin);
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.zh_Latn_wadegile"/>
        /// for Wade-Giles romanization (Mandarin).
        /// </description></item>
        /// </list>
        /// Other values may be used.
        /// </param>
        public virtual void SetPhoneticAlphabet(PdfName phoneticAlphabet) {
            VersionConforming.ValidatePdfVersionForDictEntry(GetDocument(), PdfVersion.PDF_2_0, PdfName.PhoneticAlphabet
                , PdfName.StructElem);
            Put(PdfName.PhoneticAlphabet, phoneticAlphabet);
        }

        /// <summary>
        /// Attribute for a structure element that indicates the phonetic alphabet used by a
        /// <see cref="iText.Kernel.Pdf.PdfName.Phoneme"/>
        /// attribute.
        /// </summary>
        /// <remarks>
        /// Attribute for a structure element that indicates the phonetic alphabet used by a
        /// <see cref="iText.Kernel.Pdf.PdfName.Phoneme"/>
        /// attribute.
        /// Applies to the structure element and its children, except where overridden by a child structure element.
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// which defines phonetic alphabet used by a
        /// <see cref="iText.Kernel.Pdf.PdfName.Phoneme"/>
        /// , or null if not defined,
        /// default value
        /// <see cref="iText.Kernel.Pdf.PdfName.ipa"/>
        /// . See
        /// <see cref="SetPhoneticAlphabet(iText.Kernel.Pdf.PdfName)"/>
        /// for other possible values.
        /// </returns>
        public virtual PdfName GetPhoneticAlphabet() {
            return GetPdfObject().GetAsName(PdfName.PhoneticAlphabet);
        }

        /// <summary>Adds file associated with structure element and identifies the relationship between them.</summary>
        /// <remarks>
        /// Adds file associated with structure element and identifies the relationship between them.
        /// <para />
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the structure element dictionary.
        /// If description is provided, it also will add file description to catalog Names tree.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="description">the file description</param>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(String description, PdfFileSpec fs) {
            if (null == ((PdfDictionary)fs.GetPdfObject()).Get(PdfName.AFRelationship)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagging.PdfStructElem));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ASSOCIATED_FILE_SPEC_SHALL_INCLUDE_AFRELATIONSHIP);
            }
            if (null != description) {
                GetDocument().GetCatalog().GetNameTree(PdfName.EmbeddedFiles).AddEntry(description, fs.GetPdfObject());
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
        /// Adds file associated with structure element and identifies the relationship between them.
        /// </summary>
        /// <remarks>
        /// <para />
        /// Adds file associated with structure element and identifies the relationship between them.
        /// <para />
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the structure element dictionary.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(PdfFileSpec fs) {
            AddAssociatedFile(null, fs);
        }

        /// <summary>Returns files associated with structure element.</summary>
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

        public virtual iText.Kernel.Pdf.Tagging.PdfStructElem Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        public override void Flush() {
            PdfDictionary pageDict = GetPdfObject().GetAsDictionary(PdfName.Pg);
            if (pageDict == null || pageDict.GetIndirectReference() != null && pageDict.GetIndirectReference().IsFree(
                )) {
                GetPdfObject().Remove(PdfName.Pg);
            }
            PdfDocument doc = GetDocument();
            if (doc != null) {
                doc.CheckIsoConformance(new TagStructElementValidationContext(GetPdfObject()));
            }
            base.Flush();
        }

//\cond DO_NOT_DOCUMENT
        internal static void AddKidObject(PdfDictionary parent, int index, PdfObject kid) {
            if (parent.IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_ADD_KID_TO_THE_FLUSHED_ELEMENT);
            }
            if (!parent.ContainsKey(PdfName.P)) {
                throw new PdfException(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_SHALL_CONTAIN_PARENT_OBJECT, parent
                    );
            }
            PdfObject k = parent.Get(PdfName.K);
            if (k == null) {
                parent.Put(PdfName.K, kid);
            }
            else {
                PdfArray a;
                if (k is PdfArray) {
                    a = (PdfArray)k;
                }
                else {
                    a = new PdfArray();
                    a.Add(k);
                    parent.Put(PdfName.K, a);
                }
                if (index == -1) {
                    a.Add(kid);
                }
                else {
                    a.Add(index, kid);
                }
            }
            parent.SetModified();
            if (kid is PdfDictionary && IsStructElem((PdfDictionary)kid)) {
                if (!parent.IsIndirect()) {
                    throw new PdfException(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_DICTIONARY_SHALL_BE_AN_INDIRECT_OBJECT_IN_ORDER_TO_HAVE_CHILDREN
                        );
                }
                ((PdfDictionary)kid).Put(PdfName.P, parent);
                kid.SetModified();
            }
        }
//\endcond

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        protected internal virtual PdfDocument GetDocument() {
            PdfDictionary structDict = GetPdfObject();
            PdfIndirectReference indRef = structDict.GetIndirectReference();
            if (indRef == null && structDict.GetAsDictionary(PdfName.P) != null) {
                // If parent is direct - it's definitely an invalid structure tree.
                // MustBeIndirect state won't be met during reading, and all newly created struct elements shall have ind ref.
                indRef = structDict.GetAsDictionary(PdfName.P).GetIndirectReference();
            }
            return indRef != null ? indRef.GetDocument() : null;
        }

        private PdfDocument GetDocEnsureIndirectForKids() {
            PdfDocument doc = GetDocument();
            if (doc == null) {
                throw new PdfException(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_DICTIONARY_SHALL_BE_AN_INDIRECT_OBJECT_IN_ORDER_TO_HAVE_CHILDREN
                    );
            }
            return doc;
        }

        private void AddKidObjectToStructElemList(PdfObject k, IList<IStructureNode> list) {
            if (k.IsFlushed()) {
                list.Add(null);
                return;
            }
            list.Add(ConvertPdfObjectToIPdfStructElem(k));
        }

        private IStructureNode ConvertPdfObjectToIPdfStructElem(PdfObject obj) {
            IStructureNode elem = null;
            switch (obj.GetObjectType()) {
                case PdfObject.DICTIONARY: {
                    PdfDictionary d = (PdfDictionary)obj;
                    if (IsStructElem(d)) {
                        elem = new iText.Kernel.Pdf.Tagging.PdfStructElem(d);
                    }
                    else {
                        if (PdfName.MCR.Equals(d.GetAsName(PdfName.Type))) {
                            elem = new PdfMcrDictionary(d, this);
                        }
                        else {
                            if (PdfName.OBJR.Equals(d.GetAsName(PdfName.Type))) {
                                elem = new PdfObjRef(d, this);
                            }
                        }
                    }
                    break;
                }

                case PdfObject.NUMBER: {
                    elem = new PdfMcrNumber((PdfNumber)obj, this);
                    break;
                }

                default: {
                    break;
                }
            }
            return elem;
        }

        private int RemoveKidObject(PdfObject kid) {
            PdfObject k = GetK();
            if (k == null || !k.IsArray() && k != kid && k != kid.GetIndirectReference()) {
                return -1;
            }
            int removedIndex = -1;
            if (k.IsArray()) {
                PdfArray kidsArray = (PdfArray)k;
                removedIndex = RemoveObjectFromArray(kidsArray, kid);
            }
            if (!k.IsArray() || k.IsArray() && ((PdfArray)k).IsEmpty()) {
                GetPdfObject().Remove(PdfName.K);
                removedIndex = 0;
            }
            SetModified();
            return removedIndex;
        }

        private static int RemoveObjectFromArray(PdfArray array, PdfObject toRemove) {
            int i;
            for (i = 0; i < array.Size(); ++i) {
                PdfObject obj = array.Get(i);
                if (obj == toRemove || obj == toRemove.GetIndirectReference()) {
                    array.Remove(i);
                    break;
                }
            }
            return i;
        }
    }
}
