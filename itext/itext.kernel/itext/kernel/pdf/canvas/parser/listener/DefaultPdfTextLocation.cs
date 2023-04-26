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
using System;
using iText.Kernel.Geom;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>This class acts as a default implementation of IPdfTextLocation</summary>
    public class DefaultPdfTextLocation : IPdfTextLocation {
        private int pageNr;

        private Rectangle rectangle;

        private String text;

        public DefaultPdfTextLocation(int pageNr, Rectangle rect, String text) {
            this.pageNr = pageNr;
            this.rectangle = rect;
            this.text = text;
        }

        public virtual Rectangle GetRectangle() {
            return rectangle;
        }

        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.DefaultPdfTextLocation SetRectangle(Rectangle rectangle
            ) {
            this.rectangle = rectangle;
            return this;
        }

        public virtual String GetText() {
            return text;
        }

        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.DefaultPdfTextLocation SetText(String text) {
            this.text = text;
            return this;
        }

        public virtual int GetPageNumber() {
            return pageNr;
        }

        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.DefaultPdfTextLocation SetPageNr(int pageNr) {
            this.pageNr = pageNr;
            return this;
        }
    }
}
