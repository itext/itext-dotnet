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
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class for fill canvas color canvas tag validation.</summary>
    public class FillColorValidationContext : AbstractColorValidationContext {
        /// <summary>
        /// Instantiates a new
        /// <see cref="FillColorValidationContext"/>
        /// based on graphics state, resources and content stream.
        /// </summary>
        /// <param name="canvasGraphicsState">the canvas graphics state</param>
        /// <param name="resources">the resources</param>
        /// <param name="stream">the content stream</param>
        public FillColorValidationContext(CanvasGraphicsState canvasGraphicsState, PdfResources resources, PdfStream
             stream)
            : base(canvasGraphicsState, resources, stream) {
        }

        public override ValidationType GetType() {
            return ValidationType.FILL_COLOR;
        }
    }
}
