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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA1GraphicsTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA1GraphicsTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA1GraphicsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetFillColor(ColorConstants
                .RED));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.DEVICERGB_AND_DEVICECMYK_COLORSPACES_CANNOT_BE_USED_BOTH_IN_ONE_FILE
                , e.Message);
            canvas.MoveTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.Fill();
            Exception e2 = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.DEVICERGB_AND_DEVICECMYK_COLORSPACES_CANNOT_BE_USED_BOTH_IN_ONE_FILE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest2() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest3() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, null);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetFillColor(ColorConstants.GREEN);
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.IF_DEVICE_RGB_CMYK_GRAY_USED_IN_FILE_THAT_FILE_SHALL_CONTAIN_PDFA_OUTPUTINTENT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest4() {
            String outPdf = destinationFolder + "pdfA1b_colorCheckTest4.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1b_colorCheckTest4.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetFillColor(ColorConstants.GREEN);
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void EgsCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetTransferFunction(new PdfName("Test"))));
                NUnit.Framework.Assert.AreEqual(PdfAConformanceException.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_TR_KEY
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EgsCheckTest2() {
            String outPdf = destinationFolder + "pdfA1b_egsCheckTest2.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1b_egsCheckTest2.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetExtGState(new PdfExtGState().SetTransferFunction2(PdfName.Default));
            canvas.Rectangle(30, 30, 100, 100).Fill();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void EgsCheckTest3() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetTransferFunction2(new PdfName("Test"))));
                NUnit.Framework.Assert.AreEqual(PdfAConformanceException.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_TR_2_KEY_WITH_A_VALUE_OTHER_THAN_DEFAULT
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EgsCheckTest4() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetRenderingIntent(new PdfName("Test"))));
                NUnit.Framework.Assert.AreEqual(PdfAConformanceException.IF_SPECIFIED_RENDERING_SHALL_BE_ONE_OF_THE_FOLLOWING_RELATIVECOLORIMETRIC_ABSOLUTECOLORIMETRIC_PERCEPTUAL_OR_SATURATION
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(100, 100));
            PdfCanvas xObjCanvas = new PdfCanvas(xObject, doc);
            xObjCanvas.Rectangle(30, 30, 10, 10).Fill();
            PdfTransparencyGroup group = new PdfTransparencyGroup();
            xObject.SetGroup(group);
            canvas.AddXObjectFittedIntoRectangle(xObject, new Rectangle(300, 300));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.A_GROUP_OBJECT_WITH_AN_S_KEY_WITH_A_VALUE_OF_TRANSPARENCY_SHALL_NOT_BE_INCLUDED_IN_A_FORM_XOBJECT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest2() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetSoftMask(new PdfName("Test"))));
                NUnit.Framework.Assert.AreEqual(PdfAConformanceException.THE_SMASK_KEY_IS_NOT_ALLOWED_IN_EXTGSTATE, e.Message
                    );
            }
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest3() {
            String outPdf = destinationFolder + "pdfA1b_transparencyCheckTest3.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1b_transparencyCheckTest3.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetExtGState(new PdfExtGState().SetSoftMask(PdfName.None));
            canvas.Rectangle(30, 30, 100, 100).Fill();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
