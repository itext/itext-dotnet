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
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// The class represents a basic implementation of
    /// <see cref="AccessibilityProperties"/>
    /// that preserves specified
    /// accessibility properties.
    /// </summary>
    /// <remarks>
    /// The class represents a basic implementation of
    /// <see cref="AccessibilityProperties"/>
    /// that preserves specified
    /// accessibility properties. Accessibility properties are used to define properties of
    /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem">structure elements</see>
    /// in Tagged PDF documents via
    /// <see cref="TagTreePointer"/>
    /// API.
    /// </remarks>
    public class DefaultAccessibilityProperties : AccessibilityProperties {
        protected internal String role;

        protected internal String language;

        protected internal String actualText;

        protected internal String alternateDescription;

        protected internal String expansion;

        protected internal IList<PdfStructureAttributes> attributesList = new List<PdfStructureAttributes>();

        protected internal String phoneme;

        protected internal String phoneticAlphabet;

        protected internal PdfNamespace @namespace;

        protected internal IList<TagTreePointer> refs = new List<TagTreePointer>();

        private byte[] structElemId;

        /// <summary>
        /// Instantiates a new
        /// <see cref="DefaultAccessibilityProperties"/>
        /// instance based on structure element role.
        /// </summary>
        /// <param name="role">the structure element role</param>
        public DefaultAccessibilityProperties(String role) {
            this.role = role;
        }

        public override String GetRole() {
            return role;
        }

        public override AccessibilityProperties SetRole(String role) {
            this.role = role;
            return this;
        }

        public override String GetLanguage() {
            return language;
        }

        public override AccessibilityProperties SetLanguage(String language) {
            this.language = language;
            return this;
        }

        public override String GetActualText() {
            return actualText;
        }

        public override AccessibilityProperties SetActualText(String actualText) {
            this.actualText = actualText;
            return this;
        }

        public override String GetAlternateDescription() {
            return alternateDescription;
        }

        public override AccessibilityProperties SetAlternateDescription(String alternateDescription) {
            this.alternateDescription = alternateDescription;
            return this;
        }

        public override String GetExpansion() {
            return expansion;
        }

        public override AccessibilityProperties SetExpansion(String expansion) {
            this.expansion = expansion;
            return this;
        }

        public override AccessibilityProperties AddAttributes(PdfStructureAttributes attributes) {
            return AddAttributes(-1, attributes);
        }

        public override AccessibilityProperties AddAttributes(int index, PdfStructureAttributes attributes) {
            if (attributes != null) {
                if (index > 0) {
                    attributesList.Add(index, attributes);
                }
                else {
                    attributesList.Add(attributes);
                }
            }
            return this;
        }

        public override AccessibilityProperties ClearAttributes() {
            attributesList.Clear();
            return this;
        }

        public override IList<PdfStructureAttributes> GetAttributesList() {
            return attributesList;
        }

        public override String GetPhoneme() {
            return this.phoneme;
        }

        public override AccessibilityProperties SetPhoneme(String phoneme) {
            this.phoneme = phoneme;
            return this;
        }

        public override String GetPhoneticAlphabet() {
            return this.phoneticAlphabet;
        }

        public override AccessibilityProperties SetPhoneticAlphabet(String phoneticAlphabet) {
            this.phoneticAlphabet = phoneticAlphabet;
            return this;
        }

        public override PdfNamespace GetNamespace() {
            return this.@namespace;
        }

        public override AccessibilityProperties SetNamespace(PdfNamespace @namespace) {
            this.@namespace = @namespace;
            return this;
        }

        public override AccessibilityProperties AddRef(TagTreePointer treePointer) {
            refs.Add(new TagTreePointer(treePointer));
            return this;
        }

        public override IList<TagTreePointer> GetRefsList() {
            return JavaCollectionsUtil.UnmodifiableList(refs);
        }

        public override AccessibilityProperties ClearRefs() {
            refs.Clear();
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public override byte[] GetStructureElementId() {
            return this.structElemId;
        }

        /// <summary><inheritDoc/></summary>
        public override AccessibilityProperties SetStructureElementId(byte[] id) {
            this.structElemId = JavaUtil.ArraysCopyOf(id, id.Length);
            return this;
        }
    }
}
