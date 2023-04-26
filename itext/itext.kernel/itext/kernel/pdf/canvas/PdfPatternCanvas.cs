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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Canvas {
    /// <summary>A PdfCanvas instance with an inherent tiling pattern.</summary>
    public class PdfPatternCanvas : PdfCanvas {
        private readonly PdfPattern.Tiling tilingPattern;

        /// <summary>Creates PdfPatternCanvas from content stream of page, form XObject, pattern etc.</summary>
        /// <param name="contentStream">The content stream</param>
        /// <param name="resources">The resources, a specialized dictionary that can be used by PDF instructions in the content stream
        ///     </param>
        /// <param name="document">The document that the resulting content stream will be written to</param>
        public PdfPatternCanvas(PdfStream contentStream, PdfResources resources, PdfDocument document)
            : base(contentStream, resources, document) {
            this.tilingPattern = new PdfPattern.Tiling(contentStream);
        }

        /// <summary>Creates PdfPatternCanvas for a document from a provided Tiling pattern</summary>
        /// <param name="pattern">The Tiling pattern must be colored</param>
        /// <param name="document">The document that the resulting content stream will be written to</param>
        public PdfPatternCanvas(PdfPattern.Tiling pattern, PdfDocument document)
            : base((PdfStream)pattern.GetPdfObject(), pattern.GetResources(), document) {
            this.tilingPattern = pattern;
        }

        public override PdfCanvas SetColor(PdfColorSpace colorSpace, float[] colorValue, PdfPattern pattern, bool 
            fill) {
            CheckNoColor();
            return base.SetColor(colorSpace, colorValue, pattern, fill);
        }

        private void CheckNoColor() {
            if (!tilingPattern.IsColored()) {
                throw new PdfException(KernelExceptionMessageConstant.CONTENT_STREAM_MUST_NOT_INVOKE_OPERATORS_THAT_SPECIFY_COLORS_OR_OTHER_COLOR_RELATED_PARAMETERS
                    );
            }
        }
    }
}
