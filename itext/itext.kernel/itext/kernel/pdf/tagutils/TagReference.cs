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
