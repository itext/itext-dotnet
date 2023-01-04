/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ParagraphTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ParagraphTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ParagraphTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CannotPlaceABigChunkOnALineTest01() {
            String outFileName = destinationFolder + "cannotPlaceABigChunkOnALineTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_cannotPlaceABigChunkOnALineTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0));
            p.Add(new Text("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa").SetBorder
                (new SolidBorder(ColorConstants.RED, 0)));
            p.Add(new Text("b").SetFontSize(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CannotPlaceABigChunkOnALineTest02() {
            String outFileName = destinationFolder + "cannotPlaceABigChunkOnALineTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_cannotPlaceABigChunkOnALineTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0));
            p.Add(new Text("smaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaall").SetFontSize(5).SetBorder
                (new SolidBorder(ColorConstants.RED, 0)));
            p.Add(new Text("biiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiig"
                ).SetFontSize(20).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ForceOverflowForTextRendererPartialResult01() {
            String outFileName = destinationFolder + "forceOverflowForTextRendererPartialResult01.pdf";
            String cmpFileName = sourceFolder + "cmp_forceOverflowForTextRendererPartialResult01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0)).SetTextAlignment(TextAlignment
                .RIGHT);
            for (int i = 0; i < 5; i++) {
                p.Add(new Text("aaaaaaaaaaaaaaaaaaaaa" + i).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            }
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES, LogLevel = LogLevelConstants
            .INFO)]
        public virtual void WordWasSplitAndItWillFitOntoNextLineTest02() {
            // TODO DEVSIX-4622
            String outFileName = destinationFolder + "wordWasSplitAndItWillFitOntoNextLineTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_wordWasSplitAndItWillFitOntoNextLineTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().Add(new Text("Short").SetBackgroundColor(ColorConstants.YELLOW)).Add
                (new Text(" Loooooooooooooooooooong").SetBackgroundColor(ColorConstants.RED)).SetWidth(90).SetBorder(new 
                SolidBorder(1));
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
