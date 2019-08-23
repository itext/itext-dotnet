/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;

namespace iText.Forms.Xfdf {
    public class XfdfObject {
        private FObject f;

        private IdsObject ids;

        private FieldsObject fields;

        private AnnotsObject annots;

        private IList<AttributeObject> attributes;

        public virtual FObject GetF() {
            return f;
        }

        public virtual void SetF(FObject f) {
            this.f = f;
        }

        public virtual IdsObject GetIds() {
            return ids;
        }

        public virtual void SetIds(IdsObject ids) {
            this.ids = ids;
        }

        public virtual FieldsObject GetFields() {
            return fields;
        }

        public virtual void SetFields(FieldsObject fields) {
            this.fields = fields;
        }

        public virtual AnnotsObject GetAnnots() {
            return annots;
        }

        public virtual void SetAnnots(AnnotsObject annots) {
            this.annots = annots;
        }

        public virtual IList<AttributeObject> GetAttributes() {
            return attributes;
        }

        public virtual void SetAttributes(IList<AttributeObject> attributes) {
            this.attributes = attributes;
        }

        public virtual void MergeToPdf(PdfDocument pdfDocument, String pdfDocumentName) {
            XfdfReader reader = new XfdfReader();
            reader.MergeXfdfIntoPdf(this, pdfDocument, pdfDocumentName);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Transform.TransformerException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        public virtual void WriteToFile(String filename) {
            using (Stream os = new FileStream(filename, FileMode.Create)) {
                WriteToFile(os);
            }
        }

        /// <exception cref="Javax.Xml.Transform.TransformerException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        public virtual void WriteToFile(Stream os) {
            XfdfWriter writer = new XfdfWriter(os);
            writer.Write(this);
        }
    }
}
