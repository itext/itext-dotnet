/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
