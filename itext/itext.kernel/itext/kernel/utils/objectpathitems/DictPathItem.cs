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
