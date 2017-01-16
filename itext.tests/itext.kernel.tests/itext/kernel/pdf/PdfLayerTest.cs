using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfLayerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfLayerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfLayerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestInStamperMode1() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "input_layered.pdf"), new PdfWriter(destinationFolder
                 + "output_copy_layered.pdf"));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "output_copy_layered.pdf"
                , sourceFolder + "input_layered.pdf", destinationFolder, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestInStamperMode2() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "input_layered.pdf"), new PdfWriter(destinationFolder
                 + "output_layered.pdf"));
            PdfCanvas canvas = new PdfCanvas(pdfDoc, 1);
            PdfLayer newLayer = new PdfLayer("appended", pdfDoc);
            canvas.BeginLayer(newLayer).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA), 
                18).MoveText(200, 600).ShowText("APPENDED CONTENT").EndText().EndLayer();
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            foreach (PdfLayer layer in allLayers) {
                if (layer.IsLocked()) {
                    layer.SetLocked(false);
                }
                if ("Grouped layers".Equals(layer.GetTitle())) {
                    layer.AddChild(newLayer);
                }
            }
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "output_layered.pdf", 
                sourceFolder + "cmp_output_layered.pdf", destinationFolder, "diff"));
        }
    }
}
