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
using System;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Test;

namespace iText.Layout.Renderer {
    public abstract class RendererUnitTest : ExtendedITextTest {
        // This also can be converted to a @Rule to have it all at hand in the future
        protected internal static Document CreateDummyDocument() {
            Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
            // setting margins to 0, because it's a dummy parent and this way it would less likely
            // interfere with other calculations
            document.SetMargins(0, 0, 0, 0);
            return document;
        }

        protected internal static TextRenderer CreateLayoutedTextRenderer(String text, Document document) {
            TextRenderer renderer = (TextRenderer)new TextRenderer(new Text(text)).SetParent(document.GetRenderer());
            renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(1000, 1000))));
            return renderer;
        }

        protected internal static ImageRenderer CreateLayoutedImageRenderer(float width, float height, Document document
            ) {
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(width, height));
            Image img = new Image(xObject);
            ImageRenderer renderer = (ImageRenderer)new ImageRenderer(img).SetParent(document.GetRenderer());
            renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(1000, 1000))));
            return renderer;
        }

        protected internal static LayoutArea CreateLayoutArea(float width, float height) {
            return new LayoutArea(1, new Rectangle(width, height));
        }

        protected internal static LayoutContext CreateLayoutContext(float width, float height) {
            return new LayoutContext(CreateLayoutArea(width, height));
        }
    }
}
