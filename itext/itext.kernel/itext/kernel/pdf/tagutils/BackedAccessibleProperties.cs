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
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal class BackedAccessibleProperties : AccessibilityProperties {
        private PdfStructElem backingElem;

        internal BackedAccessibleProperties(PdfStructElem backingElem) {
            // TODO introduce IAccessibleProperties interface before iText 7.1 released
            // TODO move getRole/setRole from IAccessibleElement to it?
            this.backingElem = backingElem;
        }

        public override String GetLanguage() {
            return ToUnicodeString(backingElem.GetLang());
        }

        public override AccessibilityProperties SetLanguage(String language) {
            backingElem.SetLang(new PdfString(language));
            return this;
        }

        public override String GetActualText() {
            return ToUnicodeString(backingElem.GetActualText());
        }

        public override AccessibilityProperties SetActualText(String actualText) {
            backingElem.SetActualText(new PdfString(actualText));
            return this;
        }

        public override String GetAlternateDescription() {
            return ToUnicodeString(backingElem.GetAlt());
        }

        public override AccessibilityProperties SetAlternateDescription(String alternateDescription) {
            backingElem.SetAlt(new PdfString(alternateDescription));
            return this;
        }

        public override String GetExpansion() {
            return ToUnicodeString(backingElem.GetE());
        }

        public override AccessibilityProperties SetExpansion(String expansion) {
            backingElem.SetE(new PdfString(expansion));
            return this;
        }

        public override AccessibilityProperties AddAttributes(PdfDictionary attributes) {
            return AddAttributes(-1, attributes);
        }

        public override AccessibilityProperties AddAttributes(int index, PdfDictionary attributes) {
            if (attributes == null) {
                return this;
            }
            PdfObject attributesObject = backingElem.GetAttributes(false);
            PdfObject combinedAttributes = CombineAttributesList(attributesObject, index, JavaCollectionsUtil.SingletonList
                (attributes), backingElem.GetPdfObject().GetAsNumber(PdfName.R));
            backingElem.SetAttributes(combinedAttributes);
            return this;
        }

        public override AccessibilityProperties ClearAttributes() {
            backingElem.GetPdfObject().Remove(PdfName.A);
            return this;
        }

        public override IList<PdfDictionary> GetAttributesList() {
            List<PdfDictionary> attributesList = new List<PdfDictionary>();
            PdfObject elemAttributesObj = backingElem.GetAttributes(false);
            if (elemAttributesObj != null) {
                if (elemAttributesObj.IsDictionary()) {
                    attributesList.Add((PdfDictionary)elemAttributesObj);
                }
                else {
                    if (elemAttributesObj.IsArray()) {
                        PdfArray attributesArray = (PdfArray)elemAttributesObj;
                        foreach (PdfObject attributeObj in attributesArray) {
                            if (attributeObj.IsDictionary()) {
                                attributesList.Add((PdfDictionary)attributeObj);
                            }
                        }
                    }
                }
            }
            return attributesList;
        }

        public override AccessibilityProperties SetPhoneme(String phoneme) {
            backingElem.SetPhoneme(new PdfString(phoneme));
            return this;
        }

        public override String GetPhoneme() {
            return ToUnicodeString(backingElem.GetPhoneme());
        }

        public override AccessibilityProperties SetPhoneticAlphabet(PdfName phoneticAlphabet) {
            backingElem.SetPhoneticAlphabet(phoneticAlphabet);
            return this;
        }

        public override PdfName GetPhoneticAlphabet() {
            return backingElem.GetPhoneticAlphabet();
        }

        public override AccessibilityProperties SetNamespace(PdfNamespace @namespace) {
            backingElem.SetNamespace(@namespace);
            PdfDocument doc = backingElem.GetPdfObject().GetIndirectReference().GetDocument();
            doc.GetTagStructureContext().EnsureNamespaceRegistered(@namespace);
            return this;
        }

        public override PdfNamespace GetNamespace() {
            return backingElem.GetNamespace();
        }

        public override AccessibilityProperties AddRef(TagTreePointer treePointer) {
            backingElem.AddRef(treePointer.GetCurrentStructElem());
            return this;
        }

        public override IList<TagTreePointer> GetRefsList() {
            IList<TagTreePointer> refsList = new List<TagTreePointer>();
            foreach (PdfStructElem @ref in backingElem.GetRefsList()) {
                refsList.Add(new TagTreePointer(@ref));
            }
            return refsList;
        }

        public override AccessibilityProperties ClearRefs() {
            backingElem.GetPdfObject().Remove(PdfName.Ref);
            return this;
        }

        internal override void SetToStructElem(PdfStructElem elem) {
        }

        // ignore, because all attributes are directly set to the structElem
        private String ToUnicodeString(PdfString pdfString) {
            return pdfString != null ? pdfString.ToUnicodeString() : null;
        }
    }
}
