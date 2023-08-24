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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>
    /// Implementation of
    /// <see cref="IAnnotationFlattener"/>
    /// for highlight text markup annotations.
    /// </summary>
    public class HighLightTextMarkupAnnotationFlattener : AbstractTextMarkupAnnotationFlattener {
        /// <summary>
        /// Creates a new
        /// <see cref="HighLightTextMarkupAnnotationFlattener"/>
        /// instance.
        /// </summary>
        public HighLightTextMarkupAnnotationFlattener()
            : base() {
        }

        /// <summary>Creates a canvas.</summary>
        /// <remarks>Creates a canvas. It will draw below the other items on the canvas.</remarks>
        /// <param name="page">the page to draw the annotation on</param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// the annotation will be drawn upon.
        /// </returns>
        protected internal override PdfCanvas CreateCanvas(PdfPage page) {
            return new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), page.GetDocument());
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool Draw(PdfAnnotation annotation, PdfPage page) {
            PdfCanvas under = CreateCanvas(page);
            float[] values = GetQuadPointsAsFloatArray(annotation);
            under.SaveState().SetColor(GetColor(annotation), true).MoveTo(values[0], values[1]).LineTo(values[2], values
                [3]).LineTo(values[6], values[7]).LineTo(values[4], values[5]).LineTo(values[0], values[1]).Fill().RestoreState
                ();
            return true;
        }
    }
}
