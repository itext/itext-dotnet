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
using System;
using iText.Kernel.Geom;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>This class acts as a default implementation of IPdfTextLocation</summary>
    public class DefaultPdfTextLocation : IPdfTextLocation {
        private Rectangle rectangle;

        private String text;

        /// <summary>Creates new pdf text location.</summary>
        /// <param name="rect">text rectangle on pdf canvas</param>
        /// <param name="text">actual text on designated area of canvas</param>
        public DefaultPdfTextLocation(Rectangle rect, String text) {
            this.rectangle = rect;
            this.text = text;
        }

        /// <summary><inheritDoc/></summary>
        public virtual Rectangle GetRectangle() {
            return rectangle;
        }

        /// <summary>Sets text rectangle (occupied area) for this pdf text location.</summary>
        /// <param name="rectangle">new text rectangle</param>
        /// <returns>
        /// this
        /// <c>DefaultPdfTextLocation</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.DefaultPdfTextLocation SetRectangle(Rectangle rectangle
            ) {
            this.rectangle = rectangle;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetText() {
            return text;
        }

        /// <summary>Sets text for this pdf text location.</summary>
        /// <param name="text">new text</param>
        /// <returns>
        /// this
        /// <c>DefaultPdfTextLocation</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.DefaultPdfTextLocation SetText(String text) {
            this.text = text;
            return this;
        }
    }
}
