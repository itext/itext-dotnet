/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
