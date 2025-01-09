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
using iText.Forms.Exceptions;

namespace iText.Forms.Xfdf {
    /// <summary>Represents the attribute of any XFDF element.</summary>
    public class AttributeObject {
        private String name;

        private String value;

        /// <summary>Creates an instance with given attribute name and value.</summary>
        /// <param name="name">the name of the attribute, constrained by XML attributes specification.</param>
        /// <param name="value">the value of the attribute, constrained by XML attributes specification.</param>
        public AttributeObject(String name, String value) {
            if (name == null || value == null) {
                throw new XfdfException(XfdfException.ATTRIBUTE_NAME_OR_VALUE_MISSING);
            }
            this.name = name;
            this.value = value;
        }

        /// <summary>Returns attribute name.</summary>
        /// <returns>a string representation of attribute name, case-sensitive as per XML specification.</returns>
        public virtual String GetName() {
            return name;
        }

        /// <summary>Returns attribute value.</summary>
        /// <returns>a string representation of attribute value.</returns>
        public virtual String GetValue() {
            return value;
        }
    }
}
