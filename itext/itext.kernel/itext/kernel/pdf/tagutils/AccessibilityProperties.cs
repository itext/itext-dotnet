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
using iText.Commons.Utils;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// The accessibility properties are used to define properties of
    /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem">structure elements</see>
    /// in Tagged PDF documents via
    /// <see cref="TagTreePointer"/>
    /// API.
    /// </summary>
    public abstract class AccessibilityProperties {
        /// <summary>Gets the role of element.</summary>
        /// <remarks>
        /// Gets the role of element.
        /// <para />
        /// See also
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles"/>.
        /// </remarks>
        /// <returns>the role</returns>
        public virtual String GetRole() {
            return null;
        }

        /// <summary>Sets the role of element.</summary>
        /// <remarks>
        /// Sets the role of element.
        /// <para />
        /// See also
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles"/>.
        /// <para />
        /// Calling this method with a null argument will make the tagging on the associated layout
        /// element "neutral". The effect is that all children of the layout element will be
        /// tagged as if they were direct children of the parent element.
        /// </remarks>
        /// <param name="role">the role to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetRole(String role) {
            return this;
        }

        /// <summary>Gets the language identifier of element.</summary>
        /// <remarks>
        /// Gets the language identifier of element. Should be in format xy-ZK (for example en-US).
        /// <para />
        /// For more information see PDF Specification ISO 32000-1 section 14.9.2.
        /// </remarks>
        /// <returns>the language</returns>
        public virtual String GetLanguage() {
            return null;
        }

        /// <summary>Sets the language identifier of element.</summary>
        /// <remarks>
        /// Sets the language identifier of element. Should be in format xy-ZK (for example en-US).
        /// <para />
        /// For more information see PDF Specification ISO 32000-1 section 14.9.2.
        /// </remarks>
        /// <param name="language">the language to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetLanguage(String language) {
            return this;
        }

        /// <summary>Gets the actual text of element.</summary>
        /// <returns>the actual text</returns>
        public virtual String GetActualText() {
            return null;
        }

        /// <summary>Sets the actual text of element.</summary>
        /// <param name="actualText">the actual text to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetActualText(String actualText) {
            return this;
        }

        /// <summary>Gets the alternate description of element.</summary>
        /// <returns>the alternate description</returns>
        public virtual String GetAlternateDescription() {
            return null;
        }

        /// <summary>Sets the alternate description of element.</summary>
        /// <param name="alternateDescription">the alternation description to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetAlternateDescription(String alternateDescription) {
            return this;
        }

        /// <summary>Gets the expansion of element.</summary>
        /// <remarks>
        /// Gets the expansion of element.
        /// <para />
        /// Expansion it is the expanded form of an abbreviation of structure element.
        /// </remarks>
        /// <returns>the expansion</returns>
        public virtual String GetExpansion() {
            return null;
        }

        /// <summary>Sets the expansion of element.</summary>
        /// <remarks>
        /// Sets the expansion of element.
        /// <para />
        /// Expansion it is the expanded form of an abbreviation of structure element.
        /// </remarks>
        /// <param name="expansion">the expansion to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetExpansion(String expansion) {
            return this;
        }

        /// <summary>Gets the phoneme of element.</summary>
        /// <remarks>
        /// Gets the phoneme of element.
        /// <para />
        /// For more information see
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.SetPhoneme(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        /// <returns>the phoneme</returns>
        public virtual String GetPhoneme() {
            return null;
        }

        /// <summary>Sets the phoneme of element.</summary>
        /// <remarks>
        /// Sets the phoneme of element.
        /// <para />
        /// For more information see
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.SetPhoneme(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        /// <param name="phoneme">the phoneme to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetPhoneme(String phoneme) {
            return this;
        }

        /// <summary>Gets the phonetic alphabet of element.</summary>
        /// <remarks>
        /// Gets the phonetic alphabet of element.
        /// <para />
        /// For more information see
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.SetPhoneticAlphabet(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        /// <returns>the phonetic alphabet</returns>
        public virtual String GetPhoneticAlphabet() {
            return null;
        }

        /// <summary>Sets the phonetic alphabet of element.</summary>
        /// <remarks>
        /// Sets the phonetic alphabet of element.
        /// <para />
        /// For more information see
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.SetPhoneticAlphabet(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        /// <param name="phoneticAlphabet">the phonetic alphabet to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetPhoneticAlphabet(String phoneticAlphabet) {
            return this;
        }

        /// <summary>Gets the namespace of element.</summary>
        /// <returns>the namespace</returns>
        public virtual PdfNamespace GetNamespace() {
            return null;
        }

        /// <summary>Sets the namespace of element.</summary>
        /// <param name="namespace">the namespace to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetNamespace(PdfNamespace @namespace) {
            return this;
        }

        /// <summary>Adds the reference to other tagged element.</summary>
        /// <remarks>
        /// Adds the reference to other tagged element.
        /// <para />
        /// For more information see
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.AddRef(iText.Kernel.Pdf.Tagging.PdfStructElem)"/>.
        /// </remarks>
        /// <param name="treePointer">the reference to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties AddRef(TagTreePointer treePointer) {
            return this;
        }

        /// <summary>Gets the list of references to other tagged elements.</summary>
        /// <remarks>
        /// Gets the list of references to other tagged elements.
        /// <para />
        /// For more information see
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.AddRef(iText.Kernel.Pdf.Tagging.PdfStructElem)"/>.
        /// </remarks>
        /// <returns>the list of references</returns>
        public virtual IList<TagTreePointer> GetRefsList() {
            return JavaCollectionsUtil.EmptyList<TagTreePointer>();
        }

        /// <summary>Clears the list of references to other tagged elements.</summary>
        /// <remarks>
        /// Clears the list of references to other tagged elements.
        /// <para />
        /// For more information see
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.AddRef(iText.Kernel.Pdf.Tagging.PdfStructElem)"/>.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties ClearRefs() {
            return this;
        }

        /// <summary>Adds the attributes to the element.</summary>
        /// <param name="attributes">the attributes to be added</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties AddAttributes(PdfStructureAttributes attributes) {
            return this;
        }

        /// <summary>Adds the attributes to the element with specified index.</summary>
        /// <remarks>
        /// Adds the attributes to the element with specified index.
        /// <para />
        /// If an attribute with the same O and NS entries is specified more than once, the later (in array order)
        /// entry shall take precedence. For more information see PDF Specification ISO-32000 section 14.7.6.
        /// </remarks>
        /// <param name="index">the attributes index</param>
        /// <param name="attributes">the attributes to be added</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties AddAttributes(int index, PdfStructureAttributes attributes) {
            return this;
        }

        /// <summary>Clears the list of attributes.</summary>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties ClearAttributes() {
            return this;
        }

        /// <summary>Gets the attributes list.</summary>
        /// <returns>the attributes list</returns>
        public virtual IList<PdfStructureAttributes> GetAttributesList() {
            return JavaCollectionsUtil.EmptyList<PdfStructureAttributes>();
        }

        /// <summary>Gets the associated structure element's ID string, if it has one.</summary>
        /// <remarks>
        /// Gets the associated structure element's ID string, if it has one.
        /// <para />
        /// See also
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.GetStructureElementId()"/>.
        /// </remarks>
        /// <returns>the structure element's ID string, or null if there is none</returns>
        public virtual byte[] GetStructureElementId() {
            return null;
        }

        /// <summary>Sets the associated structure element's ID.</summary>
        /// <remarks>
        /// Sets the associated structure element's ID. Passing
        /// <see langword="null"/>
        /// removes the ID.
        /// <para />
        /// See also
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.SetStructureElementId(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        /// <param name="id">the element's ID to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetStructureElementId(byte[] id) {
            return this;
        }

        /// <summary>Sets the associated structure element's ID.</summary>
        /// <remarks>
        /// Sets the associated structure element's ID. Passing
        /// <see langword="null"/>
        /// removes the ID.
        /// If non-null, the argument will be encoded in UTF-8 (without BOM), since ID
        /// strings are considered binary data in PDF.
        /// <para />
        /// See also
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem.SetStructureElementId(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        /// <param name="id">the element's ID to be set</param>
        /// <returns>
        /// this
        /// <see cref="AccessibilityProperties"/>
        /// instance
        /// </returns>
        public virtual AccessibilityProperties SetStructureElementIdString(String id) {
            return this.SetStructureElementId(id == null ? null : id.GetBytes(System.Text.Encoding.UTF8));
        }
    }
}
