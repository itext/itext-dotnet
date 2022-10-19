/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    [NUnit.Framework.Category("IntegrationTest")]
    public class RegexBasedLocationExtractionStrategyTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/RegexBasedLocationExtractionStrategyTest/";

        [NUnit.Framework.Test]
        public virtual void Test01() {
            System.Console.Out.WriteLine(new FileInfo(sourceFolder).FullName);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "in01.pdf"));
            // build strategy
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy(iText.Commons.Utils.StringUtil.RegexCompile
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
            NUnit.Framework.Assert.AreEqual(1, locationList.Count);
            IPdfTextLocation loc = locationList[0];
            NUnit.Framework.Assert.AreEqual("{{Signature}}", loc.GetText());
            NUnit.Framework.Assert.AreEqual(23, (int)loc.GetRectangle().GetX());
            NUnit.Framework.Assert.AreEqual(375, (int)loc.GetRectangle().GetY());
            NUnit.Framework.Assert.AreEqual(55, (int)loc.GetRectangle().GetWidth());
            NUnit.Framework.Assert.AreEqual(11, (int)loc.GetRectangle().GetHeight());
            // close
            pdfDocument.Close();
        }

        // https://jira.itextsupport.com/browse/DEVSIX-1940
        // text is 'calligraphy' and 'll' is composing a ligature
        [NUnit.Framework.Test]
        public virtual void TestLigatureBeforeLigature() {
            System.Console.Out.WriteLine(new FileInfo(sourceFolder).FullName);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "ligature.pdf"));
            // build strategy
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("ca");
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
            NUnit.Framework.Assert.AreEqual(1, locationList.Count);
            IPdfTextLocation loc = locationList[0];
            NUnit.Framework.Assert.AreEqual("ca", loc.GetText());
            Rectangle rect = loc.GetRectangle();
            NUnit.Framework.Assert.AreEqual(36, rect.GetX(), 0.0001);
            NUnit.Framework.Assert.AreEqual(655.4600, rect.GetY(), 0.0001);
            NUnit.Framework.Assert.AreEqual(25.1000, rect.GetWidth(), 0.0001);
            NUnit.Framework.Assert.AreEqual(20, rect.GetHeight(), 0.0001);
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestLigatureCrossLigature() {
            System.Console.Out.WriteLine(new FileInfo(sourceFolder).FullName);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "ligature.pdf"));
            // build strategy
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("al");
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
            NUnit.Framework.Assert.AreEqual(1, locationList.Count);
            IPdfTextLocation loc = locationList[0];
            NUnit.Framework.Assert.AreEqual("al", loc.GetText());
            Rectangle rect = loc.GetRectangle();
            NUnit.Framework.Assert.AreEqual(48.7600, rect.GetX(), 0.0001);
            NUnit.Framework.Assert.AreEqual(655.4600, rect.GetY(), 0.0001);
            NUnit.Framework.Assert.AreEqual(25.9799, rect.GetWidth(), 0.0001);
            NUnit.Framework.Assert.AreEqual(20, rect.GetHeight(), 0.0001);
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestLigatureInLigature() {
            System.Console.Out.WriteLine(new FileInfo(sourceFolder).FullName);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "ligature.pdf"));
            // build strategy
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("l");
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
            NUnit.Framework.Assert.AreEqual(1, locationList.Count);
            IPdfTextLocation loc = locationList[0];
            NUnit.Framework.Assert.AreEqual("l", loc.GetText());
            Rectangle rect = loc.GetRectangle();
            NUnit.Framework.Assert.AreEqual(61.0999, rect.GetX(), 0.0001);
            NUnit.Framework.Assert.AreEqual(655.4600, rect.GetY(), 0.0001);
            NUnit.Framework.Assert.AreEqual(13.6399, rect.GetWidth(), 0.0001);
            NUnit.Framework.Assert.AreEqual(20, rect.GetHeight(), 0.0001);
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestRotatedText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "rotatedText.pdf"));
            // build strategy
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("abc");
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
            NUnit.Framework.Assert.AreEqual(2, locationList.Count);
            NUnit.Framework.Assert.IsTrue(locationList[0].GetRectangle().EqualsWithEpsilon(new Rectangle(188.512f, 450f
                , 14.800003f, 25.791992f)));
            NUnit.Framework.Assert.IsTrue(locationList[1].GetRectangle().EqualsWithEpsilon(new Rectangle(36f, 746.688f
                , 25.792f, 14.799988f)));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void RegexStartedWithWhiteSpaceTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "regexStartedWithWhiteSpaceTest.pdf"
                ));
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("\\sstart"
                );
            new PdfCanvasProcessor(extractionStrategy).ProcessPageContent(pdfDocument.GetPage(1));
            IList<IPdfTextLocation> locations = new List<IPdfTextLocation>(extractionStrategy.GetResultantLocations());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, locations.Count);
            NUnit.Framework.Assert.AreEqual(" start", locations[0].GetText());
            NUnit.Framework.Assert.IsTrue(new Rectangle(92.3f, 743.3970f, 20.6159f, 13.2839f).EqualsWithEpsilon(locations
                [0].GetRectangle()));
        }

        [NUnit.Framework.Test]
        public virtual void RegexStartedWithNewLineTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "regexStartedWithNewLineTest.pdf"));
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("\\nstart"
                );
            new PdfCanvasProcessor(extractionStrategy).ProcessPageContent(pdfDocument.GetPage(1));
            IList<IPdfTextLocation> locations = new List<IPdfTextLocation>(extractionStrategy.GetResultantLocations());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, locations.Count);
            NUnit.Framework.Assert.AreEqual("\nstart", locations[0].GetText());
            NUnit.Framework.Assert.IsTrue(new Rectangle(56.8f, 729.5970f, 20.6159f, 13.2839f).EqualsWithEpsilon(locations
                [0].GetRectangle()));
        }

        [NUnit.Framework.Test]
        public virtual void RegexWithWhiteSpacesTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "regexWithWhiteSpacesTest.pdf"));
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("\\sstart\\s"
                );
            new PdfCanvasProcessor(extractionStrategy).ProcessPageContent(pdfDocument.GetPage(1));
            IList<IPdfTextLocation> locations = new List<IPdfTextLocation>(extractionStrategy.GetResultantLocations());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, locations.Count);
            NUnit.Framework.Assert.AreEqual(" start ", locations[0].GetText());
            NUnit.Framework.Assert.IsTrue(new Rectangle(92.3f, 743.3970f, 20.6159f, 13.2839f).EqualsWithEpsilon(locations
                [0].GetRectangle()));
        }

        [NUnit.Framework.Test]
        public virtual void RegexWithNewLinesTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "regexWithNewLinesTest.pdf"));
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("\\nstart\\n"
                );
            new PdfCanvasProcessor(extractionStrategy).ProcessPageContent(pdfDocument.GetPage(1));
            IList<IPdfTextLocation> locations = new List<IPdfTextLocation>(extractionStrategy.GetResultantLocations());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, locations.Count);
            NUnit.Framework.Assert.AreEqual("\nstart\n", locations[0].GetText());
            NUnit.Framework.Assert.IsTrue(new Rectangle(56.8f, 729.5970f, 20.6159f, 13.2839f).EqualsWithEpsilon(locations
                [0].GetRectangle()));
        }

        [NUnit.Framework.Test]
        public virtual void RegexWithNewLineBetweenWordsTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "regexWithNewLineBetweenWordsTest.pdf"
                ));
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("hello\\nworld"
                );
            new PdfCanvasProcessor(extractionStrategy).ProcessPageContent(pdfDocument.GetPage(1));
            IList<IPdfTextLocation> locations = new List<IPdfTextLocation>(extractionStrategy.GetResultantLocations());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(2, locations.Count);
            NUnit.Framework.Assert.AreEqual("hello\nworld", locations[0].GetText());
            NUnit.Framework.Assert.AreEqual("hello\nworld", locations[1].GetText());
            NUnit.Framework.Assert.IsTrue(new Rectangle(56.8f, 729.5970f, 27.8999f, 13.2839f).EqualsWithEpsilon(locations
                [0].GetRectangle()));
            NUnit.Framework.Assert.IsTrue(new Rectangle(56.8f, 743.3970f, 23.9039f, 13.2839f).EqualsWithEpsilon(locations
                [1].GetRectangle()));
        }

        [NUnit.Framework.Test]
        public virtual void RegexWithOnlyNewLine() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "regexWithNewLinesTest.pdf"));
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("\\n");
            new PdfCanvasProcessor(extractionStrategy).ProcessPageContent(pdfDocument.GetPage(1));
            IList<IPdfTextLocation> locations = new List<IPdfTextLocation>(extractionStrategy.GetResultantLocations());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(0, locations.Count);
        }

        [NUnit.Framework.Test]
        public virtual void RegexWithOnlyWhiteSpace() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "regexWithWhiteSpacesTest.pdf"));
            RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy(" ");
            new PdfCanvasProcessor(extractionStrategy).ProcessPageContent(pdfDocument.GetPage(1));
            IList<IPdfTextLocation> locations = new List<IPdfTextLocation>(extractionStrategy.GetResultantLocations());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(0, locations.Count);
        }

        [NUnit.Framework.Test]
        public virtual void SortCompareTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "sortCompare.pdf"))) {
                RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("a");
                PdfCanvasProcessor pdfCanvasProcessor = new PdfCanvasProcessor(extractionStrategy);
                pdfCanvasProcessor.ProcessPageContent(pdfDocument.GetPage(1));
                pdfCanvasProcessor.ProcessPageContent(pdfDocument.GetPage(2));
                IList<IPdfTextLocation> locations = new List<IPdfTextLocation>(extractionStrategy.GetResultantLocations());
                NUnit.Framework.Assert.AreEqual(13, locations.Count);
            }
        }
    }
}
