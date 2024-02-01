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

namespace iText.Forms.Xfdf {
    /// <summary>Represents ids element, child of the xfdf element.</summary>
    /// <remarks>
    /// Represents ids element, child of the xfdf element.
    /// Corresponds to the ID key in the file dictionary.
    /// The two attributes are file identifiers for the source or target file designated by the f element, taken
    /// from the ID entry in the file’s trailer dictionary.
    /// Attributes: original, modified.
    /// For more details see paragraph 6.2.3 in Xfdf document specification.
    /// </remarks>
    public class IdsObject {
        /// <summary>
        /// This attribute corresponds to the permanent identifier which
        /// is based on the contents of the file at the time it was originally created.
        /// </summary>
        /// <remarks>
        /// This attribute corresponds to the permanent identifier which
        /// is based on the contents of the file at the time it was originally created.
        /// This value does not change when the file is incrementally updated.
        /// The value shall be a hexadecimal number.
        /// A common value for this is an MD5 checksum.
        /// </remarks>
        private String original;

        /// <summary>
        /// The attribute contains a unique identifier for the
        /// modified version of the pdf and corresponding xfdf document.
        /// </summary>
        /// <remarks>
        /// The attribute contains a unique identifier for the
        /// modified version of the pdf and corresponding xfdf document. The
        /// modified attribute corresponds to the changing identifier that is based
        /// on the file's contents at the time it was last updated.
        /// The value shall be a hexadecimal number.
        /// A common value for this is an MD5 checksum.
        /// </remarks>
        private String modified;

        public IdsObject() {
        }

        /// <summary>
        /// Gets the string value of the permanent identifier which
        /// is based on the contents of the file at the time it was originally created.
        /// </summary>
        /// <remarks>
        /// Gets the string value of the permanent identifier which
        /// is based on the contents of the file at the time it was originally created.
        /// This value does not change when the file is incrementally updated.
        /// The value shall be a hexadecimal number.
        /// </remarks>
        /// <returns>the permanent identifier value</returns>
        public virtual String GetOriginal() {
            return original;
        }

        /// <summary>
        /// Sets the string value of the permanent identifier which
        /// is based on the contents of the file at the time it was originally created.
        /// </summary>
        /// <remarks>
        /// Sets the string value of the permanent identifier which
        /// is based on the contents of the file at the time it was originally created.
        /// This value does not change when the file is incrementally updated.
        /// The value shall be a hexadecimal number.
        /// A common value for this is an MD5 checksum.
        /// </remarks>
        /// <param name="original">the permanent identifier value</param>
        /// <returns>
        /// current
        /// <see cref="IdsObject">ids object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.IdsObject SetOriginal(String original) {
            this.original = original;
            return this;
        }

        /// <summary>
        /// Gets the string value of the unique identifier for the
        /// modified version of the pdf and corresponding xfdf document.
        /// </summary>
        /// <remarks>
        /// Gets the string value of the unique identifier for the
        /// modified version of the pdf and corresponding xfdf document. The
        /// modified attribute corresponds to the changing identifier that is based
        /// on the file's contents at the time it was last updated.
        /// The value shall be a hexadecimal number.
        /// </remarks>
        /// <returns>the unique identifier value</returns>
        public virtual String GetModified() {
            return modified;
        }

        /// <summary>
        /// Sets the string value of the unique identifier for the
        /// modified version of the pdf and corresponding xfdf document.
        /// </summary>
        /// <remarks>
        /// Sets the string value of the unique identifier for the
        /// modified version of the pdf and corresponding xfdf document. The
        /// modified attribute corresponds to the changing identifier that is based
        /// on the file's contents at the time it was last updated.
        /// The value shall be a hexadecimal number.
        /// A common value for this is an MD5 checksum.
        /// </remarks>
        /// <param name="modified">the unique identifier value</param>
        /// <returns>
        /// current
        /// <see cref="IdsObject">ids object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.IdsObject SetModified(String modified) {
            this.modified = modified;
            return this;
        }
    }
}
