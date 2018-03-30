using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.StyledXmlParser;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    public class LineSvgNodeRendererTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/LineSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/LineSvgNodeRendererTest/";

        public const String expectedExceptionMessage = SvgLogMessageConstant.FLOAT_PARSING_NAN;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LineRendererTest() {
            String filename = "lineSvgRendererTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            lineProperties.Put("x1", "100");
            lineProperties.Put("y1", "800");
            lineProperties.Put("x2", "300");
            lineProperties.Put("y2", "800");
            lineProperties.Put("stroke", "green");
            lineProperties.Put("stroke-width", "25");
            LineSvgNodeRenderer root = new LineSvgNodeRenderer();
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LineWithEmpyAttributesTest() {
            String filename = "lineWithEmpyAttributesTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            LineSvgNodeRenderer root = new LineSvgNodeRenderer();
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidAttributeTest01() {
            bool isThrown = false;
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new LineSvgNodeRenderer();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            lineProperties.Put("x1", "1");
            lineProperties.Put("y1", "800");
            lineProperties.Put("x2", "notAnum");
            lineProperties.Put("y2", "alsoNotANum");
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            try {
                root.Draw(context);
            }
            catch (SvgProcessingException e) {
                isThrown = true;
                NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, e.Message, "Correct exception wasn't thrown");
            }
            finally {
                doc.Close();
            }
            NUnit.Framework.Assert.IsTrue(isThrown, "Exception wasn't thrown");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED)]
        public virtual void InvalidAttributeTest02() {
            bool isThrown = false;
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            lineProperties.Put("x1", "100");
            lineProperties.Put("y1", "800");
            lineProperties.Put("x2", "1 0");
            lineProperties.Put("y2", "0 2 0");
            lineProperties.Put("stroke", "orange");
            String filename = "invalidAttributes02.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            LineSvgNodeRenderer root = new LineSvgNodeRenderer();
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            try {
                root.Draw(context);
            }
            catch (SvgProcessingException e) {
                isThrown = true;
                NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, e.Message, "Correct exception wasn't thrown");
            }
            finally {
                doc.Close();
            }
            doc.Close();
            NUnit.Framework.Assert.IsTrue(isThrown, "Exception wasn't thrown");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EmptyPointsListTest() {
            String filename = "lineEmptyPointsListTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new LineSvgNodeRenderer();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            int numPoints = ((LineSvgNodeRenderer)root).attributesAndStyles.Count;
            NUnit.Framework.Assert.AreEqual(numPoints, 0);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }
        //TODO(RND-823) We'll need an integration test with the entire (not yet created) pipeline as well
    }
}
