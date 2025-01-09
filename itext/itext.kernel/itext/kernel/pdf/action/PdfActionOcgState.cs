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
