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
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA2LayoutOcgTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA2LayoutOcgTest/";

        [NUnit.Framework.SetUp]
        public virtual void Configure() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CheckIfOcgForPdfA2Works() {
            String fileName = "createdOcgPdfA.pdf";
            Stream colorStream = new FileStream(sourceFolder + "color/sRGB_CS_profile.icm", FileMode.Open, FileAccess.Read
                );
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp/PdfA2LayoutOcgTest/cmp_" + fileName;
            PdfDocument pdfDoc = new PdfADocument(new PdfWriter(outFileName), PdfAConformanceLevel.PDF_A_2A, new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", colorStream));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            pdfDoc.AddNewPage();
            iText.Layout.Element.Image image1 = new Image(ImageDataFactory.Create(sourceFolder + "images/manualTransparency_for_png.png"
                ));
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDoc, 1);
            iText.Layout.Canvas canvas1 = new iText.Layout.Canvas(pdfCanvas, new Rectangle(0, 0, 590, 420));
            PdfLayer imageLayer1 = new PdfLayer("*SomeTest_image$here@.1", pdfDoc);
            imageLayer1.SetOn(true);
            pdfCanvas.BeginLayer(imageLayer1);
            canvas1.Add(image1);
            pdfCanvas.EndLayer();
            canvas1.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff01_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outFileName));
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    }
}
