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
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class for extended graphics state validation.</summary>
    public class ExtendedGStateValidationContext : IValidationContext, IGraphicStateValidationParameter, IContentStreamValidationParameter {
        private readonly CanvasGraphicsState graphicsState;

        private readonly PdfStream contentStream;

        /// <summary>
        /// Instantiates a new
        /// <see cref="ExtendedGStateValidationContext"/>
        /// based on graphics state and content stream.
        /// </summary>
        /// <param name="graphicsState">the graphics state</param>
        /// <param name="contentStream">the content stream</param>
        public ExtendedGStateValidationContext(CanvasGraphicsState graphicsState, PdfStream contentStream) {
            this.graphicsState = graphicsState;
            this.contentStream = contentStream;
        }

        public virtual PdfStream GetContentStream() {
            return contentStream;
        }

        public virtual CanvasGraphicsState GetGraphicsState() {
            return graphicsState;
        }

        public virtual ValidationType GetType() {
            return ValidationType.EXTENDED_GRAPHICS_STATE;
        }
    }
}
