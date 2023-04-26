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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Collection {
    public class PdfCollectionSchema : PdfObjectWrapper<PdfDictionary> {
        public PdfCollectionSchema(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Creates a Collection Schema dictionary.</summary>
        public PdfCollectionSchema()
            : this(new PdfDictionary()) {
        }

        /// <summary>Adds a Collection field to the Schema.</summary>
        /// <param name="name">the name of the collection field</param>
        /// <param name="field">a Collection Field</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollectionSchema AddField(String name, PdfCollectionField field
            ) {
            GetPdfObject().Put(new PdfName(name), field.GetPdfObject());
            return this;
        }

        /// <summary>Retrieves a Collection field from the Schema.</summary>
        /// <param name="name">is the name of the collection field</param>
        /// <returns>
        /// a
        /// <see cref="PdfCollectionField">Collection field</see>
        /// </returns>
        public virtual PdfCollectionField GetField(String name) {
            return new PdfCollectionField(GetPdfObject().GetAsDictionary(new PdfName(name)));
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
