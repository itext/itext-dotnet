/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    public abstract class AccessibilityProperties {
        public virtual String GetRole() {
            return null;
        }

        public virtual AccessibilityProperties SetRole(String role) {
            return this;
        }

        public virtual String GetLanguage() {
            return null;
        }

        public virtual AccessibilityProperties SetLanguage(String language) {
            return this;
        }

        public virtual String GetActualText() {
            return null;
        }

        public virtual AccessibilityProperties SetActualText(String actualText) {
            return this;
        }

        public virtual String GetAlternateDescription() {
            return null;
        }

        public virtual AccessibilityProperties SetAlternateDescription(String alternateDescription) {
            return this;
        }

        public virtual String GetExpansion() {
            return null;
        }

        public virtual AccessibilityProperties SetExpansion(String expansion) {
            return this;
        }

        public virtual String GetPhoneme() {
            return null;
        }

        public virtual AccessibilityProperties SetPhoneme(String phoneme) {
            return this;
        }

        public virtual String GetPhoneticAlphabet() {
            return null;
        }

        public virtual AccessibilityProperties SetPhoneticAlphabet(String phoneticAlphabet) {
            return this;
        }

        public virtual PdfNamespace GetNamespace() {
            return null;
        }

        public virtual AccessibilityProperties SetNamespace(PdfNamespace @namespace) {
            return this;
        }

        public virtual AccessibilityProperties AddRef(TagTreePointer treePointer) {
            return this;
        }

        public virtual IList<TagTreePointer> GetRefsList() {
            return JavaCollectionsUtil.EmptyList<TagTreePointer>();
        }

        public virtual AccessibilityProperties ClearRefs() {
            return this;
        }

        public virtual AccessibilityProperties AddAttributes(PdfStructureAttributes attributes) {
            return this;
        }

        public virtual AccessibilityProperties AddAttributes(int index, PdfStructureAttributes attributes) {
            return this;
        }

        public virtual AccessibilityProperties ClearAttributes() {
            return this;
        }

        public virtual IList<PdfStructureAttributes> GetAttributesList() {
            return JavaCollectionsUtil.EmptyList<PdfStructureAttributes>();
        }
    }
}
