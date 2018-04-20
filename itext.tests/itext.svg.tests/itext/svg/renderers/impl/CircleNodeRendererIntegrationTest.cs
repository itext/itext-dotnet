/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel.Utils;
using iText.Svg.Converter;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class CircleNodeRendererIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/CircleSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/CircleSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void BasicCircleTest() {
            String filename = "basicCircleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle cx='100' cy='100' r='80' stroke='red' fill ='green'/>\n" + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleCxCyAbsentTest() {
            String filename = "circleCxCyAbsentTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle r='80' stroke='green' fill ='cyan'/>\n" + "\t<circle r='80' transform='translate(100,100)' stroke='red' fill ='green'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleCxAbsentTest() {
            String filename = "circleCxAbsentTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle cy='100' r='80' stroke='green' fill ='cyan'/>\n" + "\t<circle cy='100' r='80' transform='translate(100,100)' stroke='red' fill ='green'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleCxNegativeTest() {
            String filename = "circleCxNegativeTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle cx='-100' cy='100' r='80' stroke='green' fill ='cyan'/>\n" + "\t<circle cx='-100' cy='100' r='80' transform='translate(100,100)' stroke='red' fill ='green'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleCyAbsentTest() {
            String filename = "circleCyAbsentTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle cx='100' r='80' stroke='green' fill ='cyan'/>\n" + "\t<circle cx='100' r='80' transform='translate(100,100)' stroke='red' fill ='green'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleCyNegativeTest() {
            String filename = "circleCyNegativeTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle cx='100' cy='-100' r='80' stroke='green' fill ='cyan'/>\n" + "\t<circle cx='100' cy='-100' r='80' transform='translate(100,100)' stroke='red' fill ='green'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleRAbsentTest() {
            String filename = "circleRAbsentTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle cx='100' cy='100' stroke='green' fill ='cyan'/>\n" + "\t<circle cx='100' cy='100' transform='translate(100,100)' stroke='red' fill ='green'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleRNegativeTest() {
            String filename = "circleRNegativeTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle cx='100' cy='100' r='-80' stroke='green' fill ='cyan'/>\n" + "\t<circle cx='100' cy='100' r='-80' transform='translate(100,100)' stroke='red' fill ='green'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleTranslatedTest() {
            String filename = "circleTranslatedTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle r='80' stroke='green' fill ='cyan'/>\n" + "\t<circle r='80' transform='translate(100,100)' stroke='red' fill ='green'/>\n"
                 + "\t<line x1='0' y1='0' x2='100' y2='0' transform='translate(100,100)' stroke='black' />\n" + "\t<line x1='0' y1='0' x2='0' y2='100' transform='translate(100,100)' stroke='black'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleRotatedTest() {
            String filename = "circleRotatedTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle r='80' stroke='green' fill ='cyan'/>\n" + "\t<circle r='80' transform='translate(100,100) rotate(25)' stroke='red' fill ='green'/>\n"
                 + "\t<line x1='0' y1='0' x2='100' y2='0' transform='translate(100,100) rotate(25)' stroke='black' />\n"
                 + "\t<line x1='0' y1='0' x2='0' y2='100' transform='translate(100,100) rotate(25)' stroke='black'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleScaledUpTest() {
            String filename = "circleScaledUpTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "    <circle r='80' transform='translate(100,100) scale(2)' stroke='red' fill ='blue'/>\n" + "    <circle r='80' transform='translate(100,100)' stroke='green' fill ='cyan'/>\n"
                 + "\t<line x1='100' y1='100' x2='200' y2='100' stroke='black'/>\n" + "\t<line x1='100' y1='100' x2='100' y2='200' stroke='black'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleScaledDownTest() {
            String filename = "circleScaledDownTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "    <circle r='80' transform='translate(100,100)' stroke='red' fill ='blue'/>\n" + "    <circle r='80' transform='translate(100,100) scale(0.5)' stroke='green' fill ='cyan'/>\n"
                 + "\t<line x1='100' y1='100' x2='200' y2='100' stroke='black'/>\n" + "\t<line x1='100' y1='100' x2='100' y2='200' stroke='black'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleScaledXYTest() {
            String filename = "circleScaledXYTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "    <circle r='80' transform='translate(100,100)' stroke='red' fill ='blue'/>\n" + "    <circle r='80' transform='translate(100,100) scale(0.5,1.1)' stroke='green' fill ='cyan'/>\n"
                 + "\t<line x1='100' y1='100' x2='200' y2='100' stroke='black'/>\n" + "\t<line x1='100' y1='100' x2='100' y2='200' stroke='black'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleSkewXTest() {
            String filename = "circleSkewXTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle r='80' transform='translate(100,100)' stroke='green' fill ='cyan'/>\n" + "\t<circle r='80' transform='translate(100,100) skewX(40)' stroke='red' fill ='green'/>\n"
                 + "\t<line x1='0' y1='0' x2='100' y2='0' transform='translate(100,100) skewX(40)' stroke='black' />\n"
                 + "\t<line x1='0' y1='0' x2='0' y2='100' transform='translate(100,100) skewX(40)' stroke='black'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void CircleSkewYTest() {
            String filename = "circleSkewYTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'\n" + "     xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "\t<circle r='80' transform='translate(100,100)' stroke='green' fill ='cyan'/>\n" + "\t<circle r='80' transform='translate(100,100) skewY(40)' stroke='red' fill ='green'/>\n"
                 + "\t<line x1='0' y1='0' x2='100' y2='0' transform='translate(100,100) skewY(40)' stroke='black' />\n"
                 + "\t<line x1='0' y1='0' x2='0' y2='100' transform='translate(100,100) skewY(40)' stroke='black' />\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }
    }
}
