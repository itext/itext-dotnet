/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils.Objectpathitems {
    /// <summary>
    /// Direct path item (see
    /// <see cref="ObjectPath"/>
    /// , which describes transition to the
    /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
    /// entry which value is now a currently comparing direct object.
    /// </summary>
    public sealed class DictPathItem : LocalPathItem {
        private readonly PdfName key;

        /// <summary>
        /// Creates an instance of the
        /// <see cref="DictPathItem"/>.
        /// </summary>
        /// <param name="key">
        /// the key which defines to which entry of the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// the transition was performed.
        /// </param>
        public DictPathItem(PdfName key)
            : base() {
            this.key = key;
        }

        public override String ToString() {
            return "Dict key: " + key;
        }

        public override int GetHashCode() {
            return key.GetHashCode();
        }

        public override bool Equals(Object obj) {
            return obj != null && obj.GetType() == GetType() && key.Equals(((iText.Kernel.Utils.Objectpathitems.DictPathItem
                )obj).key);
        }

        /// <summary>
        /// The key which defines to which entry of the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// the transition was performed.
        /// </summary>
        /// <remarks>
        /// The key which defines to which entry of the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// the transition was performed.
        /// See
        /// <see cref="DictPathItem"/>
        /// for more info.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// which is the key which defines
        /// to which entry of the dictionary the transition was performed.
        /// </returns>
        public PdfName GetKey() {
            return key;
        }

        protected internal override XmlNode ToXmlNode(XmlDocument document) {
            XmlElement element = document.CreateElement("dictKey");
            element.AppendChild(document.CreateTextNode(key.ToString()));
            return element;
        }
    }
}
