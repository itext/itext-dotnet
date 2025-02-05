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
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BlockRendererTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/BlockRendererTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/BlockRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyMinHeightForSpecificDimensionsCausingFloatPrecisionErrorTest() {
            float divHeight = 42.55f;
            Div div = new Div();
            div.SetHeight(UnitValue.CreatePointValue(divHeight));
            float occupiedHeight = 17.981995f;
            float leftHeight = 24.567993f;
            NUnit.Framework.Assert.IsTrue(occupiedHeight + leftHeight < divHeight);
            BlockRenderer blockRenderer = (BlockRenderer)div.CreateRendererSubTree();
            blockRenderer.occupiedArea = new LayoutArea(1, new Rectangle(0, 267.9681f, 0, occupiedHeight));
            AbstractRenderer renderer = blockRenderer.ApplyMinHeight(OverflowPropertyValue.FIT, new Rectangle(0, 243.40012f
                , 0, leftHeight));
            NUnit.Framework.Assert.IsNull(renderer);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, Count = 2, LogLevel
             = LogLevelConstants.ERROR)]
        public virtual void ParentBoxWrapAroundChildBoxesTest() {
            // TODO DEVSIX-6488 all elements should be layouted first in case when parent box should wrap around child boxes
            String cmpFileName = SOURCE_FOLDER + "cmp_parentBoxWrapAroundChildBoxes.pdf";
            String outFile = DESTINATION_FOLDER + "parentBoxWrapAroundChildBoxes.pdf";
            int enoughDivsToOccupyWholePage = 30;
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div();
            div.SetBackgroundColor(ColorConstants.CYAN);
            div.SetProperty(Property.POSITION, LayoutPosition.ABSOLUTE);
            Div childDiv = new Div();
            childDiv.Add(new Paragraph("ChildDiv"));
            childDiv.SetBackgroundColor(ColorConstants.YELLOW);
            childDiv.SetWidth(100);
            for (int i = 0; enoughDivsToOccupyWholePage > i; i++) {
                div.Add(childDiv);
            }
            Div divThatDoesntFitButItsWidthShouldBeConsidered = new Div();
            divThatDoesntFitButItsWidthShouldBeConsidered.Add(new Paragraph("ChildDiv1"));
            divThatDoesntFitButItsWidthShouldBeConsidered.SetBackgroundColor(ColorConstants.GREEN);
            divThatDoesntFitButItsWidthShouldBeConsidered.SetWidth(200);
            div.Add(divThatDoesntFitButItsWidthShouldBeConsidered);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, DESTINATION_FOLDER)
                );
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Div div = new Div();
            div.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN));
            DivRenderer renderer = (DivRenderer)div.GetRenderer();
            PdfFont font = renderer.GetResolvedFont(pdfDocument);
            NUnit.Framework.Assert.AreEqual("Times-Roman", font.GetFontProgram().GetFontNames().GetFontName());
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontWithPdfDocumentNullTest() {
            Div div = new Div();
            DivRenderer renderer = (DivRenderer)div.GetRenderer();
            PdfFont font = renderer.GetResolvedFont(null);
            NUnit.Framework.Assert.IsNull(font);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontFromFontProviderTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Div div = new Div();
            FontProvider provider = new FontProvider();
            provider.GetFontSet().AddFont(StandardFonts.COURIER, null, "courier");
            div.SetProperty(Property.FONT_PROVIDER, provider);
            div.SetProperty(Property.FONT, new String[] { "courier" });
            DivRenderer renderer = (DivRenderer)div.GetRenderer();
            PdfFont font = renderer.GetResolvedFont(pdfDocument);
            NUnit.Framework.Assert.AreEqual("Courier", font.GetFontProgram().GetFontNames().GetFontName());
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontFromFontProviderNullTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Div div = new Div();
            div.SetProperty(Property.FONT_PROVIDER, null);
            div.SetProperty(Property.FONT, new String[] { "courier" });
            DivRenderer renderer = (DivRenderer)div.GetRenderer();
            PdfFont font = renderer.GetResolvedFont(pdfDocument);
            NUnit.Framework.Assert.AreEqual("Helvetica", font.GetFontProgram().GetFontNames().GetFontName());
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontFromFontProviderNullAndDocNullTest() {
            Div div = new Div();
            div.SetProperty(Property.FONT_PROVIDER, null);
            div.SetProperty(Property.FONT, new String[] { "courier" });
            DivRenderer renderer = (DivRenderer)div.GetRenderer();
            PdfFont font = renderer.GetResolvedFont(null);
            NUnit.Framework.Assert.IsNull(font);
        }
    }
}
