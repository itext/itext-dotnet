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
    /// for squiggly annotations.
    /// </summary>
    public class SquigglyTextMarkupAnnotationFlattener : AbstractTextMarkupAnnotationFlattener {
        private const double HEIGHT = 1;

        private const double ADVANCE = 1;

        /// <summary>
        /// Creates a new
        /// <see cref="SquigglyTextMarkupAnnotationFlattener"/>
        /// instance.
        /// </summary>
        public SquigglyTextMarkupAnnotationFlattener()
            : base() {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool Draw(PdfAnnotation annotation, PdfPage page) {
            PdfCanvas under = CreateCanvas(page);
            float[] quadPoints = GetQuadPointsAsFloatArray(annotation);
            double baseLineHeight = quadPoints[7] + 1.25;
            double maxHeight = baseLineHeight + HEIGHT;
            double minHeight = baseLineHeight - HEIGHT;
            double maxWidth = page.GetPageSize().GetWidth();
            double xCoordinate = quadPoints[4];
            double endX = quadPoints[6];
            under.SaveState().SetStrokeColor(GetColor(annotation));
            while (xCoordinate <= endX) {
                if (xCoordinate >= maxWidth) {
                    //safety check to avoid infinite loop
                    break;
                }
                under.MoveTo(xCoordinate, baseLineHeight);
                xCoordinate += ADVANCE;
                under.LineTo(xCoordinate, maxHeight);
                xCoordinate += 2 * ADVANCE;
                under.LineTo(xCoordinate, minHeight);
                xCoordinate += ADVANCE;
                under.LineTo(xCoordinate, baseLineHeight);
                under.Stroke();
            }
            under.RestoreState();
            return true;
        }
    }
}
