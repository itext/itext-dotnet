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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>
    /// Implementation of
    /// <see cref="IAnnotationFlattener"/>
    /// for strikeout annotations.
    /// </summary>
    public class StrikeOutTextMarkupAnnotationFlattener : AbstractTextMarkupAnnotationFlattener {
        /// <summary>
        /// Creates a new
        /// <see cref="StrikeOutTextMarkupAnnotationFlattener"/>
        /// instance.
        /// </summary>
        public StrikeOutTextMarkupAnnotationFlattener()
            : base() {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool Draw(PdfAnnotation annotation, PdfPage page) {
            PdfCanvas under = CreateCanvas(page);
            float[] quadPoints = GetQuadPointsAsFloatArray(annotation);
            double height = quadPoints[7] + ((quadPoints[1] - quadPoints[7]) / 2);
            under.SaveState().SetStrokeColor(GetColor(annotation)).MoveTo(quadPoints[4], height).LineTo(quadPoints[6], 
                height).Stroke().RestoreState();
            return true;
        }
    }
}
