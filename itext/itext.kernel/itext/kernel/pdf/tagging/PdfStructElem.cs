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
using iTextSharp.Kernel;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;

namespace iTextSharp.Kernel.Pdf.Tagging {
    /// <summary>
    /// To be able to be wrapped with this
    /// <see cref="iTextSharp.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iTextSharp.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </summary>
    public class PdfStructElem : PdfObjectWrapper<PdfDictionary>, IPdfStructElem {
        public static int Unknown = 0;

        public static int Grouping = 1;

        public static int BlockLevel = 2;

        public static int InlineLevel = 3;

        public static int Illustration = 4;

        public static ICollection<PdfName> groupingRoles = new HashSet<PdfName>();

        public static ICollection<PdfName> blockLevelRoles = new HashSet<PdfName>();

        public static ICollection<PdfName> inlineLevelRoles = new HashSet<PdfName>();

        public static ICollection<PdfName> illustrationRoles = new HashSet<PdfName>();

        static PdfStructElem() {
            groupingRoles.Add(PdfName.Document);
            groupingRoles.Add(PdfName.Part);
            groupingRoles.Add(PdfName.Art);
            groupingRoles.Add(PdfName.Sect);
            groupingRoles.Add(PdfName.Div);
            groupingRoles.Add(PdfName.BlockQuote);
            groupingRoles.Add(PdfName.Caption);
            groupingRoles.Add(PdfName.Caption);
            groupingRoles.Add(PdfName.TOC);
            groupingRoles.Add(PdfName.TOCI);
            groupingRoles.Add(PdfName.Index);
            groupingRoles.Add(PdfName.NonStruct);
            groupingRoles.Add(PdfName.Private);
            blockLevelRoles.Add(PdfName.P);
            blockLevelRoles.Add(PdfName.H);
            blockLevelRoles.Add(PdfName.H1);
            blockLevelRoles.Add(PdfName.H2);
            blockLevelRoles.Add(PdfName.H3);
            blockLevelRoles.Add(PdfName.H4);
            blockLevelRoles.Add(PdfName.H5);
            blockLevelRoles.Add(PdfName.H6);
            blockLevelRoles.Add(PdfName.L);
            blockLevelRoles.Add(PdfName.Lbl);
            blockLevelRoles.Add(PdfName.LI);
            blockLevelRoles.Add(PdfName.LBody);
            blockLevelRoles.Add(PdfName.Table);
            blockLevelRoles.Add(PdfName.TR);
            blockLevelRoles.Add(PdfName.TH);
            blockLevelRoles.Add(PdfName.TD);
            blockLevelRoles.Add(PdfName.THead);
            blockLevelRoles.Add(PdfName.TBody);
            blockLevelRoles.Add(PdfName.TFoot);
            inlineLevelRoles.Add(PdfName.Span);
            inlineLevelRoles.Add(PdfName.Quote);
            inlineLevelRoles.Add(PdfName.Note);
            inlineLevelRoles.Add(PdfName.Reference);
            inlineLevelRoles.Add(PdfName.BibEntry);
            inlineLevelRoles.Add(PdfName.Code);
            inlineLevelRoles.Add(PdfName.Link);
            inlineLevelRoles.Add(PdfName.Annot);
            inlineLevelRoles.Add(PdfName.Ruby);
            inlineLevelRoles.Add(PdfName.Warichu);
            inlineLevelRoles.Add(PdfName.RB);
            inlineLevelRoles.Add(PdfName.RT);
            inlineLevelRoles.Add(PdfName.RP);
            inlineLevelRoles.Add(PdfName.WT);
            inlineLevelRoles.Add(PdfName.WP);
            illustrationRoles.Add(PdfName.Figure);
            illustrationRoles.Add(PdfName.Formula);
            illustrationRoles.Add(PdfName.Form);
        }

        protected internal int type = Unknown;

        /// <param name="pdfObject">must be an indirect object.</param>
        public PdfStructElem(PdfDictionary pdfObject)
            : base(pdfObject) {
            EnsureObjectIsAddedToDocument(pdfObject);
            SetForbidRelease();
        }

        public PdfStructElem(PdfDocument document, PdfName role, PdfPage page)
            : this(document, role) {
            GetPdfObject().Put(PdfName.Pg, page.GetPdfObject());
        }

