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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Collection {
    public class PdfCollectionItem : PdfObjectWrapper<PdfDictionary> {
        private PdfCollectionSchema schema;

        public PdfCollectionItem(PdfCollectionSchema schema)
            : base(new PdfDictionary()) {
            this.schema = schema;
        }

        /// <summary>Sets the value of the collection item.</summary>
        /// <param name="key">is a key with which the specified value is to be associated</param>
        /// <param name="value">is a value to be associated with the specified key</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollectionItem AddItem(String key, String value) {
            PdfCollectionField field = schema.GetField(key);
            GetPdfObject().Put(new PdfName(key), field.GetValue(value));
            return this;
        }

        /// <summary>Sets the date value of the collection item.</summary>
        /// <param name="key">is a key with which the specified date value is to be associated</param>
        /// <param name="date">
        /// is a
        /// <see cref="iText.Kernel.Pdf.PdfDate">PDF date</see>
        /// value to be associated with the specified key
        /// </param>
        public virtual void AddItem(String key, PdfDate date) {
            PdfCollectionField field = schema.GetField(key);
            if (field.subType == PdfCollectionField.DATE) {
                GetPdfObject().Put(new PdfName(key), date.GetPdfObject());
            }
        }

        /// <summary>Sets the number value of the collection item.</summary>
        /// <param name="key">is a key with which the specified number value is to be associated</param>
        /// <param name="number">
        /// is a
        /// <see cref="iText.Kernel.Pdf.PdfNumber">PDF number</see>
        /// value to be associated with the specified key
        /// </param>
        public virtual void AddItem(String key, PdfNumber number) {
            PdfCollectionField field = schema.GetField(key);
            if (field.subType == PdfCollectionField.NUMBER) {
                GetPdfObject().Put(new PdfName(key), number);
            }
        }

        /// <summary>Adds a prefix for the Collection item.</summary>
        /// <remarks>
        /// Adds a prefix for the Collection item.
        /// You can only use this method after you have set the value of the item.
        /// </remarks>
        /// <param name="key">is a key identifying the Collection item</param>
        /// <param name="prefix">is a prefix to be added</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollectionItem SetPrefix(String key, String prefix) {
            PdfName fieldName = new PdfName(key);
            PdfObject obj = GetPdfObject().Get(fieldName);
            if (obj == null) {
                throw new PdfException(KernelExceptionMessageConstant.YOU_MUST_SET_A_VALUE_BEFORE_ADDING_A_PREFIX);
            }
            PdfDictionary subItem = new PdfDictionary();
            subItem.Put(PdfName.D, obj);
            subItem.Put(PdfName.P, new PdfString(prefix));
            GetPdfObject().Put(fieldName, subItem);
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
