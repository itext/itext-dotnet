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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA4EmbeddedFilesCheckTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA4EmbeddedFilesCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        // Test with successful creation PDF/A-4F (the same for PDF/A-4E and PDF/A-4) in
        // the embedded files meaning can be found in other tests (e.g. PdfA4CatalogCheckTest).
        [NUnit.Framework.Test]
        public virtual void PdfA4fWithoutEmbeddedFilesTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4fWithoutEmbeddedFilesTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_4F, CreateOutputIntent());
            doc.AddNewPage();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.NAME_DICTIONARY_SHALL_CONTAIN_EMBEDDED_FILES_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fWithEmbeddedFilesWithoutFTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4fWithEmbeddedFilesWithoutFTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_4F, CreateOutputIntent());
            doc.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(doc, "file".GetBytes(), "description", "file.txt", null
                , null, null);
            PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
            fsDict.Remove(PdfName.F);
            doc.AddFileAttachment("file.txt", fs);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fWithEmbeddedFilesWithoutUFTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4fWithEmbeddedFilesWithoutUFTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_4F, CreateOutputIntent());
            doc.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(doc, "file".GetBytes(), "description", "file.txt", null
                , null, null);
            PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
            fsDict.Remove(PdfName.UF);
            doc.AddFileAttachment("file.txt", fs);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fWithEmbeddedFilesWithoutAFRTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4fWithEmbeddedFilesWithoutAFRTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_4F, CreateOutputIntent());
            doc.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(doc, "file".GetBytes(), "description", "file.txt", null
                , null, null);
            PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
            fsDict.Remove(PdfName.AFRelationship);
            doc.AddFileAttachment("file.txt", fs);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_AFRELATIONSHIP_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4eWithEmbeddedFilesWithoutFTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4eWithEmbeddedFilesWithoutFTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_4E, CreateOutputIntent());
            doc.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(doc, "file".GetBytes(), "description", "file.txt", null
                , null, null);
            PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
            fsDict.Remove(PdfName.F);
            doc.AddFileAttachment("file.txt", fs);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4WithEmbeddedFilesWithoutAFRTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4WithEmbeddedFilesWithoutAFRTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_4, CreateOutputIntent());
            doc.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(doc, "file".GetBytes(), "description", "file.txt", null
                , null, null);
            PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
            fsDict.Remove(PdfName.AFRelationship);
            doc.AddFileAttachment("file.txt", fs);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_AFRELATIONSHIP_KEY
                , e.Message);
        }

        private PdfOutputIntent CreateOutputIntent() {
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "sRGB Color Space Profile.icm"));
        }
    }
}
