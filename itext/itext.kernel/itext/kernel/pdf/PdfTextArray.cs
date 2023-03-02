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
using System.Collections.Generic;
using System.Text;
using iText.Kernel.Font;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// <see cref="PdfTextArray"/>
    /// defines an array with displacements and
    /// <see cref="PdfString"/>
    /// -objects.
    /// </summary>
    /// <remarks>
    /// <see cref="PdfTextArray"/>
    /// defines an array with displacements and
    /// <see cref="PdfString"/>
    /// -objects.
    /// <para />
    /// A
    /// <see cref="PdfTextArray"/>
    /// is used with the operator TJ in
    /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>.
    /// The first object in this array has to be a
    /// <see cref="PdfString"/>
    /// ;
    /// see reference manual version 1.3 section 8.7.5, pages 346-347.
    /// OR
    /// see reference manual version 1.6 section 5.3.2, pages 378-379.
    /// To emit a more efficient array, we consolidate repeated numbers or strings into single array entries.
    /// For example: "add( 50 ); add( -50 );" will REMOVE the combined zero from the array.
    /// </remarks>
    public class PdfTextArray : PdfArray {
        private float lastNumber = float.NaN;

        private StringBuilder lastString;

        public override void Add(PdfObject pdfObject) {
            if (pdfObject.IsNumber()) {
                Add(((PdfNumber)pdfObject).FloatValue());
            }
            else {
                if (pdfObject is PdfString) {
                    Add(((PdfString)pdfObject).GetValueBytes());
                }
            }
        }

        /// <summary>
        /// Adds content of the
        /// <c>PdfArray</c>.
        /// </summary>
        /// <param name="a">
        /// the
        /// <c>PdfArray</c>
        /// to be added
        /// </param>
        /// <seealso cref="System.Collections.IList{E}.AddAll(System.Collections.ICollection{E})"/>
        public override void AddAll(PdfArray a) {
            if (a != null) {
                AddAll(a.list);
            }
        }

        /// <summary>Adds the Collection of PdfObjects.</summary>
        /// <param name="c">the Collection of PdfObjects to be added</param>
        /// <seealso cref="System.Collections.IList{E}.AddAll(System.Collections.ICollection{E})"/>
        public override void AddAll(ICollection<PdfObject> c) {
            foreach (PdfObject obj in c) {
                Add(obj);
            }
        }

        public virtual bool Add(float number) {
            // adding zero doesn't modify the TextArray at all
            if (number != 0) {
                if (!float.IsNaN(lastNumber)) {
                    lastNumber = number + lastNumber;
                    if (lastNumber != 0) {
                        Set(Size() - 1, new PdfNumber(lastNumber));
                    }
                    else {
                        Remove(Size() - 1);
                        lastNumber = float.NaN;
                    }
                }
                else {
                    lastNumber = number;
                    base.Add(new PdfNumber(lastNumber));
                }
                lastString = null;
                return true;
            }
            return false;
        }

        public virtual bool Add(String text, PdfFont font) {
            // adding an empty string doesn't modify the TextArray at all
            return Add(font.ConvertToBytes(text));
        }

        public virtual bool Add(byte[] text) {
            return Add(new PdfString(text).GetValue());
        }

        protected internal virtual bool Add(String text) {
            if (text.Length > 0) {
                if (lastString != null) {
                    lastString.Append(text);
                    Set(Size() - 1, new PdfString(lastString.ToString()));
                }
                else {
                    lastString = new StringBuilder(text);
                    base.Add(new PdfString(lastString.ToString()));
                }
                lastNumber = float.NaN;
                return true;
            }
            return false;
        }
    }
}
