/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class FootnoteAnchorRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DirectPropertyAndStyleOnFootnoteAnchorAreCopiedToTextAnchorInFootnoteTest() {
            Footnote footnote = new Footnote("footnote text");
            FootnoteAnchor anchor = new FootnoteAnchor(new Text("anchor text"), footnote);
            anchor.SetOpacity(0.25f);
            anchor.AddStyle(new Style().SetRotationAngle(0.5f));
            IElement injectedText = LayoutAnchorAndGetInjectedAnchor(anchor);
            NUnit.Framework.Assert.AreNotSame(anchor.GetFootnoteAnchor(), injectedText);
            NUnit.Framework.Assert.AreEqual(0.25f, injectedText.GetProperty<float?>(Property.OPACITY), 1e-10);
            NUnit.Framework.Assert.AreEqual(0.5f, injectedText.GetProperty<float?>(Property.ROTATION_ANGLE), 1e-10);
        }

        [NUnit.Framework.Test]
        public virtual void DirectPropertyAndStyleOnFootnoteAnchorAreCopiedToImageAnchorInFootnoteTest() {
            Footnote footnote = new Footnote("footnote text");
            FootnoteAnchor anchor = new FootnoteAnchor(new Image(ImageDataFactory.CreateRawImage(new byte[] { 50, 20 }
                )), footnote);
            anchor.SetOpacity(0.7f);
            anchor.AddStyle(new Style().SetRotationAngle(0.75f));
            IElement injectedImage = LayoutAnchorAndGetInjectedAnchor(anchor);
            NUnit.Framework.Assert.AreNotSame(anchor.GetFootnoteAnchor(), injectedImage);
            NUnit.Framework.Assert.AreEqual(0.7f, injectedImage.GetProperty<float?>(Property.OPACITY), 1e-10);
            NUnit.Framework.Assert.AreEqual(0.75f, injectedImage.GetProperty<float?>(Property.ROTATION_ANGLE), 1e-10);
        }

        [NUnit.Framework.Test]
        public virtual void DirectPropertyAndStyleOnFootnoteAnchorAreCopiedToTextAnchorInMainTextTest() {
            Footnote footnote = new Footnote("footnote text");
            FootnoteAnchor anchor = new FootnoteAnchor(new Text("anchor text"), footnote);
            anchor.SetOpacity(0.25f);
            anchor.AddStyle(new Style().SetRotationAngle(0.5f));
            LayoutAnchorAndGetInjectedAnchor(anchor);
            IElement mainTextAnchor = anchor.GetFootnoteAnchor();
            NUnit.Framework.Assert.AreEqual(0.25f, mainTextAnchor.GetProperty<float?>(Property.OPACITY), 1e-10);
            NUnit.Framework.Assert.AreEqual(0.5f, mainTextAnchor.GetProperty<float?>(Property.ROTATION_ANGLE), 1e-10);
        }

        [NUnit.Framework.Test]
        public virtual void DirectPropertyAndStyleOnFootnoteAnchorAreCopiedToImageAnchorInMainTextTest() {
            Footnote footnote = new Footnote("footnote text");
            FootnoteAnchor anchor = new FootnoteAnchor(new iText.Layout.Element.Image(ImageDataFactory.CreateRawImage(
                new byte[] { 50, 20 })), footnote);
            anchor.SetOpacity(0.7f);
            anchor.AddStyle(new Style().SetRotationAngle(0.75f));
            LayoutAnchorAndGetInjectedAnchor(anchor);
            IElement mainTextAnchor = anchor.GetFootnoteAnchor();
            NUnit.Framework.Assert.AreEqual(0.7f, mainTextAnchor.GetProperty<float?>(Property.OPACITY), 1e-10);
            NUnit.Framework.Assert.AreEqual(0.75f, mainTextAnchor.GetProperty<float?>(Property.ROTATION_ANGLE), 1e-10);
        }

        [NUnit.Framework.Test]
        public virtual void CopyPropertiesAndStylesDoesNotOverrideDirectPropertyOnAnchorSymbolCopyTest() {
            Footnote footnote = new Footnote("footnote text");
            Text anchorSymbol = new Text("anchor text").SetOpacity(0.9f);
            FootnoteAnchor anchor = new FootnoteAnchor(anchorSymbol, footnote);
            anchor.SetOpacity(0.1f);
            IElement injectedText = LayoutAnchorAndGetInjectedAnchor(anchor);
            NUnit.Framework.Assert.AreEqual(0.9f, injectedText.GetProperty<float?>(Property.OPACITY), 1e-10);
        }

        [NUnit.Framework.Test]
        public virtual void CopyPropertiesAndStylesDoesNotOverrideStylePropertyOnAnchorSymbolCopyTest() {
            Footnote footnote = new Footnote("footnote text");
            Text anchorSymbol = new Text("anchor text");
            anchorSymbol.AddStyle(new Style().SetRotationAngle(1.2f));
            FootnoteAnchor anchor = new FootnoteAnchor(anchorSymbol, footnote);
            anchor.AddStyle(new Style().SetRotationAngle(0.4f));
            IElement injectedText = LayoutAnchorAndGetInjectedAnchor(anchor);
            NUnit.Framework.Assert.AreEqual(1.2f, injectedText.GetProperty<float?>(Property.ROTATION_ANGLE), 1e-10);
        }

        private static IElement LayoutAnchorAndGetInjectedAnchor(FootnoteAnchor anchor) {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            using (Document document = new Document(pdfDocument)) {
                FootnoteAnchorRenderer renderer = new FootnoteAnchorRenderer(anchor);
                renderer.SetParent(document.GetRenderer());
                renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(500, 500))));
                return FootnotesUtil.GetInjectedFootnoteAnchor(anchor.GetFootnote());
            }
        }
    }
}
