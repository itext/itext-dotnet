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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Action {
    /// <summary>This class is a wrapper around a Windows launch parameter dictionary.</summary>
    public class PdfWin : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Creates a new wrapper around an existing Windows launch parameter dictionary.</summary>
        /// <param name="pdfObject">the dictionary to create a wrapper for</param>
        public PdfWin(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Creates a new wrapper around a newly created Windows launch parameter dictionary.</summary>
        /// <param name="f">
        /// the file name of the application that shall be launched or the document that shall be opened or printed,
        /// in standard Windows pathname format. If the name string includes a backslash character (\),
        /// the backslash shall itself be preceded by a backslash.
        /// </param>
        public PdfWin(PdfString f)
            : this(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.F, f);
        }

        /// <summary>Creates a new wrapper around a newly created Windows launch parameter dictionary.</summary>
        /// <param name="f">
        /// the file name of the application that shall be launched or the document that shall be opened or printed,
        /// in standard Windows pathname format. If the name string includes a backslash character (\),
        /// the backslash shall itself be preceded by a backslash
        /// </param>
        /// <param name="d">a bye string specifying the default directory in standard DOS syntax</param>
        /// <param name="o">
        /// an ASCII string specifying the operation to perform on the file. Shall be one of the following:
        /// "open", "print"
        /// </param>
        /// <param name="p">
        /// a parameter string that shall be passed to the application.
        /// This entry shall be omitted if a document is abound to be opened
        /// </param>
        public PdfWin(PdfString f, PdfString d, PdfString o, PdfString p)
            : this(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.F, f);
            GetPdfObject().Put(PdfName.D, d);
            GetPdfObject().Put(PdfName.O, o);
            GetPdfObject().Put(PdfName.P, p);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
