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
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils.Objectpathitems {
    /// <summary>
    /// An item in the indirect path (see
    /// <see cref="ObjectPath"/>.
    /// </summary>
    /// <remarks>
    /// An item in the indirect path (see
    /// <see cref="ObjectPath"/>
    /// . It encapsulates two corresponding objects from the two
    /// comparing documents that were met to get to the path base objects during comparing process.
    /// </remarks>
    public sealed class IndirectPathItem {
        private readonly PdfIndirectReference cmpObject;

        private readonly PdfIndirectReference outObject;

        /// <summary>
        /// Creates
        /// <see cref="IndirectPathItem"/>
        /// instance for two corresponding objects from two comparing documents.
        /// </summary>
        /// <param name="cmpObject">an object from the cmp document.</param>
        /// <param name="outObject">an object from the out document.</param>
        public IndirectPathItem(PdfIndirectReference cmpObject, PdfIndirectReference outObject) {
            this.cmpObject = cmpObject;
            this.outObject = outObject;
        }

        /// <summary>
        /// Method returns a
        /// <see cref="IndirectPathItem"/>
        /// object from the cmp object that was met to get
        /// to the path base objects during comparing process.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="IndirectPathItem"/>
        /// object from the cmp object.
        /// </returns>
        public PdfIndirectReference GetCmpObject() {
            return cmpObject;
        }

        /// <summary>
        /// Method returns a
        /// <see cref="IndirectPathItem"/>
        /// object that was met to get to the path base
        /// objects during comparing process.
        /// </summary>
        /// <returns>an object from the out object</returns>
        public PdfIndirectReference GetOutObject() {
            return outObject;
        }

        public override int GetHashCode() {
            return cmpObject.GetHashCode() * 31 + outObject.GetHashCode();
        }

        public override bool Equals(Object obj) {
            return obj != null && (obj.GetType() == GetType() && cmpObject.Equals(((iText.Kernel.Utils.Objectpathitems.IndirectPathItem
                )obj).cmpObject) && outObject.Equals(((iText.Kernel.Utils.Objectpathitems.IndirectPathItem)obj).outObject
                ));
        }
    }
}
