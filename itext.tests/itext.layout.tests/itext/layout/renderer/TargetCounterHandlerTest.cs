/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Splitting;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TargetCounterHandlerTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/renderer/TargetCounterHandlerTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/renderer/TargetCounterHandlerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererAddByIDTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            DivRenderer divRenderer = new DivRenderer(new Div());
            divRenderer.SetParent(documentRenderer);
            String id = "id5";
            divRenderer.SetProperty(Property.ID, id);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(4, new Rectangle(50, 50)));
            divRenderer.Layout(layoutContext);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            NUnit.Framework.Assert.AreEqual((int?)4, TargetCounterHandler.GetPageByID(divRenderer, id));
        }

        [NUnit.Framework.Test]
        public virtual void TextRendererAddByIDTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            TextRenderer textRenderer = new TextRenderer(new Text("a"));
            textRenderer.SetProperty(Property.TEXT_RISE, 20F);
            textRenderer.SetProperty(Property.CHARACTER_SPACING, 20F);
            textRenderer.SetProperty(Property.WORD_SPACING, 20F);
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            textRenderer.SetProperty(Property.FONT_SIZE, new UnitValue(UnitValue.POINT, 20));
            textRenderer.SetProperty(Property.SPLIT_CHARACTERS, new DefaultSplitCharacters());
            textRenderer.SetParent(documentRenderer);
            String id = "id7";
            textRenderer.SetProperty(Property.ID, id);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(4, new Rectangle(50, 50)));
            textRenderer.Layout(layoutContext);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            NUnit.Framework.Assert.AreEqual((int?)4, TargetCounterHandler.GetPageByID(textRenderer, id));
        }

        [NUnit.Framework.Test]
        public virtual void TableRendererAddByIDTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            TableRenderer tableRenderer = new TableRenderer(new Table(5));
            tableRenderer.SetParent(documentRenderer);
            String id = "id5";
            tableRenderer.SetProperty(Property.ID, id);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(4, new Rectangle(50, 50)));
            tableRenderer.Layout(layoutContext);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            NUnit.Framework.Assert.AreEqual((int?)4, TargetCounterHandler.GetPageByID(tableRenderer, id));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererAddByIDTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            ParagraphRenderer paragraphRenderer = new ParagraphRenderer(new Paragraph());
            paragraphRenderer.SetParent(documentRenderer);
            String id = "id5";
            paragraphRenderer.SetProperty(Property.ID, id);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(4, new Rectangle(50, 50)));
            paragraphRenderer.Layout(layoutContext);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            NUnit.Framework.Assert.AreEqual((int?)4, TargetCounterHandler.GetPageByID(paragraphRenderer, id));
        }

        [NUnit.Framework.Test]
        public virtual void ImageRendererAddByIDTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            ImageRenderer imageRenderer = new ImageRenderer(new Image(ImageDataFactory.CreateRawImage(new byte[] { 50, 
                21 })));
            imageRenderer.SetParent(documentRenderer);
            String id = "id6";
            imageRenderer.SetProperty(Property.ID, id);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(4, new Rectangle(50, 50)));
            imageRenderer.Layout(layoutContext);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            NUnit.Framework.Assert.AreEqual((int?)4, TargetCounterHandler.GetPageByID(imageRenderer, id));
        }

        [NUnit.Framework.Test]
        public virtual void LineRendererAddByIDTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(documentRenderer);
            String id = "id6";
            lineRenderer.SetProperty(Property.ID, id);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(4, new Rectangle(50, 50)));
            lineRenderer.Layout(layoutContext);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            NUnit.Framework.Assert.AreEqual((int?)4, TargetCounterHandler.GetPageByID(lineRenderer, id));
        }

        [NUnit.Framework.Test]
        public virtual void TargetCounterHandlerEndToEndLayoutTest() {
            String targetPdf = DESTINATION_FOLDER + "targetCounterHandlerEndToEndLayoutTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_targetCounterHandlerEndToEndLayoutTest.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(targetPdf)), PageSize.A4, false);
            Text pageNumPlaceholder = new Text("x");
            String id = "1";
            pageNumPlaceholder.SetProperty(Property.ID, id);
            pageNumPlaceholder.SetNextRenderer(new TargetCounterHandlerTest.TargetCounterAwareTextRenderer(pageNumPlaceholder
                ));
            Paragraph intro = new Paragraph("The paragraph is on page ").Add(pageNumPlaceholder);
            document.Add(intro);
            document.Add(new AreaBreak());
            Paragraph text = new Paragraph("This is main text");
            text.SetProperty(Property.ID, id);
            text.SetNextRenderer(new TargetCounterHandlerTest.TargetCounterAwareParagraphRenderer(text));
            document.Add(text);
            document.Relayout();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(targetPdf, cmpPdf, DESTINATION_FOLDER, "diff"
                ));
        }

        private class TargetCounterAwareTextRenderer : TextRenderer {
            public TargetCounterAwareTextRenderer(Text link)
                : base(link) {
            }

            public override LayoutResult Layout(LayoutContext layoutContext) {
                int? targetPageNumber = TargetCounterHandler.GetPageByID(this, this.GetProperty<String>(Property.ID));
                if (targetPageNumber != null) {
                    SetText(targetPageNumber.ToString());
                }
                return base.Layout(layoutContext);
            }

            public override IRenderer GetNextRenderer() {
                return new TargetCounterHandlerTest.TargetCounterAwareTextRenderer((Text)GetModelElement());
            }
        }

        private class TargetCounterAwareParagraphRenderer : ParagraphRenderer {
            public TargetCounterAwareParagraphRenderer(Paragraph modelElement)
                : base(modelElement) {
            }

            public override IRenderer GetNextRenderer() {
                return new TargetCounterHandlerTest.TargetCounterAwareParagraphRenderer((Paragraph)modelElement);
            }

            public override LayoutResult Layout(LayoutContext layoutContext) {
                LayoutResult result = base.Layout(layoutContext);
                TargetCounterHandler.AddPageByID(this);
                return result;
            }
        }
    }
}
