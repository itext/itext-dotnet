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
