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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Collection {
    public class PdfCollectionSort : PdfObjectWrapper<PdfDictionary> {
        public PdfCollectionSort(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Constructs a PDF Collection Sort Dictionary.</summary>
        /// <param name="key">the key of the field that will be used to sort entries</param>
        public PdfCollectionSort(String key)
            : this(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.S, new PdfName(key));
        }

        /// <summary>Constructs a PDF Collection Sort Dictionary.</summary>
        /// <param name="keys">the keys of the fields that will be used to sort entries</param>
        public PdfCollectionSort(String[] keys)
            : this(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.S, new PdfArray(JavaUtil.ArraysAsList(keys), true));
        }

        /// <summary>Defines the sort order of the field (ascending or descending).</summary>
        /// <param name="ascending">true is the default, use false for descending order</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollectionSort SetSortOrder(bool ascending) {
            PdfObject obj = GetPdfObject().Get(PdfName.S);
            if (obj.IsName()) {
                GetPdfObject().Put(PdfName.A, PdfBoolean.ValueOf(ascending));
            }
            else {
                throw new PdfException(KernelExceptionMessageConstant.YOU_HAVE_TO_DEFINE_A_BOOLEAN_ARRAY_FOR_THIS_COLLECTION_SORT_DICTIONARY
                    );
            }
            return this;
        }

        /// <summary>Defines the sort order of the field (ascending or descending).</summary>
        /// <param name="ascending">an array with every element corresponding with a name of a field.</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollectionSort SetSortOrder(bool[] ascending) {
            PdfObject obj = GetPdfObject().Get(PdfName.S);
            if (obj.IsArray()) {
                if (((PdfArray)obj).Size() != ascending.Length) {
                    throw new PdfException(KernelExceptionMessageConstant.NUMBER_OF_BOOLEANS_IN_THE_ARRAY_DOES_NOT_CORRESPOND_WITH_THE_NUMBER_OF_FIELDS
                        );
                }
                GetPdfObject().Put(PdfName.A, new PdfArray(ascending));
                return this;
            }
            else {
                throw new PdfException(KernelExceptionMessageConstant.YOU_NEED_A_SINGLE_BOOLEAN_FOR_THIS_COLLECTION_SORT_DICTIONARY
                    );
            }
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
