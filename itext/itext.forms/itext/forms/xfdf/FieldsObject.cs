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
using System.Collections.Generic;

namespace iText.Forms.Xfdf {
    /// <summary>Represents the fields element, a child of the xfdf element and is the container for form field elements.
    ///     </summary>
    /// <remarks>
    /// Represents the fields element, a child of the xfdf element and is the container for form field elements.
    /// Content model: ( field* ).
    /// Attributes: none.
    /// For more details see paragraph 6.3.1 in Xfdf specification.
    /// </remarks>
    public class FieldsObject {
        /// <summary>Represents a list of children fields</summary>
        private IList<FieldObject> fieldList;

        /// <summary>
        /// Creates an instance of
        /// <see cref="FieldsObject"/>.
        /// </summary>
        public FieldsObject() {
            this.fieldList = new List<FieldObject>();
        }

        /// <summary>Gets a list of children fields</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// containing all children
        /// <see cref="FieldObject">field objects</see>
        /// </returns>
        public virtual IList<FieldObject> GetFieldList() {
            return fieldList;
        }

        /// <summary>Adds a new field to the list.</summary>
        /// <param name="field">FieldObject containing the info about the form field</param>
        /// <returns>
        /// current
        /// <see cref="FieldsObject">fields object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.FieldsObject AddField(FieldObject field) {
            this.fieldList.Add(field);
            return this;
        }
    }
}
