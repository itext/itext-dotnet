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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal class BackedAccessibilityProperties : AccessibilityProperties {
        private TagTreePointer pointerToBackingElem;

        internal BackedAccessibilityProperties(TagTreePointer pointerToBackingElem) {
            this.pointerToBackingElem = new TagTreePointer(pointerToBackingElem);
        }

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
}
