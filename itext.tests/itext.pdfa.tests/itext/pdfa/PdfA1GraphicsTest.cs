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
using System.IO;
using iText.Commons.Utils;
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
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetFillColor(ColorConstants
                .RED));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICERGB_AND_DEVICECMYK_COLORSPACES_CANNOT_BE_USED_BOTH_IN_ONE_FILE
                , e.Message);
            canvas.MoveTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.Fill();
            Exception e2 = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT
                , e2.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest2() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest3() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, null);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.SetFillColor(ColorConstants.GREEN);
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.IF_DEVICE_RGB_CMYK_GRAY_USED_IN_FILE_THAT_FILE_SHALL_CONTAIN_PDFA_OUTPUTINTENT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest4() {
            String outPdf = destinationFolder + "pdfA1b_colorCheckTest4.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1b_colorCheckTest4.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
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
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetTransferFunction(new PdfName("Test"))));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_TR_KEY
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EgsCheckTest2() {
            String outPdf = destinationFolder + "pdfA1b_egsCheckTest2.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1b_egsCheckTest2.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
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
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetTransferFunction2(new PdfName("Test"))));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_TR_2_KEY_WITH_A_VALUE_OTHER_THAN_DEFAULT
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EgsCheckTest4() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetRenderingIntent(new PdfName("Test"))));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.IF_SPECIFIED_RENDERING_SHALL_BE_ONE_OF_THE_FOLLOWING_RELATIVECOLORIMETRIC_ABSOLUTECOLORIMETRIC_PERCEPTUAL_OR_SATURATION
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(100, 100));
            PdfCanvas xObjCanvas = new PdfCanvas(xObject, doc);
            xObjCanvas.Rectangle(30, 30, 10, 10).Fill();
            PdfTransparencyGroup group = new PdfTransparencyGroup();
            xObject.SetGroup(group);
            canvas.AddXObjectFittedIntoRectangle(xObject, new Rectangle(300, 300));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_GROUP_OBJECT_WITH_AN_S_KEY_WITH_A_VALUE_OF_TRANSPARENCY_SHALL_NOT_BE_INCLUDED_IN_A_FORM_XOBJECT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest2() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetSoftMask(new PdfName("Test"))));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_SMASK_KEY_IS_NOT_ALLOWED_IN_EXTGSTATE, e.
                    Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest3() {
            String outPdf = destinationFolder + "pdfA1b_transparencyCheckTest3.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1b_transparencyCheckTest3.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, outputIntent);
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
