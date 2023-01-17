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
