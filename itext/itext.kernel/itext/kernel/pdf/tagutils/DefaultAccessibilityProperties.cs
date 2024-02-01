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
