using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    public class RegexBasedLocationExtractionStrategyTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/RegexBasedLocationExtractionStrategyTest/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Test01() {
            System.Console.Out.WriteLine(new FileInfo(sourceFolder).FullName);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "in01.pdf"));
            // build strategy
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy(iText.IO.Util.StringUtil.RegexCompile
                ("\\{\\{Signature\\}\\}"));
            // get locations
            IList<IPdfTextLocation> locationList = new List<IPdfTextLocation>();
            for (int x = 1; x <= pdfDocument.GetNumberOfPages(); x++) {
                new PdfCanvasProcessor(extractionStrategy).ProcessPageContent(pdfDocument.GetPage(x));
                foreach (IPdfTextLocation location in extractionStrategy.GetResultantLocations()) {
                    if (location != null) {
                        locationList.Add(location);
                    }
                }
            }
            // compare
            NUnit.Framework.Assert.AreEqual(locationList.Count, 1);
            IPdfTextLocation loc = locationList[0];
            NUnit.Framework.Assert.AreEqual(loc.GetText(), "{{Signature}}");
            NUnit.Framework.Assert.AreEqual(23, (int)loc.GetRectangle().GetX());
            NUnit.Framework.Assert.AreEqual(375, (int)loc.GetRectangle().GetY());
            NUnit.Framework.Assert.AreEqual(52, (int)loc.GetRectangle().GetWidth());
            NUnit.Framework.Assert.AreEqual(11, (int)loc.GetRectangle().GetHeight());
            // close
            pdfDocument.Close();
        }
    }
}
