using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

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
        public virtual void LnvalidAttributeTest01() {
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
                NUnit.Framework.Assert.AreEqual(expectedExceptionMessage.Trim(), e.Message, "Exception wasn't thrown");
            }
            finally {
                doc.Close();
            }
            NUnit.Framework.Assert.IsTrue(isThrown, "Exception wasn't thrown");
        }

        [NUnit.Framework.Test]
        public virtual void LnvalidAttributeTest02() {
            bool isThrown = false;
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new LineSvgNodeRenderer();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            lineProperties.Put("x1", "100");
            lineProperties.Put("y1", "800");
            lineProperties.Put("x2", "1 0");
            lineProperties.Put("y2", "0 2 0");
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            try {
                root.Draw(context);
            }
            catch (SvgProcessingException e) {
                isThrown = true;
                NUnit.Framework.Assert.AreEqual(expectedExceptionMessage.Trim(), e.Message, "Exception wasn't thrown");
            }
            finally {
                doc.Close();
            }
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
        //TODO We'll need an integration test with the entire (not yet created) pipeline as well
    }
}
