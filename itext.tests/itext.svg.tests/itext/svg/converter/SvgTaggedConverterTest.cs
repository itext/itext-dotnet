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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Svg.Processors.Impl;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Svg.Converter {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class SvgTaggedConverterTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/converter/SvgTaggedConverterTest/";

        public static readonly String DEST_FOLDER = TestUtil.GetOutputPath() + "/svg/converter/SvgTaggedConverterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DEST_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSvgTagged() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "simple.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_simple.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destination, writerProperties));
            pdfDocument.AddNewPage();
            SvgConverter.DrawOnDocument(FileUtil.GetInputStreamForFile(source), pdfDocument, 1);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destination, cmpFile, DEST_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleUACompliantSvgTagged() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "simpleUACompliantSvgTagged.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            pdfDocument.AddNewPage();
            SvgConverter.DrawOnDocument(FileUtil.GetInputStreamForFile(source), pdfDocument, 1);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSvgTaggedWithConverterProperties() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "simpleSvgTaggedWithConverterProperties.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            pdfDocument.AddNewPage();
            SvgConverterProperties properties = new SvgConverterProperties();
            SvgConverter.DrawOnDocument(FileUtil.GetInputStreamForFile(source), pdfDocument, 1, properties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSvgTaggedWithConverterPropertiesEmptyAlternateDescriptionEmpty() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "simpleSvgTaggedWithConverterPropertiesEmptyAlternateDescriptionEmpty.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            pdfDocument.AddNewPage();
            SvgConverterProperties properties = new SvgConverterProperties();
            properties.GetAccessibilityProperties().SetAlternateDescription("");
            SvgConverter.DrawOnDocument(FileUtil.GetInputStreamForFile(source), pdfDocument, 1, properties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSvgTaggedWithConverterPropertiesEmptyAlternateDescriptionSomeContent() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "simpleSvgTaggedWithConverterPropertiesEmptyAlternateDescriptionSomeContent.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            pdfDocument.AddNewPage();
            SvgConverterProperties properties = new SvgConverterProperties();
            properties.GetAccessibilityProperties().SetAlternateDescription("Hello there, ");
            SvgConverter.DrawOnDocument(FileUtil.GetInputStreamForFile(source), pdfDocument, 1, properties);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(destination));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void SimpleSvgTaggedWithConverterPropertiesTaggedAsArtifact() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "simpleSvgTaggedWithConverterPropertiesTaggedAsArtifact.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            pdfDocument.AddNewPage();
            SvgConverterProperties properties = new SvgConverterProperties();
            properties.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
            SvgConverter.DrawOnDocument(FileUtil.GetInputStreamForFile(source), pdfDocument, 1, properties);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(destination));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void ConvertToImage() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "convertToImage.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            Image image = SvgConverter.ConvertToImage(FileUtil.GetInputStreamForFile(source), pdfDocument);
            image.GetAccessibilityProperties().SetAlternateDescription("Hello!");
            Document document = new Document(pdfDocument);
            document.Add(image);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(destination));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void ConvertToImageWithProps() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "convertToImageWithProps.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            SvgConverterProperties props = new SvgConverterProperties();
            props.GetAccessibilityProperties().SetAlternateDescription("Bing bong");
            Image image = SvgConverter.ConvertToImage(FileUtil.GetInputStreamForFile(source), pdfDocument, props);
            NUnit.Framework.Assert.AreEqual("Bing bong", image.GetAccessibilityProperties().GetAlternateDescription());
            NUnit.Framework.Assert.AreEqual(StandardRoles.FIGURE, image.GetAccessibilityProperties().GetRole());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToImageWithPropsArtifacts() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "convertToImageWithPropsArtifacts.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            SvgConverterProperties props = new SvgConverterProperties();
            props.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
            Image image = SvgConverter.ConvertToImage(FileUtil.GetInputStreamForFile(source), pdfDocument, props);
            NUnit.Framework.Assert.AreEqual(StandardRoles.ARTIFACT, image.GetAccessibilityProperties().GetRole());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPage01() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "drawOnPage01.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_drawOnPage01.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destination, writerProperties));
            PdfPage page = pdfDocument.AddNewPage();
            SvgConverter.DrawOnPage(FileUtil.GetInputStreamForFile(source), page);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destination, cmpFile, DEST_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageWithUaCompliant() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "drawOnPageUa.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            PdfPage page = pdfDocument.AddNewPage();
            SvgConverter.DrawOnPage(FileUtil.GetInputStreamForFile(source), page);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPage02() {
            String source = SOURCE_FOLDER + "simple.svg";
            String destination = DEST_FOLDER + "drawOnPage02.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfUADocument pdfDocument = new PdfUADocument(new PdfWriter(destination, writerProperties), new PdfUAConfig
                (PdfUAConformance.PDF_UA_2, "ua title", "en-US"));
            SvgConverterProperties converterProperties = new SvgConverterProperties();
            converterProperties.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
            PdfPage page = pdfDocument.AddNewPage();
            SvgConverter.DrawOnPage(FileUtil.GetInputStreamForFile(source), page, converterProperties);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(destination));
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    }
}
