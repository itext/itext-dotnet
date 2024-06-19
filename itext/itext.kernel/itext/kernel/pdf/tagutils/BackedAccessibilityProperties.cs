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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
//\cond DO_NOT_DOCUMENT
    internal class BackedAccessibilityProperties : AccessibilityProperties {
        private readonly TagTreePointer pointerToBackingElem;

//\cond DO_NOT_DOCUMENT
        internal BackedAccessibilityProperties(TagTreePointer pointerToBackingElem) {
            this.pointerToBackingElem = new TagTreePointer(pointerToBackingElem);
        }
//\endcond

        public override String GetRole() {
            return GetBackingElem().GetRole().GetValue();
        }

        public override AccessibilityProperties SetRole(String role) {
            GetBackingElem().SetRole(PdfStructTreeRoot.ConvertRoleToPdfName(role));
            return this;
        }

        public override String GetLanguage() {
            return ToUnicodeString(GetBackingElem().GetLang());
        }

        public override AccessibilityProperties SetLanguage(String language) {
            GetBackingElem().SetLang(new PdfString(language, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public override String GetActualText() {
            return ToUnicodeString(GetBackingElem().GetActualText());
        }

        public override AccessibilityProperties SetActualText(String actualText) {
            GetBackingElem().SetActualText(new PdfString(actualText, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public override String GetAlternateDescription() {
            return ToUnicodeString(GetBackingElem().GetAlt());
        }

        public override AccessibilityProperties SetAlternateDescription(String alternateDescription) {
            GetBackingElem().SetAlt(new PdfString(alternateDescription, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public override String GetExpansion() {
            return ToUnicodeString(GetBackingElem().GetE());
        }

        public override AccessibilityProperties SetExpansion(String expansion) {
            GetBackingElem().SetE(new PdfString(expansion, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public override AccessibilityProperties AddAttributes(PdfStructureAttributes attributes) {
            return AddAttributes(-1, attributes);
        }

        public override AccessibilityProperties AddAttributes(int index, PdfStructureAttributes attributes) {
            if (attributes == null) {
                return this;
            }
            PdfObject attributesObject = GetBackingElem().GetAttributes(false);
            PdfObject combinedAttributes = AccessibilityPropertiesToStructElem.CombineAttributesList(attributesObject, 
                index, JavaCollectionsUtil.SingletonList(attributes), GetBackingElem().GetPdfObject().GetAsNumber(PdfName
                .R));
            GetBackingElem().SetAttributes(combinedAttributes);
            return this;
        }

        public override AccessibilityProperties ClearAttributes() {
            GetBackingElem().GetPdfObject().Remove(PdfName.A);
            return this;
        }

        public override IList<PdfStructureAttributes> GetAttributesList() {
            List<PdfStructureAttributes> attributesList = new List<PdfStructureAttributes>();
            PdfObject elemAttributesObj = GetBackingElem().GetAttributes(false);
            if (elemAttributesObj != null) {
                if (elemAttributesObj.IsDictionary()) {
                    attributesList.Add(new PdfStructureAttributes((PdfDictionary)elemAttributesObj));
                }
                else {
                    if (elemAttributesObj.IsArray()) {
                        PdfArray attributesArray = (PdfArray)elemAttributesObj;
                        foreach (PdfObject attributeObj in attributesArray) {
                            if (attributeObj.IsDictionary()) {
                                attributesList.Add(new PdfStructureAttributes((PdfDictionary)attributeObj));
                            }
                        }
                    }
                }
            }
            return attributesList;
        }

        public override AccessibilityProperties SetPhoneme(String phoneme) {
            GetBackingElem().SetPhoneme(new PdfString(phoneme));
            return this;
        }

        public override String GetPhoneme() {
            return ToUnicodeString(GetBackingElem().GetPhoneme());
        }

        public override AccessibilityProperties SetPhoneticAlphabet(String phoneticAlphabet) {
            GetBackingElem().SetPhoneticAlphabet(PdfStructTreeRoot.ConvertRoleToPdfName(phoneticAlphabet));
            return this;
        }

        public override String GetPhoneticAlphabet() {
            return GetBackingElem().GetPhoneticAlphabet().GetValue();
        }

        public override AccessibilityProperties SetNamespace(PdfNamespace @namespace) {
            GetBackingElem().SetNamespace(@namespace);
            pointerToBackingElem.GetContext().EnsureNamespaceRegistered(@namespace);
            return this;
        }

        public override PdfNamespace GetNamespace() {
            return GetBackingElem().GetNamespace();
        }

        public override AccessibilityProperties AddRef(TagTreePointer treePointer) {
            GetBackingElem().AddRef(treePointer.GetCurrentStructElem());
            return this;
        }

        public override IList<TagTreePointer> GetRefsList() {
            IList<TagTreePointer> refsList = new List<TagTreePointer>();
            foreach (PdfStructElem @ref in GetBackingElem().GetRefsList()) {
                refsList.Add(new TagTreePointer(@ref, pointerToBackingElem.GetDocument()));
            }
            return JavaCollectionsUtil.UnmodifiableList(refsList);
        }

        /// <summary><inheritDoc/></summary>
        public override byte[] GetStructureElementId() {
            PdfString value = this.GetBackingElem().GetStructureElementId();
            return value == null ? null : value.GetValueBytes();
        }

        /// <summary><inheritDoc/></summary>
        public override AccessibilityProperties SetStructureElementId(byte[] id) {
            PdfString value = id == null ? null : new PdfString(id).SetHexWriting(true);
            this.GetBackingElem().SetStructureElementId(value);
            return this;
        }

        public override AccessibilityProperties ClearRefs() {
            GetBackingElem().GetPdfObject().Remove(PdfName.Ref);
            return this;
        }

        private PdfStructElem GetBackingElem() {
            return pointerToBackingElem.GetCurrentStructElem();
        }

        private String ToUnicodeString(PdfString pdfString) {
            return pdfString != null ? pdfString.ToUnicodeString() : null;
        }
    }
//\endcond
}
