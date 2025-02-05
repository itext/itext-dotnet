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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    /// <summary>This class tests the LocationExtractionStrategy framework.</summary>
    /// <remarks>
    /// This class tests the LocationExtractionStrategy framework.
    /// It uses RegexBasedLocationExtractionStrategy, and searches for the word "Alice" in the book
    /// "Alice in Wonderland" by Lewis Caroll on page 1.
    /// </remarks>
    [NUnit.Framework.Category("IntegrationTest")]
    public class LocationExtractTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/LocationExtractionTest/";

        [NUnit.Framework.Test]
        public virtual void TestLocationExtraction() {
            String inputFile = sourceFolder + "aliceInWonderland.pdf";
            PdfReader reader = new PdfReader(inputFile);
            PdfDocument pdfDocument = new PdfDocument(reader);
            // calculate marked areas
            PdfPage page = pdfDocument.GetPage(1);
            ICollection<Rectangle> rectangleCollection = ProcessPage(new RegexBasedLocationExtractionStrategy("Alice")
                , page);
            // close document
            pdfDocument.Close();
            // compare rectangles
            ICollection<Rectangle> expectedRectangles = new HashSet<Rectangle>();
            expectedRectangles.Add(new Rectangle(174.67166f, 150.19658f, 29.191528f, 14.982529f));
            expectedRectangles.Add(new Rectangle(200.95114f, 326.95657f, 29.297531f, 14.982544f));
            expectedRectangles.Add(new Rectangle(250.17247f, 376.51657f, 29.191544f, 14.982544f));
            expectedRectangles.Add(new Rectangle(434.33588f, 457.1566f, 29.191467f, 14.982544f));
            expectedRectangles.Add(new Rectangle(374.3493f, 519.1966f, 29.191528f, 14.982483f));
            expectedRectangles.Add(new Rectangle(510.3833f, 618.4366f, 29.380737f, 14.982483f));
            expectedRectangles.Add(new Rectangle(84.0f, 649.3966f, 29.297523f, 14.982483f));
            NUnit.Framework.Assert.IsTrue(expectedRectangles.Count == rectangleCollection.Count);
            NUnit.Framework.Assert.IsTrue(FuzzyContainsAll(rectangleCollection, expectedRectangles));
        }

        private ICollection<Rectangle> ProcessPage(ILocationExtractionStrategy strategy, PdfPage page) {
            PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
            parser.ProcessPageContent(page);
            IList<Rectangle> retval = new List<Rectangle>();
            foreach (IPdfTextLocation l in strategy.GetResultantLocations()) {
                retval.Add(l.GetRectangle());
            }
            return retval;
        }

        /// <summary>Comparing floats does not usually yield proper results for equality.</summary>
        /// <remarks>
        /// Comparing floats does not usually yield proper results for equality.
        /// This function exists specifically to overcome that obstacle.
        /// </remarks>
        /// <param name="rs"/>
        /// <param name="r"/>
        /// <returns/>
        private bool FuzzyContains(ICollection<Rectangle> rs, Rectangle r) {
            int x = (int)r.GetX();
            int y = (int)r.GetY();
            int w = (int)r.GetWidth();
            int h = (int)r.GetHeight();
            foreach (Rectangle r0 in rs) {
                int x0 = (int)r0.GetX();
                int y0 = (int)r0.GetY();
                int w0 = (int)r0.GetWidth();
                int h0 = (int)r0.GetHeight();
                if (x0 == x && y0 == y && w0 == w && h0 == h) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>This function tests whether a first collection contains all elements of a second collection.</summary>
        /// <remarks>
        /// This function tests whether a first collection contains all elements of a second collection.
        /// This method does not perform its job fast, but is only used for testing.
        /// </remarks>
        /// <param name="rs0"/>
        /// <param name="rs1"/>
        /// <returns>true iff rs0 contains all elements of rs1</returns>
        private bool FuzzyContainsAll(ICollection<Rectangle> rs0, ICollection<Rectangle> rs1) {
            foreach (Rectangle r1 in rs1) {
                if (!FuzzyContains(rs0, r1)) {
                    return false;
                }
            }
            return true;
        }
    }
}
