/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class EllipseSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/EllipseSvgNodeRendererIntegrationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/EllipseSvgNodeRendererIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipseTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicEllipse");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCxCyAbsentTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseCxCyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCxAbsentTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseCxAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCxNegativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseCxNegative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCyNegativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseCyNegative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCyAbsentTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseCyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRxAbsentTest() {
            //TODO: update cmp_ when DEVSIX-3119
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseRxAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRyAbsentTest() {
            //TODO: update cmp_ when DEVSIX-3119
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseRyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRxNegativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseRxNegative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRyNegativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseRyNegative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseTranslatedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseTranslated");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRotatedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseRotated");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseScaledUpTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseScaledUp");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseScaledDownTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseScaledDown");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseScaledXYTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseScaledXY");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseSkewXTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseSkewX");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseSkewYTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseSkewY");
        }

        [NUnit.Framework.Test]
        public virtual void ParseParametersAndCalculateCoordinatesWithBetterPrecisionEllipseTest() {
            String filename = "parseParametersAndCalculateCoordinatesWithBetterPrecisionEllipseTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            EllipseSvgNodeRenderer ellipseRenderer = new EllipseSvgNodeRenderer();
            ellipseRenderer.SetAttribute(SvgConstants.Attributes.CX, "170.3");
            ellipseRenderer.SetAttribute(SvgConstants.Attributes.CY, "339.5");
            ellipseRenderer.SetAttribute(SvgConstants.Attributes.RX, "6");
            ellipseRenderer.SetAttribute(SvgConstants.Attributes.RY, "6");
            // Parse parameters with better precision (in double type) in the method CssUtils#parseAbsoluteLength
            ellipseRenderer.SetParameters();
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            // Calculate coordinates with better precision (in double type) in the method EllipseSvgNodeRenderer#doDraw
            ellipseRenderer.Draw(context);
            String pageContentBytes = iText.IO.Util.JavaUtil.GetStringForBytes(doc.GetPage(1).GetContentBytes(), System.Text.Encoding
                .UTF8);
            doc.Close();
            String expectedResult = "132.22 254.63 m\n" + "132.22 257.11 130.21 259.13 127.72 259.13 c\n" + "125.24 259.13 123.22 257.11 123.22 254.63 c\n"
                 + "123.22 252.14 125.24 250.13 127.72 250.13 c\n" + "130.21 250.13 132.22 252.14 132.22 254.63 c";
            NUnit.Framework.Assert.IsTrue(pageContentBytes.Contains(expectedResult));
        }

        [NUnit.Framework.Test]
        public virtual void EllipseWithBigStrokeWidthTest() {
            // TODO: DEVSIX-3932 update cmp_ after fix
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipseWithBigStrokeWidth");
        }
    }
}
