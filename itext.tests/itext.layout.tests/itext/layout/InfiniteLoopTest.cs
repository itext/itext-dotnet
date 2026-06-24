using System;
using iText.IO.Image;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class InfiniteLoopTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/InfiniteLoopTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/InfiniteLoopTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            //We need to clean this because it generate a very big pdf file which is not used.
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void InfiniteLoopWithPartialResultTest() {
            String outFileName = DESTINATION_FOLDER + "infiniteLoopWithPartialResult.pdf";
            DocumentProperties documentProperties = new DocumentProperties();
            documentProperties.RegisterDependency(typeof(LayoutInfiniteLoopResolver), () => new LayoutInfiniteLoopResolver
                (10_000));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName), documentProperties)) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph paragraph = new _Paragraph_55();
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(paragraph));
                }
            }
        }

        private sealed class _Paragraph_55 : Paragraph {
            public _Paragraph_55() {
            }

            protected internal override IRenderer MakeNewRenderer() {
                return new InfiniteLoopTest.ReturningPartialRenderer(this);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 450)]
        public virtual void VeryBigLayoutThrowsTest() {
            String outFileName = DESTINATION_FOLDER + "veryBigLayoutThrows.pdf";
            DocumentProperties documentProperties = new DocumentProperties();
            documentProperties.RegisterDependency(typeof(LayoutInfiniteLoopResolver), () => new LayoutInfiniteLoopResolver
                (300));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName), documentProperties)) {
                pdfDocument.SetDefaultPageSize(PageSize.A10);
                using (Document document = new Document(pdfDocument)) {
                    Div container = new Div();
                    for (int i = 0; i < 451; ++i) {
                        PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "Desert.jpg"));
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 50);
                        container.Add(image);
                    }
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(container));
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 450)]
        public virtual void NotBigEnoughLayoutDoesntThrowTest() {
            String outFileName = DESTINATION_FOLDER + "notBigEnoughLayoutDoesntThrow.pdf";
            DocumentProperties documentProperties = new DocumentProperties();
            documentProperties.RegisterDependency(typeof(LayoutInfiniteLoopResolver), () => new LayoutInfiniteLoopResolver
                (300));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName), documentProperties)) {
                pdfDocument.SetDefaultPageSize(PageSize.A10);
                using (Document document = new Document(pdfDocument)) {
                    Div container = new Div();
                    for (int i = 0; i < 450; ++i) {
                        PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "Desert.jpg"));
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 50);
                        container.Add(image);
                    }
                    NUnit.Framework.Assert.DoesNotThrow(() => document.Add(container));
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 600)]
        public virtual void LimitWithKeepTogetherEvenSmallerTest() {
            String outFileName = DESTINATION_FOLDER + "limitWithKeepTogetherEvenSmaller.pdf";
            DocumentProperties documentProperties = new DocumentProperties();
            documentProperties.RegisterDependency(typeof(LayoutInfiniteLoopResolver), () => new LayoutInfiniteLoopResolver
                (300));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName), documentProperties)) {
                pdfDocument.SetDefaultPageSize(PageSize.A10);
                using (Document document = new Document(pdfDocument)) {
                    Div container = new Div();
                    for (int i = 0; i < 301; ++i) {
                        PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "Desert.jpg"));
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 50);
                        image.SetProperty(Property.KEEP_TOGETHER, true);
                        container.Add(image);
                    }
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(container));
                }
            }
        }

        private class ReturningPartialRenderer : ParagraphRenderer {
            public ReturningPartialRenderer(Paragraph modelElement)
                : base(modelElement) {
            }

            public override LayoutResult Layout(LayoutContext layoutContext) {
                LayoutResult layoutResult = base.Layout(layoutContext);
                layoutResult.SetStatus(LayoutResult.PARTIAL);
                layoutResult.SetOverflowRenderer(this);
                layoutResult.SetSplitRenderer(this);
                return layoutResult;
            }
        }
    }
}
