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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfStreamTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfStreamTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfStreamTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void StreamAppendDataOnJustCopiedWithCompression() {
            String srcFile = sourceFolder + "pageWithContent.pdf";
            String cmpFile = sourceFolder + "cmp_streamAppendDataOnJustCopiedWithCompression.pdf";
            String destFile = destinationFolder + "streamAppendDataOnJustCopiedWithCompression.pdf";
            PdfDocument srcDocument = new PdfDocument(new PdfReader(srcFile));
            PdfDocument document = new PdfDocument(new PdfWriter(destFile));
            srcDocument.CopyPagesTo(1, 1, document);
            srcDocument.Close();
            String newContentString = "BT\n" + "/F1 36 Tf\n" + "50 700 Td\n" + "(new content here!) Tj\n" + "ET";
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            document.GetPage(1).GetLastContentStream().SetData(newContent, true);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RunLengthEncodingTest01() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            String srcFile = sourceFolder + "runLengthEncodedImages.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile));
            PdfImageXObject im1 = document.GetPage(1).GetResources().GetImage(new PdfName("Im1"));
            PdfImageXObject im2 = document.GetPage(1).GetResources().GetImage(new PdfName("Im2"));
            byte[] imgBytes1 = im1.GetImageBytes();
            byte[] imgBytes2 = im2.GetImageBytes();
            document.Close();
            byte[] cmpImgBytes1 = ReadFile(sourceFolder + "cmp_img1.jpg");
            byte[] cmpImgBytes2 = ReadFile(sourceFolder + "cmp_img2.jpg");
            NUnit.Framework.Assert.AreEqual(imgBytes1, cmpImgBytes1);
            NUnit.Framework.Assert.AreEqual(imgBytes2, cmpImgBytes2);
        }

        [NUnit.Framework.Test]
        public virtual void IndirectRefInFilterAndNoTaggedPdfTest() {
            String inFile = sourceFolder + "indirectRefInFilterAndNoTaggedPdf.pdf";
            String outFile = destinationFolder + "destIndirectRefInFilterAndNoTaggedPdf.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(inFile));
            PdfDocument outDoc = new PdfDocument(new PdfReader(inFile), new PdfWriter(outFile));
            outDoc.Close();
            PdfDocument doc = new PdfDocument(new PdfReader(outFile));
            PdfStream outStreamIm1 = doc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            PdfStream outStreamIm2 = doc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im2"));
            PdfStream cmpStreamIm1 = srcDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new 
                PdfName("Im1"));
            PdfStream cmpStreamIm2 = srcDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new 
                PdfName("Im2"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStreamIm1, cmpStreamIm1));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStreamIm2, cmpStreamIm2));
            srcDoc.Close();
            outDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CryptFilterFlushedBeforeReadStreamTest() {
            String file = sourceFolder + "cryptFilterTest.pdf";
            String destFile = destinationFolder + "cryptFilterReadStreamTest.pdf";
            PdfReader reader = new PdfReader(file, new ReaderProperties().SetPassword("World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                )));
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption("World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ), "Hello".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1), permissions, encryptionType);
            PdfWriter writer = new PdfWriter(destFile, writerProperties.AddXmpMetadata());
            PdfDocument doc = new PdfDocument(reader, writer);
            ((PdfStream)doc.GetPdfObject(5)).GetBytes();
            //Simulating that this flush happened automatically before normal stream flushing in close method
            ((PdfStream)doc.GetPdfObject(5)).Get(PdfName.Filter).Flush();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.FLUSHED_STREAM_FILTER_EXCEPTION
                , "5", "0"), exception.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CryptFilterFlushedBeforeStreamTest() {
            String file = sourceFolder + "cryptFilterTest.pdf";
            String destFile = destinationFolder + "cryptFilterStreamNotReadTest.pdf";
            PdfReader reader = new PdfReader(file, new ReaderProperties().SetPassword("World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                )));
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption("World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ), "Hello".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1), permissions, encryptionType);
            PdfWriter writer = new PdfWriter(destFile, writerProperties.AddXmpMetadata());
            PdfDocument doc = new PdfDocument(reader, writer);
            //Simulating that this flush happened automatically before normal stream flushing in close method
            ((PdfStream)doc.GetPdfObject(5)).Get(PdfName.Filter).Flush();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.FLUSHED_STREAM_FILTER_EXCEPTION
                , "5", "0"), exception.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CryptFilterFlushedAfterStreamTest() {
            String file = sourceFolder + "cryptFilterTest.pdf";
            String cmpFile = sourceFolder + "cmp_cryptFilterTest.pdf";
            String destFile = destinationFolder + "cryptFilterTest.pdf";
            byte[] user = "Hello".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            byte[] owner = "World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            PdfReader reader = new PdfReader(file, new ReaderProperties().SetPassword(owner));
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption(user, owner, permissions, 
                encryptionType);
            PdfWriter writer = new PdfWriter(destFile, writerProperties.AddXmpMetadata());
            writer.SetCompressionLevel(-1);
            PdfDocument doc = new PdfDocument(reader, writer);
            PdfObject cryptFilter = ((PdfStream)doc.GetPdfObject(5)).Get(PdfName.Filter);
            doc.GetPdfObject(5).Flush();
            //Simulating that this flush happened automatically before normal stream flushing in close method
            cryptFilter.Flush();
            doc.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(destFile, cmpFile, destinationFolder, "diff_", user, user
                );
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, Count = 2, LogLevel = LogLevelConstants
            .INFO)]
        [NUnit.Framework.Test]
        public virtual void IndirectFilterInCatalogTest() {
            String file = sourceFolder + "indFilterInCatalog.pdf";
            String cmpFile = sourceFolder + "cmp_indFilterInCatalog.pdf";
            String destFile = destinationFolder + "indFilterInCatalog.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, Count = 2, LogLevel = LogLevelConstants
            .INFO)]
        [NUnit.Framework.Test]
        public virtual void UserDefinedCompressionWithIndirectFilterInCatalogTest() {
            String file = sourceFolder + "indFilterInCatalog.pdf";
            String cmpFile = sourceFolder + "cmp_indFilterInCatalog.pdf";
            String destFile = destinationFolder + "indFilterInCatalog.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            PdfStream stream = (PdfStream)doc.GetPdfObject(5);
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, Count = 2, LogLevel = LogLevelConstants
            .INFO)]
        [NUnit.Framework.Test]
        public virtual void IndirectFilterFlushedBeforeStreamTest() {
            String file = sourceFolder + "indFilterInCatalog.pdf";
            String cmpFile = sourceFolder + "cmp_indFilterInCatalog.pdf";
            String destFile = destinationFolder + "indFilterInCatalog.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            // Simulate the case in which filter is somehow already flushed before stream.
            // Either directly by user or because of any other reason.
            PdfObject filterObject = pdfDoc.GetPdfObject(6);
            //Simulating that this flush happened automatically before normal stream flushing in close method
            filterObject.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, Count = 2, LogLevel = LogLevelConstants
            .INFO)]
        [NUnit.Framework.Test]
        public virtual void IndirectFilterMarkedToBeFlushedBeforeStreamTest() {
            String file = sourceFolder + "indFilterInCatalog.pdf";
            String cmpFile = sourceFolder + "cmp_indFilterInCatalog.pdf";
            String destFile = destinationFolder + "indFilterInCatalog.pdf";
            PdfWriter writer = new PdfWriter(destFile);
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(file), writer);
            // Simulate the case when indirect filter object is marked to be flushed before the stream itself.
            PdfObject filterObject = pdfDoc.GetPdfObject(6);
            filterObject.GetIndirectReference().SetState(PdfObject.MUST_BE_FLUSHED);
            // The image stream will be marked as MUST_BE_FLUSHED after page is flushed.
            pdfDoc.GetFirstPage().GetPdfObject().GetIndirectReference().SetState(PdfObject.MUST_BE_FLUSHED);
            // There was a NPE because FlateFilter was already flushed.
            writer.FlushWaitingObjects(JavaCollectionsUtil.EmptySet<PdfIndirectReference>());
            // There also was a NPE because FlateFilter was already flushed.
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, LogLevel = LogLevelConstants.INFO
            )]
        [NUnit.Framework.Test]
        public virtual void DecodeParamsFlushedBeforeStreamTest() {
            String file = sourceFolder + "decodeParamsTest.pdf";
            String cmpFile = sourceFolder + "cmp_decodeParamsTest.pdf";
            String destFile = destinationFolder + "decodeParamsTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            PdfStream stream = (PdfStream)doc.GetPdfObject(7);
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            //Simulating that this flush happened automatically before normal stream flushing in close method
            stream.Get(PdfName.DecodeParms).MakeIndirect(stream.GetIndirectReference().GetDocument()).Flush();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, LogLevel = LogLevelConstants.INFO
            )]
        [NUnit.Framework.Test]
        public virtual void DecodeParamsPredictorFlushedBeforeStreamTest() {
            String file = sourceFolder + "decodeParamsTest.pdf";
            String cmpFile = sourceFolder + "cmp_decodeParamsPredictorTest.pdf";
            String destFile = destinationFolder + "decodeParamsPredictorTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            PdfStream stream = (PdfStream)doc.GetPdfObject(7);
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            //Simulating that this flush happened automatically before normal stream flushing in close method
            ((PdfDictionary)stream.Get(PdfName.DecodeParms)).Get(PdfName.Predictor).MakeIndirect(stream.GetIndirectReference
                ().GetDocument()).Flush();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, LogLevel = LogLevelConstants.INFO
            )]
        [NUnit.Framework.Test]
        public virtual void DecodeParamsColumnsFlushedBeforeStreamTest() {
            String file = sourceFolder + "decodeParamsTest.pdf";
            String cmpFile = sourceFolder + "cmp_decodeParamsColumnsTest.pdf";
            String destFile = destinationFolder + "decodeParamsColumnsTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            PdfStream stream = (PdfStream)doc.GetPdfObject(7);
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            //Simulating that this flush happened automatically before normal stream flushing in close method
            ((PdfDictionary)stream.Get(PdfName.DecodeParms)).Get(PdfName.Columns).MakeIndirect(stream.GetIndirectReference
                ().GetDocument()).Flush();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, LogLevel = LogLevelConstants.INFO
            )]
        [NUnit.Framework.Test]
        public virtual void DecodeParamsColorsFlushedBeforeStreamTest() {
            String file = sourceFolder + "decodeParamsTest.pdf";
            String cmpFile = sourceFolder + "cmp_decodeParamsColorsTest.pdf";
            String destFile = destinationFolder + "decodeParamsColorsTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            PdfStream stream = (PdfStream)doc.GetPdfObject(7);
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            //Simulating that this flush happened automatically before normal stream flushing in close method
            ((PdfDictionary)stream.Get(PdfName.DecodeParms)).Get(PdfName.Colors).MakeIndirect(stream.GetIndirectReference
                ().GetDocument()).Flush();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, LogLevel = LogLevelConstants.INFO
            )]
        [NUnit.Framework.Test]
        public virtual void DecodeParamsBitsPerComponentFlushedBeforeStreamTest() {
            String file = sourceFolder + "decodeParamsTest.pdf";
            String cmpFile = sourceFolder + "cmp_decodeParamsBitsPerComponentTest.pdf";
            String destFile = destinationFolder + "decodeParamsBitsPerComponentTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            PdfStream stream = (PdfStream)doc.GetPdfObject(7);
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            //Simulating that this flush happened automatically before normal stream flushing in close method
            ((PdfDictionary)stream.Get(PdfName.DecodeParms)).Get(PdfName.BitsPerComponent).MakeIndirect(stream.GetIndirectReference
                ().GetDocument()).Flush();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, Count = 2, LogLevel = LogLevelConstants
            .INFO)]
        [NUnit.Framework.Test]
        public virtual void FlateFilterFlushedWhileDecodeTest() {
            String file = sourceFolder + "decodeParamsTest.pdf";
            String cmpFile = sourceFolder + "cmp_flateFilterFlushedWhileDecodeTest.pdf";
            String destFile = destinationFolder + "flateFilterFlushedWhileDecodeTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            PdfStream stream = (PdfStream)doc.GetPdfObject(7);
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            stream.Remove(PdfName.Filter);
            stream.Put(PdfName.Filter, new PdfName(PdfName.FlateDecode.value));
            //Simulating that this flush happened automatically before normal stream flushing in close method
            stream.Get(PdfName.Filter).MakeIndirect(stream.GetIndirectReference().GetDocument()).Flush();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED, Count = 2, LogLevel = LogLevelConstants
            .INFO)]
        [NUnit.Framework.Test]
        public virtual void ArrayFlateFilterFlushedWhileDecodeTest() {
            String file = sourceFolder + "decodeParamsTest.pdf";
            String cmpFile = sourceFolder + "cmp_arrayFlateFilterFlushedWhileDecodeTest.pdf";
            String destFile = destinationFolder + "arrayFlateFilterFlushedWhileDecodeTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(file), new PdfWriter(destFile));
            PdfStream stream = (PdfStream)doc.GetPdfObject(7);
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            stream.Remove(PdfName.Filter);
            stream.Put(PdfName.Filter, new PdfArray(new PdfName(PdfName.FlateDecode.value)));
            //Simulating that this flush happened automatically before normal stream flushing in close method
            stream.Get(PdfName.Filter).MakeIndirect(stream.GetIndirectReference().GetDocument()).Flush();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }
    }
}