        public PdfStructElem(PdfDocument document, PdfName role, PdfAnnotation annot)
            : this(document, role) {
            if (annot.GetPage() == null) {
                throw new PdfException(PdfException.AnnotShallHaveReferenceToPage);
            }
            GetPdfObject().Put(PdfName.Pg, annot.GetPage().GetPdfObject());
        }

        public PdfStructElem(PdfDocument document, PdfName role)
            : this(((PdfDictionary)new PdfDictionary().MakeIndirect(document))) {
            GetPdfObject().Put(PdfName.Type, PdfName.StructElem);
            GetPdfObject().Put(PdfName.S, role);
        }

        /// <summary>Method to to distinguish struct elements from other elements of the logical tree (like mcr or struct tree root).
        ///     </summary>
        public static bool IsStructElem(PdfDictionary dictionary) {
            return (PdfName.StructElem.Equals(dictionary.GetAsName(PdfName.Type)) || dictionary.ContainsKey(PdfName.S)
                );
        }

        // required key of the struct elem
        /// <summary>Gets attributes object.</summary>
        /// <param name="createNewIfNull">
        /// sometimes attributes object may not exist.
        /// Pass
        /// <see langword="true"/>
        /// if you want to create empty dictionary in such case.
        /// The attributes dictionary will be stored inside element.
        /// </param>
        /// <returns>attributes dictionary.</returns>
        /// <exception cref="iTextSharp.Kernel.PdfException"/>
        public virtual PdfObject GetAttributes(bool createNewIfNull) {
            PdfObject attributes = GetPdfObject().Get(PdfName.A);
            if (attributes == null && createNewIfNull) {
                attributes = new PdfDictionary();
                SetAttributes(attributes);
            }
            return attributes;
        }

        public virtual void SetAttributes(PdfObject attributes) {
            GetPdfObject().Put(PdfName.A, attributes);
        }

        public virtual PdfString GetLang() {
            return GetPdfObject().GetAsString(PdfName.Lang);
        }

        public virtual void SetLang(PdfString lang) {
            GetPdfObject().Put(PdfName.Lang, lang);
        }

        public virtual PdfString GetAlt() {
            return GetPdfObject().GetAsString(PdfName.Alt);
        }

        public virtual void SetAlt(PdfString alt) {
            GetPdfObject().Put(PdfName.Alt, alt);
        }

        public virtual PdfString GetActualText() {
            return GetPdfObject().GetAsString(PdfName.ActualText);
        }

        public virtual void SetActualText(PdfString actualText) {
            GetPdfObject().Put(PdfName.ActualText, actualText);
        }

        public virtual PdfString GetE() {
            return GetPdfObject().GetAsString(PdfName.E);
        }

        public virtual void SetE(PdfString e) {
            GetPdfObject().Put(PdfName.E, e);
        }

        public virtual PdfName GetRole() {
            return GetPdfObject().GetAsName(PdfName.S);
        }

        public virtual void SetRole(PdfName role) {
            GetPdfObject().Put(PdfName.S, role);
        }

        public virtual iTextSharp.Kernel.Pdf.Tagging.PdfStructElem AddKid(iTextSharp.Kernel.Pdf.Tagging.PdfStructElem
             kid) {
            return AddKid(-1, kid);
        }

        public virtual iTextSharp.Kernel.Pdf.Tagging.PdfStructElem AddKid(int index, iTextSharp.Kernel.Pdf.Tagging.PdfStructElem
             kid) {
            if (GetStructElementType() == InlineLevel || GetStructElementType() == Illustration) {
                throw new PdfException(PdfException.InlineLevelOrIllustrationElementCannotContainKids, GetPdfObject());
            }
            AddKidObject(GetPdfObject(), index, kid.GetPdfObject());
            return kid;
        }

        public virtual PdfMcr AddKid(PdfMcr kid) {
            return AddKid(-1, kid);
        }

        public virtual PdfMcr AddKid(int index, PdfMcr kid) {
            GetDocument().GetStructTreeRoot().GetParentTreeHandler().RegisterMcr(kid);
            AddKidObject(GetPdfObject(), index, kid.GetPdfObject());
            return kid;
        }

        public virtual IPdfStructElem RemoveKid(int index) {
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
            IPdfStructElem removedKid = ConvertPdfObjectToIPdfStructElem(k);
            if (removedKid is PdfMcr) {
                GetDocument().GetStructTreeRoot().GetParentTreeHandler().UnregisterMcr((PdfMcr)removedKid);
            }
            return removedKid;
        }

