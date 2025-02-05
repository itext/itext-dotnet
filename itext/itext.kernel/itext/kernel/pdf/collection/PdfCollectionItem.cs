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
