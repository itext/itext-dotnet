/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
    /// <summary>Represents xfdf element, the top level element in an xfdf document.</summary>
    /// <remarks>
    /// Represents xfdf element, the top level element in an xfdf document.
    /// For more details see paragraph 6.2.1 in Xfdf document specification.
    /// Content model: ( f? &amp; ids? &amp; fields? &amp; annots? )
    /// Attributes: xml:space, xmlns.
    /// </remarks>
    public class XfdfObject {
        /// <summary>Represents f element, child of the xfdf element.</summary>
        /// <remarks>
        /// Represents f element, child of the xfdf element.
        /// Corresponds to the F key in the file dictionary.
        /// </remarks>
        private FObject f;

        /// <summary>Represents ids element, a child of the xfdf element.</summary>
        /// <remarks>
        /// Represents ids element, a child of the xfdf element.
        /// Corresponds to the ID key in the file dictionary.
        /// </remarks>
        private IdsObject ids;

        /// <summary>Represents the fields element, a child of the xfdf element and is the container for form field elements.
        ///     </summary>
        private FieldsObject fields;

        /// <summary>Represent annots element, a child of the xfdf element and is the container for annot elements.</summary>
        private AnnotsObject annots;

        /// <summary>A list of attributes of xfdf object.</summary>
        private IList<AttributeObject> attributes;

        /// <summary>Gets the f element, child of the xfdf element.</summary>
        /// <remarks>
        /// Gets the f element, child of the xfdf element.
        /// Corresponds to the F key in the file dictionary.
        /// </remarks>
        /// <returns>the f element</returns>
        public virtual FObject GetF() {
            return f;
        }

        /// <summary>Sets f element, child of the xfdf element.</summary>
        /// <remarks>
        /// Sets f element, child of the xfdf element.
        /// Corresponds to the F key in the file dictionary.
        /// </remarks>
        /// <param name="f">element</param>
        public virtual void SetF(FObject f) {
            this.f = f;
        }

        /// <summary>Gets the ids element, child of the xfdf element.</summary>
        /// <remarks>
        /// Gets the ids element, child of the xfdf element.
        /// Corresponds to the ID key in the file dictionary.
        /// </remarks>
        /// <returns>the ids element</returns>
        public virtual IdsObject GetIds() {
            return ids;
        }

        /// <summary>Sets ids element, child of the xfdf element.</summary>
        /// <remarks>
        /// Sets ids element, child of the xfdf element.
        /// Corresponds to the ID key in the file dictionary.
        /// </remarks>
        /// <param name="ids">element</param>
        public virtual void SetIds(IdsObject ids) {
            this.ids = ids;
        }

        /// <summary>Gets the fields element, a child of the xfdf element and is the container for form field elements.
        ///     </summary>
        /// <returns>the fields element</returns>
        public virtual FieldsObject GetFields() {
            return fields;
        }

        /// <summary>Sets fields element, a child of the xfdf element and is the container for form field elements.</summary>
        /// <param name="fields">element</param>
        public virtual void SetFields(FieldsObject fields) {
            this.fields = fields;
        }

        /// <summary>Gets the annots element, a child of the xfdf element and is the container for annot elements.</summary>
        /// <returns>the annots element</returns>
        public virtual AnnotsObject GetAnnots() {
            return annots;
        }

        /// <summary>Sets the annots element, a child of the xfdf element and is the container for annot elements.</summary>
        /// <param name="annots">element</param>
        public virtual void SetAnnots(AnnotsObject annots) {
            this.annots = annots;
        }

        /// <summary>Gets the list of attributes of xfdf object.</summary>
        /// <returns>the list of attributes</returns>
        public virtual IList<AttributeObject> GetAttributes() {
            return attributes;
        }

        /// <summary>Sets the list of attributes of xfdf object.</summary>
        /// <param name="attributes">list of attributes objects</param>
        public virtual void SetAttributes(IList<AttributeObject> attributes) {
            this.attributes = attributes;
        }

        /// <summary>Merges info from XfdfObject to pdf document.</summary>
        /// <param name="pdfDocument">the target document for merge.</param>
        /// <param name="pdfDocumentName">
        /// the name of the target document. Will be checked in the merge process to determined
        /// if it is the same as href attribute of f element of merged XfdfObject. If the names are
        /// different, a warning will be thrown.
        /// </param>
        public virtual void MergeToPdf(PdfDocument pdfDocument, String pdfDocumentName) {
            XfdfReader reader = new XfdfReader();
            reader.MergeXfdfIntoPdf(this, pdfDocument, pdfDocumentName);
        }

        /// <summary>Writes info from XfdfObject to .xfdf file.</summary>
        /// <param name="filename">name of the target file.</param>
        public virtual void WriteToFile(String filename) {
            using (Stream os = new FileStream(filename, FileMode.Create)) {
                WriteToFile(os);
            }
        }

        /// <summary>Writes info from XfdfObject to .xfdf file.</summary>
        /// <param name="os">target output stream.</param>
        public virtual void WriteToFile(Stream os) {
            XfdfWriter writer = new XfdfWriter(os);
            writer.Write(this);
        }
    }
}
