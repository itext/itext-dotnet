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
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Action {
    /// <summary>This is a helper class for optional content states use in Set-OCG-State actions.</summary>
    /// <remarks>
    /// This is a helper class for optional content states use in Set-OCG-State actions.
    /// See
    /// <see cref="PdfAction.CreateSetOcgState(System.Collections.Generic.IList{E})"/>.
    /// </remarks>
    public class PdfActionOcgState {
        /// <summary>
        /// Can be:
        /// <see cref="iText.Kernel.Pdf.PdfName.OFF"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.ON"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Toggle"/>
        /// </summary>
        private PdfName state;

        /// <summary>Optional content group dictionaries</summary>
        private IList<PdfDictionary> ocgs;

        /// <summary>Constructs an optional content state object.</summary>
        /// <param name="state">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// describing the state. Shall be one of the following:
        /// <see cref="iText.Kernel.Pdf.PdfName.OFF"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.ON"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Toggle"/>
        /// </param>
        /// <param name="ocgs">a list of the OCG dictionaries</param>
        public PdfActionOcgState(PdfName state, IList<PdfDictionary> ocgs) {
            this.state = state;
            this.ocgs = ocgs;
        }

        /// <summary>Gets the state the optional content groups should be switched to</summary>
        /// <returns>
        /// the state, one of the following:
        /// <see cref="iText.Kernel.Pdf.PdfName.OFF"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.ON"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Toggle"/>
        /// </returns>
        public virtual PdfName GetState() {
            return state;
        }

        /// <summary>Gets a list of optional content groups that shall have the state changed</summary>
        /// <returns>the list of optional content groups</returns>
        public virtual IList<PdfDictionary> GetOcgs() {
            return ocgs;
        }

        /// <summary>
        /// Gets a list of
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// that is describing this particular optional content group states.
        /// </summary>
        /// <returns>
        /// a list of
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// for construction of a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// </returns>
        public virtual IList<PdfObject> GetObjectList() {
            IList<PdfObject> states = new List<PdfObject>();
            states.Add(state);
            states.AddAll(ocgs);
            return states;
        }
    }
}
