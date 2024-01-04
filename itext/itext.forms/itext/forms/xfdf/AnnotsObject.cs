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
using System.Collections.Generic;

namespace iText.Forms.Xfdf {
    /// <summary>Represent annots tag in xfdf document structure.</summary>
    /// <remarks>
    /// Represent annots tag in xfdf document structure.
    /// Content model: ( text | caret | freetext | fileattachment | highlight | ink | line | link
    /// | circle | square | polygon | polyline | sound | squiggly | stamp |
    /// strikeout | underline )*.
    /// Attributes: none.
    /// For more details see paragraph 6.4.1 in Xfdf specification.
    /// </remarks>
    public class AnnotsObject {
        /// <summary>Represents a list of children annotations.</summary>
        private IList<AnnotObject> annotsList;

        /// <summary>Creates an instance with the empty list of children annotations.</summary>
        public AnnotsObject() {
            annotsList = new List<AnnotObject>();
        }

        /// <summary>Gets children annotations.</summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="AnnotObject"/>
        /// each representing a child annotation of this annots tag.
        /// </returns>
        public virtual IList<AnnotObject> GetAnnotsList() {
            return annotsList;
        }

        /// <summary>
        /// Adds a new
        /// <see cref="AnnotObject"/>
        /// to the list of children annotations.
        /// </summary>
        /// <param name="annot">
        /// 
        /// <see cref="AnnotObject"/>
        /// containing info about pdf document annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AnnotsObject"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotsObject AddAnnot(AnnotObject annot) {
            this.annotsList.Add(annot);
            return this;
        }
    }
}
