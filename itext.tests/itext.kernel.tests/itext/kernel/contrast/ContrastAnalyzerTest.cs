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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Test;

namespace iText.Kernel.Contrast {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ContrastAnalyzerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BlackTextOnNoBackGround() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.MoveTo(250, 250);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.ShowText("Test");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            NUnit.Framework.Assert.AreEqual(4, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
            dummyDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void WhiteTextOnNoBackGround() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.ShowText("Test");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(4, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteLetterOnBlackBackGroundWhereBackgroundCompletlyCovers() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.Rectangle(30, 30, page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2);
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.Fill();
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.ShowText("T");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteTextOnBlackBackGroundWhereBackgroundCompletlyCovers() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.Rectangle(30, 30, page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2);
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.Fill();
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.ShowText("AT");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(2, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteLetterBlackBackGroundWhereBackgroundHalfCovers() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.Rectangle(260, 250, 200, 200);
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.Fill();
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("T");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(2, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas()[1].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BlackLetterNoFillCovers() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("T");
            canvas.EndText();
            canvas.Rectangle(100, 100, 500, 500).Clip().EndPath();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21.0, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TextDrawnOutsideOfPageShouldNotBeAnalyzed() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.MoveText(-100, -100);
            // Position text outside the page
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("T");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            // No text should be analyzed as it's outside the page
            NUnit.Framework.Assert.AreEqual(0, results.Count);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-9718: Clipping path handling needs to be improved in the ContrastAnalyzer"
            )]
        public virtual void ClipTextShouldNotShowUp() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            // Text positioning
            float x = 50;
            float y = 700;
            canvas.SaveState();
            canvas.Rectangle(50, 690, 200, 60);
            // clip width cuts text
            canvas.Clip();
            canvas.EndPath();
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 48);
            canvas.MoveText(x, y);
            canvas.ShowText("1234567890");
            canvas.EndText();
            canvas.RestoreState();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            //8 characters should be fully or partially visible
            // 9 and 0 should be clipped out
            NUnit.Framework.Assert.AreEqual(8, results.Count);
            foreach (ContrastResult result in results) {
                //will need to change this as for 8 it's partially visible
                //so there will be 2 contrast ratios
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21.0, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteLetterBlackBackGroundWhereBacgroundDoesNotIntersect() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.Rectangle(20, 25, 90, 90);
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.Fill();
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.GREEN, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("T");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(1.37, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteLetter2BackgroundTogetherOverlapSoDefaultBackgroundShouldBeIgnored() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.Rectangle(260, 250, 200, 200);
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.Fill();
            canvas.Rectangle(200, 250, 60, 100);
            canvas.SetColor(ColorConstants.ORANGE, true);
            canvas.Fill();
            canvas.BeginText();
            canvas.MoveText(250, 270);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("T");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(2, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(1.553, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
                NUnit.Framework.Assert.AreEqual(21, result.GetOverlappingAreas()[1].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteTextOnOrangegroundWithLayerOn() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            // Create a layer that is initially ON
            PdfLayer layer = new PdfLayer("Background Layer", dummyDoc);
            layer.SetOn(true);
            PdfCanvas canvas = new PdfCanvas(page);
            // Draw black rectangle background inside the layer
            canvas.BeginLayer(layer);
            canvas.Rectangle(30, 30, page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2);
            canvas.SetColor(ColorConstants.ORANGE, true);
            canvas.Fill();
            canvas.EndLayer();
            // Draw white text on top (outside the layer)
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("Test");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            // When layer is ON, the black background should be visible
            NUnit.Framework.Assert.AreEqual(4, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(1.5539, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteTextOnOrangeBackGroundWithLayerOff() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            // Create a layer that is initially OFF
            PdfLayer layer = new PdfLayer("Background Layer", dummyDoc);
            layer.SetOn(false);
            PdfCanvas canvas = new PdfCanvas(page);
            // Draw black rectangle background inside the layer
            canvas.BeginLayer(layer);
            canvas.Rectangle(30, 30, page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2);
            canvas.SetColor(ColorConstants.ORANGE, true);
            canvas.Fill();
            canvas.EndLayer();
            // Draw white text on top (outside the layer)
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("Test");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            // When layer is OFF, the black background should still be analyzed
            // because PdfCanvasProcessor processes all content regardless of layer state
            // The layer state only affects viewer display, not the content structure
            NUnit.Framework.Assert.AreEqual(4, results.Count);
            foreach (ContrastResult result in results) {
                // The analyzer should still detect the black background even if layer is off
                // because it analyzes the actual PDF content stream
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteLetterBlackBackgroundCircle() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.Circle(300, 300, 100);
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.Fill();
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("T");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteLetterBlackBackgroundTriangle() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.MoveTo(200, 200);
            canvas.LineTo(400, 200);
            canvas.LineTo(300, 400);
            canvas.ClosePath();
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.Fill();
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("T");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WhiteLetterBlackBackgroundTriangleHalfIntersects() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = dummyDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.MoveTo(240, 200);
            canvas.LineTo(400, 200);
            canvas.LineTo(300, 400);
            canvas.ClosePath();
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.Fill();
            canvas.BeginText();
            canvas.MoveText(250, 250);
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 32);
            canvas.ShowText("T");
            canvas.EndText();
            IList<ContrastResult> results = new ContrastAnalyzer(true).CheckPageContrast(page);
            dummyDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, results.Count);
            foreach (ContrastResult result in results) {
                NUnit.Framework.Assert.AreEqual(2, result.GetOverlappingAreas().Count);
                NUnit.Framework.Assert.AreEqual(21, result.GetOverlappingAreas()[0].GetContrastRatio(), 0.1);
                NUnit.Framework.Assert.AreEqual(1, result.GetOverlappingAreas()[1].GetContrastRatio(), 0.1);
            }
        }
    }
}
