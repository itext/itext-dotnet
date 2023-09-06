/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA4CatalogCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA4CatalogCheckTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA4CatalogCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleDocTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck01.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void InvalidCatalogVersionCheckTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck02.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            doc.GetCatalog().Put(PdfName.Version, new PdfString("1.7"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_CATALOG_VERSION_SHALL_CONTAIN_RIGHT_PDF_VERSION
                , "2"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptInTrailerTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck03.pdf";
            byte[] userPassword = "user".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            byte[] ownerPassword = "owner".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS | EncryptionConstants.ALLOW_DEGRADED_PRINTING;
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (userPassword, ownerPassword, permissions, EncryptionConstants.ENCRYPTION_AES_256).SetFullCompressionMode
                (false));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.KEYWORD_ENCRYPT_SHALL_NOT_BE_USED_IN_THE_TRAILER_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptedDocumentTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck03.pdf";
            byte[] userPassword = "user".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            byte[] ownerPassword = "owner".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS | EncryptionConstants.ALLOW_DEGRADED_PRINTING;
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (userPassword, ownerPassword, permissions, EncryptionConstants.ENCRYPTION_AES_256).SetFullCompressionMode
                (true));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.KEYWORD_ENCRYPT_SHALL_NOT_BE_USED_IN_THE_TRAILER_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AbsentPieceInfoTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck04.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary info = new PdfDictionary();
            String timeValue = "D:19860426012347+04'00'";
            info.Put(PdfName.ModDate, new PdfDate(PdfDate.Decode(timeValue)).GetPdfObject());
            doc.GetTrailer().Put(PdfName.Info, info);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DOCUMENT_SHALL_NOT_CONTAIN_INFO_UNLESS_THERE_IS_PIECE_INFO
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ValidCatalogCheckTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck05.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfA4CatalogCheckTest/cmp_pdfA4_catalogCheck05.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            doc.GetCatalog().Put(PdfName.PieceInfo, new PdfDictionary());
            doc.Close();
            // This is required to check if ModDate is inside Info dictionary
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void InvalidInfoTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck05.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            doc.GetTrailer().Put(PdfName.Info, new PdfDictionary());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DOCUMENT_INFO_DICTIONARY_SHALL_ONLY_CONTAIN_MOD_DATE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ReadValidDocumentTest() {
            String outPdf = destinationFolder + "simplePdfA4_output01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "pdfs/simplePdfA4.pdf");
            PdfDocument document = new PdfADocument(reader, writer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void InvalidVersionInCatalogTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck06.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            doc.GetCatalog().Put(PdfName.Version, new PdfString("1.7"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_CATALOG_VERSION_SHALL_CONTAIN_RIGHT_PDF_VERSION
                , 2), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedVersionInCatalogTest() {
            String outPdf = destinationFolder + "pdfA4_catalogCheck07.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            doc.GetCatalog().Put(PdfName.Version, new PdfString("2ae"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_CATALOG_VERSION_SHALL_CONTAIN_RIGHT_PDF_VERSION
                , 2), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ReadDocumentWithInvalidVersionTest() {
            PdfWriter writer = new PdfWriter(destinationFolder + "simplePdfA4_output02.pdf");
            PdfReader reader = new PdfReader(sourceFolder + "pdfs/pdfA4WithInvalidVersion.pdf");
            PdfDocument document = new PdfADocument(reader, writer);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => document.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_FILE_HEADER_SHALL_CONTAIN_RIGHT_PDF_VERSION
                , 2), e.Message);
        }
    }
}
