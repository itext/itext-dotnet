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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Function {
    public sealed class PdfFunctionFactory {
        public const int FUNCTION_TYPE_0 = 0;

        public const int FUNCTION_TYPE_2 = 2;

        public const int FUNCTION_TYPE_3 = 3;

        public const int FUNCTION_TYPE_4 = 4;

        private PdfFunctionFactory() {
        }

        /// <summary>
        /// Factory method to create a function instance based on an existing
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>.
        /// </summary>
        /// <param name="pdfObject">
        /// Either a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// or a
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// representing
        /// a function
        /// </param>
        /// <returns>
        /// Depending on the type, a
        /// <see cref="PdfType0Function"/>
        /// , a
        /// <see cref="PdfType2Function"/>
        /// ,
        /// a
        /// <see cref="PdfType3Function"/>
        /// or a
        /// <see cref="PdfType4Function"/>
        /// </returns>
        public static IPdfFunction Create(PdfObject pdfObject) {
            if (pdfObject.IsDictionary() || pdfObject.IsStream()) {
                PdfDictionary dict = (PdfDictionary)pdfObject;
                switch (dict.GetAsNumber(PdfName.FunctionType).IntValue()) {
                    case FUNCTION_TYPE_0: {
                        if (pdfObject.GetObjectType() != PdfObject.STREAM) {
                            throw new PdfException(KernelExceptionMessageConstant.FUCTIONFACTORY_INVALID_OBJECT_TYPE_TYPE0);
                        }
                        return new PdfType0Function((PdfStream)pdfObject);
                    }

                    case FUNCTION_TYPE_2: {
                        return new PdfType2Function(dict);
                    }

                    case FUNCTION_TYPE_3: {
                        return new PdfType3Function(dict);
                    }

                    case FUNCTION_TYPE_4: {
                        if (pdfObject.GetObjectType() != PdfObject.STREAM) {
                            throw new PdfException(KernelExceptionMessageConstant.FUCTIONFACTORY_INVALID_OBJECT_TYPE_TYPE4);
                        }
                        return new PdfType4Function((PdfStream)pdfObject);
                    }

                    default: {
                        throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.FUCTIONFACTORY_INVALID_FUNCTION_TYPE
                            , dict.GetAsNumber(PdfName.FunctionType).IntValue()));
                    }
                }
            }
            throw new PdfException(KernelExceptionMessageConstant.FUCTIONFACTORY_INVALID_OBJECT_TYPE);
        }
    }
}
