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
