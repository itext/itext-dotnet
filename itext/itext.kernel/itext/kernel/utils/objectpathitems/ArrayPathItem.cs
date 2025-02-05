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
using System.Xml;

namespace iText.Kernel.Utils.Objectpathitems {
    /// <summary>
    /// Direct path item (see
    /// <see cref="ObjectPath"/>
    /// , which describes transition to the
    /// <see cref="iText.Kernel.Pdf.PdfArray"/>
    /// element which is now a currently comparing direct object.
    /// </summary>
    public sealed class ArrayPathItem : LocalPathItem {
        private readonly int index;

        /// <summary>
        /// Creates an instance of the
        /// <see cref="ArrayPathItem"/>.
        /// </summary>
        /// <param name="index">
        /// the index which defines element of the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// to which
        /// the transition was performed.
        /// </param>
        public ArrayPathItem(int index)
            : base() {
            this.index = index;
        }

        public override String ToString() {
            return "Array index: " + index;
        }

        public override int GetHashCode() {
            return index;
        }

        public override bool Equals(Object obj) {
            return obj != null && obj.GetType() == GetType() && index == ((iText.Kernel.Utils.Objectpathitems.ArrayPathItem
                )obj).index;
        }

        /// <summary>
        /// The index which defines element of the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// to which the transition was performed.
        /// </summary>
        /// <remarks>
        /// The index which defines element of the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// to which the transition was performed.
        /// See
        /// <see cref="ArrayPathItem"/>
        /// for more info.
        /// </remarks>
        /// <returns>the index which defines element of the array to which the transition was performed</returns>
        public int GetIndex() {
            return index;
        }

        protected internal override XmlNode ToXmlNode(XmlDocument document) {
            XmlElement element = document.CreateElement("arrayIndex");
            element.AppendChild(document.CreateTextNode(index.ToString()));
            return element;
        }
    }
}
