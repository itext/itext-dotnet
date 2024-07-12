/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfXrefTableTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfXrefTableTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfXrefTableTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, LogLevel = 
            LogLevelConstants.ERROR)]
        public virtual void OpenInvalidDocWithHugeRefTest() {
            String inputFile = SOURCE_FOLDER + "invalidDocWithHugeRef.pdf";
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new _MemoryLimitsAwareHandler_67();
            NUnit.Framework.Assert.DoesNotThrow(() => new PdfDocument(new PdfReader(inputFile, new ReaderProperties().
                SetMemoryLimitsAwareHandler(memoryLimitsAwareHandler))));
        }

        private sealed class _MemoryLimitsAwareHandler_67 : MemoryLimitsAwareHandler {
            public _MemoryLimitsAwareHandler_67() {
            }

            public override void CheckIfXrefStructureExceedsTheLimit(int requestedCapacity) {
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, LogLevel = 
            LogLevelConstants.ERROR)]
        public virtual void OpenInvalidDocWithHugeRefTestDefaultMemoryLimitAwareHandler() {
            String inputFile = SOURCE_FOLDER + "invalidDocWithHugeRef.pdf";
            NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => new PdfDocument(new PdfReader(inputFile
                )));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, LogLevel = 
            LogLevelConstants.ERROR)]
        public virtual void OpenWithWriterInvalidDocWithHugeRefTest() {
            String inputFile = SOURCE_FOLDER + "invalidDocWithHugeRef.pdf";
            MemoryStream outputStream = new ByteArrayOutputStream();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(new PdfReader(inputFile
                ), new PdfWriter(outputStream)));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STRUCTURE_SIZE_EXCEEDED_THE_LIMIT, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestCreateAndUpdateXMP() {
            String created = DESTINATION_FOLDER + "testCreateAndUpdateXMP_create.pdf";
            String updated = DESTINATION_FOLDER + "testCreateAndUpdateXMP_update.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(created));
            pdfDocument.AddNewPage();
            // create XMP metadata
            pdfDocument.GetXmpMetadata(true);
            pdfDocument.Close();
            pdfDocument = new PdfDocument(CompareTool.CreateOutputReader(created), CompareTool.CreateTestPdfWriter(updated
                ));
            PdfXrefTable xref = pdfDocument.GetXref();
            PdfDictionary catalog = pdfDocument.GetCatalog().GetPdfObject();
            ((PdfIndirectReference)catalog.Remove(PdfName.Metadata)).SetFree();
            PdfIndirectReference ref0 = xref.Get(0);
            PdfIndirectReference freeRef = xref.Get(6);
            pdfDocument.Close();
            /*
            Current xref structure:
            xref
            0 8
            0000000006 65535 f % this is object 0; 6 refers to free object 6
            0000000203 00000 n
            0000000510 00000 n
            0000000263 00000 n
            0000000088 00000 n
            0000000015 00000 n
            0000000000 00001 f % this is object 6; 0 refers to free object 0; note generation number
            0000000561 00000 n
            */
            NUnit.Framework.Assert.IsTrue(freeRef.IsFree());
            NUnit.Framework.Assert.AreEqual(ref0.offsetOrIndex, freeRef.objNr);
            NUnit.Framework.Assert.AreEqual(1, freeRef.genNr);
        }

        [NUnit.Framework.Test]
        public virtual void TestCreateAndUpdateTwiceXMP() {
            String created = DESTINATION_FOLDER + "testCreateAndUpdateTwiceXMP_create.pdf";
            String updated = DESTINATION_FOLDER + "testCreateAndUpdateTwiceXMP_update.pdf";
            String updatedAgain = DESTINATION_FOLDER + "testCreateAndUpdateTwiceXMP_updatedAgain.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(created));
            pdfDocument.AddNewPage();
            // create XMP metadata
            pdfDocument.GetXmpMetadata(true);
            pdfDocument.Close();
            pdfDocument = new PdfDocument(CompareTool.CreateOutputReader(created), CompareTool.CreateTestPdfWriter(updated
                ));
            PdfDictionary catalog = pdfDocument.GetCatalog().GetPdfObject();
            ((PdfIndirectReference)catalog.Remove(PdfName.Metadata)).SetFree();
            pdfDocument.Close();
            pdfDocument = new PdfDocument(CompareTool.CreateOutputReader(updated), CompareTool.CreateTestPdfWriter(updatedAgain
                ));
            catalog = pdfDocument.GetCatalog().GetPdfObject();
            ((PdfIndirectReference)catalog.Remove(PdfName.Metadata)).SetFree();
            PdfXrefTable xref = pdfDocument.GetXref();
            PdfIndirectReference ref0 = xref.Get(0);
            PdfIndirectReference freeRef1 = xref.Get(6);
            PdfIndirectReference freeRef2 = xref.Get(7);
            pdfDocument.Close();
            /*
            Current xref structure:
            xref
            0 9
            0000000006 65535 f % this is object 0; 6 refers to free object 6
            0000000203 00000 n
            0000000510 00000 n
            0000000263 00000 n
            0000000088 00000 n
            0000000015 00000 n
            0000000007 00001 f % this is object 6; 7 refers to free object 7; note generation number
            0000000000 00001 f % this is object 7; 0 refers to free object 0; note generation number
            0000000561 00000 n
            */
            NUnit.Framework.Assert.IsTrue(freeRef1.IsFree());
            NUnit.Framework.Assert.AreEqual(ref0.offsetOrIndex, freeRef1.objNr);
            NUnit.Framework.Assert.AreEqual(1, freeRef1.genNr);
            NUnit.Framework.Assert.IsTrue(freeRef2.IsFree());
            NUnit.Framework.Assert.AreEqual(freeRef1.offsetOrIndex, freeRef2.objNr);
            NUnit.Framework.Assert.AreEqual(1, freeRef2.genNr);
            pdfDocument.Close();
        }
    }
}
