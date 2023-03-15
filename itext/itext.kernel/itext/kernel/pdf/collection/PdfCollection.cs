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
    public class PdfCollection : PdfObjectWrapper<PdfDictionary> {
        /// <summary>A type of initial view</summary>
        public const int DETAILS = 0;

        /// <summary>A type of initial view</summary>
        public const int TILE = 1;

        /// <summary>A type of initial view</summary>
        public const int HIDDEN = 2;

        public PdfCollection(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Constructs a PDF Collection.</summary>
        public PdfCollection()
            : this(new PdfDictionary()) {
        }

        /// <summary>Sets the Collection schema dictionary.</summary>
        /// <param name="schema">an overview of the collection fields</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollection SetSchema(PdfCollectionSchema schema) {
            GetPdfObject().Put(PdfName.Schema, schema.GetPdfObject());
            return this;
        }

        /// <summary>Gets the Collection schema dictionary.</summary>
        /// <returns>the Collection schema dictionary</returns>
        public virtual PdfCollectionSchema GetSchema() {
            return new PdfCollectionSchema(GetPdfObject().GetAsDictionary(PdfName.Schema));
        }

        /// <summary>
        /// Identifies the document that will be initially presented
        /// in the user interface.
        /// </summary>
        /// <param name="documentName">a string that identifies an entry in the EmbeddedFiles name tree</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollection SetInitialDocument(String documentName) {
            GetPdfObject().Put(PdfName.D, new PdfString(documentName));
            return this;
        }

        /// <summary>
        /// Retrieves the document that will be initially presented
        /// in the user interface.
        /// </summary>
        /// <returns>a pdf string that identifies an entry in the EmbeddedFiles name tree</returns>
        public virtual PdfString GetInitialDocument() {
            return GetPdfObject().GetAsString(PdfName.D);
        }

        /// <summary>Sets the initial view.</summary>
        /// <param name="viewType">is a type of view</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollection SetView(int viewType) {
            switch (viewType) {
                default: {
                    GetPdfObject().Put(PdfName.View, PdfName.D);
                    break;
                }

                case TILE: {
                    GetPdfObject().Put(PdfName.View, PdfName.T);
                    break;
                }

                case HIDDEN: {
                    GetPdfObject().Put(PdfName.View, PdfName.H);
                    break;
                }
            }
            return this;
        }

        /// <summary>Check if view is in details mode.</summary>
        /// <returns>true if view is in details mode and false otherwise</returns>
        public virtual bool IsViewDetails() {
            PdfName view = GetPdfObject().GetAsName(PdfName.View);
            return view == null || view.Equals(PdfName.D);
        }

        /// <summary>Check if view is in tile mode.</summary>
        /// <returns>true if view is in tile mode and false otherwise</returns>
        public virtual bool IsViewTile() {
            return PdfName.T.Equals(GetPdfObject().GetAsName(PdfName.View));
        }

        /// <summary>Check if view is hidden.</summary>
        /// <returns>true if view is hidden and false otherwise</returns>
        public virtual bool IsViewHidden() {
            return PdfName.H.Equals(GetPdfObject().GetAsName(PdfName.View));
        }

        /// <summary>Sets the Collection sort dictionary.</summary>
        /// <param name="sort">is the Collection sort dictionary</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Kernel.Pdf.Collection.PdfCollection SetSort(PdfCollectionSort sort) {
            GetPdfObject().Put(PdfName.Sort, sort.GetPdfObject());
            return this;
        }

        /// <summary>Getter for the Collection sort dictionary.</summary>
        /// <returns>the Collection sort</returns>
        public virtual PdfCollectionSort GetSort() {
            return new PdfCollectionSort(GetPdfObject().GetAsDictionary(PdfName.Sort));
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
