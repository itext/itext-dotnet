/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    public class TextRendererTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NextRendererTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            pdfDoc.AddNewPage();
            Document doc = new Document(pdfDoc);
            RootRenderer documentRenderer = doc.GetRenderer();
            Text text = new Text("hello");
            text.SetNextRenderer(new TextRenderer(text));
            IRenderer textRenderer1 = text.GetRenderer().SetParent(documentRenderer);
            IRenderer textRenderer2 = text.GetRenderer().SetParent(documentRenderer);
            LayoutArea area = new LayoutArea(1, new Rectangle(100, 100, 100, 100));
            LayoutContext layoutContext = new LayoutContext(area);
            doc.Close();
            LayoutResult result1 = textRenderer1.Layout(layoutContext);
            LayoutResult result2 = textRenderer2.Layout(layoutContext);
            NUnit.Framework.Assert.AreEqual(result1.GetOccupiedArea(), result2.GetOccupiedArea());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT)]
        public virtual void SetTextException() {
            String val = "other text";
            String fontName = "Helvetica";
            TextRenderer rend = (TextRenderer)new Text("basic text").GetRenderer();
            FontProvider fp = new FontProvider();
            fp.AddFont(fontName);
            rend.SetProperty(Property.FONT_PROVIDER, fp);
            rend.SetProperty(Property.FONT, fontName);
            rend.SetText(val);
            NUnit.Framework.Assert.AreEqual(val, rend.GetText().ToString());
        }

        /// <summary>
        /// This test assumes that absolute positioning for
        /// <see cref="iText.Layout.Element.Text"/>
        /// elements is
        /// not supported. Adding this support is the subject of DEVSIX-1393.
        /// </summary>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT)]
        public virtual void SetFontAsText() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            pdfDoc.AddNewPage();
            Document doc = new Document(pdfDoc);
            Text txt = new Text("text");
            txt.SetProperty(Property.POSITION, LayoutPosition.ABSOLUTE);
            txt.SetProperty(Property.TOP, 5f);
            FontProvider fp = new FontProvider();
            fp.AddFont("Helvetica");
            txt.SetProperty(Property.FONT_PROVIDER, fp);
            txt.SetFont("Helvetica");
            doc.Add(new Paragraph().Add(txt));
            doc.Close();
        }
    }
}
