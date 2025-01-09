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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// The class is used to provide connection between structure element of
    /// Tagged PDF document and marked content sequence in PDF stream.
    /// </summary>
    /// <remarks>
    /// The class is used to provide connection between structure element of
    /// Tagged PDF document and marked content sequence in PDF stream.
    /// <para />
    /// See
    /// <see cref="TagTreePointer.GetTagReference(int)"/>
    /// and
    /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas.OpenTag(TagReference)"/>.
    /// </remarks>
    public class TagReference {
        protected internal TagTreePointer tagPointer;

        protected internal int insertIndex;

        protected internal PdfStructElem referencedTag;

        protected internal PdfName role;

        protected internal PdfDictionary properties;

        /// <summary>
        /// Creates a
        /// <see cref="TagReference"/>
        /// instance which represents a reference to
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>.
        /// </summary>
        /// <param name="referencedTag">
        /// a structure element to which marked content will link (if insertIndex is -1,
        /// otherwise to MC will link to kid with insertIndex of passed structure element)
        /// </param>
        /// <param name="tagPointer">the tag pointer to structure element</param>
        /// <param name="insertIndex">
        /// if insertIndex is -1, the referencedTag will be used as a
        /// source of reference, otherwise the kid will be used
        /// </param>
        protected internal TagReference(PdfStructElem referencedTag, TagTreePointer tagPointer, int insertIndex) {
            this.role = referencedTag.GetRole();
            this.referencedTag = referencedTag;
            this.tagPointer = tagPointer;
            this.insertIndex = insertIndex;
        }

        /// <summary>Gets role of structure element.</summary>
        /// <returns>the role of structure element</returns>
        public virtual PdfName GetRole() {
            return role;
        }

        /// <summary>Creates next marked content identifier, which will be used to mark content in PDF stream.</summary>
        /// <returns>the marked content identifier</returns>
        public virtual int CreateNextMcid() {
            return tagPointer.CreateNextMcidForStructElem(referencedTag, insertIndex);
        }

        /// <summary>Adds property, which will be associated with marked-content sequence.</summary>
        /// <param name="name">the name of the property</param>
        /// <param name="value">the value of the property</param>
        /// <returns>
        /// the
        /// <see cref="TagReference"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagReference AddProperty(PdfName name, PdfObject value) {
            if (properties == null) {
                properties = new PdfDictionary();
            }
            properties.Put(name, value);
            return this;
        }

        /// <summary>Removes property.</summary>
        /// <remarks>Removes property. The property will not be associated with marked-content sequence.</remarks>
        /// <param name="name">the name of property to be deleted</param>
        /// <returns>
        /// the
        /// <see cref="TagReference"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagReference RemoveProperty(PdfName name) {
            if (properties != null) {
                properties.Remove(name);
            }
            return this;
        }

        /// <summary>Gets property which related to specified name.</summary>
        /// <param name="name">the name of the property</param>
        /// <returns>the value of the property</returns>
        public virtual PdfObject GetProperty(PdfName name) {
            if (properties == null) {
                return null;
            }
            return properties.Get(name);
        }

        /// <summary>
        /// Gets properties, which will be associated with marked-content sequence as
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <returns>the properties</returns>
        public virtual PdfDictionary GetProperties() {
            return properties;
        }
    }
}
