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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Navigation {
    public abstract class PdfDestination : PdfObjectWrapper<PdfObject> {
        protected internal PdfDestination(PdfObject pdfObject)
            : base(pdfObject) {
        }

        public abstract PdfObject GetDestinationPage(IPdfNameTreeAccess names);

        /// <summary>
        /// Creates
        /// <see cref="PdfDestination"/>
        /// implementation based on provided
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>.
        /// </summary>
        /// <param name="pdfObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// from which
        /// <see cref="PdfDestination"/>
        /// shall be created
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfDestination"/>
        /// implementation
        /// </returns>
        public static iText.Kernel.Pdf.Navigation.PdfDestination MakeDestination(PdfObject pdfObject) {
            return MakeDestination(pdfObject, true);
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfDestination"/>
        /// implementation based on provided
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>.
        /// </summary>
        /// <param name="pdfObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// from which
        /// <see cref="PdfDestination"/>
        /// shall be created
        /// </param>
        /// <param name="throwException">
        /// if
        /// <see langword="true"/>
        /// , throws exception in case of invalid parameter
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfDestination"/>
        /// implementation
        /// </returns>
        public static iText.Kernel.Pdf.Navigation.PdfDestination MakeDestination(PdfObject pdfObject, bool throwException
            ) {
            if (pdfObject.GetObjectType() == PdfObject.STRING) {
                return new PdfStringDestination((PdfString)pdfObject);
            }
            else {
                if (pdfObject.GetObjectType() == PdfObject.NAME) {
                    return new PdfNamedDestination((PdfName)pdfObject);
                }
                else {
                    if (pdfObject.GetObjectType() == PdfObject.ARRAY) {
                        PdfArray destArray = (PdfArray)pdfObject;
                        if (destArray.IsEmpty()) {
                            if (throwException) {
                                throw new ArgumentException();
                            }
                            else {
                                return null;
                            }
                        }
                        else {
                            PdfObject firstObj = destArray.Get(0);
                            // In case of explicit destination for remote go-to action this is a page number
                            if (firstObj.IsNumber()) {
                                return new PdfExplicitRemoteGoToDestination(destArray);
                            }
                            // In case of explicit destination for not remote go-to action this is a page dictionary
                            if (firstObj.IsDictionary() && PdfName.Page.Equals(((PdfDictionary)firstObj).GetAsName(PdfName.Type))) {
                                return new PdfExplicitDestination(destArray);
                            }
                            // In case of structure destination this is a struct element dictionary or a string ID. Type is not required for structure elements
                            return new PdfStructureDestination(destArray);
                        }
                    }
                    else {
                        if (throwException) {
                            throw new NotSupportedException();
                        }
                        else {
                            return null;
                        }
                    }
                }
            }
        }
    }
}
