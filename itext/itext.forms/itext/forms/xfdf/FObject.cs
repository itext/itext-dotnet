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
    /// <summary>Represents f element, child of the xfdf element.</summary>
    /// <remarks>
    /// Represents f element, child of the xfdf element.
    /// Corresponds to the F key in the file dictionary.
    /// Specifies the source file or target file: the PDF document that this XFDF file was exported from or is intended to be
    /// imported into.
    /// Attributes: href.
    /// For more details see paragraph 6.2.2 in Xfdf document specification.
    /// </remarks>
    public class FObject {
        /// <summary>The name of the source or target file.</summary>
        private String href;

        public FObject(String href) {
            this.href = href;
        }

        /// <summary>Gets the name of the source or target file.</summary>
        /// <returns>the name of the source or target file</returns>
        public virtual String GetHref() {
            return href;
        }

        /// <summary>Sets the name of the source or target file.</summary>
        /// <param name="href">the name of the source or target file</param>
        /// <returns>
        /// current
        /// <see cref="FObject">f object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.FObject SetHref(String href) {
            this.href = href;
            return this;
        }
    }
}
