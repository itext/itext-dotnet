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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Collection {
    public class PdfCollectionField : PdfObjectWrapper<PdfDictionary> {
        /// <summary>A possible type of collection field.</summary>
        public const int TEXT = 0;

        /// <summary>A possible type of collection field.</summary>
        public const int DATE = 1;

        /// <summary>A possible type of collection field.</summary>
        public const int NUMBER = 2;

        /// <summary>A possible type of collection field.</summary>
        public const int FILENAME = 3;

        /// <summary>A possible type of collection field.</summary>
        public const int DESC = 4;

        /// <summary>A possible type of collection field.</summary>
        public const int MODDATE = 5;

        /// <summary>A possible type of collection field.</summary>
        public const int CREATIONDATE = 6;

        /// <summary>A possible type of collection field.</summary>
        public const int SIZE = 7;

        protected internal int subType;

        protected internal PdfCollectionField(PdfDictionary pdfObject)
            : base(pdfObject) {
            String subType = pdfObject.GetAsName(PdfName.Subtype).GetValue();
            switch (subType) {
                case "D": {
                    this.subType = DATE;
                    break;
                }

                case "N": {
                    this.subType = NUMBER;
                    break;
                }

                case "F": {
                    this.subType = FILENAME;
                    break;
                }

                case "Desc": {
                    this.subType = DESC;
                    break;
                }

                case "ModDate": {
                    this.subType = MODDATE;
                    break;
                }

                case "CreationDate": {
                    this.subType = CREATIONDATE;
                    break;
                }

                case "Size": {
                    this.subType = SIZE;
                    break;
                }

                default: {
                    this.subType = TEXT;
                    break;
                }
            }
        }

        /// <summary>Creates a PdfCollectionField.</summary>
        /// <param name="name">the field name</param>
        /// <param name="subType">the field subtype</param>
        public PdfCollectionField(String name, int subType)
            : base(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.N, new PdfString(name));
            this.subType = subType;
            switch (subType) {
                default: {
                    GetPdfObject().Put(PdfName.Subtype, PdfName.S);
                    break;
                }

                case DATE: {
                    GetPdfObject().Put(PdfName.Subtype, PdfName.D);
                    break;
                }

                case NUMBER: {
                    GetPdfObject().Put(PdfName.Subtype, PdfName.N);
                    break;
                }

                case FILENAME: {
                    GetPdfObject().Put(PdfName.Subtype, PdfName.F);
                    break;
                }

                case DESC: {
                    GetPdfObject().Put(PdfName.Subtype, PdfName.Desc);
                    break;
                }

                case MODDATE: {
                    GetPdfObject().Put(PdfName.Subtype, PdfName.ModDate);
                    break;
                }

                case CREATIONDATE: {
                    GetPdfObject().Put(PdfName.Subtype, PdfName.CreationDate);
                    break;
                }

                case SIZE: {
                    GetPdfObject().Put(PdfName.Subtype, PdfName.Size);
                    break;
                }
            }
        }

        /// <summary>The relative order of the field name.</summary>
        /// <remarks>The relative order of the field name. Fields are sorted in ascending order.</remarks>
        /// <param name="order">a number indicating the order of the field</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollectionField SetOrder(int order) {
            GetPdfObject().Put(PdfName.O, new PdfNumber(order));
            return this;
        }

        /// <summary>Retrieves the order of the field name.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfNumber">PDF number</see>
        /// showing the order of the field name
        /// </returns>
        public virtual PdfNumber GetOrder() {
            return GetPdfObject().GetAsNumber(PdfName.O);
        }

        /// <summary>Sets the initial visibility of the field.</summary>
        /// <param name="visible">is a state of visibility</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollectionField SetVisibility(bool visible) {
            GetPdfObject().Put(PdfName.V, PdfBoolean.ValueOf(visible));
            return this;
        }

        /// <summary>Retrieves the initial visibility of the field.</summary>
        /// <returns>
        /// the initial visibility of the field as
        /// <see cref="iText.Kernel.Pdf.PdfBoolean">PDF boolean</see>
        /// value
        /// </returns>
        public virtual PdfBoolean GetVisibility() {
            return GetPdfObject().GetAsBoolean(PdfName.V);
        }

        /// <summary>Indication if the field value should be editable in the viewer.</summary>
        /// <param name="editable">is a state of editable</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollectionField SetEditable(bool editable) {
            GetPdfObject().Put(PdfName.E, PdfBoolean.ValueOf(editable));
            return this;
        }

        /// <summary>Retrieves the state of the editable of the field.</summary>
        /// <returns>
        /// true if filed is editable and false otherwise. Returned value is presented
        /// as
        /// <see cref="iText.Kernel.Pdf.PdfBoolean">pdf boolean</see>.
        /// </returns>
        public virtual PdfBoolean GetEditable() {
            return GetPdfObject().GetAsBoolean(PdfName.E);
        }

        /// <summary>Converts string to appropriate pdf value.</summary>
        /// <param name="value">is a plain string representation of the value</param>
        /// <returns>
        /// resulting
        /// <see cref="iText.Kernel.Pdf.PdfObject">PDF object</see>
        /// </returns>
        public virtual PdfObject GetValue(String value) {
            switch (subType) {
                case TEXT: {
                    return new PdfString(value);
                }

                case DATE: {
                    return new PdfDate(PdfDate.Decode(value)).GetPdfObject();
                }

                case NUMBER: {
                    return new PdfNumber(Double.Parse(value.Trim(), System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            throw new PdfException(KernelExceptionMessageConstant.UNACCEPTABLE_FIELD_VALUE).SetMessageParams(value, GetPdfObject
                ().GetAsString(PdfName.N).GetValue());
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
