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
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Signatures.Cms {
    /// <summary>This class represents Attribute structure.</summary>
    public class CmsAttribute {
        private readonly String type;

        private readonly IAsn1Object value;

        /// <summary>Creates an attribute.</summary>
        /// <param name="type">the type of the attribute</param>
        /// <param name="value">the value</param>
        public CmsAttribute(String type, IAsn1Object value) {
            this.type = type;
            this.value = value;
        }

        /// <summary>Returns the type of the attribute.</summary>
        /// <returns>the type of the attribute.</returns>
        public virtual String GetType() {
            return type;
        }

        /// <summary>Returns the value of the attribute.</summary>
        /// <returns>the value of the attribute.</returns>
        public virtual IAsn1Object GetValue() {
            return value;
        }
    }
}
