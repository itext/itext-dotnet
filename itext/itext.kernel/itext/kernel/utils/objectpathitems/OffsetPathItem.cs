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
using System.Xml;

namespace iText.Kernel.Utils.Objectpathitems {
    /// <summary>
    /// Direct path item (see
    /// <see cref="ObjectPath"/>
    /// , which describes transition to the
    /// specific position in
    /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
    /// </summary>
    public sealed class OffsetPathItem : LocalPathItem {
        private readonly int offset;

        /// <summary>
        /// Creates an instance of the
        /// <see cref="OffsetPathItem"/>.
        /// </summary>
        /// <param name="offset">
        /// bytes offset to the specific position in
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// </param>
        public OffsetPathItem(int offset)
            : base() {
            this.offset = offset;
        }

        /// <summary>
        /// The bytes offset of the stream which defines specific position in the
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// , to which transition
        /// was performed.
        /// </summary>
        /// <returns>an integer defining bytes offset to the specific position in stream.</returns>
        public int GetOffset() {
            return offset;
        }

        public override String ToString() {
            return "Offset: " + offset;
        }

        public override int GetHashCode() {
            return offset;
        }

        public override bool Equals(Object obj) {
            return obj != null && obj.GetType() == GetType() && offset == ((iText.Kernel.Utils.Objectpathitems.OffsetPathItem
                )obj).offset;
        }

        protected internal override XmlNode ToXmlNode(XmlDocument document) {
            XmlElement element = document.CreateElement("offset");
            element.AppendChild(document.CreateTextNode(offset.ToString()));
            return element;
        }
    }
}
