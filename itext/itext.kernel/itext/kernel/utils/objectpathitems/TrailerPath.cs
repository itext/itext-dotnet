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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils.Objectpathitems {
    public sealed class TrailerPath : ObjectPath {
        private readonly PdfDocument outDocument;

        private readonly PdfDocument cmpDocument;

        private const String INITIAL_LINE = "Base cmp object: trailer. Base out object: trailer";

        public TrailerPath(PdfDocument cmpDoc, PdfDocument outDoc)
            : base() {
            outDocument = outDoc;
            cmpDocument = cmpDoc;
        }

        public TrailerPath(iText.Kernel.Utils.Objectpathitems.TrailerPath trailerPath)
            : base() {
            outDocument = trailerPath.GetOutDocument();
            cmpDocument = trailerPath.GetCmpDocument();
            path = trailerPath.GetLocalPath();
        }

        public TrailerPath(PdfDocument cmpDoc, PdfDocument outDoc, Stack<LocalPathItem> path)
            : base() {
            this.outDocument = outDoc;
            this.cmpDocument = cmpDoc;
            this.path = path;
        }

        /// <summary>
        /// Method returns current out
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object.
        /// </summary>
        /// <returns>
        /// current out
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object.
        /// </returns>
        public PdfDocument GetOutDocument() {
            return outDocument;
        }

        /// <summary>
        /// Method returns current cmp
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object.
        /// </summary>
        /// <returns>
        /// current cmp
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object.
        /// </returns>
        public PdfDocument GetCmpDocument() {
            return cmpDocument;
        }

        /// <summary>
        /// Creates an xml node that describes this
        /// <see cref="TrailerPath"/>
        /// instance.
        /// </summary>
        /// <param name="document">xml document, to which this xml node will be added.</param>
        /// <returns>
        /// an xml node describing this
        /// <see cref="TrailerPath"/>
        /// instance.
        /// </returns>
        public override XmlNode ToXmlNode(XmlDocument document) {
            XmlElement element = document.CreateElement("path");
            XmlElement baseNode = document.CreateElement("base");
            baseNode.SetAttribute("cmp", "trailer");
            baseNode.SetAttribute("out", "trailer");
            element.AppendChild(baseNode);
            foreach (LocalPathItem pathItem in path) {
                element.AppendChild(pathItem.ToXmlNode(document));
            }
            return element;
        }

        /// <summary>
        /// Method returns a string representation of this
        /// <see cref="TrailerPath"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// a string representation of this
        /// <see cref="TrailerPath"/>
        /// instance.
        /// </returns>
        public override String ToString() {
            StringBuilder sb = new StringBuilder(INITIAL_LINE.Length);
            sb.Append(INITIAL_LINE);
            foreach (LocalPathItem pathItem in path) {
                sb.Append('\n');
                sb.Append(pathItem.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Method returns a hash code of this
        /// <see cref="TrailerPath"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// a int hash code of this
        /// <see cref="TrailerPath"/>
        /// instance.
        /// </returns>
        public override int GetHashCode() {
            int hashCode = outDocument.GetHashCode() * 31 + cmpDocument.GetHashCode();
            foreach (LocalPathItem pathItem in path) {
                hashCode *= 31;
                hashCode += pathItem.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Method returns true if this
        /// <see cref="TrailerPath"/>
        /// instance equals to the passed object.
        /// </summary>
        /// <returns>
        /// true - if this
        /// <see cref="TrailerPath"/>
        /// instance equals to the passed object.
        /// </returns>
        public override bool Equals(Object obj) {
            if (obj == null) {
                return false;
            }
            return obj.GetType() == GetType() && outDocument.Equals(((iText.Kernel.Utils.Objectpathitems.TrailerPath)obj
                ).outDocument) && cmpDocument.Equals(((iText.Kernel.Utils.Objectpathitems.TrailerPath)obj).cmpDocument
                ) && Enumerable.SequenceEqual(path, ((ObjectPath)obj).path);
        }
    }
}
