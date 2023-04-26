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
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Renderer {
    public class DrawContext {
        private PdfDocument document;

        private PdfCanvas canvas;

        private bool taggingEnabled;

        public DrawContext(PdfDocument document, PdfCanvas canvas)
            : this(document, canvas, false) {
        }

        public DrawContext(PdfDocument document, PdfCanvas canvas, bool enableTagging) {
            this.document = document;
            this.canvas = canvas;
            this.taggingEnabled = enableTagging;
        }

        public virtual PdfDocument GetDocument() {
            return document;
        }

        public virtual PdfCanvas GetCanvas() {
            return canvas;
        }

        public virtual bool IsTaggingEnabled() {
            return taggingEnabled;
        }

        public virtual void SetTaggingEnabled(bool taggingEnabled) {
            this.taggingEnabled = taggingEnabled;
        }
    }
}