        public virtual int RemoveKid(IPdfStructElem kid) {
            if (kid is PdfMcr) {
                PdfMcr mcr = (PdfMcr)kid;
                GetDocument().GetStructTreeRoot().GetParentTreeHandler().UnregisterMcr(mcr);
                return RemoveKidObject(mcr.GetPdfObject());
            }
            else {
                if (kid is iTextSharp.Kernel.Pdf.Tagging.PdfStructElem) {
                    return RemoveKidObject(((iTextSharp.Kernel.Pdf.Tagging.PdfStructElem)kid).GetPdfObject());
                }
            }
            return -1;
        }

        /// <returns>parent of the current structure element. If parent is already flushed it returns null.</returns>
        public virtual IPdfStructElem GetParent() {
            PdfDictionary parent = GetPdfObject().GetAsDictionary(PdfName.P);
            if (parent == null || parent.IsFlushed()) {
                return null;
            }
            if (IsStructElem(parent)) {
                return new iTextSharp.Kernel.Pdf.Tagging.PdfStructElem(parent);
            }
            else {
                PdfName type = parent.GetAsName(PdfName.Type);
                if (PdfName.StructTreeRoot.Equals(type)) {
                    return GetDocument().GetStructTreeRoot();
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
        public virtual IList<IPdfStructElem> GetKids() {
            PdfObject k = GetK();
            IList<IPdfStructElem> kids = new List<IPdfStructElem>();
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

        public static int IdentifyType(PdfDocument doc, PdfName role) {
            PdfDictionary roleMap = doc.GetStructTreeRoot().GetRoleMap();
            if (roleMap.ContainsKey(role)) {
                role = roleMap.GetAsName(role);
            }
            if (groupingRoles.Contains(role)) {
                return Grouping;
            }
            else {
                if (blockLevelRoles.Contains(role)) {
                    return BlockLevel;
                }
                else {
                    if (inlineLevelRoles.Contains(role)) {
                        return InlineLevel;
                    }
                    else {
                        if (illustrationRoles.Contains(role)) {
                            return Illustration;
                        }
                        else {
                            return Unknown;
                        }
                    }
                }
            }
        }

        public virtual iTextSharp.Kernel.Pdf.Tagging.PdfStructElem Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        public override void Flush() {
            //TODO log that to prevent undefined behaviour, use StructTreeRoot#flushStructElem method
            base.Flush();
        }

        protected internal virtual int GetStructElementType() {
            if (type == Unknown) {
                PdfName role = GetPdfObject().GetAsName(PdfName.S);
                type = IdentifyType(GetDocument(), role);
            }
            return type;
        }

        internal static void AddKidObject(PdfDictionary parent, int index, PdfObject kid) {
            if (parent.IsFlushed()) {
                throw new PdfException(PdfException.CannotAddKidToTheFlushedElement);
            }
            if (!parent.ContainsKey(PdfName.P)) {
                throw new PdfException(PdfException.StructureElementShallContainParentObject, parent);
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
            if (kid is PdfDictionary && IsStructElem((PdfDictionary)kid)) {
                ((PdfDictionary)kid).Put(PdfName.P, parent);
            }
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        protected internal virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        private void AddKidObjectToStructElemList(PdfObject k, IList<IPdfStructElem> list) {
            if (k.IsFlushed()) {
                list.Add(null);
                return;
            }
            list.Add(ConvertPdfObjectToIPdfStructElem(k));
        }

        private IPdfStructElem ConvertPdfObjectToIPdfStructElem(PdfObject obj) {
            if (obj.IsIndirectReference()) {
                obj = ((PdfIndirectReference)obj).GetRefersTo();
            }
            IPdfStructElem elem = null;
            switch (obj.GetObjectType()) {
                case PdfObject.DICTIONARY: {
                    PdfDictionary d = (PdfDictionary)obj;
                    if (IsStructElem(d)) {
                        elem = new iTextSharp.Kernel.Pdf.Tagging.PdfStructElem(d);
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
                if (kidsArray.IsEmpty()) {
                    GetPdfObject().Remove(PdfName.K);
                }
            }
            if (!k.IsArray() || k.IsArray() && ((PdfArray)k).IsEmpty()) {
                GetPdfObject().Remove(PdfName.K);
                removedIndex = 0;
            }
            return removedIndex;
        }

        internal static int RemoveObjectFromArray(PdfArray array, PdfObject toRemove) {
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
